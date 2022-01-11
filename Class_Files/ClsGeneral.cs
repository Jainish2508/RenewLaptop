using iTextSharp.text;
using iTextSharp.text.html;
using iTextSharp.text.pdf;
using Newtonsoft.Json;
using Renew_Laptop.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;

namespace Renew_Laptop.Class_Files
{
    public class ClsGeneral
    {
        private static string clsstring, country, ex, p_name, valid;
        private RenewLaptopEntities db = new RenewLaptopEntities();

        public string IP_Country_From_DB(GeneralObj obj)
        {
            string cip = null;
            try
            {
                if (obj.ip.IndexOf(".") > 0)
                {
                    string[] iprange = obj.ip.Split('.');
                    for (int i = 0; i < iprange.Length; i++)
                    {
                        cip += iprange[i].PadLeft(3, '0');
                    }
                }
                country = db.ip2country(cip).FirstOrDefault().ToString();
                return (!string.IsNullOrEmpty(country)) ? country : "IN";
            }
            catch (Exception e)
            {
                ex = e.Message;
                if (string.IsNullOrEmpty(country))
                    country = null;
                Error_method(ex, obj);
                return null;
            }
        }

        [Obsolete]
        public async Task<bool> send_contact_mail(GeneralObj obj, ContactViewModel contactViewModel)
        {
            string timestamp = DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss");
            string from, to, sub;
            try
            {
                from = ConfigurationManager.AppSettings.Get("from");
                to = ConfigurationManager.AppSettings.Get("to");
                var msg = new MailMessage();
                msg.From = new MailAddress(from, "Renew Laptop Customer Support");
                msg.To.Add(new MailAddress(to));
                msg.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                var creds = new NetworkCredential
                {
                    UserName = ConfigurationManager.AppSettings.Get("Username"),
                    Password = ConfigurationManager.AppSettings.Get("Password")
                };
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = creds;
                smtp.Host = ConfigurationManager.AppSettings.Get("Host").ToString();
                smtp.Port = Convert.ToInt32(ConfigurationManager.AppSettings.Get("Port"));
                smtp.EnableSsl = true;
                sub = "Enquiry on " + contactViewModel.subject;
                var body = "<p><b>Name:</b> {0}</p>" +
                                "<p><b>Email:</b> {1}</p>" +
                                "<p><b>Phone:</b> {2}</p>" +
                                "<p><b>Message: </b>{3}</p>" +
                                "<p><b>IP Address: </b>{4}/{5}</p>" +
                                "<p><b>Date: </b>{6}</p>";
                msg.Subject = sub;
                msg.Body = string.Format(body,
                        contactViewModel.name,
                        contactViewModel.email,
                        contactViewModel.phone,
                        contactViewModel.message,
                        obj.ip,
                        obj.country,
                        timestamp);
                await Task.Run(() => Insert_Data(obj, contactViewModel, timestamp));

                smtp.Send(msg);
                return true;
            }
            catch (Exception e)
            {
                ex = e.Message;
                if (string.IsNullOrEmpty(country))
                    country = null;
                Error_method(ex, obj);
                return false;
            }
        }

        [Obsolete]
        public async Task<string> Create_Order(Product product, GeneralObj obj)
        {
            try
            {
                string razorpay_key = ConfigurationManager.AppSettings.Get("RazorPayKey");
                Razorpay.Api.RazorpayClient client = new Razorpay.Api.RazorpayClient("rzp_live_er0PYOxiFK91t3", razorpay_key);
                Dictionary<string, object> keyValues = new Dictionary<string, object>();
                keyValues.Add("amount", product.UnitPrice * 100);
                keyValues.Add("currency", "INR");
                Razorpay.Api.Order razorpayOrder = client.Order.Create(keyValues);
                return razorpayOrder["id"].ToString();
            }
            catch (Exception e)
            {
                ex = e.Message;
                if (string.IsNullOrEmpty(obj.country))
                    obj.country = null;
                Error_method(ex, obj);
            }
            return null;
        }

        [Obsolete]
        public async Task<bool> new_order(GeneralObj obj, Customer customer, int? product_id, [Optional] string paymentid, [Optional] string orderid)
        {
            Order order = new Order();
            OrderDetail orderDetail = new OrderDetail();
            Payment payment = new Payment();
            DateTime timestamp = DateTime.Parse(DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));
            DateTime valid_time;
            double amount = 0.00;
            product_id = (product_id == null || product_id == 0) ? 1 : product_id;
            try
            {
                #region Populating payment model
                switch (product_id)
                {
                    case 1:
                        amount = 0.00;
                        p_name = "Diagnostic Plan";
                        valid = "30 Days";
                        valid_time = timestamp.AddDays(30);
                        break;

                    case 2:
                        amount = 2800.00;
                        p_name = "One-Time Fix Plan";
                        valid = "30 Days or 1 Time";
                        valid_time = timestamp.AddDays(30);
                        break;

                    case 3:
                        amount = 4800.00;
                        p_name = "Annual Support Plan";
                        valid = "1 Year";
                        valid_time = timestamp.AddYears(1);
                        break;

                    default:
                        amount = 0.00;
                        p_name = "Diagnostic Plan";
                        valid = "30 Days";
                        valid_time = timestamp.AddDays(30);
                        break;
                }
                payment.TransactionID = paymentid;
                payment.PaymentDate = timestamp;
                payment.ValidUpTo = valid_time;

                #endregion Populating payment model

                #region Populating order model

                order.TransacType = (product_id == 1) ? "No" : "Razorpay";
                order.Deleted = false;
                order.BillDate = timestamp;
                order.IP = obj.ip + "/" + obj.country;

                #endregion Populating order model

                #region Populating OrderDetails model

                orderDetail.ProductID = (int)product_id;
                orderDetail.RazorOrderId = (!string.IsNullOrEmpty(orderid)) ? orderid : "NULL";
                orderDetail.Price = amount;
                orderDetail.Quantity = 1;
                orderDetail.GST = (18 * amount) / 100;
                orderDetail.Discount = 0.00;
                orderDetail.Total = (orderDetail.Price * orderDetail.Quantity);

                #endregion Populating OrderDetails model

                if (await Task.Run(() => Create_Order(obj, customer, payment, order, orderDetail) && Send_order_mail(obj, customer, payment, order, orderDetail)))
                    return true;
            }
            catch (Exception e)
            {
                ex = e.Message;
                if (string.IsNullOrEmpty(obj.country))
                    obj.country = null;
                Error_method(ex, obj);
            }
            return false;
        }

        [Obsolete]
        public bool Send_order_mail(GeneralObj obj, Customer customer, Payment payment, Order order, OrderDetail orderDetail)
        {
            string filepath;
            try
            {
                if (!obj.servername.Equals("localhost"))
                    filepath = @"E:\kunden\homepages\15\d833659789\www\RenewLaptop\Invoice.html";
                else
                    filepath = @"E:\Work\Website Work\RenewLaptop\v1.0\Renew_Laptop\Invoice.html";
                StreamReader sr = new StreamReader(filepath);
                string mailbody = sr.ReadToEnd();
                sr.Close();

                #region Replacing values

                mailbody = mailbody.Replace("[Name]", string.Format("{0} {1}", customer.FirstName, customer.LastName));
                mailbody = mailbody.Replace("[Email]", customer.Email);
                mailbody = mailbody.Replace("[Phone]", customer.Phone);
                mailbody = mailbody.Replace("[Address]", (!string.IsNullOrEmpty(customer.AddressLine2) ? customer.AddressLine1 + customer.AddressLine2 + "," + customer.City + "," + customer.State + "-" + customer.PostalCode : customer.AddressLine1 + "," + customer.City + "," + customer.State + "-" + customer.PostalCode));
                mailbody = mailbody.Replace("[Transaction Id]", payment.TransactionID);
                mailbody = mailbody.Replace("[Company]", (!string.IsNullOrEmpty(customer.CompanyName) ? customer.CompanyName : "-"));
                mailbody = mailbody.Replace("[GST]", (!string.IsNullOrEmpty(customer.GSTIN) ? customer.GSTIN : "-"));
                mailbody = mailbody.Replace("[Date]", order.BillDate.ToString());
                mailbody = mailbody.Replace("[Amount]", (orderDetail.Quantity * orderDetail.Price).ToString());
                mailbody = mailbody.Replace("[Item]", p_name);
                mailbody = mailbody.Replace("[Price]", orderDetail.Price.ToString());
                mailbody = mailbody.Replace("[Qty.]", orderDetail.Quantity.ToString());
                mailbody = mailbody.Replace("[Total]", orderDetail.Total.ToString());
                mailbody = mailbody.Replace("[Valid Upto]", payment.ValidUpTo.ToString());
                mailbody = mailbody.Replace("[Valid]", valid);
                mailbody = mailbody.Replace("[Tax]", orderDetail.GST.ToString());

                #endregion Replacing values

                MailMessage msg = new MailMessage();
                msg.From = new MailAddress(ConfigurationManager.AppSettings.Get("from"), "Renew Laptop");
                msg.To.Add(new MailAddress(customer.Email));
                msg.Bcc.Add(new MailAddress(ConfigurationManager.AppSettings.Get("to_admin")));
                msg.IsBodyHtml = true;
                msg.Subject = "Order Receipt for " + p_name;
                msg.Body = mailbody;

                #region SMTP Configuration

                SmtpClient smtp = new SmtpClient();
                var creds = new NetworkCredential()
                {
                    UserName = ConfigurationManager.AppSettings.Get("Username"),
                    Password = ConfigurationManager.AppSettings.Get("Password")
                };
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = creds;
                smtp.Host = ConfigurationManager.AppSettings.Get("Host");
                smtp.Port = Convert.ToInt32(ConfigurationManager.AppSettings.Get("Port"));
                smtp.EnableSsl = true;

                #endregion SMTP Configuration

                smtp.Send(msg);
                return true;
            }
            catch (Exception e)
            {
                ex = e.Message;
                if (string.IsNullOrEmpty(obj.country))
                    obj.country = null;
                Error_method(ex, obj);
            }
            return false;
        }

        [Obsolete]
        public bool Create_Order(GeneralObj obj, Customer customer, Payment payment, Order order, OrderDetail orderDetail)
        {
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    db.Create_Order(payment.TransactionID, payment.PaymentDate, payment.ValidUpTo, customer.FirstName, customer.LastName, customer.Email, customer.Phone, customer.CompanyName, customer.GSTIN, customer.AddressLine1, customer.AddressLine2, customer.City, customer.State, customer.PostalCode, order.TransacType, order.Deleted, order.BillDate, string.Format("{0}/{1}", obj.ip, obj.country), orderDetail.ProductID, orderDetail.RazorOrderId, orderDetail.Price, orderDetail.Quantity, orderDetail.GST, orderDetail.Discount, orderDetail.Total);
                    ts.Complete();
                }
                return true;
            }
            catch (Exception e)
            {
                obj.ex = e.Message;
                if (string.IsNullOrEmpty(obj.country))
                    obj.country = null;
                Error_method(obj.ex, obj);
            }
            return false;
        }

        [Obsolete]
        public byte[] CreateInvoice(GeneralObj obj, string servername, string order_id, string fname, string lname, string email, string phone, [Optional] string company_name, [Optional] string gstin, string address, string Desc, int pid)
        {
            byte[] result;
            string ex;
            try
            {
                string timestamp = DateTime.Now.ToShortDateString();
                using (var ms = new MemoryStream())
                {
                    Document pdfDoc = new Document(PageSize.A4, 25, 25, 25, 25);
                    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, ms);

                    var titleFont = FontFactory.GetFont("Calibri", 12, BaseColor.BLACK);
                    var titleFontBlue = FontFactory.GetFont("Arial", 14, Font.NORMAL, BaseColor.BLUE);
                    var boldTableFont = FontFactory.GetFont("Arial", 8, Font.BOLD);
                    var linkFont = FontFactory.GetFont("Calibri", 9, new BaseColor(System.Drawing.Color.FromArgb(3, 158, 227)));
                    var bodyFont = FontFactory.GetFont("Calibri", 9, BaseColor.BLACK);
                    var EmailFont = FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLUE);
                    BaseColor TabelHeaderBackGroundColor = WebColors.GetRGBColor("#EEEEEE");

                    Rectangle pageSize = writer.PageSize;
                    // Open the Document for writing
                    pdfDoc.Open();
                    //Add elements to the document here

                    PdfPTable headertable = new PdfPTable(3);
                    headertable.DefaultCell.Border = Rectangle.NO_BORDER;
                    headertable.WidthPercentage = 100;
                    headertable.SetWidths(new float[] { 250f, 150f, 150f });
                    iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(HttpContext.Current.Server.MapPath("~/Content/images/logos/logo.png"));
                    logo.ScaleAbsolute(250, 120);
                    logo.ScalePercent(15);

                    {
                        PdfPCell logocell = new PdfPCell(logo);
                        logocell.Border = Rectangle.NO_BORDER;
                        logocell.BorderColorBottom = new BaseColor(System.Drawing.Color.FromArgb(3, 158, 227));
                        logocell.BorderWidthBottom = 1f;
                        logocell.HorizontalAlignment = Element.ALIGN_LEFT;
                        headertable.AddCell(logocell);
                    }

                    {
                        PdfPTable nested = new PdfPTable(1);
                        nested.DefaultCell.Border = Rectangle.NO_BORDER;
                        nested.HorizontalAlignment = Element.ALIGN_RIGHT;
                        Anchor web = new Anchor("www.renew-laptop.com", linkFont);
                        web.Reference = "https://www.renew-laptop.com";
                        PdfPCell webcell = new PdfPCell(web);
                        webcell.PaddingBottom = 5f;
                        webcell.Border = Rectangle.NO_BORDER;
                        webcell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        nested.AddCell(webcell);
                        Anchor company_phone = new Anchor("+91-99740 97022", linkFont);
                        company_phone.Reference = "tel:+91-99740 97022";
                        PdfPCell phonecell = new PdfPCell(company_phone);
                        phonecell.PaddingBottom = 5f;
                        phonecell.Border = Rectangle.NO_BORDER;
                        phonecell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        nested.AddCell(phonecell);
                        Anchor company_email = new Anchor("pratik@renew-laptop.com", linkFont);
                        company_email.Reference = "mailto:pratik@renew-laptop.com";
                        PdfPCell emailcell = new PdfPCell(company_email);
                        emailcell.PaddingBottom = 5f;
                        emailcell.Border = Rectangle.NO_BORDER;
                        emailcell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        nested.AddCell(emailcell);
                        PdfPCell nest = new PdfPCell(nested);
                        nest.Border = Rectangle.NO_BORDER;
                        nest.BorderColorBottom = new BaseColor(System.Drawing.Color.FromArgb(3, 158, 227));
                        nest.BorderWidthBottom = 1f;
                        nest.PaddingTop = 20f;
                        nest.PaddingBottom = 15f;
                        headertable.AddCell(nest);
                    }

                    {
                        PdfPTable nested = new PdfPTable(1);
                        nested.DefaultCell.Border = Rectangle.NO_BORDER;
                        nested.HorizontalAlignment = Element.ALIGN_RIGHT;
                        PdfPCell addressheading = new PdfPCell(new Phrase("Address:", titleFont));
                        addressheading.Border = Rectangle.NO_BORDER;
                        addressheading.HorizontalAlignment = Element.ALIGN_RIGHT;
                        addressheading.PaddingBottom = 8f;
                        nested.AddCell(addressheading);
                        PdfPCell addressline1 = new PdfPCell(new Phrase("Shop No. 3 FF, Devpriya Complex,", bodyFont));
                        addressline1.Border = Rectangle.NO_BORDER;
                        addressline1.HorizontalAlignment = Element.ALIGN_RIGHT;
                        addressline1.PaddingBottom = 5f;
                        nested.AddCell(addressline1);
                        PdfPCell addressline2 = new PdfPCell(new Phrase("Opp. Natraj Medical, Nr. Sima Hall,", bodyFont));
                        addressline2.Border = Rectangle.NO_BORDER;
                        addressline2.HorizontalAlignment = Element.ALIGN_RIGHT;
                        addressline2.PaddingBottom = 5f;
                        nested.AddCell(addressline2);
                        PdfPCell addressline3 = new PdfPCell(new Phrase("Satellite, Ahmedabad-380015", bodyFont));
                        addressline3.Border = Rectangle.NO_BORDER;
                        addressline3.HorizontalAlignment = Element.ALIGN_RIGHT;
                        //addressline3.PaddingBottom = 5f;
                        nested.AddCell(addressline3);
                        PdfPCell nest = new PdfPCell(nested);
                        nest.Border = Rectangle.NO_BORDER;
                        nest.BorderColorBottom = new BaseColor(System.Drawing.Color.FromArgb(3, 158, 227));
                        nest.BorderWidthBottom = 1f;
                        //nest.PaddingTop = 15f;
                        headertable.AddCell(nest);
                    }

                    PdfPTable maintable = new PdfPTable(3);
                    maintable.DefaultCell.Border = Rectangle.NO_BORDER;
                    maintable.WidthPercentage = 100;
                    maintable.SetWidths(new float[] { 100f, 150f, 150f });

                    {
                        PdfPTable nested = new PdfPTable(1);
                        nested.DefaultCell.Border = Rectangle.NO_BORDER;
                        nested.HorizontalAlignment = Element.ALIGN_LEFT;
                        PdfPCell invoiceheading = new PdfPCell(new Phrase("Invoice To,", titleFont));
                        invoiceheading.Border = Rectangle.NO_BORDER;
                        invoiceheading.HorizontalAlignment = Element.ALIGN_LEFT;
                        invoiceheading.PaddingBottom = 8f;
                        nested.AddCell(invoiceheading);
                        PdfPCell invoice_name = new PdfPCell(new Phrase(string.Format("{0} {1}", fname, lname), bodyFont));
                        invoice_name.Border = Rectangle.NO_BORDER;
                        invoice_name.HorizontalAlignment = Element.ALIGN_LEFT;
                        invoice_name.PaddingBottom = 5f;
                        nested.AddCell(invoice_name);
                        PdfPCell invoice_address = new PdfPCell(new Phrase("Shop No. 3 FF, Devpriya Complex, Satellite, Ahmedabad 380015", bodyFont));
                        invoice_address.Border = Rectangle.NO_BORDER;
                        invoice_address.HorizontalAlignment = Element.ALIGN_LEFT;
                        invoice_address.PaddingBottom = 5f;
                        nested.AddCell(invoice_address);
                        PdfPCell invoice_phone = new PdfPCell(new Phrase("+91-7600416878", bodyFont));
                        invoice_phone.Border = Rectangle.NO_BORDER;
                        invoice_phone.HorizontalAlignment = Element.ALIGN_LEFT;
                        invoice_phone.PaddingBottom = 5f;
                        nested.AddCell(invoice_phone);
                        PdfPCell invoice_email = new PdfPCell(new Phrase("sjainish31@yahoo.com", bodyFont)); //Change email
                        invoice_email.Border = Rectangle.NO_BORDER;
                        invoice_email.HorizontalAlignment = Element.ALIGN_LEFT;
                        invoice_email.PaddingBottom = 5f;
                        nested.AddCell(invoice_email);
                        PdfPCell nest = new PdfPCell(nested);
                        nest.Border = Rectangle.NO_BORDER;
                        nest.PaddingTop = 15f;
                        nest.PaddingBottom = 10f;
                        maintable.AddCell(nest);
                    }
                    {
                        PdfPTable nested = new PdfPTable(1);
                        nested.DefaultCell.Border = Rectangle.NO_BORDER;
                        nested.HorizontalAlignment = Element.ALIGN_LEFT;
                        PdfPCell invoice_company_heading = new PdfPCell(new Phrase("Company Name:", titleFont));
                        invoice_company_heading.Border = Rectangle.NO_BORDER;
                        invoice_company_heading.HorizontalAlignment = Element.ALIGN_LEFT;
                        invoice_company_heading.PaddingBottom = 8f;
                        nested.AddCell(invoice_company_heading);
                        PdfPCell invoice_company = new PdfPCell(new Phrase("-", bodyFont));
                        invoice_company.Border = Rectangle.NO_BORDER;
                        invoice_company.HorizontalAlignment = Element.ALIGN_LEFT;
                        invoice_company.PaddingBottom = 5f;
                        nested.AddCell(invoice_company);
                        PdfPCell invoice_gstin_heading = new PdfPCell(new Phrase("GSTIN: ", titleFont));
                        invoice_gstin_heading.Border = Rectangle.NO_BORDER;
                        invoice_gstin_heading.HorizontalAlignment = Element.ALIGN_LEFT;
                        invoice_gstin_heading.PaddingBottom = 8f;
                        nested.AddCell(invoice_gstin_heading);
                        PdfPCell invoice_gstin = new PdfPCell(new Phrase("-", bodyFont));
                        invoice_gstin.Border = Rectangle.NO_BORDER;
                        invoice_gstin.HorizontalAlignment = Element.ALIGN_LEFT;
                        invoice_gstin.PaddingBottom = 5f;
                        nested.AddCell(invoice_gstin);
                        PdfPCell nest = new PdfPCell(nested);
                        nest.Border = Rectangle.NO_BORDER;
                        nest.PaddingTop = 15f;
                        nest.PaddingBottom = 10f;
                        nest.PaddingLeft = 10f;
                        maintable.AddCell(nest);
                    }
                    {
                        PdfPTable nested = new PdfPTable(1);
                        nested.DefaultCell.Border = Rectangle.NO_BORDER;
                        nested.HorizontalAlignment = Element.ALIGN_LEFT;
                        PdfPCell invoice_orderid_heading = new PdfPCell(new Phrase("Order ID:", titleFont));
                        invoice_orderid_heading.Border = Rectangle.NO_BORDER;
                        invoice_orderid_heading.HorizontalAlignment = Element.ALIGN_RIGHT;
                        invoice_orderid_heading.PaddingBottom = 8f;
                        nested.AddCell(invoice_orderid_heading);
                        PdfPCell invoice_orderid = new PdfPCell(new Phrase("-", bodyFont));
                        invoice_orderid.Border = Rectangle.NO_BORDER;
                        invoice_orderid.HorizontalAlignment = Element.ALIGN_RIGHT;
                        invoice_orderid.PaddingBottom = 5f;
                        nested.AddCell(invoice_orderid);
                        PdfPCell invoice_number_heading = new PdfPCell(new Phrase("Invoice ID:", titleFont));
                        invoice_number_heading.Border = Rectangle.NO_BORDER;
                        invoice_number_heading.HorizontalAlignment = Element.ALIGN_RIGHT;
                        invoice_number_heading.PaddingBottom = 8f;
                        nested.AddCell(invoice_number_heading);
                        PdfPCell invoice_number = new PdfPCell(new Phrase("# 123-123456", bodyFont));
                        invoice_number.Border = Rectangle.NO_BORDER;
                        invoice_number.HorizontalAlignment = Element.ALIGN_RIGHT;
                        invoice_number.PaddingBottom = 5f;
                        nested.AddCell(invoice_number);
                        PdfPCell invoice_timestamp_heading = new PdfPCell(new Phrase("Date of Issue: ", titleFont));
                        invoice_timestamp_heading.Border = Rectangle.NO_BORDER;
                        invoice_timestamp_heading.HorizontalAlignment = Element.ALIGN_RIGHT;
                        invoice_timestamp_heading.PaddingBottom = 8f;
                        nested.AddCell(invoice_timestamp_heading);
                        PdfPCell invoice_timestamp = new PdfPCell(new Phrase(timestamp, bodyFont));
                        invoice_timestamp.Border = Rectangle.NO_BORDER;
                        invoice_timestamp.HorizontalAlignment = Element.ALIGN_RIGHT;
                        invoice_timestamp.PaddingBottom = 5f;
                        nested.AddCell(invoice_timestamp);
                        PdfPCell nest = new PdfPCell(nested);
                        nest.Border = Rectangle.NO_BORDER;
                        nest.PaddingTop = 15f;
                        nest.PaddingBottom = 10f;
                        maintable.AddCell(nest);
                    }

                    PdfPTable itemtable = new PdfPTable(4);
                    itemtable.DefaultCell.Border = Rectangle.NO_BORDER;
                    itemtable.WidthPercentage = 80;
                    itemtable.SetWidths(new float[] { 40f, 25f, 25f, 25f });

                    {
                        PdfPTable nested = new PdfPTable(1);
                        nested.DefaultCell.Border = Rectangle.NO_BORDER;
                        nested.HorizontalAlignment = Element.ALIGN_LEFT;
                        PdfPCell invoice_desc_heading = new PdfPCell(new Phrase("Description", titleFont));
                        invoice_desc_heading.Border = Rectangle.NO_BORDER;
                        invoice_desc_heading.BorderColorBottom = new BaseColor(System.Drawing.Color.FromArgb(181, 179, 179));
                        invoice_desc_heading.BorderWidthBottom = 1f;
                        invoice_desc_heading.HorizontalAlignment = Element.ALIGN_LEFT;
                        invoice_desc_heading.PaddingBottom = 8f;
                        nested.AddCell(invoice_desc_heading);
                        PdfPCell invoice_desc = new PdfPCell(new Phrase("Diagnostic Subcription Plan", bodyFont));
                        invoice_desc.Border = Rectangle.NO_BORDER;
                        invoice_desc.HorizontalAlignment = Element.ALIGN_LEFT;
                        nested.AddCell(invoice_desc);
                        PdfPCell nest = new PdfPCell(nested);
                        nest.Border = Rectangle.NO_BORDER;
                        nest.BorderColorTop = new BaseColor(System.Drawing.Color.FromArgb(3, 158, 227));
                        nest.BorderWidthTop = 1f;
                        nest.PaddingTop = 15f;
                        nest.PaddingBottom = 10f;
                        itemtable.AddCell(nest);
                    }

                    {
                        PdfPTable nested = new PdfPTable(1);
                        nested.DefaultCell.Border = Rectangle.NO_BORDER;
                        nested.HorizontalAlignment = Element.ALIGN_LEFT;
                        PdfPCell invoice_sr_heading = new PdfPCell(new Phrase("Unit Price", titleFont));
                        invoice_sr_heading.Border = Rectangle.NO_BORDER;
                        invoice_sr_heading.BorderColorBottom = new BaseColor(System.Drawing.Color.FromArgb(181, 179, 179));
                        invoice_sr_heading.BorderWidthBottom = 1f;
                        invoice_sr_heading.PaddingBottom = 8f;
                        invoice_sr_heading.HorizontalAlignment = Element.ALIGN_LEFT;
                        nested.AddCell(invoice_sr_heading);
                        PdfPCell invoice_desc = new PdfPCell(new Phrase("Rs. 0.00", bodyFont));
                        invoice_desc.Border = Rectangle.NO_BORDER;
                        invoice_desc.HorizontalAlignment = Element.ALIGN_LEFT;
                        nested.AddCell(invoice_desc);
                        PdfPCell nest = new PdfPCell(nested);
                        nest.Border = Rectangle.NO_BORDER;
                        nest.BorderColorTop = new BaseColor(System.Drawing.Color.FromArgb(3, 158, 227));
                        nest.BorderWidthTop = 1f;
                        nest.PaddingTop = 15f;
                        nest.PaddingBottom = 10f;
                        itemtable.AddCell(nest);
                    }

                    {
                        PdfPTable nested = new PdfPTable(1);
                        nested.DefaultCell.Border = Rectangle.NO_BORDER;
                        nested.HorizontalAlignment = Element.ALIGN_LEFT;
                        PdfPCell invoice_qty_heading = new PdfPCell(new Phrase("Qty", titleFont));
                        invoice_qty_heading.Border = Rectangle.NO_BORDER;
                        invoice_qty_heading.BorderColorBottom = new BaseColor(System.Drawing.Color.FromArgb(181, 179, 179));
                        invoice_qty_heading.BorderWidthBottom = 1f;
                        invoice_qty_heading.PaddingBottom = 8f;
                        invoice_qty_heading.HorizontalAlignment = Element.ALIGN_LEFT;
                        nested.AddCell(invoice_qty_heading);
                        PdfPCell invoice_qty = new PdfPCell(new Phrase("1", bodyFont));
                        invoice_qty.Border = Rectangle.NO_BORDER;
                        invoice_qty.HorizontalAlignment = Element.ALIGN_LEFT;
                        nested.AddCell(invoice_qty);
                        PdfPCell nest = new PdfPCell(nested);
                        nest.Border = Rectangle.NO_BORDER;
                        nest.BorderColorTop = new BaseColor(System.Drawing.Color.FromArgb(3, 158, 227));
                        nest.BorderWidthTop = 1f;
                        nest.PaddingTop = 15f;
                        nest.PaddingBottom = 10f;
                        itemtable.AddCell(nest);
                    }

                    {
                        PdfPTable nested = new PdfPTable(1);
                        nested.DefaultCell.Border = Rectangle.NO_BORDER;
                        nested.HorizontalAlignment = Element.ALIGN_LEFT;
                        PdfPCell invoice_amount_heading = new PdfPCell(new Phrase("Amount", titleFont));
                        invoice_amount_heading.Border = Rectangle.NO_BORDER;
                        invoice_amount_heading.BorderColorBottom = new BaseColor(System.Drawing.Color.FromArgb(181, 179, 179));
                        invoice_amount_heading.BorderWidthBottom = 1f;
                        invoice_amount_heading.PaddingBottom = 8f;
                        invoice_amount_heading.HorizontalAlignment = Element.ALIGN_RIGHT;
                        nested.AddCell(invoice_amount_heading);
                        PdfPCell invoice_amount = new PdfPCell(new Phrase("Rs. 0.00", bodyFont));
                        invoice_amount.Border = Rectangle.NO_BORDER;
                        invoice_amount.HorizontalAlignment = Element.ALIGN_RIGHT;
                        nested.AddCell(invoice_amount);
                        PdfPCell nest = new PdfPCell(nested);
                        nest.Border = Rectangle.NO_BORDER;
                        nest.BorderColorTop = new BaseColor(System.Drawing.Color.FromArgb(3, 158, 227));
                        nest.BorderWidthTop = 1f;
                        nest.PaddingTop = 15f;
                        nest.PaddingBottom = 10f;
                        itemtable.AddCell(nest);
                    }

                    PdfPTable totaltable = new PdfPTable(2);
                    totaltable.DefaultCell.Border = Rectangle.NO_BORDER;
                    totaltable.WidthPercentage = 80;
                    totaltable.SetWidths(new float[] { 350f, 350f });

                    {
                        PdfPTable nested = new PdfPTable(1);
                        nested.DefaultCell.Border = Rectangle.NO_BORDER;
                        nested.HorizontalAlignment = Element.ALIGN_LEFT;
                        PdfPCell invoice_note = new PdfPCell(new Phrase("Invoice Summary & Notes", titleFont));
                        invoice_note.Border = Rectangle.NO_BORDER;
                        invoice_note.HorizontalAlignment = Element.ALIGN_LEFT;
                        invoice_note.PaddingBottom = 8f;
                        nested.AddCell(invoice_note);
                        PdfPCell invoice_note_text = new PdfPCell(new Phrase("NOTE: After successfully making order your subcription is valid only for 30 days after the date of invoice is created. There is 10 days refund policy in which you can send us an email to cancel your order and each subscription is cannot club with any other subcription. Bring the copy of invoice to the center while you are making visit or show an eletronic copy of invoice for the security checks.", bodyFont));
                        invoice_note_text.Border = Rectangle.NO_BORDER;
                        invoice_note_text.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
                        invoice_note_text.PaddingBottom = 5f;
                        invoice_note_text.PaddingRight = 15f;
                        nested.AddCell(invoice_note_text);
                        PdfPCell nest = new PdfPCell(nested);
                        nest.Border = Rectangle.NO_BORDER;
                        nest.BorderColorRight = new BaseColor(System.Drawing.Color.FromArgb(3, 158, 227));
                        nest.BorderWidthRight = 2f;
                        nest.PaddingTop = 15f;
                        nest.PaddingBottom = 10f;
                        totaltable.AddCell(nest);
                    }

                    {
                        PdfPTable nested = new PdfPTable(2);
                        nested.DefaultCell.Border = Rectangle.NO_BORDER;
                        nested.HorizontalAlignment = Element.ALIGN_LEFT;
                        PdfPCell invoice_subtotal_heading = new PdfPCell(new Phrase("Subtotal:", titleFont));
                        invoice_subtotal_heading.Border = Rectangle.NO_BORDER;
                        invoice_subtotal_heading.HorizontalAlignment = Element.ALIGN_LEFT;
                        invoice_subtotal_heading.PaddingBottom = 18f;
                        nested.AddCell(invoice_subtotal_heading);
                        PdfPCell invoice_subtotal_amount = new PdfPCell(new Phrase("Rs. 0.00", titleFont));
                        invoice_subtotal_amount.Border = Rectangle.NO_BORDER;
                        invoice_subtotal_amount.HorizontalAlignment = Element.ALIGN_RIGHT;
                        invoice_subtotal_amount.PaddingBottom = 18f;
                        nested.AddCell(invoice_subtotal_amount);
                        PdfPCell invoice_gst_heading = new PdfPCell(new Phrase("GST (18%):", titleFont));
                        invoice_gst_heading.Border = Rectangle.NO_BORDER;
                        invoice_gst_heading.HorizontalAlignment = Element.ALIGN_LEFT;
                        invoice_gst_heading.PaddingBottom = 18f;
                        nested.AddCell(invoice_gst_heading);
                        PdfPCell invoice_gst_amount = new PdfPCell(new Phrase("Rs. 0.00", titleFont));
                        invoice_gst_amount.Border = Rectangle.NO_BORDER;
                        invoice_gst_amount.HorizontalAlignment = Element.ALIGN_RIGHT;
                        invoice_gst_amount.PaddingBottom = 18f;
                        nested.AddCell(invoice_gst_amount);
                        PdfPCell invoice_total_heading = new PdfPCell(new Phrase("Total:", titleFont));
                        invoice_total_heading.Border = Rectangle.NO_BORDER;
                        invoice_total_heading.HorizontalAlignment = Element.ALIGN_LEFT;
                        invoice_total_heading.BorderColorTop = new BaseColor(System.Drawing.Color.FromArgb(3, 158, 227));
                        invoice_total_heading.BorderWidthTop = 2f;
                        invoice_total_heading.PaddingTop = 18f;
                        nested.AddCell(invoice_total_heading);
                        PdfPCell invoice_total_amount = new PdfPCell(new Phrase("Rs. 0.00", titleFont));
                        invoice_total_amount.Border = Rectangle.NO_BORDER;
                        invoice_total_amount.HorizontalAlignment = Element.ALIGN_RIGHT;
                        invoice_total_amount.BorderColorTop = new BaseColor(System.Drawing.Color.FromArgb(3, 158, 227));
                        invoice_total_amount.BorderWidthTop = 2f;
                        invoice_total_amount.PaddingTop = 18f;
                        nested.AddCell(invoice_total_amount);
                        PdfPCell nest = new PdfPCell(nested);
                        nest.Border = Rectangle.NO_BORDER;
                        nest.PaddingTop = 15f;
                        nest.PaddingLeft = 5f;
                        totaltable.AddCell(nest);
                    }

                    pdfDoc.Add(headertable);
                    maintable.PaddingTop = 50f;
                    pdfDoc.Add(maintable);
                    itemtable.PaddingTop = 50f;
                    pdfDoc.Add(itemtable);
                    totaltable.PaddingTop = 50f;
                    pdfDoc.Add(totaltable);

                    PdfContentByte cb = new PdfContentByte(writer);

                    BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, true);
                    cb = new PdfContentByte(writer);
                    cb = writer.DirectContent;
                    cb.BeginText();
                    cb.SetFontAndSize(bf, 8);
                    cb.SetTextMatrix(pageSize.GetLeft(120), 20);
                    cb.ShowText("This is a computer generated copy and it's valid without any signature or seal.");
                    cb.EndText();

                    //Move the pointer and draw line to separate footer section from rest of page
                    cb.MoveTo(40, pdfDoc.PageSize.GetBottom(50));
                    cb.LineTo(pdfDoc.PageSize.Width - 40, pdfDoc.PageSize.GetBottom(50));
                    cb.Stroke();

                    pdfDoc.Close();
                    ms.Close();
                    result = ms.GetBuffer();
                    using (MemoryStream input = new MemoryStream(result))
                    {
                        using (MemoryStream output = new MemoryStream())
                        {
                            PdfReader reader = new PdfReader(input);
                            PdfEncryptor.Encrypt(reader, output, true, null, null, PdfWriter.AllowScreenReaders);
                            result = output.GetBuffer();
                        }
                    }
                }
                return result;
            }
            catch (Exception e)
            {
                ex = e.Message;
                if (string.IsNullOrEmpty(country))
                    country = null;
                clsstring = (!string.IsNullOrEmpty(clsstring) ? ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString : null);
                Error_method(ex, obj);
                return null;
            }
        }

        [Obsolete]
        public void Insert_Data(GeneralObj obj, ContactViewModel contactViewModel, string timestamp)
        {
            string ex;
            try
            {
                db.Insert_Message(contactViewModel.name, contactViewModel.email, contactViewModel.phone, contactViewModel.subject, contactViewModel.message, obj.ip, obj.country, timestamp);
            }
            catch (Exception e)
            {
                ex = e.Message;
                if (string.IsNullOrEmpty(country))
                    country = null;
                Error_method(ex, obj);
            }
        }

        public static ContactViewModel.CaptchaResponse ValidateCaptcha(string response)
        {
            string secret = ConfigurationManager.AppSettings["recaptchaPrivateKey"];
            var client = new WebClient();
            var jsonResult = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secret, response));
            return JsonConvert.DeserializeObject<ContactViewModel.CaptchaResponse>(jsonResult.ToString());
        }

        public void Error_method(string error, GeneralObj obj)
        {
            string timestamp = DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss");
            db.error(error, timestamp, obj.ip, obj.country);
        }
    }
}
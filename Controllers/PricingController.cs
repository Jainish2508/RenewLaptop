using Microsoft.Security.Application;
using Renew_Laptop.Class_Files;
using Renew_Laptop.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Renew_Laptop.Controllers
{
    public class PricingController : Controller
    {
        // GET: Pricing
        private static GeneralObj obj = new GeneralObj();

        private readonly ClsGeneral general = new ClsGeneral();
        private RenewLaptopEntities db = new RenewLaptopEntities();

        [HandleError]
        public ActionResult Index()
        {
            try
            {
                obj.servername = Request.ServerVariables["SERVER_NAME"];
                obj.ip = this.Request.GetIpAddress();
                obj.country = this.Request.GetCountry();
                if (!string.IsNullOrEmpty(obj.ip) && !string.IsNullOrEmpty(obj.country))
                    return View();
                else
                {
                    obj.country = general.IP_Country_From_DB(obj);
                    if (!string.IsNullOrEmpty(obj.country))
                        return View();
                    else
                    {
                        ViewBag.Exception = "Country Empty";
                        return RedirectToActionPermanent("index", "error", obj);
                    }
                }
            }
            catch (Exception e)
            {
                ViewBag.Exception = e.Message;
                return RedirectToActionPermanent("index", "error");
            }
        }

        public ActionResult CheckOut(int? product_id = 1)
        {
            try
            {
                product_id = (product_id == 0) ? 1 : product_id;
                ViewBag.product_info = db.Products.Where(p => p.ProductID == product_id).FirstOrDefault();
                return View();
            }
            catch (Exception e)
            {
                ViewBag.Exception = e.Message;
                return RedirectToActionPermanent("index", "error");
            }
        }

        [HttpPost]
        [Obsolete]
        public async Task<ActionResult> Checkout(Customer customer, int? product_id, string recaptcha)
        {
            string razororder_id;
            try
            {
                product_id = (product_id == 0) ? 1 : product_id;
                ContactViewModel.CaptchaResponse response = ClsGeneral.ValidateCaptcha(Request.Form["g-recaptcha-response"]);
                if (ModelState.IsValid && response.Success)
                {
                    foreach (var c in customer.GetType().GetProperties())
                    {
                        if (c.PropertyType.Name.ToLower() is "string")
                            Sanitizer.GetSafeHtmlFragment(c.ToString());
                    }
                    if (product_id == 2 || product_id == 3)
                    {
                        Product product = db.Products.Where(p => p.ProductID == product_id).FirstOrDefault();
                        razororder_id = await general.Create_Order(product, obj);
                        TempData["OrderID"] = razororder_id;
                        ViewBag.product_info = product;
                    }
                    else
                    {
                        const string src = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890";
                        int l = 15;
                        var sb = new StringBuilder();
                        Random rn = new Random();
                        for (var i = 0; i < l; i++)
                        {
                            var c = src[rn.Next(0, src.Length)];
                            sb.Append(c);
                        }
                        string paymentid = sb.ToString() + DateTime.Now.ToString("ddMMyyyyHHmmss");
                        if (await Task.Run(() => general.new_order(obj, customer, product_id, paymentid, null)))
                            return RedirectToAction("success", "pricing", new { payment_id = paymentid });
                        else
                            return RedirectToAction("failed", "pricing", new { payment_id = paymentid });
                    }
                }
                else
                {
                    ViewBag.Exception = "Model or response is not valid";
                    return RedirectToActionPermanent("index", "error");
                }
                return PartialView("payment", customer);
            }
            catch (Exception ex)
            {
                ViewBag.Exception = ex.Message;
                return RedirectToActionPermanent("index", "error");
            }
        }

        [HttpPost]
        [Obsolete]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Complete(Customer customer, int? product_id)
        {
            try
            {
                string paymentid = Request.Form["rzp_paymentid"];
                string orderid = Request.Form["rzp_orderid"];
                string razorpay_key = ConfigurationManager.AppSettings.Get("RazorPayKey");

                Razorpay.Api.RazorpayClient client = new Razorpay.Api.RazorpayClient("rzp_live_er0PYOxiFK91t3", razorpay_key);

                Razorpay.Api.Payment payment = client.Payment.Fetch(paymentid);

                Dictionary<string, object> options = new Dictionary<string, object>();
                options.Add("amount", payment.Attributes["amount"]);
                Razorpay.Api.Payment paymentcapture = payment.Capture(options);
                string amt = paymentcapture.Attributes["amount"];

                if (paymentcapture.Attributes["status"] == "captured")
                {
                    if (await general.new_order(obj, customer, product_id, paymentid, orderid))
                        return RedirectToAction("success","pricing", new { payment_id = paymentid });
                    else
                        return RedirectToActionPermanent("index", "error");
                }
                else
                    return RedirectToAction("failed");
            }
            catch (Exception ex)
            {
                ViewBag.Exception = ex.Message;
                return RedirectToActionPermanent("index", "error");
            }
        }

        public ActionResult Success(string payment_id)
        {
            if (!string.IsNullOrEmpty(payment_id))
            {
                ViewBag.PaymentId = payment_id;
                return View();
            }
            else
                return RedirectToActionPermanent("index", "pricing");
        }

        public ActionResult Failed()
        {
            return View();
        }
    }
}
using Microsoft.Security.Application;
using Renew_Laptop.Class_Files;
using Renew_Laptop.Models;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Renew_Laptop.Controllers
{
    [HandleError]
    public class ContactController : Controller
    {
        // GET: Contact
        private static GeneralObj obj = new GeneralObj();

        private readonly ClsGeneral general = new ClsGeneral();

        public ActionResult Index()
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
                    return RedirectToActionPermanent("index", "error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Obsolete]
        public async Task<ActionResult> Submit(ContactViewModel contactViewModels, string recaptcha)
        {
            string name, email, subject, message, phone;
            try
            {
                ContactViewModel.CaptchaResponse response = ClsGeneral.ValidateCaptcha(recaptcha);
                if (ModelState.IsValid && response.Success)
                {
                    name = Sanitizer.GetSafeHtmlFragment(contactViewModels.name);
                    email = Sanitizer.GetSafeHtmlFragment(contactViewModels.email);
                    subject = Sanitizer.GetSafeHtmlFragment(contactViewModels.subject);
                    message = Sanitizer.GetSafeHtmlFragment(contactViewModels.message);
                    phone = Sanitizer.GetSafeHtmlFragment(contactViewModels.phone);

                    if (await Task.Run(() => general.send_contact_mail(obj, contactViewModels)))
                        return Json(true);
                    else
                        return Json(false);
                }
                else
                {
                    return Json(false);
                }
            }
            catch (Exception e)
            {
                obj.ex = e.Message;
                Session["GeneralObj"] = obj;
                return RedirectToActionPermanent("index", "error");
            }
        }
    }
}
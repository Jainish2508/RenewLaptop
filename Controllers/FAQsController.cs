using Renew_Laptop.Class_Files;
using System.Web.Mvc;

namespace Renew_Laptop.Controllers
{
    public class FAQsController : Controller
    {
        // GET: FAQs
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
    }
}
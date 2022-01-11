using Renew_Laptop.Class_Files;
using System.Web.Mvc;

namespace Renew_Laptop.Controllers
{
    public class ServicesController : Controller
    {
        // GET: Service
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

        [ActionName("Data-Recovery")]
        public ActionResult DataRecovery()
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

        [ActionName("Computer-Repair")]
        public ActionResult ComputerRepair()
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

        [ActionName("Network-Solutions")]
        public ActionResult NetworkSolutions()
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
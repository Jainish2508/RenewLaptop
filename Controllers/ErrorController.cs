using Renew_Laptop.Class_Files;
using System.Web.Mvc;

namespace Renew_Laptop.Controllers
{
    public class ErrorController : Controller
    {
        private static GeneralObj obj = new GeneralObj();
        private readonly ClsGeneral general = new ClsGeneral();
        // GET: Error

        [HandleError]
        public ActionResult Index(GeneralObj obj)
        {
            obj.servername = Request.ServerVariables["SERVER_NAME"];
            obj.ip = this.Request.GetIpAddress();
            obj.country = this.Request.GetCountry();
            if (!string.IsNullOrEmpty(obj.ip) && !string.IsNullOrEmpty(obj.country))
                return View();
            else
                obj.country = general.IP_Country_From_DB(obj);
            var exception = ControllerContext.RouteData.Values["exception"];
            if (!string.IsNullOrEmpty(ViewBag.Exception))
                exception = ViewBag.Exception;
            else if (string.IsNullOrEmpty(exception.ToString()))
                exception = "No exception caught Status Code: 500";
            general.Error_method(exception.ToString(), obj);
            ViewBag.StatusCode = "500";
            return View();
        }

        public ActionResult E403()
        {
            obj.servername = Request.ServerVariables["SERVER_NAME"];
            obj.ip = this.Request.GetIpAddress();
            obj.country = this.Request.GetCountry();
            if (!string.IsNullOrEmpty(obj.ip) && !string.IsNullOrEmpty(obj.country))
                return View();
            else
                obj.country = general.IP_Country_From_DB(obj);
            var exception = ControllerContext.RouteData.Values["exception"];
            if (!string.IsNullOrEmpty(exception.ToString()))
                exception = "No exception caught Status Code: 403";
            general.Error_method(exception.ToString(), obj);
            ViewBag.StatusCode = "403";
            return View("index");
        }

        public ActionResult E404()
        {
            obj.servername = Request.ServerVariables["SERVER_NAME"];
            obj.ip = this.Request.GetIpAddress();
            obj.country = this.Request.GetCountry();
            if (!string.IsNullOrEmpty(obj.ip) && !string.IsNullOrEmpty(obj.country))
                return View();
            else
                obj.country = general.IP_Country_From_DB(obj);
            var exception = ControllerContext.RouteData.Values["exception"];
            if (!string.IsNullOrEmpty(exception.ToString()))
                exception = "No exception caught Status Code: 404";
            general.Error_method(exception.ToString(), obj);
            ViewBag.StatusCode = "404";
            return View("index");
        }

        public ActionResult E500()
        {
            obj.servername = Request.ServerVariables["SERVER_NAME"];
            obj.ip = this.Request.GetIpAddress();
            obj.country = this.Request.GetCountry();
            if (!string.IsNullOrEmpty(obj.ip) && !string.IsNullOrEmpty(obj.country))
                return View();
            else
                obj.country = general.IP_Country_From_DB(obj);
            var exception = ControllerContext.RouteData.Values["exception"];
            if (!string.IsNullOrEmpty(exception.ToString()))
                exception = "No exception caught Status Code: 500";
            general.Error_method(exception.ToString(), obj);
            ViewBag.StatusCode = "500";
            return View("index");
        }
    }
}
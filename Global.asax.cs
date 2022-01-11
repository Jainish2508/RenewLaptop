using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Rewrite;
using Renew_Laptop.App_Start;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace Renew_Laptop
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            //Remove All View Engine
            ViewEngines.Engines.Clear();
            //Add Razor View Engine
            ViewEngines.Engines.Add(new CustomViewEngine());
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            MvcHandler.DisableMvcResponseHeader = true;
        }

        public void Configure(IApplicationBuilder app)
        {
            var options = new RewriteOptions().AddRedirectToHttpsPermanent();
            app.UseRewriter(options);
        }

        protected void Application_PreSendRequestHeaders()
        {
            Response.Headers.Remove("X-AspNetMvc-Version");
            Response.Headers.Remove("X-AspNet-Version");
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            if (sender is HttpApplication app && app.Context != null)
                app.Context.Response.Headers.Remove("Server");
            if (HttpContext.Current.Request.Url.AbsoluteUri.ToLower().EndsWith("/index"))
            {
                string url = HttpContext.Current.Request.Url.AbsoluteUri.ToLower();
                int lastSlash = url.LastIndexOf('/');
                url = (lastSlash > -1) ? url.Substring(0, lastSlash) : url;
                Response.Redirect(url, false);
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            if (Request.IsSecureConnection == true && HttpContext.Current.Request.Url.Scheme == "https")
            {
                if (Request.Cookies.Count > 0)
                {
                    foreach (string s in Request.Cookies.AllKeys)
                    {
                        Request.Cookies[s].Secure = true;
                        Request.Cookies[s].Expires = DateTime.Now.AddMinutes(30);
                    }
                }
                if (Response.Cookies.Count > 0)
                {
                    foreach (string s in Response.Cookies.AllKeys)
                    {
                        if (s == FormsAuthentication.FormsCookieName || "asp.net_sessionid".Equals(s, StringComparison.InvariantCultureIgnoreCase))
                        {
                            Response.Cookies[s].Secure = true;
                            Response.Cookies[s].Expires = DateTime.Now.AddMinutes(30);
                        }
                    }
                }
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var exception = Server.GetLastError();
            var httpException = exception as HttpException;
            var request = Request;
            Response.Clear();
            Server.ClearError();
            Response.TrySkipIisCustomErrors = true;
            var routeData = new RouteData();
            routeData.Values["controller"] = "Error";
            routeData.Values["action"] = "Index";
            routeData.Values["exception"] = exception;
            Response.StatusCode = 500;

            if (httpException != null)
            {
                Response.StatusCode = httpException.GetHttpCode();

                switch (Response.StatusCode)
                {
                    case 403:
                        routeData.Values["action"] = "E403";
                        break;

                    case 500:
                        routeData.Values["action"] = "E500";
                        break;

                    case 404:
                        routeData.Values["action"] = "E404";
                        break;

                    default:
                        routeData.Values["action"] = "Index";
                        break;
                }
            }
            HttpContext.Current.Response.ContentType = "text/html";

            IController errorsController = new Controllers.ErrorController();
            var rc = new RequestContext(new HttpContextWrapper(Context), routeData);
            errorsController.Execute(rc);
        }
    }
}
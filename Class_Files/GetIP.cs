using System.Web;

namespace Renew_Laptop.Class_Files
{
    public static class GetIP
    {
        public static string GetIpAddress(this HttpRequestBase request)
        {
            if (request.Headers["CF-CONNECTING-IP"] != null)
                return request.Headers["CF-CONNECTING-IP"];

            var ipaddress = request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipaddress))
            {
                var addr = ipaddress.Split('.');
                if (addr.Length > 0)
                    return addr[0];
            }

            return request.UserHostAddress;
        }

        public static string GetCountry(this HttpRequestBase request)
        {
            return request.Headers["CF-IPCOUNTRY"];
        }
    }
}
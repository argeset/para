using System;
using System.Web.Mvc;

using Para.Server.Contract.Argument;

namespace Para.Client.Web.Controllers
{
    public class BaseController : Controller
    {
        protected static void SetDay(string gun, BaseArgument argument)
        {
            argument.Time = DateTime.Today.ToString("yyyyMMdd");

            if (string.IsNullOrWhiteSpace(gun)) return;

            try
            {
                var year = int.Parse(gun.Substring(0, 4));
                var month = int.Parse(gun.Substring(4, 2));
                var day = int.Parse(gun.Substring(6, 2));

                argument.Time = new DateTime(year, month, day).ToString("yyyyMMdd");
            }
            catch { }
        }
    }
}
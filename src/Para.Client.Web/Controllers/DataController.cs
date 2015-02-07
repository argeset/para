using System;
using System.Globalization;
using System.Web.Mvc;

using Para.Server.Contract;
using Para.Server.Contract.Argument;
using Para.Server.Contract.Enum;

namespace Para.Client.Web.Controllers
{
    public class DataController : BaseController
    {
        private readonly IParaService _paraService;

        public DataController(IParaService paraService)
        {
            _paraService = paraService;
        }

        [HttpGet]
        public ViewResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult Deger(string hedef = "USD", string tur = "efektif", string gun = "")
        {
            Currency target;
            if (!Enum.TryParse(hedef, out target)) return Json("hatalı para birimi hedefi > " + hedef, JsonRequestBehavior.AllowGet);

            var argument = new GetValueArgument { Target = target };
            switch (tur)
            {
                case "efektif":
                    argument.Type = CurrencyValueType.Banknote;
                    break;
                case "forex":
                    argument.Type = CurrencyValueType.Forex;
                    break;
                default:
                    return Json("hatalı tür > " + tur, JsonRequestBehavior.AllowGet);
            }

            SetDay(gun, argument);
            var response = _paraService.GetValue(argument);

            return Json(new { response.Value, Message = response.Message.ToString() }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public string SadeceDeger(string hedef = "USD", string tur = "efektif", string gun = "")
        {
            Currency target;
            if (!Enum.TryParse(hedef, out target)) return "hatalı para birimi hedefi > " + hedef;

            var argument = new GetValueArgument { Target = target };
            switch (tur)
            {
                case "efektif":
                    argument.Type = CurrencyValueType.Banknote;
                    break;
                case "forex":
                    argument.Type = CurrencyValueType.Forex;
                    break;
                default:
                    return "hatalı tür > " + tur;
            }

            SetDay(gun, argument);
            var response = _paraService.GetValue(argument);

            return response.Value.ToString(CultureInfo.InvariantCulture);
        }

        [HttpGet]
        public JsonResult Cevrim(decimal? tutar, string hedef = "USD", string tur = "efektif", string gun = "")
        {
            if (tutar == null)
            {
                return Json("lütfen tutar belirtiniz", JsonRequestBehavior.AllowGet);
            }

            Currency target;
            if (!Enum.TryParse(hedef, out target)) return Json("hatalı para birimi hedefi > " + hedef, JsonRequestBehavior.AllowGet);

            var argument = new ConvertValueArgument { Target = target, Amount = tutar };
            switch (tur)
            {
                case "efektif":
                    argument.Type = CurrencyValueType.Banknote;
                    break;
                case "forex":
                    argument.Type = CurrencyValueType.Forex;
                    break;
                default:
                    return Json("hatalı tür > " + tur, JsonRequestBehavior.AllowGet);
            }

            SetDay(gun, argument);

            var response = _paraService.ConvertValue(argument);

            return Json(new { response.Value, Message = response.Message.ToString() }, JsonRequestBehavior.AllowGet);
        }

        private static void SetDay(string gun, BaseArgument argument)
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
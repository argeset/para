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
        public JsonResult Deger(string kaynak = "USD", string hedef = "USD", string tur = "efektif", string gun = "")
        {
            Currency source;
            if (!Enum.TryParse(kaynak, out source)) return Json("hatalı para birimi hedefi > " + hedef, JsonRequestBehavior.AllowGet);

            Currency target;
            if (!Enum.TryParse(hedef, out target)) return Json("hatalı para birimi hedefi > " + hedef, JsonRequestBehavior.AllowGet);

            var argument = new GetValueArgument { Source = source, Target = target, };
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

            return Json(new { response.Value, Message = response.Message.ToString(), Date = argument.Time }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public string SadeceDeger(string kaynak = "USD", string hedef = "USD", string tur = "efektif", string gun = "")
        {

            Currency source;
            if (!Enum.TryParse(kaynak, out source)) return "hatalı para birimi hedefi > " + kaynak;

            Currency target;
            if (!Enum.TryParse(hedef, out target)) return "hatalı para birimi hedefi > " + hedef;

            var argument = new GetValueArgument { Source = source, Target = target, };
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
        public JsonResult Cevrim(decimal? tutar, string kaynak = "USD", string hedef = "USD", string tur = "efektif", string gun = "")
        {
            if (tutar == null)
            {
                return Json("lütfen tutar belirtiniz", JsonRequestBehavior.AllowGet);
            }

            Currency source;
            if (!Enum.TryParse(kaynak, out source)) return Json("hatalı para birimi hedefi > " + hedef, JsonRequestBehavior.AllowGet);

            Currency target;
            if (!Enum.TryParse(hedef, out target)) return Json("hatalı para birimi hedefi > " + hedef, JsonRequestBehavior.AllowGet);

            var argument = new ConvertValueArgument { Source = source, Target = target, Amount = tutar };
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

            return Json(new { response.Value, Message = response.Message.ToString(), Date = argument.Time }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ExchangeRate(string day)
        {
            if (string.IsNullOrEmpty(day)) day = DateTime.Today.AddDays(-1).ToString("yyyyMMdd");

            var response = _paraService.GetExchangeRate(day);

            return Json(response, JsonRequestBehavior.AllowGet);
        }
    }
}
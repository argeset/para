using System;

using Para.Server.Contract.Enum;

namespace Para.Server.Contract.Argument
{
    public class BaseArgument
    {
        public CurrencyValueSource ValueSource { get; set; }
        public CurrencyValueType Type { get; set; }
        public Currency Source { get; set; }
        public Currency Target { get; set; }
        public string Time { get; set; }

        public static string GetTodayKey()
        {
            return DateTime.Today.ToString("yyyyMMdd");
        }
    }
}
using System.Collections.Generic;
using Para.Server.Contract.Argument;
using Para.Server.Contract.Enum;
using Para.Server.Contract.Response;

namespace Para.Server.Business.Strategy
{
    public abstract class BaseStrategy
    {
        public static Dictionary<CurrencyValueSource, BaseStrategy> Strategies = new Dictionary<CurrencyValueSource, BaseStrategy> { { CurrencyValueSource.TCMB, new TCMBStrategy() } };

        public abstract Response GetValue(string day, Currency source, Currency target, CurrencyValueType type);
        public abstract Response ConvertValue(string day, Currency source, Currency target, CurrencyValueType type, decimal? amount);
        public abstract void SaveValue();
        public abstract List<ExchangeRate> GetValuesDbWork(string day);
    }
}
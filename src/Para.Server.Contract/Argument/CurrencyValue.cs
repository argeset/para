using Para.Server.Contract.Enum;

namespace Para.Server.Contract.Argument
{
    public class CurrencyValue
    {
        public string Day { get; set; }
        public Currency Source { get; set; }
        public Currency Target { get; set; }
        public CurrencyValueSource ValueSoruce { get; set; }
        public CurrencyValueType ValueType { get; set; }
        public decimal Value { get; set; }
    }
}
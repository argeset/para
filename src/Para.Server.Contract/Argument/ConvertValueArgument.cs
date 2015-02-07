using Para.Server.Contract.Enum;

namespace Para.Server.Contract.Argument
{
    public class ConvertValueArgument : BaseArgument
    {
        public ConvertValueArgument()
        {
            ValueSource = CurrencyValueSource.TCMB;
            Type = CurrencyValueType.Banknote;
            Source = Currency.TL;
            Target = Currency.USD;
            Time = GetTodayKey();
        }

        public decimal? Amount { get; set; }
    }
}
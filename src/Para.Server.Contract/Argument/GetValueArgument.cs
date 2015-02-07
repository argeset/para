using Para.Server.Contract.Enum;

namespace Para.Server.Contract.Argument
{
    public class GetValueArgument : BaseArgument
    {
        public GetValueArgument()
        {
            ValueSource = CurrencyValueSource.TCMB;
            Type = CurrencyValueType.Banknote;
            Source = Currency.TL;
            Target = Currency.USD;
            Time = GetTodayKey();
        }
    }
}

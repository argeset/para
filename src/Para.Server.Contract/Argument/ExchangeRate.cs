namespace Para.Server.Contract.Argument
{
    public class ExchangeRate
    {
        public string CurrencyCode { get; set; }
        public string Unit { get; set; }
        public string Currency { get; set; }
        public string BankNoteBuying { get; set; }
        public string BankNoteSelling { get; set; }
        public string ForexBuying { get; set; }
        public string ForexSelling { get; set; }
        public string Source { get; set; }
        public string Target { get; set; }
        public string CurrencyDate { get; set; }
    }
}

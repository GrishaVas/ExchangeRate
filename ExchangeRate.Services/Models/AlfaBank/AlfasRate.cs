namespace ExchangeRate.Services.Models.AlfaBank
{
    public class AlfasRate
    {
        public decimal sellRate { get; set; }
        public string sellIso { get; set; }
        public decimal buyRate { get; set; }
        public string buyIso { get; set; }
        public string date { get; set; }
    }
}

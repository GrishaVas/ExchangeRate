using System.Text.Json;
using ExchangeRate.Services.Abstractions;
using ExchangeRate.Services.Models;
using ExchangeRate.Services.Models.AlfaBank;
using Microsoft.Extensions.Configuration;

namespace ExchangeRate.Services.Implemintations
{
    public class AlfaBankService : BankService, IBankService
    {
        public string Name { get => "AlfaBank"; }
        private readonly IConfiguration _conf;
        private readonly string _alfaBankUrl;

        public AlfaBankService(IConfiguration conf)
        {
            _conf = conf;
            _alfaBankUrl = _conf["AlfaBank:Url"];
        }

        public async Task<List<string>> GetCurrencies()
        {
            var alfasRates = await getRates();

            return alfasRates.Select(r => r.sellIso).ToList();
        }

        public async Task<Rate> GetCurrencyRateForDate(string currencyName, DateTime date)
        {
            var aRates = await getRates();
            var alfasRate = aRates.FirstOrDefault(r => r.sellIso.ToLower() == currencyName.ToLower() &&
                DateTime.Parse(r.date) == date);

            if (alfasRate == null)
            {
                return null;
            }

            var rate = new Rate
            {
                Buy = alfasRate.buyRate,
                Sell = alfasRate.sellRate,
                Date = date
            };

            return rate;
        }

        public async Task<List<Rate>> GetCurrencyRates(string currencyName, DateTime dateFrom, DateTime dateTo)
        {
            var aRates = await getRates();
            var alfasRates = aRates.Where(r => r.sellIso.ToLower() == currencyName.ToLower() &&
                DateTime.Parse(r.date) >= dateFrom &&
                DateTime.Parse(r.date) <= dateTo);

            var rates = alfasRates.Select(r => new Rate
            {
                Buy = r.buyRate,
                Sell = r.sellRate,
                Date = DateTime.Parse(r.date)
            }).ToList();

            return rates;
        }

        private async Task<List<AlfasRate>> getRates()
        {
            var response = await Get(_alfaBankUrl);

            return JsonSerializer.Deserialize<AlfasRates>(response).rates.Where(r => r.buyIso == "BYN").ToList();
        }
    }
}

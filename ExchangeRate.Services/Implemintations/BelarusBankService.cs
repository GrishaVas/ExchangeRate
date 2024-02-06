using System.Text.Json;
using ExchangeRate.Services.Abstractions;
using ExchangeRate.Services.Models;
using ExchangeRate.Services.Models.BelarusBank;
using Microsoft.Extensions.Configuration;

namespace ExchangeRate.Services.Implemintations
{
    public class BelarusBankService : BankService, IBankService
    {
        public string Name { get => "BelarusBank"; }
        private readonly IConfiguration _conf;
        private readonly string _belarusBankUrl;

        public BelarusBankService(IConfiguration conf)
        {
            _conf = conf;
            _belarusBankUrl = _conf["BelarusBank:Url"];
        }

        public async Task<List<string>> GetCurrencies()
        {
            var propertiesNames = typeof(BelarusRate).GetProperties().Select(p => p.Name);
            var currenciesNames = propertiesNames.Where(pn => pn.Contains("CARD_in")).Select(pn => pn.Substring(0, 3)).ToList();

            return currenciesNames;
        }

        public async Task<Rate> GetCurrencyRateForDate(string currencyName, DateTime date)
        {
            var bRates = await getRates();
            var belarusRate = bRates.FirstOrDefault(r => DateTime.Parse(r.kurs_date_time) == date);
            var currency = getCurrency(currencyName, belarusRate);

            if (belarusRate == null || currency.@in == null && currency.@out == null)
            {
                return null;
            }

            var rate = new Rate
            {
                Buy = currency.@in,
                Sell = currency.@out,
                Date = date
            };

            return rate;
        }

        public async Task<List<Rate>> GetCurrencyRates(string currencyName, DateTime dateFrom, DateTime dateTo)
        {
            var bRates = await getRates();
            var belarusRates = bRates.Where(r => DateTime.Parse(r.kurs_date_time) >= dateFrom &&
                DateTime.Parse(r.kurs_date_time) <= dateTo);

            var rates = belarusRates.Select(r => new Rate
            {
                Buy = getCurrency(currencyName, r).@in,
                Sell = getCurrency(currencyName, r).@out,
                Date = DateTime.Parse(r.kurs_date_time)
            }).ToList();

            return rates;
        }

        private (decimal? @in, decimal? @out) getCurrency(string currencyName, BelarusRate rate)
        {
            var @in = rate.GetType().GetProperty(currencyName.ToUpper() + "CARD_in")?.GetValue(rate);
            var @out = rate.GetType().GetProperty(currencyName.ToUpper() + "CARD_out")?.GetValue(rate);

            return ((decimal)@in, (decimal)@out);
        }

        private async Task<List<BelarusRate>> getRates()
        {
            var response = await Get(_belarusBankUrl);

            return JsonSerializer.Deserialize<List<BelarusRate>>(response);
        }
    }
}

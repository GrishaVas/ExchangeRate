using System.Text.Json;
using ExchangeRate.Services.Abstractions;
using ExchangeRate.Services.Models;
using ExchangeRate.Services.Models.NationalBank;
using Microsoft.Extensions.Configuration;

namespace ExchangeRate.Services.Implemintations
{
    public class NationalBankService : BankService, IBankService
    {
        public string Name { get => "NationalBank"; }
        private readonly IConfiguration _conf;
        private readonly string _currenciesUrl;
        private readonly string _ratesUrl;
        private readonly string _dynamicsUrl;

        public NationalBankService(IConfiguration conf)
        {
            _conf = conf;
            _currenciesUrl = _conf["NationalBank:CurrenciesUrl"];
            _ratesUrl = _conf["NationalBank:RatesUrl"];
            _dynamicsUrl = _conf["NationalBank:DynamicsUrl"];
        }

        public async Task<List<string>> GetCurrencies()
        {
            var response = await Get(_currenciesUrl);

            return JsonSerializer.Deserialize<List<NationalCurrency>>(response).Select(c => c.Cur_Abbreviation).ToList();
        }

        public async Task<Rate> GetCurrencyRateForDate(string currencyName, DateTime date)
        {
            var currency = await getCurrency(currencyName);

            if (currency == null)
            {
                return null;
            }

            var response = await Get($"{_ratesUrl}{currency.Cur_ID}?ondate={date}");

            var nRate = JsonSerializer.Deserialize<NationalRate>(response);
            var rate = new Rate
            {
                Buy = nRate.Cur_OfficialRate,
                Sell = null,
                Date = nRate.Date != null ? DateTime.Parse(nRate.Date) : null
            };

            return rate;
        }

        public async Task<List<Rate>> GetCurrencyRates(string currencyName, DateTime dateFrom, DateTime dateTo)
        {
            var currency = await getCurrency(currencyName);

            if (currency == null)
            {
                return null;
            }

            var response = await Get($"{_dynamicsUrl}{currency.Cur_ID}?endDate={dateTo}&startDate={dateFrom}");
            var nationalRates = JsonSerializer.Deserialize<List<NationalRate>>(response);

            var rates = nationalRates.Select(r => new Rate
            {
                Buy = r.Cur_OfficialRate,
                Sell = null,
                Date = DateTime.Parse(r.Date)
            }).ToList();

            return rates;
        }

        private async Task<NationalCurrency?> getCurrency(string currencyName)
        {
            var response = await Get(_currenciesUrl);

            return JsonSerializer.Deserialize<List<NationalCurrency>>(response)
                .FirstOrDefault(c => c.Cur_Abbreviation.ToLower() == currencyName.ToLower() &&
                    DateTime.Parse(c.Cur_DateEnd) >= DateTime.Now);
        }
    }
}

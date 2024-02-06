using ExchangeRate.Services.Models;

namespace ExchangeRate.Services.Abstractions
{
    public interface IBankService
    {
        public string Name { get; }
        public Task<List<string>> GetCurrencies();
        public Task<Rate> GetCurrencyRateForDate(string currencyName, DateTime date);
        public Task<List<Rate>> GetCurrencyRates(string currencyName, DateTime dateFrom, DateTime dateTo);
    }
}

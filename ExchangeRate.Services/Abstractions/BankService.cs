namespace ExchangeRate.Services.Abstractions
{
    public abstract class BankService
    {
        protected virtual async Task<string> Get(string uri)
        {
            var client = new HttpClient();
            var responseMessage = await client.GetAsync(uri);
            var result = await responseMessage.Content.ReadAsStringAsync();

            return result;
        }
    }
}

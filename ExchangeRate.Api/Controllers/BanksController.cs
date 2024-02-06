using ExchangeRate.Services.Abstractions;
using ExchangeRate.Services.Implemintations;
using ExchangeRate.Services.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExchangeRate.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BanksController : ControllerBase
    {
        private readonly List<IBankService> _banksServices;
        public BanksController(AlfaBankService alfaBankService,
            BelarusBankService belarusBankService,
            NationalBankService nationalBankService)
        {
            _banksServices = new List<IBankService>()
            {
                alfaBankService,
                belarusBankService,
                nationalBankService
            };
        }

        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<string>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Get()
        {
            return Ok(_banksServices.Select(s => s.Name).ToList());
        }

        [HttpGet("{bankName}/currencies")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<string>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(string bankName)
        {
            var service = _banksServices.FirstOrDefault(s => s.Name.ToLower() == bankName.ToLower());

            if (service == null)
            {
                return NotFound();
            }

            var currencies = await service.GetCurrencies();

            return Ok(currencies);
        }

        [HttpGet("{bankName}/currencies/{currencyName}/rate")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Rate))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(string bankName, string currencyName, [FromQuery] DateTime date)
        {
            var service = _banksServices.FirstOrDefault(s => s.Name.ToLower() == bankName.ToLower());

            if (service == null)
            {
                return NotFound();
            }

            var rate = await service.GetCurrencyRateForDate(currencyName, date);

            return Ok(rate);
        }

        [HttpGet("{bankName}/currencies/{currencyName}/rates")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Rate>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(string bankName, string currencyName, [FromQuery] DateTime dateFrom, [FromQuery] DateTime dateTo)
        {
            if (dateFrom > dateTo)
            {
                return BadRequest();
            }

            var service = _banksServices.FirstOrDefault(s => s.Name.ToLower() == bankName.ToLower());

            if (service == null)
            {
                return NotFound();
            }

            var rates = await service.GetCurrencyRates(currencyName, dateFrom, dateTo);

            if (rates == null)
            {
                return NotFound();
            }

            return Ok(rates);
        }
    }
}

using Application.Interfaces.Services;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Synaplic.UniRH.Shared.Wrapper;

namespace MudBlazorApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IataCodeController : ControllerBase
    {
        private readonly IIataCodeService _iataCodeService; 

        public IataCodeController (IIataCodeService iataCodeService)
        {
            _iataCodeService = iataCodeService;
        }

        [HttpGet(nameof(GetCityCode))]
        public async Task<Result<string>> GetCityCode(string _cityName)
        {
            await _iataCodeService.ConnectOAuth();
            return await _iataCodeService.GetCityCode(_cityName);
        }


        [HttpPost(nameof(GetToken))]
        public async Task<Result<string>> GetToken()
        {
            return await _iataCodeService.ConnectOAuth();
        }

    }
}

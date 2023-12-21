using Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MudBlazorApp.Shared.Response;
using Synaplic.UniRH.Shared.Wrapper;

namespace MudBlazorApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TravelApiController : ControllerBase
    {
        private readonly ITravelApiService _travelApiService;

        public TravelApiController(ITravelApiService travelApiService) {

            _travelApiService = travelApiService; 
        }



        [HttpPost(nameof(GetTravels))]
       public async Task<Result<TravelSearchResponse>> GetTravels(TravelRequest _travelRequest)
        {
             await _travelApiService.ConnectOAuth();
            return await _travelApiService.GetTravels(_travelRequest);
        }

        [HttpPost(nameof(GetToken))] 
        public async Task<Result<string>> GetToken()
        {
            return await _travelApiService.ConnectOAuth();
        }

      
    }
}

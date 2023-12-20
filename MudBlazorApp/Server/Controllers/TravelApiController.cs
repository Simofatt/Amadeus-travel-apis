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



        [HttpGet]
       public async Task<Result<TravelSearchResponse>> GetTravels(string origin, string DateAller, string DateRetour, string destination, int adults, int childreen, bool nonStop, string travelClass)
        {
             await _travelApiService.ConnectOAuth();
            return await _travelApiService.GetTravels(origin,DateAller, DateRetour, destination, adults, childreen, nonStop, travelClass);
        }

        [HttpPost] 
        public async Task<Result<string>> GetToken()
        {
            return await _travelApiService.ConnectOAuth();
        }

      
    }
}

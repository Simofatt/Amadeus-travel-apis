using MudBlazorApp.Shared.Response;
using Synaplic.UniRH.Shared.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
     public interface  ITravelApiService
    {
        Task<Result<string>> ConnectOAuth();
        Task<Result<TravelSearchResponse>> GetTravels(TravelRequest _travelRequest);
        void ConfigBearerTokenHeader();
    }
}

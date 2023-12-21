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
        Task<Result<TravelSearchResponse>> GetTravels(string origin, string DateAller, string? DateRetour, string destination, int adults, int childreen, string nonStop, string travelClass);
        void ConfigBearerTokenHeader();
    }
}

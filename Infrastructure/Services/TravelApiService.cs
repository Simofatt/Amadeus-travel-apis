using Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using MudBlazorApp.Shared.Response;
using Synaplic.UniRH.Shared.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace Infrastructure.Services
{
    public class TravelApiService : ITravelApiService
    {
        private string apiKey;
        private string apiSecret;
        private string bearerToken;
        private HttpClient http;

        public TravelApiService(IConfiguration config, IHttpClientFactory httpFactory)
        {
            apiKey = config["AmadeusAPI:APIKey"];
            apiSecret = config["AmadeusAPI:APISecret"];
            http = httpFactory.CreateClient("TravelApiService");

        }
        public async Task<Result<string>> ConnectOAuth()
        {
           
            var message = new HttpRequestMessage(HttpMethod.Post, "/v1/security/oauth2/token");
            message.Content = new StringContent(
                $"grant_type=client_credentials&client_id={apiKey}&client_secret={apiSecret}",
                Encoding.UTF8, "application/x-www-form-urlencoded"
            );
           

            var results = await http.SendAsync(message);
            await using var stream = await results.Content.ReadAsStreamAsync();
            var oauthResults = await JsonSerializer.DeserializeAsync<OAuthResults>(stream);

            bearerToken = oauthResults.access_token;
            return await Result<string>.SuccessAsync(bearerToken);
        }


       public async Task<Result<TravelSearchResponse>> GetTravels(string origin, string DateAller, string DateRetour, string destination, int adults, int childreen, bool nonStop, string travelClass)
       {
        var message = new HttpRequestMessage(HttpMethod.Get,$"/test.api.amadeus.com/v2/shopping/flight-offers?originLocationCode={origin}&destinationLocationCode={destination}&departureDate={DateAller}&returnDate={DateRetour}&adults={adults}&childreen={childreen}&nonStop={nonStop}&max=250&currencyCode=MAD&travelClass={travelClass}");
        ConfigBearerTokenHeader();

        var response = await http.SendAsync(message);

        if (response.IsSuccessStatusCode)
        {
                  var responseBody = await response.Content.ReadAsStringAsync();
             
            try{
                var flightOffers = Newtonsoft.Json.JsonConvert.DeserializeObject<TravelSearchResponse>(responseBody);
                return await Result<TravelSearchResponse>.SuccessAsync(flightOffers);
            }
            catch (JsonException ex)
            {
                return await Result<TravelSearchResponse>.FailAsync("Error deserializing JSON");
            }
        }
        else
        {
            return await Result<TravelSearchResponse>.FailAsync("Error fetching the data");
        }
    }


       private void ConfigBearerTokenHeader()
        {
            http.DefaultRequestHeaders.Add("Authorization", $"Bearer {bearerToken}");
        }

        private class OAuthResults
        {
            public string access_token { get; set; }
        }

    }
}

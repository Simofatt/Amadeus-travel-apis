using Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazorApp.Shared.Requests;
using MudBlazorApp.Shared.Response;
using Synaplic.UniRH.Shared.Wrapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        private string _apiKey;
        private string _apiSecret;
        private string _bearerToken;
        private HttpClient http;
        private string _preparedUrl; 

        public TravelApiService(IConfiguration config, IHttpClientFactory httpFactory)
        {
            _apiKey = config["AmadeusAPI:APIKey"];
            _apiSecret = config["AmadeusAPI:APISecret"];
            http = httpFactory.CreateClient("TravelApiService");
        }



        public async Task<Result<string>> ConnectOAuth()
        {
            var message = new HttpRequestMessage(HttpMethod.Post, "/v1/security/oauth2/token");
            message.Content = new StringContent(
                $"grant_type=client_credentials&client_id={_apiKey}&client_secret={_apiSecret}",
                Encoding.UTF8, "application/x-www-form-urlencoded"
            );
           
            var results = await http.SendAsync(message);
        

            var response = await results.Content.ReadAsStringAsync();
            var oauthResults =  Newtonsoft.Json.JsonConvert.DeserializeObject<OAuthResults>(response);

            _bearerToken = oauthResults.access_token;
               
           return await Result<string>.SuccessAsync(_bearerToken);
        }




       public async Task<Result<TravelSearchResponse>> GetTravels(TravelRequest _travelRequest)
       {
           // var dateA = _travelRequest.DateAller.HasValue ? _travelRequest.DateAller.Value.Date : (DateTime?)null;
           // var dateR = _travelRequest.DateRetour.HasValue? _travelRequest.DateRetour.Value.Date : (DateTime?)null;

            string dateA = (_travelRequest.DateAller.Value.Year + "-" + _travelRequest.DateAller.Value.Month + "-" + _travelRequest.DateAller.Value.Day).ToString();
            var NonStop = _travelRequest.NonStop.ToString().ToLower();
            if (_travelRequest.DateRetour.HasValue) _preparedUrl = $"/v2/shopping/flight-offers?originLocationCode={_travelRequest.Origin}&destinationLocationCode={_travelRequest.Destination}&departureDate={dateA.ToString()}&adults={_travelRequest.Adults}&children={_travelRequest.Childreen}&travelClass={_travelRequest.TravelClass}&nonStop={NonStop}&max=250";


            else
            {
                string dateR = (_travelRequest.DateRetour.Value.Year + "-" + _travelRequest.DateRetour.Value.Month + "-" + _travelRequest.DateRetour.Value.Day).ToString();
                _preparedUrl = $"/v2/shopping/flight-offers?originLocationCode={_travelRequest.Origin}&destinationLocationCode={_travelRequest.Destination}&departureDate={dateA.ToString()}&returnDate={dateR.ToString()}&adults={_travelRequest.Adults}&children={_travelRequest.Childreen}&travelClass={_travelRequest.TravelClass}&nonStop={NonStop}&max=250";
            }

            // var message = new HttpRequestMessage(HttpMethod.Get, "/v2/shopping/flight-offers?originLocationCode=MAD&destinationLocationCode=BCN&departureDate=2023-12-22&returnDate=2023-12-24&adults=1&children=1&travelClass=ECONOMY&nonStop=false&max=250");
            var message = new HttpRequestMessage(HttpMethod.Get, _preparedUrl);
            ConfigBearerTokenHeader();

            var response = await http.SendAsync(message);
           

            if (response.IsSuccessStatusCode)
            {
                 var responseBody = await response.Content.ReadAsStringAsync(); 
                
                    try 
                    {
                        var flightOffers = Newtonsoft.Json.JsonConvert.DeserializeObject<TravelSearchResponse>(responseBody);
                        return await Result<TravelSearchResponse>.SuccessAsync(flightOffers);
                    }
                    catch (JsonException ex)
                    {
                        Console.WriteLine(ex.Message);
                        return await Result<TravelSearchResponse>.FailAsync("Error deserializing JSON");
                    }
            }
            else
            {
                return await Result<TravelSearchResponse>.FailAsync(response.ReasonPhrase);
            }
    }


       public void ConfigBearerTokenHeader()
       {
            http.DefaultRequestHeaders.Add("Authorization", $"Bearer {_bearerToken}");
       }

      

    }
}

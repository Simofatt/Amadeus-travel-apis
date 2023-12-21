using Application.Interfaces.Services;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using MudBlazorApp.Shared.Requests;
using MudBlazorApp.Shared.Response;
using Synaplic.UniRH.Shared.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class IataCodeService : IIataCodeService
    {

        private string _apiKey;
        private string _apiSecret;
        private string _bearerToken;
        private HttpClient http;
        private string _preparedUrl;

        public IataCodeService(IConfiguration config, IHttpClientFactory httpFactory)
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
            var oauthResults = Newtonsoft.Json.JsonConvert.DeserializeObject<OAuthResults>(response);

            _bearerToken = oauthResults.access_token;

            return await Result<string>.SuccessAsync(_bearerToken);
        }


        public async Task<Result<string>> GetCityCode(string _city)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    _preparedUrl = $"/v1/reference-data/locations/cities?keyword={_city}&max=1";
                    var message = new HttpRequestMessage(HttpMethod.Get, _preparedUrl);
                    ConfigBearerTokenHeader();

                    var response = await http.SendAsync(message);


                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        if (!String.IsNullOrEmpty(responseBody))
                        {
                            var _iataCode = Newtonsoft.Json.JsonConvert.DeserializeObject<IataCodeResponse>(responseBody);
                            return await Result<string>.SuccessAsync(_iataCode.Data[0].iataCode);
                        }
                        return await Result<string>.FailAsync("The response is null ");
                    }
                    else
                    {
                        Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                        return await Result<string>.FailAsync("Fail To get the IATA CODE ");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    return await Result<string>.FailAsync("Fail To get the IATA CODE ");
                }
            }
        }

        public void ConfigBearerTokenHeader()
        {
            http.DefaultRequestHeaders.Add("Authorization", $"Bearer {_bearerToken}");
        }
    }
}

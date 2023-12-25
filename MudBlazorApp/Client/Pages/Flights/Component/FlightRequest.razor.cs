using Blazored.FluentValidation;
using H.Core;
using MudBlazor;
using MudBlazorApp.Shared.Constant;
using MudBlazorApp.Shared.Response;
using Newtonsoft.Json;
using static MudBlazor.Colors;
using static MudBlazorApp.Client.Pages.Flights.Component.FlightRequest;


namespace MudBlazorApp.Client.Pages.Flights.Component
{
    public partial class FlightRequest
    {
        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        private List<Country> _countries = new List<Country>();
        private List<string> _citiesArrival = new();
        private List<string> _citiesDeparture = new();
        private bool _loaded;
        private List<string> _travelClass = new();
        private TravelRequest _travelRequest = new();
        private List<FlightOffer> _travelList = new();
        private List<Itinerary> _itineraries = new();
        private List<Segment> _segmentList = new();
        private List<TravelerPricing> _travelPricings = new();
        private bool _search  =true ;




        protected override async Task OnInitializedAsync()
        {
            await GetCountriesAsync();
            GetTravelClassesAsync();
            _travelRequest.OneWay = true;
            _loaded = true;
        }

        private async Task<IEnumerable<string>> SearchCountry(string country)
        {
            await Task.Delay(1);

            if (string.IsNullOrEmpty(country))
                return _countries.Select(country => country.Name);

            return _countries
                .Where(x => x.Name.Contains(country, StringComparison.InvariantCultureIgnoreCase))
                .Select(country => country.Name);
        }

        private async Task<IEnumerable<string>> SearchDepartureCity(string city)
        {
             
            await Task.Delay(1);
            if (string.IsNullOrEmpty(city))
                return _citiesDeparture.Select(city => city);

            return _citiesDeparture
                .Where(x => x.Contains(city, StringComparison.InvariantCultureIgnoreCase))
                .Select(country => country);

        }
        private async Task<IEnumerable<string>> SearchArrivalCity(string city)
        {
             
            await Task.Delay(1);
            if (string.IsNullOrEmpty(city))
                return _citiesArrival.Select(city => city);

            return _citiesArrival
                .Where(x => x.Contains(city, StringComparison.InvariantCultureIgnoreCase))
                .Select(country => country);

        }

        public async Task SubmitAsync()
        {
            _search = false;
            var result = await GetIataCityCodeAsync();
            

            if (result)
            {
                if (_travelRequest.Childreen is null)
                {
                    _travelRequest.Childreen = 0;
                }
               var success=  await GetFlightsAsync();
                if (success)
                {
                    _search = true;
                    await InvokeModal();
                }
            }

        }

       

        private async Task InvokeModal()
        {
            var parameters = new DialogParameters();
            parameters.Add("_travelList", _travelList);
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Large, FullWidth = true, DisableBackdropClick = false };
            var dialog = _dialogService.Show<FlightList>(_l["Search for a flight"], parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
               
            }

        }

      

        private async Task<bool> GetFlightsAsync()
        {
            var response = await _iTravelApiClient.GetTravelsAsync(_travelRequest);
            if (response.Succeeded)
            {
                _travelList = response.Data.Data;

               foreach (var item in _travelList)
                {
                    foreach (var itinerarie in item.Itineraries)
                    {
                        _itineraries.Add(itinerarie);

                        foreach (var segment in itinerarie.Segments)
                        {
                            _segmentList.Add(segment);
                        }
                    }
                    foreach (var pricing in item.TravelerPricings)
                    {
                        _travelPricings.Add(pricing);
                    }
                }

                return true;

            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackbar.Add(message, Severity.Error);
                }
                return false;
            }
        }

        public async Task<bool> GetIataCityCodeAsync()
        {
            var result = await _iataCodeClient.GetCityCodeAsync(_travelRequest.Origin);
            var result2 = await _iataCodeClient.GetCityCodeAsync(_travelRequest.Destination);
            if (result.Succeeded && result2.Succeeded)
            {
                _travelRequest.Origin = result.Messages[0];
                _travelRequest.Destination = result2.Messages[0];
                return true;

            }
            else
            {
                foreach (var item in result.Messages)
                {
                    _snackbar.Add(item, Severity.Error);
                }

                foreach (var item in result2.Messages)
                {
                    _snackbar.Add(item, Severity.Error);
                }
                return false;
            }
            return false;

        }

        public async Task GetCountriesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(_navigationManager.BaseUri + "/data/Countries-Cities.json");
                var json = await response.Content.ReadAsStringAsync();
                Dictionary<string, List<string>> dict = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(json);


                foreach (var item in dict)
                {
                    Country country = new Country();
                    country.Name = item.Key;
                    country.Cities = new List<string>();
                    foreach (var cityName in item.Value)
                    {
                        country.Cities.Add(cityName);
                    }
                    _countries.Add(country);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void GetTravelClassesAsync()
        {
            _travelClass.Add(TravelClassConstants.Economy);
            _travelClass.Add(TravelClassConstants.PremiumEconomy);
            _travelClass.Add(TravelClassConstants.Business);
            _travelClass.Add(TravelClassConstants.First);
        }


        private async Task SelectedOriginCountryChanged(string country)
        {
            _travelRequest.OriginCountry = country;
            _citiesDeparture= _countries.FirstOrDefault(x => x.Name.Equals(country)).Cities;
            await InvokeAsync(() => StateHasChanged());


        }
        private async Task SelectedDestinationCountryChanged(string country)
        {
            _travelRequest.DestinationCountry = country;
            _citiesArrival = _countries.FirstOrDefault(x => x.Name.Equals(country)).Cities;
            await InvokeAsync(() => StateHasChanged());

        }

        public class Country
        {
            public string Name { get; set; }
            public List<string> Cities { get; set; }
        }

    }
}

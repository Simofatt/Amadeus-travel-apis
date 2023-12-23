﻿using Blazored.FluentValidation;
using MudBlazor;
using MudBlazorApp.Shared.Constant;
using MudBlazorApp.Shared.Response;
using Newtonsoft.Json;


namespace MudBlazorApp.Client.Pages.Flights.Component
{
    public partial class FlightRequest
    {
        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        private List<Country> _countries = new List<Country>();
        private List<string> _cities = new();
        private bool _loaded;
        private List<string> _travelClass = new();
        private TravelRequest _travelRequest = new();
        private List<FlightOffer> _travelList = new();
        private List<Itinerary> _itineraries = new();
        private List<Segment> _segmentList = new();
        private List<TravelerPricing> _travelPricings = new();



        protected override async Task OnInitializedAsync()
        {
            await GetCountriesAsync();
            GetTravelClassesAsync();
            _loaded = true;
        }
     

        public async Task SubmitAsync()
        {
            var result = await GetIataCityCodeAsync();

            if (result)
            {
                
               var success=  await GetFlightsAsync();
                if (success)
                {
                    await InvokeModal();
                }
            }

        }

       

        private async Task InvokeModal()
        {
            var parameters = new DialogParameters();
            parameters.Add("_travelList", _travelList);
            parameters.Add("_itineraries", _itineraries);
            parameters.Add("_segmentList", _segmentList);
            parameters.Add("_travelPricings", _travelPricings);
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Large, FullWidth = true, DisableBackdropClick = true };
            var dialog = _dialogService.Show<SearchFlight>(_l["Search for a flight"], parameters, options);
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
            _cities = _countries.FirstOrDefault(x => x.Name.Equals(country)).Cities;
            await InvokeAsync(() => StateHasChanged());


        }
        private async Task SelectedDestinationCountryChanged(string country)
        {
            _travelRequest.DestinationCountry = country;
            _cities = _countries.FirstOrDefault(x => x.Name.Equals(country)).Cities;
            await InvokeAsync(() => StateHasChanged());

        }

        public class Country
        {
            public string Name { get; set; }
            public List<string> Cities { get; set; }
        }

    }
}

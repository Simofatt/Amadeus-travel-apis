
using MudBlazor;
using MudBlazorApp.Shared.Response;
using MudBlazorApp.Shared.Constant;
using Newtonsoft.Json;
using Blazored.FluentValidation;




namespace MudBlazorApp.Client.Pages.Flights
{
    public partial class SearchFlight
    {

        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        private List<FlightOffer> _travelList = new();
        private List<Segment> _segmentList = new();
        private List<TravelerPricing> _travelPricings = new();
        private List<Itinerary> _itineraries = new();
        private FlightOffer flightOfferRequest = new();
        private TravelRequest _travelRequest = new();
        private List<string> _travelClass = new();
        private List<Country> _countries = new List<Country>();
        private List<string> _cities = new();
        private string _searchString = "";
        private bool _dense = true;
        private bool _striped = true;
        private bool _bordered = false;



        private bool _loaded;


        protected override async Task OnInitializedAsync()
        {

            //await GetFlightsAsync();
            await GetTravelClassesAsync();
            await GetCountriesAsync();


            _loaded = true;
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

        private void SelectedOriginCountryChanged(string country)
        {
            _travelRequest.OriginCountry = country;
            _cities = _countries.FirstOrDefault(x => x.Name.Equals(country)).Cities;
            foreach (var city in _cities)
            {
                Console.WriteLine(city);
            }
        }
        private void SelectedDestinationCountryChanged(string country)
        {
            _travelRequest.DestinationCountry = country;
            _cities = _countries.FirstOrDefault(x => x.Name.Equals(country)).Cities;
            foreach (var city in _cities)
            {
                Console.WriteLine(city);
            }
        }
            
        public async Task GetTravelClassesAsync()
        {
            _travelClass.Add(TravelClassConstants.Economy);
            _travelClass.Add(TravelClassConstants.PremiumEconomy);
            _travelClass.Add(TravelClassConstants.Business);
            _travelClass.Add(TravelClassConstants.First);
        }

        private async Task GetFlightsAsync()
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
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackbar.Add(message, Severity.Error);
                }
            }
        }


        private async Task ViewInformations(string Id) {


            }

        private async Task ReserveFlight(string Id)
        {

        }

          private bool Search(FlightOffer _travelResponse)
            {
              if (string.IsNullOrWhiteSpace(_searchString)) return true;
            foreach (var item in _segmentList) {
                if (item.Departure.IataCode?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
                {
                    return true;
                }
            }
            foreach (var item in _segmentList)
            {
                if (item.Arrival.IataCode?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
                {
                    return true;
                }
            }
           
              
                return false;
              
            }
        public async Task SubmitAsync() 
        {
           var result =  await GetIataCityCodeAsync(); 
           
            if(result)
            {
                
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


        /*    private async Task InvokeModal()
            {
                var parameters = new DialogParameters();
                var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Large, FullWidth = true, DisableBackdropClick = true };
                var dialog = _dialogService.Show<Register>(_l["Register New User"], parameters, options);
                var result = await dialog.Result;
                if (!result.Cancelled)
                {
                    await GetFlightsAsync();
                }
            }*/

        public class Country
        {
            public string Name { get; set; }
            public List<string> Cities { get; set; }
        }

    }
    }




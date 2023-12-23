
using MudBlazor;
using MudBlazorApp.Shared.Response;
using MudBlazorApp.Shared.Constant;
using Newtonsoft.Json;
using Blazored.FluentValidation;
using Microsoft.Win32;
using MudBlazorApp.Client.Pages.Flights.Component;
using Microsoft.AspNetCore.Components;
using System.Globalization;




namespace MudBlazorApp.Client.Pages.Flights
{
    public partial class SearchFlight
    {


        [Parameter] public List<FlightOffer> _travelList { get; set; }
        private TravelListDTO flightOfferRequest = new();
        private TravelRequest _travelRequest = new();
        private List<string> _travelClass = new();
        private List<string> _cities = new();
        private string _searchString = "";
        private bool _dense = true;
        private bool _striped = true;
        private bool _bordered = false;
        private bool _loaded;
        public List<TravelListDTO> _travelListDTO = new();

        [Parameter] public List<Itinerary> _itineraries { get; set; }
        [Parameter] public List<Segment> _segmentList { get; set; }
        [Parameter] public List<TravelerPricing> _travelPricings { get; set; }
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        public double Price = 0.0 ; 


        protected override async Task OnInitializedAsync()
        {

            if (_travelList != null) { 
                
               
                foreach(var travel in _travelList)
                {
                    var priceString = travel.Price.Total;
                    if (Double.TryParse(priceString, NumberStyles.Any, CultureInfo.InvariantCulture, out double parsedPrice))
                    {
                        // Parsing successful, update the Price variable
                        Price = parsedPrice;
                    }


                    foreach (var ite in travel.Itineraries)
                    {
                        foreach(var seg in ite.Segments)
                        {
                            _travelListDTO.Add(new TravelListDTO()
                            {

                                Departure = seg.Departure.IataCode,
                                Arrival = seg.Arrival.IataCode,
                                DepartureAt = seg.Departure.At.ToString().Substring(11),
                                ArrivalAt = seg.Arrival.At.ToString().Substring(11),
                                Duration = seg.Duration.Substring(2),
                                TotalPrice = Price

                            }
                            ) ; 
                        }
                    }
                }
                
                
                
                _loaded = true;
            
            }
        }


        private void Cancel()
        {
            MudDialog.Cancel();
        }


     

        private async Task ViewInformations(string Id) {


            }

        private async Task ReserveFlight(string Id)
        {

        }

          private bool Search(TravelListDTO _travelResponse)
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
   
     



      
    }
    }





using MudBlazor;
using MudBlazorApp.Shared.Response;
using MudBlazorApp.Shared.Constant;
using Newtonsoft.Json;
using Blazored.FluentValidation;
using Microsoft.Win32;
using MudBlazorApp.Client.Pages.Flights.Component;
using Microsoft.AspNetCore.Components;




namespace MudBlazorApp.Client.Pages.Flights
{
    public partial class SearchFlight
    {


        [Parameter] public List<FlightOffer> _travelList { get; set; }
        private FlightOffer flightOfferRequest = new();
        private TravelRequest _travelRequest = new();
        private List<string> _travelClass = new();
        private List<string> _cities = new();
        private string _searchString = "";
        private bool _dense = true;
        private bool _striped = true;
        private bool _bordered = false;
        private bool _loaded;

        [Parameter] public List<Itinerary> _itineraries { get; set; }
        [Parameter] public List<Segment> _segmentList { get; set; }
        [Parameter] public List<TravelerPricing> _travelPricings { get; set; }
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }


        protected override async Task OnInitializedAsync()
        {
          
            if(_travelList != null) _loaded = true;
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
   
     



      
    }
    }




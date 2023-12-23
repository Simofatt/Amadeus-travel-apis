using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudBlazorApp.Shared.Response
{
    public class TravelListDTO
    {
        public string Id { get; set; }
        public string Duration { get; set; }
        public string Departure { get; set; }   
        public string Arrival { get; set; }
        public string? DepartureAt { get; set; }
        public  string? ArrivalAt { get; set; }
        public double TotalPrice { get; set; }
    }
}

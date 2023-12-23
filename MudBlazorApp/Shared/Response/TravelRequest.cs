using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudBlazorApp.Shared.Response
{
    public class TravelRequest
    {
        public string Origin { get; set; }  
        public string OriginCountry { get; set; } 
        public string DestinationCountry { get; set; }
        public DateTime? DateAller { get; set; }
        public DateTime? DateRetour { get; set; }
        public  string Destination { get; set; }
        public int Adults { get; set; } 
        public int? Childreen { get; set; }
        public bool NonStop { get; set; }
        public string TravelClass { get; set; }
    }
}

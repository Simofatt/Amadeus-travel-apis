using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudBlazorApp.Shared.Response
{
    public class IataCodeResponse
    {

        public List<Data> Data { get; set; }



    }

    public class Data
    {
       public string iataCode { get; set; }
    }

}

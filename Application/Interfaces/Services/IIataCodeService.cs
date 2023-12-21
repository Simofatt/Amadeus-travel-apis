using Synaplic.UniRH.Shared.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface  IIataCodeService
    {
        Task<Result<string>> GetCityCode(string _city);
        Task<Result<string>> ConnectOAuth();
        void ConfigBearerTokenHeader();
    }
}

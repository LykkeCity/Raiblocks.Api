using System;
using Common.Log;

namespace Lykke.Service.RaiblocksApi.Client
{
    public class RaiblocksApiClient : IRaiblocksApiClient, IDisposable
    {
        private readonly ILog _log;

        public RaiblocksApiClient(string serviceUrl, ILog log)
        {
            _log = log;
        }

        public void Dispose()
        {
            //if (_service == null)
            //    return;
            //_service.Dispose();
            //_service = null;
        }
    }
}

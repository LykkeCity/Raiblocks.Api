using System;
using System.Collections.Generic;
using System.Text;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.RaiblocksApi.Core.Settings.ServiceSettings
{
    public class MonitoringServiceClientSettings
    {
        [HttpCheck("api/isalive", false)]
        public string MonitoringServiceUrl { get; set; }
    }
}

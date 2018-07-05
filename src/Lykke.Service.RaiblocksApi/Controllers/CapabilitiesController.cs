using Common.Log;
using Lykke.Service.BlockchainApi.Contract.Common;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.RaiblocksApi.Controllers
{
    [Route("api/[controller]")]
    public class CapabilitiesController : Controller
    {
        private readonly ILog _log;

        public CapabilitiesController(ILog log)
        {
            _log = log;
        }
        
        /// <summary>
        /// Get API capabilities set.
        /// </summary>
        /// <returns>API capabilities set <see cref="CapabilitiesResponse"/>.</returns>
        [HttpGet]
        [SwaggerOperation("Capabilities")]
        [ProducesResponseType(typeof(CapabilitiesResponse), (int)HttpStatusCode.OK)]
        public CapabilitiesResponse GetCapabilities()
        {
            return new CapabilitiesResponse
            {
                IsTransactionsRebuildingSupported = false,
                AreManyInputsSupported = false,
                AreManyOutputsSupported = false,
                CanReturnExplorerUrl = true,
                IsPublicAddressExtensionRequired = false,
                IsReceiveTransactionRequired = true,
                IsTestingTransfersSupported = false,
                
            };
        }
    }
}

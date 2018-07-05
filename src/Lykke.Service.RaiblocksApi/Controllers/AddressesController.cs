using Common.Log;
using Lykke.Service.BlockchainApi.Contract.Addresses;
using Lykke.Service.RaiblocksApi.AzureRepositories.Entities.Balances;
using Lykke.Service.RaiblocksApi.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Service.RaiblocksApi.Core.Helpers;

namespace Lykke.Service.RaiblocksApi.Controllers
{
    [Route("api/[controller]")]
    public class AddressesController : Controller
    {
        private readonly ILog _log;
        private readonly IBlockchainService _blockchainService;

        public AddressesController(ILog log, IBlockchainService blockchainService)
        {
            _log = log;
            _blockchainService = blockchainService;
        }

        /// <summary>
        /// Check wallet address validity
        /// </summary>
        /// <param name="address">Wallet address</param>
        /// <returns>Address validity</returns>
        [HttpGet("{address}/validity")]
        [SwaggerOperation("AddressValidity")]
        [ProducesResponseType(typeof(AddressValidationResponse), (int) HttpStatusCode.OK)]
        public async Task<AddressValidationResponse> AddressValidityAsync(string address)
        {
            return new AddressValidationResponse
            {
                IsValid = await _blockchainService.IsAddressValidAsync(address)
            };
        }

        /// <summary>
        /// Get blockchain explorer URLs.
        /// </summary>
        /// <param name="address">Wallet address/</param>
        /// <returns>Blockchain explorer URLs</returns>
        [HttpGet("{address}/explorer-url")]
        [SwaggerOperation("ExplorerUrl")]
        [ProducesResponseType(typeof(IActionResult), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotImplemented)]
        public IActionResult AddressExplorerUrl(string address)
        {
            if (_blockchainService.IsAddressValidOfflineAsync(address))
            {
                return StatusCode((int) HttpStatusCode.OK,
                    new List<string> {$"https://www.nanode.co/account/{address}"});
            }
            else
            {
                return StatusCode((int) HttpStatusCode.BadRequest, ErrorResponse.Create("Invalid address"));
            }
        }
    }
}

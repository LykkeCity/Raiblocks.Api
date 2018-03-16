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
        [ProducesResponseType(typeof(AddressValidationResponse), (int)HttpStatusCode.OK)]
        public async Task<AddressValidationResponse> AddressValidityAsync(string address)
        {
            return new AddressValidationResponse
            {
                IsValid = await _blockchainService.IsAddressValidAsync(address)
            };
        }
    }
}

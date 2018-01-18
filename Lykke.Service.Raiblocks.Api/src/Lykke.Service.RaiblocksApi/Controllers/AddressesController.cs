using Lykke.Service.BlockchainApi.Contract.Addresses;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Lykke.Service.RaiblocksApi.Controllers
{
    [Route("api/[controller]")]
    public class AddressesController : Controller
    {
        /// <summary>
        /// Check wallet address validity
        /// </summary>
        /// <param name="address">Wallet address</param>
        /// <returns>Address validity</returns>
        [HttpGet("{address}/validity")]
        [SwaggerOperation("AddressValidity")]
        [ProducesResponseType(typeof(AddressValidationResponse), (int)HttpStatusCode.OK)]
        public AddressValidationResponse AddressValidity(string address)
        {
            throw new NotImplementedException();
        }
    }
}

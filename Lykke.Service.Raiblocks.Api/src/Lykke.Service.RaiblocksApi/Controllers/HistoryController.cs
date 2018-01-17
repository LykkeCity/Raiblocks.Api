using Lykke.Service.BlockchainApi.Contract.Transactions;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Lykke.Service.RaiblocksApi.Controllers
{
    public class HistoryController : Controller
    {
        /// <summary>
        /// Start observation of the transactions that transfer fund from the address
        /// </summary>
        /// <param name="address">Wallet address</param>
        /// <returns>HttpStatusCode</returns>
        [HttpPost("from/{address}/observation")]
        [SwaggerOperation("AddHistoryObservationFrom")]
        [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.Conflict)]
        public IActionResult AddHistoryObservationFrom(string address)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Start observation of the transactions that transfer fund to the address
        /// </summary>
        /// <param name="address">Wallet address</param>
        /// <returns>HttpStatusCode</returns>
        [HttpPost("to/{address}/observation")]
        [SwaggerOperation("AddHistoryObservationFrom")]
        [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.Conflict)]
        public IActionResult AddHistoryObservationTo(string address)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get completed transactions that transfer fund from the address 
        /// </summary>
        /// <param name="address">Wallet address</param>
        /// <param name="take">Amount of the returned transactions should not exceed take</param>
        /// <param name="afterHash">Transaction hash</param>
        /// <returns>Historical transaction contract</returns>
        [HttpGet("from/{address}")]
        [SwaggerOperation("GetHistoryFrom")]
        [ProducesResponseType(typeof(IEnumerable<HistoricalTransactionContract>), (int)HttpStatusCode.OK)]
        public IEnumerable<HistoricalTransactionContract> GetHistoryFrom(string address, [FromQuery]int take = 100, [FromQuery]string afterHash = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get completed transactions that transfer fund to the address
        /// </summary>
        /// <param name="address">Wallet address</param>
        /// <param name="take">Amount of the returned transactions should not exceed take</param>
        /// <param name="afterHash">Transaction hash</param>
        /// <returns>Historical transaction contract</returns>
        [HttpGet("to/{address}")]
        [SwaggerOperation("GetHistoryTo")]
        [ProducesResponseType(typeof(IEnumerable<HistoricalTransactionContract>), (int)HttpStatusCode.OK)]
        public IEnumerable<HistoricalTransactionContract> GetHistoryTo(string address, [FromQuery]int take = 100, [FromQuery]string afterHash = null)
        {
            throw new NotImplementedException();
        }
    }
}

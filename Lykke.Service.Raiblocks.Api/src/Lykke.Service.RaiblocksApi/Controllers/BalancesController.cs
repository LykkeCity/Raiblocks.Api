using Lykke.Service.BlockchainApi.Contract;
using Lykke.Service.BlockchainApi.Contract.Balances;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Lykke.Service.RaiblocksApi.Controllers
{
    public class BalancesController : Controller
    {
        /// <summary>
        /// Remember the wallet address to observe
        /// </summary>
        /// <param name="address">Wallet address</param>
        /// <returns>HttpStatusCode</returns>
        [HttpPost("{address}/observation")]
        [SwaggerOperation("AddBalanceObservation")]
        [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.Conflict)]
        public IActionResult AddBalanceObservation(string address)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Forget the wallet address and stop observing its balance
        /// </summary>
        /// <param name="address">Wallet address</param>
        /// <returns>HttpStatusCode</returns>
        [HttpDelete("{address}/observation")]
        [SwaggerOperation("RemoveBalanceObservation")]
        [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(IActionResult), (int)HttpStatusCode.NoContent)]
        public IActionResult RemoveBalanceObservation(string address)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get balances of the observed wallets with non zero balances
        /// </summary>
        /// <param name="take">Amount of the returned wallets should not exceed take</param>
        /// <param name="continuation">Optional continuation contains context of the previous request</param>
        /// <returns>Balances of the observed wallets with non zero balances</returns>
        [HttpGet]
        [SwaggerOperation("GetBalances")]
        [ProducesResponseType(typeof(PaginationResponse<WalletBalanceContract>), (int)HttpStatusCode.OK)]
        public PaginationResponse<WalletBalanceContract> GetBalances([FromQuery]int take = 100, [FromQuery]string continuation = null)
        {
            throw new NotImplementedException();
        }

    }
}

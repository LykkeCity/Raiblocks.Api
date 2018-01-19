using Lykke.Common.Api.Contract.Responses;
using Lykke.Service.BlockchainApi.Contract;
using Lykke.Service.BlockchainApi.Contract.Balances;
using Lykke.Service.RaiblocksApi.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Net;
using System.Linq;
using System.Threading.Tasks;

namespace Lykke.Service.RaiblocksApi.Controllers
{
    [Route("api/[controller]")]
    public class BalancesController : Controller
    {
        private readonly IBlockchainService _blockchainService;
        private readonly IAssetService _assetService;

        public BalancesController(IBlockchainService blockchainService, IAssetService assetService)
        {
            _blockchainService = blockchainService;
            _assetService = assetService;
        }

        /// <summary>
        /// Remember the wallet address to observe
        /// </summary>
        /// <param name="address">Wallet address</param>
        /// <returns>HttpStatusCode</returns>
        [HttpPost("{address}/observation")]
        [SwaggerOperation("AddBalanceObservation")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.Conflict)]
        public async Task<IActionResult> AddBalanceObservation(string address)
        {
            if (!await _blockchainService.IsBalanceObserved(address) && await _blockchainService.AddBalanceObservation(address))
                return Ok();
            else
                return StatusCode((int)HttpStatusCode.Conflict, ErrorResponse.Create("Specified address is already observed"));
        }

        /// <summary>
        /// Forget the wallet address and stop observing its balance
        /// </summary>
        /// <param name="address">Wallet address</param>
        /// <returns>HttpStatusCode</returns>
        [HttpDelete("{address}/observation")]
        [SwaggerOperation("RemoveBalanceObservation")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> RemoveBalanceObservation(string address)
        {
            if (await _blockchainService.IsBalanceObserved(address) && await _blockchainService.RemoveBalanceObservation(address))
                return Ok();
            else
                return StatusCode((int)HttpStatusCode.NoContent);
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
        public async Task<PaginationResponse<WalletBalanceContract>> GetBalances([FromQuery]int take = 100, [FromQuery]string continuation = null)
        {
            var balances = await _blockchainService.GetBalances();
            return PaginationResponse.From(
                balances.continuation,
                balances.items.Select(b => new WalletBalanceContract {
                    Address = b.Address,
                    Balance = b.Balance, 
                    AssetId = _assetService.AssetId
                }).ToArray());

        }

    }
}

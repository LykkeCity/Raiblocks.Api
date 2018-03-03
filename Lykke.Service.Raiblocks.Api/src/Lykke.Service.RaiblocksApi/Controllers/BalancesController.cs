using Lykke.Common.Api.Contract.Responses;
using Lykke.Service.BlockchainApi.Contract;
using Lykke.Service.BlockchainApi.Contract.Balances;
using Lykke.Service.RaiblocksApi.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.RaiblocksApi.AzureRepositories.Entities.Balances;
using Lykke.Service.RaiblocksApi.Core.Domain.Entities.Balances;
using Lykke.Service.RaiblocksApi.Core.Helpers;
using Newtonsoft.Json.Linq;
using Common.Log;
using Lykke.Service.RaiblocksApi.Helpers;

namespace Lykke.Service.RaiblocksApi.Controllers
{
    [Route("api/[controller]")]
    public class BalancesController : Controller
    {
        private readonly IBalanceService<BalanceObservation, AddressBalance> _balanceService;
        private readonly IAssetService _assetService;
        private readonly CoinConverter _coinConverter;
        private readonly IBlockchainService _blockchainService;
        private readonly ILog _log;

        public BalancesController(IBalanceService<BalanceObservation, AddressBalance> balanceService, IBlockchainService blockchainService, IAssetService assetService, CoinConverter coinConverter, ILog log)
        {
            _balanceService = balanceService;
            _blockchainService = blockchainService;
            _assetService = assetService;
            _coinConverter = coinConverter;
            _log = log;
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
        public async Task<IActionResult> AddBalanceObservationAsync(string address)
        {
            if (_blockchainService.IsAddressValidOfflineAsync(address))
            {
                BalanceObservation balanceObservation = new BalanceObservation
                {
                    Address = address,
                };
                if (!await _balanceService.IsBalanceObservedAsync(balanceObservation) && await _balanceService.AddBalanceObservationAsync(balanceObservation))
                {
                    await _log.WriteInfoAsync(nameof(AddBalanceObservationAsync), JObject.FromObject(balanceObservation).ToString(), $"Start observe balance for {address}");
                    return Ok();
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.Conflict, ErrorResponse.Create("Specified address is already observed"));
                }
            } else
            {
                return StatusCode((int)HttpStatusCode.BadRequest, ErrorResponse.Create("Invalid address"));
            }

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
        public async Task<IActionResult> RemoveBalanceObservationAsync(string address)
        {
            if (_blockchainService.IsAddressValidOfflineAsync(address))
            {
                BalanceObservation balanceObservation = new BalanceObservation
                {
                    Address = address
                };
                if (await _balanceService.IsBalanceObservedAsync(balanceObservation) && await _balanceService.RemoveBalanceObservationAsync(balanceObservation))
                {
                    await _balanceService.RemoveBalanceAsync(new AddressBalance
                    {
                        Address = address
                    });

                    await _log.WriteInfoAsync(nameof(AddBalanceObservationAsync), JObject.FromObject(balanceObservation).ToString(), $"Stop observe balance for {address}");
                    return Ok();
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.NoContent);
                }
            }
            else
            {
                return StatusCode((int)HttpStatusCode.BadRequest, ErrorResponse.Create("Invalid address"));
            }              
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
        public async Task<IActionResult> GetBalancesAsync([FromQuery]string take = "100", [FromQuery]string continuation = null)
        {
            if (int.TryParse(take, out var takeParsed) && take != null && ValidateHeper.IsContinuationValid(continuation))
            {
                var balances = await _balanceService.GetBalancesAsync(takeParsed, continuation);
                return StatusCode((int)HttpStatusCode.OK, PaginationResponse.From(
                    balances.continuation,
                    balances.items.Select(b => new WalletBalanceContract
                    {
                        Address = b.Address,
                        Balance = _coinConverter.RawToLykkeRai(b.Balance),
                        AssetId = _assetService.AssetId,
                        Block = b.Block
                    }).ToArray()));
            }
            else
            {
                return StatusCode((int)HttpStatusCode.BadRequest, ErrorResponse.Create("Invalid params"));
            }
        }

    }
}

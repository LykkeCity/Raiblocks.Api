using Lykke.Common.Api.Contract.Responses;
using Lykke.Service.BlockchainApi.Contract.Transactions;
using Lykke.Service.RaiblocksApi.AzureRepositories.Entities.Addresses;
using Lykke.Service.RaiblocksApi.Core.Domain.Entities.Addresses;
using Lykke.Service.RaiblocksApi.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.RaiblocksApi.Core.Helpers;
using Common.Log;
using Newtonsoft.Json.Linq;
using TransactionType = Lykke.Service.RaiblocksApi.Core.Services.TransactionType;

namespace Lykke.Service.RaiblocksApi.Controllers
{
    [Route("api/transactions/[controller]")]
    public class HistoryController : Controller
    {
        private readonly IHistoryService<AddressHistoryEntry, AddressObservation, AddressOperationHistoryEntry>
            _historyService;

        private readonly IAssetService _assetService;
        private readonly CoinConverter _coinConverter;
        private readonly IBlockchainService _blockchainService;
        private readonly ILog _log;

        public HistoryController(
            IHistoryService<AddressHistoryEntry, AddressObservation, AddressOperationHistoryEntry> historyService,
            IAssetService assetService, CoinConverter coinConverter, IBlockchainService blockchainService, ILog log)
        {
            _historyService = historyService;
            _assetService = assetService;
            _coinConverter = coinConverter;
            _blockchainService = blockchainService;
            _log = log;
        }

        /// <summary>
        /// Start observation of the transactions that transfer fund from the address
        /// </summary>
        /// <param name="address">Wallet address</param>
        /// <returns>HttpStatusCode</returns>
        [HttpPost("from/{address}/observation")]
        [SwaggerOperation("AddHistoryObservationFrom")]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.Conflict)]
        public async Task<IActionResult> AddHistoryObservationFromAsync(string address)
        {
            if (_blockchainService.IsAddressValidOfflineAsync(address))
            {
                AddressObservation addressObservation = new AddressObservation
                {
                    Address = address,
                    Type = AddressObservationType.From
                };
                if (!await _historyService.IsAddressObservedAsync(addressObservation) &&
                    await _historyService.AddAddressObservationAsync(addressObservation))
                {
                    await _log.WriteInfoAsync(nameof(AddHistoryObservationFromAsync),
                        JObject.FromObject(addressObservation).ToString(), $"Start observe history from {address}");
                    return Ok();
                }

                return StatusCode((int) HttpStatusCode.Conflict,
                    ErrorResponse.Create("Transactions from the address are already observed"));
            }
            else
            {
                return StatusCode((int) HttpStatusCode.BadRequest, ErrorResponse.Create("Invalid address"));
            }
        }

        /// <summary>
        /// Start observation of the transactions that transfer fund to the address
        /// </summary>
        /// <param name="address">Wallet address</param>
        /// <returns>HttpStatusCode</returns>
        [HttpPost("to/{address}/observation")]
        [SwaggerOperation("AddHistoryObservationFrom")]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.Conflict)]
        public async Task<IActionResult> AddHistoryObservationToAsync(string address)
        {
            if (_blockchainService.IsAddressValidOfflineAsync(address))
            {
                AddressObservation addressObservation = new AddressObservation
                {
                    Address = address,
                    Type = AddressObservationType.To
                };
                if (!await _historyService.IsAddressObservedAsync(addressObservation) &&
                    await _historyService.AddAddressObservationAsync(addressObservation))
                {
                    await _log.WriteInfoAsync(nameof(AddHistoryObservationToAsync),
                        JObject.FromObject(addressObservation).ToString(), $"Start observe history to {address}");
                    return Ok();
                }

                return StatusCode((int) HttpStatusCode.Conflict,
                    ErrorResponse.Create("Transactions to the address are already observed"));
            }
            else
            {
                return StatusCode((int) HttpStatusCode.BadRequest, ErrorResponse.Create("Invalid address"));
            }
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
        [ProducesResponseType(typeof(IEnumerable<HistoricalTransactionContract>), (int) HttpStatusCode.OK)]
        public async Task<IEnumerable<HistoricalTransactionContract>> GetHistoryFromAsync(string address,
            [FromQuery] int take = 100, [FromQuery] string afterHash = null)
        {
            var history = await _historyService.GetAddressHistoryAsync(take,
                Enum.GetName(typeof(AddressObservationType), AddressObservationType.From), address, afterHash);
            var internalHistory = await _historyService.GetAddressOperationHistoryAsync(take,
                Enum.GetName(typeof(AddressObservationType), AddressObservationType.From), address);

            return history.items?.OrderByDescending(x => x.BlockCount).Select(x => new HistoricalTransactionContract
            {
                Amount = _coinConverter.RawToLykkeRai(x.Amount),
                AssetId = _assetService.AssetId,
                FromAddress = x.FromAddress,
                ToAddress = x.ToAddress,
                Hash = x.Hash,
                TransactionType = x.TransactionType == TransactionType.send
                    ? BlockchainApi.Contract.Transactions.TransactionType.Send
                    : BlockchainApi.Contract.Transactions.TransactionType.Receive
            }).Select(x =>
            {
                var operationHistory = internalHistory.FirstOrDefault(y => y.Hash == x.Hash);

                if (operationHistory != null)
                {
                    x.Timestamp = operationHistory.TransactionTimestamp;
                }

                return x;
            });
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
        [ProducesResponseType(typeof(IEnumerable<HistoricalTransactionContract>), (int) HttpStatusCode.OK)]
        public async Task<IActionResult> GetHistoryToAsync(string address, [FromQuery] string take,
            [FromQuery] string afterHash = null)
        {
            if (int.TryParse(take, out var takeParsed) && take != null)
            {
                var history = await _historyService.GetAddressHistoryAsync(takeParsed,
                    Enum.GetName(typeof(AddressObservationType), AddressObservationType.To), address, afterHash);
                var internalHistory = await _historyService.GetAddressOperationHistoryAsync(takeParsed,
                    Enum.GetName(typeof(AddressObservationType), AddressObservationType.To), address);
                return StatusCode((int) HttpStatusCode.OK, history.items?.OrderByDescending(x => x.BlockCount).Select(
                    x => new HistoricalTransactionContract
                    {
                        Amount = _coinConverter.RawToLykkeRai(x.Amount),
                        AssetId = _assetService.AssetId,
                        FromAddress = x.FromAddress,
                        ToAddress = x.ToAddress,
                        Hash = x.Hash,
                        TransactionType = x.TransactionType == TransactionType.send
                            ? BlockchainApi.Contract.Transactions.TransactionType.Send
                            : BlockchainApi.Contract.Transactions.TransactionType.Receive
                    }).Select(x =>
                {
                    var operationHistory = internalHistory.FirstOrDefault(y => y.Hash == x.Hash);

                    if (operationHistory != null)
                    {
                        x.Timestamp = operationHistory.TransactionTimestamp;
                    }

                    return x;
                }));
            }
            else
            {
                return StatusCode((int) HttpStatusCode.BadRequest, ErrorResponse.Create("Invalid params"));
            }
        }

        /// <summary>
        /// Stop observation of the transactions that transfer fund from the address
        /// </summary>
        /// <param name="address">Address</param>
        /// <returns>Status code</returns>
        [HttpDelete("from/{address}/observation")]
        [HttpDelete("from/")]
        [SwaggerOperation("DeleteHistoryFrom")]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.NoContent)]
        public async Task<IActionResult> DeleteHistoryFromAsync(string address)
        {
            if (_blockchainService.IsAddressValidOfflineAsync(address))
            {
                AddressObservation addressObservation = new AddressObservation
                {
                    Address = address,
                    Type = AddressObservationType.From
                };
                if (await _historyService.IsAddressObservedAsync(addressObservation) &&
                    await _historyService.RemoveAddressObservationAsync(addressObservation))
                {
                    await _log.WriteInfoAsync(nameof(AddHistoryObservationToAsync),
                        JObject.FromObject(addressObservation).ToString(), $"Stop observe history from {address}");
                    return Ok();
                }

                return StatusCode((int) HttpStatusCode.NoContent);
            }
            else
            {
                return StatusCode((int) HttpStatusCode.BadRequest, ErrorResponse.Create("Invalid address"));
            }
        }

        /// <summary>
        /// Stop observation of the transactions that transfer fund to the address
        /// </summary>
        /// <param name="address">Address</param>
        /// <returns>Status code</returns>
        [HttpDelete("to/{address}/observation")]
        [HttpDelete("to/")]
        [SwaggerOperation("DeleteHistoryTo")]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.NoContent)]
        public async Task<IActionResult> DeleteHistoryToAsync(string address)
        {
            if (_blockchainService.IsAddressValidOfflineAsync(address))
            {
                AddressObservation addressObservation = new AddressObservation
                {
                    Address = address,
                    Type = AddressObservationType.To
                };
                if (await _historyService.IsAddressObservedAsync(addressObservation) &&
                    await _historyService.RemoveAddressObservationAsync(addressObservation))
                {
                    await _log.WriteInfoAsync(nameof(AddHistoryObservationToAsync),
                        JObject.FromObject(addressObservation).ToString(), $"Stop observe history to {address}");
                    return Ok();
                }

                return StatusCode((int) HttpStatusCode.NoContent);
            }
            else
            {
                return StatusCode((int) HttpStatusCode.BadRequest, ErrorResponse.Create("Invalid address"));
            }
        }
    }
}

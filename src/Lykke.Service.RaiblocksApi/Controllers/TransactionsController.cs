using Lykke.Common.Api.Contract.Responses;
using Lykke.Service.BlockchainApi.Contract.Transactions;
using Lykke.Service.RaiblocksApi.AzureRepositories.Entities.Transactions;
using Lykke.Service.RaiblocksApi.Core.Domain.Entities.Transactions;
using Lykke.Service.RaiblocksApi.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Net;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Lykke.Service.RaiblocksApi.Core.Helpers;
using Lykke.Service.BlockchainApi.Contract;
using Newtonsoft.Json.Linq;
using Common.Log;
using Lykke.Service.RaiblocksApi.Helpers;

namespace Lykke.Service.RaiblocksApi.Controllers
{
    [Route("api/[controller]")]
    public class TransactionsController : Controller
    {
        private readonly ITransactionService<TransactionBody, TransactionMeta, TransactionObservation>
            _transactionService;

        private readonly IAssetService _assetService;
        private readonly IBlockchainService _blockchainService;
        private readonly CoinConverter _coinConverter;
        private readonly ILog _log;

        public TransactionsController(
            ITransactionService<TransactionBody, TransactionMeta, TransactionObservation> transactionService,
            IAssetService assetService, IBlockchainService blockchainService, CoinConverter coinConverter, ILog log)
        {
            _transactionService = transactionService;
            _assetService = assetService;
            _blockchainService = blockchainService;
            _coinConverter = coinConverter;
            _log = log;
        }

        /// <summary>
        /// Build not signed transaction to transfer from the single source to the single destination
        /// </summary>
        /// <param name="buildTransactionRequest">Build transaction request</param>
        /// <returns>Build transaction response</returns>
        [HttpPost("single")]
        [SwaggerOperation("BuildNotSignedSendTransaction")]
        [ProducesResponseType(typeof(BuildTransactionResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BlockchainErrorResponse), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BlockchainErrorResponse), (int) HttpStatusCode.Conflict)]
        public async Task<IActionResult> BuildNotSignedSingleSendTransactionAsync(
            [FromBody] BuildSingleTransactionRequest buildTransactionRequest)
        {
            if (ValidateHeper.IsBuildSingleTransactionRequestValid(buildTransactionRequest, _blockchainService))
            {
                if (await _transactionService.IsTransactionAlreadyBroadcastAsync(buildTransactionRequest.OperationId))
                {
                    return StatusCode((int) HttpStatusCode.Conflict,
                        ErrorResponse.Create(
                            "Transaction is already broadcasted or [DELETE] /api/transactions/broadcast/{operationId} is called"));
                }

                var balance = await _blockchainService.GetAddressBalanceAsync(buildTransactionRequest.FromAddress);
                if (BigInteger.Parse(balance) <
                    BigInteger.Parse(_coinConverter.LykkeRaiToRaw(buildTransactionRequest.Amount)))
                {
                    return StatusCode((int) HttpStatusCode.BadRequest,
                        BlockchainErrorResponse.FromKnownError(BlockchainErrorCode.NotEnoughBalance));
                }

                var unsignTransaction = await _transactionService.GetUnsignSendTransactionAsync(
                    buildTransactionRequest.OperationId, buildTransactionRequest.FromAddress,
                    buildTransactionRequest.ToAddress, _coinConverter.LykkeRaiToRaw(buildTransactionRequest.Amount),
                    buildTransactionRequest.AssetId,
                    buildTransactionRequest.IncludeFee);

                return StatusCode((int) HttpStatusCode.OK, new BuildTransactionResponse
                {
                    TransactionContext = unsignTransaction
                });
            }
            else
            {
                return StatusCode((int) HttpStatusCode.BadRequest, ErrorResponse.Create("Invalid params"));
            }
        }

        /// <summary>
        /// Build not signed receive transaction to transfer from the single source to the single destination
        /// </summary>
        /// <param name="buildSingleReceiveTransactionRequest">Build transaction request <see cref="BuildSingleReceiveTransactionRequest"/></param>
        /// <returns>Build transaction response <see cref="BuildTransactionResponse"/></returns>
        [HttpPost("single/receive")]
        [SwaggerOperation("BuildNotSignedReceiveTransaction")]
        [ProducesResponseType(typeof(BuildTransactionResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BlockchainErrorResponse), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BlockchainErrorResponse), (int) HttpStatusCode.Conflict)]
        [ProducesResponseType(typeof(BlockchainErrorResponse), (int) HttpStatusCode.NotImplemented)]
        public async Task<IActionResult> BuildNotSignedSingleReceiveTransactionAsync(
            [FromBody] BuildSingleReceiveTransactionRequest buildSingleReceiveTransactionRequest)
        {
            if (ValidateHeper.IsBuildSingleReceiveTransactionRequestValid(buildSingleReceiveTransactionRequest,
                _blockchainService))
            {
                if (await _transactionService.IsTransactionAlreadyBroadcastAsync(buildSingleReceiveTransactionRequest
                    .OperationId))
                {
                    return StatusCode((int) HttpStatusCode.Conflict,
                        ErrorResponse.Create(
                            "Transaction is already broadcasted or [DELETE] /api/transactions/broadcast/{operationId} is called"));
                }

                var unsignTransaction = await _transactionService.GetUnsignReceiveTransactionAsync(
                    buildSingleReceiveTransactionRequest.OperationId,
                    buildSingleReceiveTransactionRequest.SendTransactionHash);

                return StatusCode((int) HttpStatusCode.OK, new BuildTransactionResponse
                {
                    TransactionContext = unsignTransaction
                });
            }
            else
            {
                return StatusCode((int) HttpStatusCode.BadRequest, ErrorResponse.Create("Invalid params"));
            }
        }

        /// <summary>
        /// Broadcast the signed transaction
        /// </summary>
        /// <param name="broadcastTransactionRequest">Broadcast transaction request</param>
        /// <returns>Broadcasted transaction response</returns>
        [HttpPost("broadcast")]
        [SwaggerOperation("BroadcastSignedTransaction")]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.Conflict)]
        [ProducesResponseType(typeof(BlockchainErrorResponse), (int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> BroadcastSignedTransactionAsync(
            [FromBody] BroadcastTransactionRequest broadcastTransactionRequest)
        {
            if (!_blockchainService.IsSignedTransactionValid(broadcastTransactionRequest?.SignedTransaction))
            {
                return StatusCode((int) HttpStatusCode.BadRequest, ErrorResponse.Create("Transaction invalid"));
            }

            var result = await _transactionService.BroadcastSignedTransactionAsync(
                broadcastTransactionRequest.OperationId, broadcastTransactionRequest.SignedTransaction);

            if (!result)
            {
                return StatusCode((int) HttpStatusCode.Conflict,
                    ErrorResponse.Create(
                        "Transaction with specified operationId and signedTransaction has already been broadcasted"));
            }

            await _log.WriteInfoAsync(nameof(BroadcastSignedTransactionAsync),
                JObject.FromObject(broadcastTransactionRequest).ToString(),
                $"Transaction broadcasted {broadcastTransactionRequest.OperationId}");
            return Ok();
        }

        /// <summary>
        /// Get broadcasted transaction with single input and output
        /// </summary>
        /// <param name="operationId">Operation Id</param>
        /// <returns>Broadcasted transaction response</returns>
        [HttpGet("broadcast/single/{operationId}")]
        [HttpGet("broadcast/single/")]
        [SwaggerOperation("GetBroadcastedSingleTransaction")]
        [ProducesResponseType(typeof(BroadcastedSingleTransactionResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.NoContent)]
        public async Task<IActionResult> GetBroadcastedSingleTransactionAsync(string operationId)
        {
            if (Guid.TryParse(operationId, out var operationIdGuid) && operationId != null)
            {
                var txMeta = await _transactionService.GetTransactionMetaAsync(operationId);
                if (txMeta != null)
                {
                    BroadcastedTransactionState state;

                    switch (txMeta.State)
                    {
                        case TransactionState.Confirmed:
                            state = BroadcastedTransactionState.Completed;
                            break;
                        case TransactionState.Failed:
                        case TransactionState.BlockChainFailed:
                            state = BroadcastedTransactionState.Failed;
                            break;
                        default:
                            state = BroadcastedTransactionState.InProgress;
                            break;
                    }

                    return Ok(new BroadcastedSingleTransactionResponse
                    {
                        OperationId = txMeta.OperationId,
                        State = state,
                        Timestamp = (txMeta.CompleteTimestamp ?? txMeta.BroadcastTimestamp).Value,
                        Amount = _coinConverter.RawToLykkeRai(txMeta.Amount),
                        Fee = "0",
                        Hash = txMeta.Hash,
                        Error = txMeta.Error,
                        Block = txMeta.BlockCount
                    });
                }
                else
                    return StatusCode((int) HttpStatusCode.NoContent);
            }
            else
            {
                return StatusCode((int) HttpStatusCode.BadRequest, ErrorResponse.Create("Invalid guid"));
            }
        }

        /// <summary>
        /// Remove specified transaction from the broadcasted transactions
        /// </summary>
        /// <param name="operationId">Operation Id</param>
        /// <returns>HttpStatusCode</returns>
        [HttpDelete("broadcast/{operationId}")]
        [HttpDelete("broadcast/")]
        [SwaggerOperation("DeleteBroadcastedTransaction")]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.NoContent)]
        public async Task<IActionResult> DeleteBroadcastedTransactionAsync(string operationId = null)
        {
            if (Guid.TryParse(operationId, out var operationIdGuid) && operationId != null)
            {
                TransactionObservation transactionObservation = new TransactionObservation
                {
                    OperationId = new Guid(operationId)
                };
                if (await _transactionService.IsTransactionObservedAsync(transactionObservation) &&
                    await _transactionService.RemoveTransactionObservationAsync(transactionObservation))
                {
                    await _log.WriteInfoAsync(nameof(DeleteBroadcastedTransactionAsync),
                        JObject.FromObject(transactionObservation).ToString(), $"Stop observe operation {operationId}");
                    return Ok();
                }

                return StatusCode((int) HttpStatusCode.NoContent);
            }
            else
            {
                return StatusCode((int) HttpStatusCode.BadRequest, ErrorResponse.Create("Invalid guid"));
            }
        }

        #region NotImplemented

        /// <summary>
        /// Build not signed transaction with many inputs
        /// </summary>
        /// <param name="buildTransactionRequest">Build transaction request</param>
        /// <returns>Build transaction response</returns>
        [HttpPost("many-inputs")]
        [SwaggerOperation("BuildNotSignedManyInputsTransaction")]
        [ProducesResponseType(typeof(BuildTransactionResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.NotImplemented)]
        [ProducesResponseType(typeof(BlockchainErrorResponse), (int) HttpStatusCode.BadRequest)]
        public IActionResult BuildNotSignedManyInputsTransaction(
            [FromBody] BuildTransactionWithManyInputsRequest buildTransactionRequest)
        {
            return StatusCode((int) HttpStatusCode.NotImplemented);
        }

        /// <summary>
        /// Build not signed transaction with many outputs
        /// </summary>
        /// <param name="buildTransactionRequest">Build transaction request</param>
        /// <returns>Build transaction response</returns>
        [HttpPost("many-outputs")]
        [SwaggerOperation("BuildNotSignedManyOutputsTransaction")]
        [ProducesResponseType(typeof(BuildTransactionResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.NotImplemented)]
        [ProducesResponseType(typeof(BlockchainErrorResponse), (int) HttpStatusCode.BadRequest)]
        public IActionResult BuildNotSignedManyOutputsTransaction(
            [FromBody] BuildTransactionWithManyOutputsRequest buildTransactionRequest)
        {
            return StatusCode((int) HttpStatusCode.NotImplemented);
        }

        /// <summary>
        ///  Rebuild not signed transaction with the specified fee factor
        /// </summary>
        /// <param name="rebuildTransactionRequest">Rebuild transaction request</param>
        /// <returns>Rebuild transaction response</returns>
        [HttpPut]
        [SwaggerOperation("RebuildNotSignedTransaction")]
        [ProducesResponseType(typeof(RebuildTransactionResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.NotImplemented)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.NotAcceptable)]
        [ProducesResponseType(typeof(BlockchainErrorResponse), (int) HttpStatusCode.BadRequest)]
        public IActionResult RebuildNotSignedTransaction([FromBody] RebuildTransactionRequest rebuildTransactionRequest)
        {
            return StatusCode((int) HttpStatusCode.NotImplemented);
        }

        /// <summary>
        /// Get broadcasted transaction with with many inputs
        /// </summary>
        /// <param name="operationId">Operation Id</param>
        /// <returns>Broadcasted transaction response</returns>
        [HttpGet("broadcast/many-inputs/{operationId}")]
        [HttpGet("broadcast/many-inputs/")]
        [SwaggerOperation("GetBroadcastedManyInputsTransaction")]
        [ProducesResponseType(typeof(BroadcastedTransactionWithManyInputsResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.NotImplemented)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(BlockchainErrorResponse), (int) HttpStatusCode.BadRequest)]
        public IActionResult GetBroadcastedManyInputsTransaction(string operationId)
        {
            return StatusCode((int) HttpStatusCode.NotImplemented);
        }


        /// <summary>
        /// Get broadcasted transaction with with many outputs
        /// </summary>
        /// <param name="operationId">Operation Id</param>
        /// <returns>Broadcasted transaction response</returns>
        [HttpGet("broadcast/many-outputs/{operationId}")]
        [HttpGet("broadcast/many-outputs/")]
        [SwaggerOperation("GetBroadcastedManyOutputsTransaction")]
        [ProducesResponseType(typeof(BroadcastedTransactionWithManyOutputsResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.NotImplemented)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(BlockchainErrorResponse), (int) HttpStatusCode.BadRequest)]
        public IActionResult GetBroadcastedManyOutputsTransaction(string operationId)
        {
            return StatusCode((int) HttpStatusCode.NotImplemented);
        }

        #endregion
    }
}

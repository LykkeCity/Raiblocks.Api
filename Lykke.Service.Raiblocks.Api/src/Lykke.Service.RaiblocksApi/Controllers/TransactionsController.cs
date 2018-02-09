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

namespace Lykke.Service.RaiblocksApi.Controllers
{
    [Route("api/[controller]")]
    public class TransactionsController : Controller
    {
        private readonly ITransactionService<TransactionBody, TransactionMeta, TransactionObservation> _transactionService;
        private readonly IAssetService _assetService;
        private readonly IBlockchainService _blockchainService;
        private readonly CoinConverter _coinConverter;
        
        public TransactionsController(ITransactionService<TransactionBody, TransactionMeta, TransactionObservation> transactionService, IAssetService assetService, IBlockchainService blockchainService, CoinConverter coinConverter)
        {
            _transactionService = transactionService;
            _assetService = assetService;
            _blockchainService = blockchainService;
            _coinConverter = coinConverter;
        }

        /// <summary>
        /// Build not signed transaction to transfer from the single source to the single destination
        /// </summary>
        /// <param name="buildTransactionRequest">Build transaction request</param>
        /// <returns>Build transaction response</returns>
        [HttpPost("single")]
        [SwaggerOperation("BuildNotSignedTransaction")]
        [ProducesResponseType(typeof(BuildTransactionResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BlockchainErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> BuildNotSignedSingleTransactionAsync([FromBody] BuildSingleTransactionRequest buildTransactionRequest)
        {
            var balance = await _blockchainService.GetAddressBalanceAsync(buildTransactionRequest.FromAddress);
            if (BigInteger.Parse(balance) < BigInteger.Parse(_coinConverter.LykkeRaiToRaw(buildTransactionRequest.Amount)))
            {
                return StatusCode((int)HttpStatusCode.BadRequest, BlockchainErrorResponse.FromKnownError(BlockchainErrorCode.NotEnoughtBalance));
            }

            var unsignTransaction = await _transactionService.GetUnsignSendTransactionAsync(
                buildTransactionRequest.OperationId, buildTransactionRequest.FromAddress,
                buildTransactionRequest.ToAddress, _coinConverter.LykkeRaiToRaw(buildTransactionRequest.Amount), buildTransactionRequest.AssetId,
                buildTransactionRequest.IncludeFee);
            
            return StatusCode((int)HttpStatusCode.OK, new BuildTransactionResponse
            {
                TransactionContext = unsignTransaction
            });
        }

        /// <summary>
        /// Broadcast the signed transaction
        /// </summary>
        /// <param name="broadcastTransactionRequest">Broadcast transaction request</param>
        /// <returns>Broadcasted transaction response</returns>
        [HttpPost("broadcast")]
        [SwaggerOperation("BroadcastSignedTransaction")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        [ProducesResponseType(typeof(BlockchainErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> BroadcastSignedTransactionAsync([FromBody] BroadcastTransactionRequest broadcastTransactionRequest)
        {
            var result = await _transactionService.BroadcastSignedTransactionAsync(
                broadcastTransactionRequest.OperationId, broadcastTransactionRequest.SignedTransaction);

            if (result)
            {
                return StatusCode((int)HttpStatusCode.Conflict, ErrorResponse.Create("Transaction with specified operationId and signedTransaction has already been broadcasted"));
            }

            return Ok();
        }

        /// <summary>
        /// Get broadcasted transaction with single input and output
        /// </summary>
        /// <param name="operationId">Operation Id</param>
        /// <returns>Broadcasted transaction response</returns>
        [HttpGet("broadcast/single/{operationId}")]
        [SwaggerOperation("GetBroadcastedSingleTransaction")]
        [ProducesResponseType(typeof(BroadcastedSingleTransactionResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> GetBroadcastedSingleTransactionAsync(string operationId)
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
                return StatusCode((int)HttpStatusCode.NoContent, ErrorResponse.Create("Specified transaction not found"));
        }

        /// <summary>
        /// Remove specified transaction from the broadcasted transactions
        /// </summary>
        /// <param name="operationId">Operation Id</param>
        /// <returns>HttpStatusCode</returns>
        [HttpDelete("broadcast/{operationId}")]
        [SwaggerOperation("DeleteBroadcastedTransaction")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> DeleteBroadcastedTransactionAsync(string operationId)
        {
            TransactionObservation transactionObservation = new TransactionObservation
            {
                OperationId = new Guid(operationId)
            };
            if (await _transactionService.IsTransactionObservedAsync(transactionObservation) && await _transactionService.RemoveTransactionObservationAsync(transactionObservation))
                return Ok();
            else
                return StatusCode((int)HttpStatusCode.NoContent);
        }

        #region NotImplemented

        /// <summary>
        /// Build not signed transaction with many inputs
        /// </summary>
        /// <param name="buildTransactionRequest">Build transaction request</param>
        /// <returns>Build transaction response</returns>
        [HttpPost("many-inputs")]
        [SwaggerOperation("BuildNotSignedManyInputsTransaction")]
        [ProducesResponseType(typeof(BuildTransactionResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotImplemented)]
        [ProducesResponseType(typeof(BlockchainErrorResponse), (int)HttpStatusCode.BadRequest)]
        public IActionResult BuildNotSignedManyInputsTransaction([FromBody] BuildTransactionWithManyInputsRequest buildTransactionRequest)
        {
            return StatusCode((int)HttpStatusCode.NotImplemented);
        }

        /// <summary>
        /// Build not signed transaction with many outputs
        /// </summary>
        /// <param name="buildTransactionRequest">Build transaction request</param>
        /// <returns>Build transaction response</returns>
        [HttpPost("many-outputs")]
        [SwaggerOperation("BuildNotSignedManyOutputsTransaction")]
        [ProducesResponseType(typeof(BuildTransactionResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotImplemented)]
        [ProducesResponseType(typeof(BlockchainErrorResponse), (int)HttpStatusCode.BadRequest)]
        public IActionResult BuildNotSignedManyOutputsTransaction([FromBody] BuildTransactionWithManyOutputsRequest buildTransactionRequest)
        {
            return StatusCode((int)HttpStatusCode.NotImplemented);
        }

        /// <summary>
        ///  Rebuild not signed transaction with the specified fee factor
        /// </summary>
        /// <param name="rebuildTransactionRequest">Rebuild transaction request</param>
        /// <returns>Rebuild transaction response</returns>
        [HttpPut]
        [SwaggerOperation("RebuildNotSignedTransaction")]
        [ProducesResponseType(typeof(RebuildTransactionResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotImplemented)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotAcceptable)]
        [ProducesResponseType(typeof(BlockchainErrorResponse), (int)HttpStatusCode.BadRequest)]
        public IActionResult RebuildNotSignedTransaction([FromBody] RebuildTransactionRequest rebuildTransactionRequest)
        {
            return StatusCode((int)HttpStatusCode.NotImplemented);
        }

        /// <summary>
        /// Get broadcasted transaction with with many inputs
        /// </summary>
        /// <param name="operationId">Operation Id</param>
        /// <returns>Broadcasted transaction response</returns>
        [HttpGet("broadcast/many-inputs/{operationId}")]
        [SwaggerOperation("GetBroadcastedManyInputsTransaction")]
        [ProducesResponseType(typeof(BroadcastedTransactionWithManyInputsResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotImplemented)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(BlockchainErrorResponse), (int)HttpStatusCode.BadRequest)]
        public IActionResult GetBroadcastedManyInputsTransaction(string operationId)
        {
            return StatusCode((int)HttpStatusCode.NotImplemented);
        }


        /// <summary>
        /// Get broadcasted transaction with with many outputs
        /// </summary>
        /// <param name="operationId">Operation Id</param>
        /// <returns>Broadcasted transaction response</returns>
        [HttpGet("broadcast/many-outputs/{operationId}")]
        [SwaggerOperation("GetBroadcastedManyOutputsTransaction")]
        [ProducesResponseType(typeof(BroadcastedTransactionWithManyOutputsResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotImplemented)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(BlockchainErrorResponse), (int)HttpStatusCode.BadRequest)]
        public IActionResult GetBroadcastedManyOutputsTransaction(string operationId)
        {
            return StatusCode((int)HttpStatusCode.NotImplemented);
        }

        #endregion
    }
}

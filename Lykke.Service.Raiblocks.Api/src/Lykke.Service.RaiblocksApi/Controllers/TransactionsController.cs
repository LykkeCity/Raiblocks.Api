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
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.RaiblocksApi.Controllers
{
    [Route("api/[controller]")]
    public class TransactionsController : Controller
    {

        private readonly ITransactionService<TransactionBody, TransactionMeta, TransactionObservation> _transactionService;
        private readonly IAssetService _assetService;

        public TransactionsController(ITransactionService<TransactionBody, TransactionMeta, TransactionObservation> transactionService, IAssetService assetService)
        {
            _transactionService = transactionService;
            _assetService = assetService;
        }

        /// <summary>
        /// Build not signed transaction
        /// </summary>
        /// <param name="buildTransactionRequest">Build transaction request</param>
        /// <returns>Build transaction response</returns>
        [HttpPost]
        [SwaggerOperation("BuildNotSignedTransaction")]
        [ProducesResponseType(typeof(BuildTransactionResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotAcceptable)]
        public async Task<IActionResult> BuildNotSignedTransaction([FromBody] BuildTransactionRequest buildTransactionRequest)
        {
            //TODO: blockChain Request
            TransactionMeta transactionMeta = new TransactionMeta
            {
                OperationId = buildTransactionRequest.OperationId,
                FromAddress = buildTransactionRequest.FromAddress,
                ToAddress = buildTransactionRequest.ToAddress,
                AssetId = buildTransactionRequest.AssetId,
                Amount = buildTransactionRequest.Amount,
                IncludeFee = buildTransactionRequest.IncludeFee,
                State = TransactionState.NotSigned
            };
            await _transactionService.SaveTransactionMeta(transactionMeta);

            throw new NotImplementedException();

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
        public IActionResult RebuildNotSignedTransaction([FromBody] RebuildTransactionRequest rebuildTransactionRequest)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Broadcast the signed transaction
        /// </summary>
        /// <param name="broadcastTransactionRequest">Broadcast transaction request</param>
        /// <returns>Broadcasted transaction response</returns>
        [HttpPost("broadcast")]
        [SwaggerOperation("BroadcastSignedTransaction")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.Conflict)]
        public IActionResult BroadcastSignedTransaction([FromBody] BroadcastTransactionRequest broadcastTransactionRequest)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get broadcasted transaction
        /// </summary>
        /// <param name="operationId">Operation Id</param>
        /// <returns>Broadcasted transaction response</returns>
        [HttpGet("broadcast/{operationId}")]
        [SwaggerOperation("GetBroadcastedTransaction")]
        [ProducesResponseType(typeof(BroadcastedTransactionResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> GetBroadcastedTransaction(string operationId)
        {
            var txMeta = await _transactionService.GetTransactionMeta(operationId);
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

                return Ok(new BroadcastedTransactionResponse
                {
                    OperationId = txMeta.OperationId,
                    State = state,
                    Timestamp = (txMeta.CompleteTimestamp ?? txMeta.BroadcastTimestamp).Value,
                    Amount = txMeta.Amount,
                    Fee = "0",
                    Hash = txMeta.Hash,
                    Error = txMeta.Error
                });
            }
            else
                return StatusCode((int)HttpStatusCode.Conflict, ErrorResponse.Create("Specified transaction not found"));
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
        public IActionResult DeleteBroadcastedTransaction(string operationId)
        {
            throw new NotImplementedException();
        }
    }
}

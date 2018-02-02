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

namespace Lykke.Service.RaiblocksApi.Controllers
{
    [Route("api/[controller]")]
    public class TransactionsController : Controller
    {

        private readonly ITransactionService<TransactionBody, TransactionMeta, TransactionObservation> _transactionService;
        private readonly IAssetService _assetService;
        private readonly IBlockchainService _blockchainService;

        public TransactionsController(ITransactionService<TransactionBody, TransactionMeta, TransactionObservation> transactionService, IAssetService assetService, IBlockchainService blockchainService)
        {
            _transactionService = transactionService;
            _assetService = assetService;
            _blockchainService = blockchainService;
        }

        /// <summary>
        /// Build not signed transaction to transfer from the single source to the single destination
        /// </summary>
        /// <param name="buildTransactionRequest">Build transaction request</param>
        /// <returns>Build transaction response</returns>
        [HttpPost("single")]
        [SwaggerOperation("BuildNotSignedTransaction")]
        [ProducesResponseType(typeof(BuildTransactionResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotAcceptable)]
        public async Task<IActionResult> BuildNotSignedSingleTransaction([FromBody] BuildSingleTransactionRequest buildTransactionRequest)
        {
            TransactionBody transactionBody = await _transactionService.GetTransactionBodyById(buildTransactionRequest.OperationId);

            TransactionExecutionError? error = null;

            if(transactionBody == null)
            {
                TransactionMeta transactionMeta = new TransactionMeta
                {
                    OperationId = buildTransactionRequest.OperationId,
                    FromAddress = buildTransactionRequest.FromAddress,
                    ToAddress = buildTransactionRequest.ToAddress,
                    AssetId = buildTransactionRequest.AssetId,
                    Amount = buildTransactionRequest.Amount,
                    IncludeFee = buildTransactionRequest.IncludeFee,
                    State = TransactionState.NotSigned,
                    CreateTimestamp = DateTime.Now
                };

                var balance = await _blockchainService.GetAddressBalanceAsync(transactionMeta.ToAddress);
                if (BigInteger.Parse(balance) < BigInteger.Parse(transactionMeta.Amount))
                {
                    error = TransactionExecutionError.NotEnoughtBalance;
                }

                var unsignTransaction = await _blockchainService.CreateUnsignSendTransaction(transactionMeta.FromAddress, transactionMeta.ToAddress, transactionMeta.Amount);

                await _transactionService.SaveTransactionMeta(transactionMeta);

                transactionBody = new TransactionBody
                {
                    OperationId = buildTransactionRequest.OperationId,
                    UnsignedTransaction = unsignTransaction
                };

                await _transactionService.SaveTransactionBody(transactionBody);
            }
       
            return StatusCode((int)HttpStatusCode.OK, new BuildTransactionResponse
            {
                ErrorCode = error,
                TransactionContext = transactionBody.UnsignedTransaction
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
        [ProducesResponseType(typeof(BroadcastTransactionResponse), (int)HttpStatusCode.Conflict)]
        public async Task<IActionResult> BroadcastSignedTransactionAsync([FromBody] BroadcastTransactionRequest broadcastTransactionRequest)
        {
            TransactionBody transactionBody = await _transactionService.GetTransactionBodyById(broadcastTransactionRequest.OperationId);
            if (transactionBody == null)
            {
                transactionBody = new TransactionBody
                {
                    OperationId = broadcastTransactionRequest.OperationId
                };
            }

            transactionBody.SignedTransaction = broadcastTransactionRequest.SignedTransaction;

            await _transactionService.UpdateTransactionBodyAsync(transactionBody);

            var txMeta = await _transactionService.GetTransactionMeta(broadcastTransactionRequest.OperationId.ToString());

            if (txMeta.State == TransactionState.Breadcasted || txMeta.State == TransactionState.Confirmed)
            {
                return StatusCode((int)HttpStatusCode.Conflict, ErrorResponse.Create("Transaction with specified operationId and signedTransaction has already been broadcasted"));
            }

            var result = await _blockchainService.BroadcastSignedTransactionAsync(transactionBody.SignedTransaction);

            var response = new BroadcastTransactionResponse();
            if (result.error == null)
            {
                txMeta.Hash = result.hash;
                txMeta.State = TransactionState.Breadcasted;
                txMeta.BroadcastTimestamp = DateTime.Now;
            }
            else
            {
                txMeta.Error = result.error;
                txMeta.State = TransactionState.Failed;
                response.ErrorCode = TransactionExecutionError.Unknown;
            }

            await _transactionService.UpdateTransactionMeta(txMeta);

            return Ok(response);

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
        public async Task<IActionResult> GetBroadcastedSingleTransaction(string operationId)
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

                return Ok(new BroadcastedSingleTransactionResponse
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
        public IActionResult GetBroadcastedManyOutputsTransaction(string operationId)
        {
            return StatusCode((int)HttpStatusCode.NotImplemented);
        }

        #endregion
    }
}

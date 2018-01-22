using Lykke.Common.Api.Contract.Responses;
using Lykke.Service.BlockchainApi.Contract.Transactions;
using Lykke.Service.RaiblocksApi.AzureRepositories.Entities.Transactions;
using Lykke.Service.RaiblocksApi.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Lykke.Service.RaiblocksApi.Controllers
{
    [Route("api/[controller]")]
    public class TransactionsController : Controller
    {

        private readonly ITransactionService<TransactionBody, TransactionMeta, TransactionObservation> _balanceService;
        private readonly IAssetService _assetService;

        public TransactionsController(ITransactionService<TransactionBody, TransactionMeta, TransactionObservation> balanceService, IAssetService assetService)
        {
            _balanceService = balanceService;
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
        public IActionResult BuildNotSignedTransaction([FromBody] BuildTransactionRequest buildTransactionRequest)
        {
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
        public IActionResult GetBroadcastedTransaction(string operationId)
        {
            throw new NotImplementedException();
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

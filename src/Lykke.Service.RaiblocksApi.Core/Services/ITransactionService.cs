using Lykke.Service.RaiblocksApi.Core.Domain.Entities.Transactions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.RaiblocksApi.Core.Services
{
    public interface ITransactionService<TTransactionBody, TTransactionMeta, TTransactionObservation>
        where TTransactionBody : ITransactionBody
        where TTransactionMeta : ITransactionMeta
        where TTransactionObservation : ITransactionObservation
    {
        /// <summary>
        /// Get transaction meta by operation Id
        /// </summary>
        /// <param name="id">Operation Id</param>
        /// <returns>Transaction meta</returns>
        Task<TTransactionMeta> GetTransactionMetaAsync(string id);

        /// <summary>
        /// Save transaction meta
        /// </summary>
        /// <param name="transactionMeta">Transaction meta</param>
        /// <returns>true if created, false if existed before</returns>
        Task<bool> SaveTransactionMetaAsync(TTransactionMeta transactionMeta);

        /// <summary>
        /// Save transaction body
        /// </summary>
        /// <param name="transactionBody">Transaction body</param>
        /// <returns>true if created, false if existed before</returns>
        Task<bool> SaveTransactionBodyAsync(TTransactionBody transactionBody);

        /// <summary>
        /// Get transaction body by operation id
        /// </summary>
        /// <param name="operationId">Operation Id</param>
        /// <returns>Transaction body</returns>
        Task<TTransactionBody> GetTransactionBodyByIdAsync(Guid operationId);

        /// <summary>
        /// Update transaction body
        /// </summary>
        /// <param name="transactionBody">Transaction body</param>
        /// <returns>A Task object that represents the asynchronous operation</returns>
        Task UpdateTransactionBodyAsync(TTransactionBody transactionBody);

        /// <summary>
        /// Update transaction meta
        /// </summary>
        /// <param name="transactionMeta">Transaction body</param>
        /// <returns>A Task object that represents the asynchronous operation</returns>
        Task UpdateTransactionMeta(TTransactionMeta transactionMeta);

        /// <summary>
        /// Observe tansaction
        /// </summary>
        /// <param name="transactionObservation">Transaction observation</param>
        /// <returns>true if created, false if existed before</returns>
        Task<bool> CreateObservationAsync(TTransactionObservation transactionObservation);

        /// <summary>
        /// Check is transaction already observed
        /// </summary>
        /// <param name="transactionObservation">Transaction observation</param>
        /// <returns>true if already observed</returns>
        Task<bool> IsTransactionObservedAsync(TTransactionObservation transactionObservation);

        /// <summary>
        /// Stop observe trancaction
        /// </summary>
        /// <param name="transactionObservation"></param>
        /// <returns>A Task object that represents the asynchronous operation</returns>
        Task<bool> RemoveTransactionObservationAsync(TTransactionObservation transactionObservation);

        /// <summary>
        /// Get observed transaction
        /// </summary>
        /// <param name="pageSize">Abount of transaction observation</param>
        /// <param name="continuation">ontinuation data</param>
        /// <returns>ontinuation data and transaction observation</returns>
        Task<(string continuation, IEnumerable<TTransactionObservation> items)> GetTransactionObservationAsync(int pageSize, string continuation);

        /// <summary>
        /// Get new or exist unsigned transaction
        /// </summary>
        /// <param name="operationId">Operation Id</param>
        /// <param name="fromAddress">Address from</param>
        /// <param name="toAddress">Address to</param>
        /// <param name="amount">Amount</param>
        /// <param name="assetId">Asset Id</param>
        /// <param name="includeFee">Include fee</param>
        /// <returns>Unsigned transaction context</returns>
        Task<string> GetUnsignSendTransactionAsync(Guid operationId, string fromAddress, string toAddress,
            string amount, string assetId = "XBR", bool includeFee = false);

        /// <summary>
        /// Get new or exist unsigned recive transaction
        /// </summary>
        /// <param name="operationId">Operation Id</param>
        /// <param name="sendTransactionHash">Send transaction hash</param>
        /// <returns>Unsigned transaction context</returns>
        Task<string> GetUnsignReciveTransactionAsync(Guid operationId, string sendTransactionHash);

        /// <summary>
        /// Publish signed transaction to network
        /// </summary>
        /// <param name="operationId">Operation Id</param>
        /// <param name="signedTransaction">Signed transaction</param>
        /// <returns>true if publish, false if already publish</returns>
        Task<bool> BroadcastSignedTransactionAsync(Guid operationId, string signedTransaction);

        /// <summary>
        /// Check is transaction already broacasted.
        /// </summary>
        /// <param name="operationId">Operation id.</param>
        /// <returns>true if broadcasted.</returns>
        Task<bool> IsTransactionAlreadyBroadcastAsync(Guid operationId);
    }
}

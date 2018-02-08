using Lykke.Service.RaiblocksApi.Core.Domain.Entities.Transactions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.RaiblocksApi.Core.Services
{
    public interface ITransactionService<TransactionBody, TransactionMeta, TransactionObservation>
        where TransactionBody : ITransactionBody
        where TransactionMeta : ITransactionMeta
        where TransactionObservation : ITransactionObservation
    {
        /// <summary>
        /// Get transaction meta by operation Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<TransactionMeta> GetTransactionMetaAsync(string id);

        /// <summary>
        /// Save transaction meta
        /// </summary>
        /// <param name="transactionMeta"></param>
        /// <returns></returns>
        Task<bool> SaveTransactionMetaAsync(TransactionMeta transactionMeta);

        /// <summary>
        /// Save transaction body
        /// </summary>
        /// <param name="transactionBody"></param>
        /// <returns></returns>
        Task<bool> SaveTransactionBodyAsync(TransactionBody transactionBody);

        /// <summary>
        /// Get transaction body by operation id
        /// </summary>
        /// <param name="operationId"></param>
        /// <returns></returns>
        Task<TransactionBody> GetTransactionBodyByIdAsync(Guid operationId);

        /// <summary>
        /// Update transaction body
        /// </summary>
        /// <param name="transactionBody"></param>
        /// <returns></returns>
        Task UpdateTransactionBodyAsync(TransactionBody transactionBody);

        /// <summary>
        /// Update transaction meta
        /// </summary>
        /// <param name="transactionMeta"></param>
        /// <returns></returns>
        Task UpdateTransactionMeta(TransactionMeta transactionMeta);

        /// <summary>
        /// Observe tansaction
        /// </summary>
        /// <param name="transactionObservation"></param>
        /// <returns></returns>
        Task<bool> CreateObservationAsync(TransactionObservation transactionObservation);

        /// <summary>
        /// Check is transaction already observed
        /// </summary>
        /// <param name="transactionObservation"></param>
        /// <returns></returns>
        Task<bool> IsTransactionObservedAsync(TransactionObservation transactionObservation);

        /// <summary>
        /// Stop observe trancaction
        /// </summary>
        /// <param name="transactionObservation"></param>
        /// <returns></returns>
        Task<bool> RemoveTransactionObservationAsync(TransactionObservation transactionObservation);

        /// <summary>
        /// Get observed transaction
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="continuation"></param>
        /// <returns></returns>
        Task<(string continuation, IEnumerable<TransactionObservation> items)> GetTransactionObservationAsync(int pageSize, string continuation);
    }
}

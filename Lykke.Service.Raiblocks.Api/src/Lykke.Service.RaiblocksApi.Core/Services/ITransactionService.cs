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
        /// <param name="id"></param>
        /// <returns></returns>
        Task<TTransactionMeta> GetTransactionMetaAsync(string id);

        /// <summary>
        /// Save transaction meta
        /// </summary>
        /// <param name="transactionMeta"></param>
        /// <returns></returns>
        Task<bool> SaveTransactionMetaAsync(TTransactionMeta transactionMeta);

        /// <summary>
        /// Save transaction body
        /// </summary>
        /// <param name="transactionBody"></param>
        /// <returns></returns>
        Task<bool> SaveTransactionBodyAsync(TTransactionBody transactionBody);

        /// <summary>
        /// Get transaction body by operation id
        /// </summary>
        /// <param name="operationId"></param>
        /// <returns></returns>
        Task<TTransactionBody> GetTransactionBodyByIdAsync(Guid operationId);

        /// <summary>
        /// Update transaction body
        /// </summary>
        /// <param name="transactionBody"></param>
        /// <returns></returns>
        Task UpdateTransactionBodyAsync(TTransactionBody transactionBody);

        /// <summary>
        /// Update transaction meta
        /// </summary>
        /// <param name="transactionMeta"></param>
        /// <returns></returns>
        Task UpdateTransactionMeta(TTransactionMeta transactionMeta);

        /// <summary>
        /// Observe tansaction
        /// </summary>
        /// <param name="transactionObservation"></param>
        /// <returns></returns>
        Task<bool> CreateObservationAsync(TTransactionObservation transactionObservation);

        /// <summary>
        /// Check is transaction already observed
        /// </summary>
        /// <param name="transactionObservation"></param>
        /// <returns></returns>
        Task<bool> IsTransactionObservedAsync(TTransactionObservation transactionObservation);

        /// <summary>
        /// Stop observe trancaction
        /// </summary>
        /// <param name="transactionObservation"></param>
        /// <returns></returns>
        Task<bool> RemoveTransactionObservationAsync(TTransactionObservation transactionObservation);

        /// <summary>
        /// Get observed transaction
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="continuation"></param>
        /// <returns></returns>
        Task<(string continuation, IEnumerable<TTransactionObservation> items)> GetTransactionObservationAsync(int pageSize, string continuation);
    }
}

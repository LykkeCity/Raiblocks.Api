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
        Task<TransactionMeta> GetTransactionMeta(string id);
        Task<bool> SaveTransactionMeta(TransactionMeta transactionMeta);
        Task<bool> SaveTransactionBody(TransactionBody transactionBody);
        Task<TransactionBody> GetTransactionBodyById(Guid operationId);
        Task UpdateTransactionBodyAsync(TransactionBody transactionBody);
        Task UpdateTransactionMeta(TransactionMeta transactionMeta);
        Task<bool> CreateObservationAsync(TransactionObservation transactionObservation);
        Task<bool> IsTransactionObservedAsync(TransactionObservation transactionObservation);
        Task<bool> RemoveTransactionObservationAsync(TransactionObservation transactionObservation);
        Task<(string continuation, IEnumerable<TransactionObservation> items)> GetTransactionObservation(int pageSize, string continuation);
    }
}

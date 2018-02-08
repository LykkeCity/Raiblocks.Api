using Lykke.Service.RaiblocksApi.Core.Domain.Entities.Transactions;
using Lykke.Service.RaiblocksApi.Core.Repositories.Transactions;
using Lykke.Service.RaiblocksApi.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.RaiblocksApi.Services
{
    public class TransactionService<TransactionBody, TransactionMeta, TransactionObservation> : ITransactionService<TransactionBody, TransactionMeta, TransactionObservation>
        where TransactionBody : ITransactionBody
        where TransactionMeta : ITransactionMeta
        where TransactionObservation : ITransactionObservation
    {
        private readonly ITransactionBodyRepository<TransactionBody> _transactionBodyRepository;
        private readonly ITransactionMetaRepository<TransactionMeta> _transactionMetaRepository;
        private readonly ITransactionObservationRepository<TransactionObservation> _transactionObservationRepository;

        public TransactionService(ITransactionBodyRepository<TransactionBody> transactionBodyRepository, ITransactionMetaRepository<TransactionMeta> transactionMetaRepository, ITransactionObservationRepository<TransactionObservation> transactionObservationRepository)
        {
            _transactionBodyRepository = transactionBodyRepository;
            _transactionMetaRepository = transactionMetaRepository;
            _transactionObservationRepository = transactionObservationRepository;
        }

        public async Task<bool> CreateObservationAsync(TransactionObservation transactionObservation)
        {
            return await _transactionObservationRepository.CreateIfNotExistsAsync(transactionObservation);
        }

        public async Task<TransactionBody> GetTransactionBodyByIdAsync(Guid operationId)
        {
            return await _transactionBodyRepository.GetAsync(operationId.ToString());
        }

        public async Task<TransactionMeta> GetTransactionMetaAsync(string id)
        {
            return await _transactionMetaRepository.GetAsync(id);
        }

        public async Task<(string continuation, IEnumerable<TransactionObservation> items)> GetTransactionObservationAsync(int pageSize, string continuation)
        {
            return await _transactionObservationRepository.GetAsync(pageSize, continuation);
        }

        public async Task<bool> IsTransactionObservedAsync(TransactionObservation transactionObservation)
        {
            return await _transactionObservationRepository.IsExistAsync(transactionObservation);
        }

        public async Task<bool> RemoveTransactionObservationAsync(TransactionObservation transactionObservation)
        {
            return await _transactionObservationRepository.DeleteIfExistAsync(transactionObservation);
        }

        public async Task<bool> SaveTransactionBodyAsync(TransactionBody transactionBody)
        {
            return await _transactionBodyRepository.CreateIfNotExistsAsync(transactionBody);
        }

        public async Task<bool> SaveTransactionMetaAsync(TransactionMeta transactionMeta)
        {
            return await _transactionMetaRepository.CreateIfNotExistsAsync(transactionMeta);
        }

        public Task UpdateTransactionBodyAsync(TransactionBody transactionBody)
        {
            return _transactionBodyRepository.UpdateAsync(transactionBody);
        }

        public Task UpdateTransactionMeta(TransactionMeta transactionMeta)
        {
            return _transactionMetaRepository.UpdateAsync(transactionMeta);
        }

    }
}

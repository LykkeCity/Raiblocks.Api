using Lykke.Service.RaiblocksApi.Core.Domain.Entities.Transactions;
using Lykke.Service.RaiblocksApi.Core.Repositories.Transactions;
using Lykke.Service.RaiblocksApi.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.RaiblocksApi.Services
{
    public class TransactionService<T, P, Q> : ITransactionService<T, P, Q>
        where T : ITransactionBody
        where P : ITransactionMeta
        where Q : ITransactionObservation
    {
        private readonly ITransactionBodyRepository<T> _transactionBodyRepository;
        private readonly ITransactionMetaRepository<P> _transactionMetaRepository;
        private readonly ITransactionObservationRepository<Q> _transactionObservationRepository;

        public TransactionService(ITransactionBodyRepository<T> transactionBodyRepository, ITransactionMetaRepository<P> transactionMetaRepository, ITransactionObservationRepository<Q> transactionObservationRepository)
        {
            _transactionBodyRepository = transactionBodyRepository;
            _transactionMetaRepository = transactionMetaRepository;
            _transactionObservationRepository = transactionObservationRepository;
        }

        public async Task<T> GetTransactionBodyById(Guid operationId)
        {
            return await _transactionBodyRepository.GetAsync(operationId.ToString());
        }

        public async Task<P> GetTransactionMeta(string id)
        {
            return await _transactionMetaRepository.GetAsync(id);
        }

        public async Task<bool> SaveTransactionBody(T transactionBody)
        {
            return await _transactionBodyRepository.CreateIfNotExistsAsync(transactionBody);
        }

        public async Task<bool> SaveTransactionMeta(P transactionMeta)
        {
            return await _transactionMetaRepository.CreateIfNotExistsAsync(transactionMeta);
        }

        public Task UpdateTransactionBodyAsync(T transactionBody)
        {
            return _transactionBodyRepository.UpdateAsync(transactionBody);
        }

        public Task UpdateTransactionMeta(P transactionMeta)
        {
            return _transactionMetaRepository.UpdateAsync(transactionMeta);
        }
    }
}

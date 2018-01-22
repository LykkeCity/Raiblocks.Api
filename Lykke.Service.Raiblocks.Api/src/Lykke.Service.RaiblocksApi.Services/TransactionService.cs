using Lykke.Service.RaiblocksApi.Core.Repositories.Transactions;
using Lykke.Service.RaiblocksApi.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.RaiblocksApi.Services
{
    public class TransactionService<T,P,Q> : ITransactionService<T, P, Q>
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

        public async Task<P> GetTransactionMeta(string id)
        {
            return await _transactionMetaRepository.GetAsync(id);
        }
    }
}

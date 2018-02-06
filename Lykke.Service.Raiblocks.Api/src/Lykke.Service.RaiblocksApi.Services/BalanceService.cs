using Lykke.Service.RaiblocksApi.Core.Domain.Entities.Balances;
using Lykke.Service.RaiblocksApi.Core.Repositories.Balances;
using Lykke.Service.RaiblocksApi.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.RaiblocksApi.Services
{
    public class BalanceService<T, P> : IBalanceService<T, P>
    {
        private readonly IBalanceObservationRepository<T> _balanceObservationRepository;
        private readonly IAddressBalanceRepository<P> _addressBalanceRepository;

        public BalanceService(IBalanceObservationRepository<T> balanceObservationRepository, IAddressBalanceRepository<P> addressBalanceRepository)
        {
            this._balanceObservationRepository = balanceObservationRepository;
            this._addressBalanceRepository = addressBalanceRepository;
        }

        public BalanceService(IBalanceObservationRepository<T> balanceObservationRepository)
        {
            this._balanceObservationRepository = balanceObservationRepository;
        }
        public async Task<bool> AddBalanceObservation(T item)
        {
            return await _balanceObservationRepository.CreateIfNotExistsAsync(item);
        }

        public async Task<bool> RemoveBalanceObservation(T item)
        {
            return await _balanceObservationRepository.DeleteIfExistAsync(item);
        }

        public async Task<bool> IsBalanceObserved(T item)
        {
            return await _balanceObservationRepository.IsExistAsync(item);
        }

        public async Task<(string continuation, IEnumerable<P> items)> GetBalances(int take = 100, string continuation = null)
        {
            return await _addressBalanceRepository.GetAsync(take, continuation);
        }

        public async Task<(string continuation, IEnumerable<T> items)> GetBalancesObservation(int take = 100, string continuation = null)
        {
            return await _balanceObservationRepository.GetAsync(take, continuation);
        }

        public async Task<bool> AddBalance(P item)
        {
            return await _addressBalanceRepository.CreateIfNotExistsAsync(item);
        }

        public Task UpdateBalance(P item)
        {
            return _addressBalanceRepository.UpdateAsync(item);
        }

        public async Task<bool> IsBalanceExist(P item)
        {
            return await _addressBalanceRepository.IsExistAsync(item);
        }

    }
}

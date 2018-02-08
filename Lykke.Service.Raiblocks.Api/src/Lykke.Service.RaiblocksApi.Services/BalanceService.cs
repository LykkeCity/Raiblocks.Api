using Lykke.Service.RaiblocksApi.Core.Domain.Entities.Balances;
using Lykke.Service.RaiblocksApi.Core.Repositories.Balances;
using Lykke.Service.RaiblocksApi.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.RaiblocksApi.Services
{
    public class BalanceService<TBalanceObservation, TBalance> : IBalanceService<TBalanceObservation, TBalance>
        where TBalanceObservation : IBalanceObservation
        where TBalance : IAddressBalance
    {
        private readonly IBalanceObservationRepository<TBalanceObservation> _balanceObservationRepository;
        private readonly IAddressBalanceRepository<TBalance> _addressBalanceRepository;

        public BalanceService(IBalanceObservationRepository<TBalanceObservation> balanceObservationRepository, IAddressBalanceRepository<TBalance> addressBalanceRepository)
        {
            _balanceObservationRepository = balanceObservationRepository;
            _addressBalanceRepository = addressBalanceRepository;
        }

        public async Task<bool> AddBalanceObservationAsync(TBalanceObservation item)
        {
            return await _balanceObservationRepository.CreateIfNotExistsAsync(item);
        }

        public async Task<bool> RemoveBalanceObservationAsync(TBalanceObservation item)
        {
            return await _balanceObservationRepository.DeleteIfExistAsync(item);
        }

        public async Task<bool> IsBalanceObservedAsync(TBalanceObservation item)
        {
            return await _balanceObservationRepository.IsExistAsync(item);
        }

        public async Task<(string continuation, IEnumerable<TBalance> items)> GetBalancesAsync(int take = 100, string continuation = null)
        {
            return await _addressBalanceRepository.GetAsync(take, continuation);
        }

        public async Task<(string continuation, IEnumerable<TBalanceObservation> items)> GetBalancesObservationAsync(int take = 100, string continuation = null)
        {
            return await _balanceObservationRepository.GetAsync(take, continuation);
        }

        public async Task<bool> AddBalance(TBalance item)
        {
            return await _addressBalanceRepository.CreateIfNotExistsAsync(item);
        }

        public Task UpdateBalance(TBalance item)
        {
            return _addressBalanceRepository.UpdateAsync(item);
        }

        public async Task<bool> IsBalanceExistAsync(TBalance item)
        {
            return await _addressBalanceRepository.IsExistAsync(item);
        }

        public async Task<bool> RemoveBalancenAsync(TBalance item)
        {
            return await _addressBalanceRepository.DeleteIfExistAsync(item);
        }
    }
}

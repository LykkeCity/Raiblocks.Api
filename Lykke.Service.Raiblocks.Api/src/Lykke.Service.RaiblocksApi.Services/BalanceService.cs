using Lykke.Service.RaiblocksApi.Core.Domain.Entities.Balances;
using Lykke.Service.RaiblocksApi.Core.Repositories.Balances;
using Lykke.Service.RaiblocksApi.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.RaiblocksApi.Services
{
    public class BalanceService<BalanceObservation, Balance> : IBalanceService<BalanceObservation, Balance>
        where BalanceObservation : IBalanceObservation
        where Balance : IAddressBalance
    {
        private readonly IBalanceObservationRepository<BalanceObservation> _balanceObservationRepository;
        private readonly IAddressBalanceRepository<Balance> _addressBalanceRepository;

        public BalanceService(IBalanceObservationRepository<BalanceObservation> balanceObservationRepository, IAddressBalanceRepository<Balance> addressBalanceRepository)
        {
            this._balanceObservationRepository = balanceObservationRepository;
            this._addressBalanceRepository = addressBalanceRepository;
        }

        public BalanceService(IBalanceObservationRepository<BalanceObservation> balanceObservationRepository)
        {
            this._balanceObservationRepository = balanceObservationRepository;
        }
        public async Task<bool> AddBalanceObservation(BalanceObservation item)
        {
            return await _balanceObservationRepository.CreateIfNotExistsAsync(item);
        }

        public async Task<bool> RemoveBalanceObservation(BalanceObservation item)
        {
            return await _balanceObservationRepository.DeleteIfExistAsync(item);
        }

        public async Task<bool> IsBalanceObserved(BalanceObservation item)
        {
            return await _balanceObservationRepository.IsExistAsync(item);
        }

        public async Task<(string continuation, IEnumerable<Balance> items)> GetBalances(int take = 100, string continuation = null)
        {
            return await _addressBalanceRepository.GetAsync(take, continuation);
        }

        public async Task<(string continuation, IEnumerable<BalanceObservation> items)> GetBalancesObservation(int take = 100, string continuation = null)
        {
            return await _balanceObservationRepository.GetAsync(take, continuation);
        }

        public async Task<bool> AddBalance(Balance item)
        {
            return await _addressBalanceRepository.CreateIfNotExistsAsync(item);
        }

        public Task UpdateBalance(Balance item)
        {
            return _addressBalanceRepository.UpdateAsync(item);
        }

        public async Task<bool> IsBalanceExist(Balance item)
        {
            return await _addressBalanceRepository.IsExistAsync(item);
        }

        public async Task<bool> RemoveBalancenAsync(Balance item)
        {
            return await _addressBalanceRepository.DeleteIfExistAsync(item);
        }
    }
}

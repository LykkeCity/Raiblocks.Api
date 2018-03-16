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

        /// <summary>
        /// Observe address balance
        /// </summary>
        /// <param name="item">Balance observation entity</param>
        /// <returns>true if created, false if existed before</returns>
        public async Task<bool> AddBalanceObservationAsync(TBalanceObservation item)
        {
            return await _balanceObservationRepository.CreateIfNotExistsAsync(item);
        }

        /// <summary>
        /// Stop observe address balance
        /// </summary>
        /// <param name="item">Balance observation entity</param>
        /// <returns>A Task object that represents the asynchronous operation</returns>
        public async Task<bool> RemoveBalanceObservationAsync(TBalanceObservation item)
        {
            return await _balanceObservationRepository.DeleteIfExistAsync(item);
        }

        /// <summary>
        /// Check is address balance already observed
        /// </summary>
        /// <param name="item">Balance observation entity</param>
        /// <returns>true if already observed</returns>
        public async Task<bool> IsBalanceObservedAsync(TBalanceObservation item)
        {
            return await _balanceObservationRepository.IsExistAsync(item);
        }

        /// <summary>
        /// Get balances
        /// </summary>
        /// <param name="take">Amount of balances</param>
        /// <param name="continuation">continuation data</param>
        /// <returns>continuation data and balances</returns>
        public async Task<(string continuation, IEnumerable<TBalance> items)> GetBalancesAsync(int take = 100, string continuation = null)
        {
            return await _addressBalanceRepository.GetAsync(take, continuation);
        }

        /// <summary>
        /// Get observed addresses
        /// </summary>
        /// <param name="take"></param>
        /// <param name="continuation"></param>
        /// <returns>A Task object that represents the asynchronous operation.</returns>
        public async Task<(string continuation, IEnumerable<TBalanceObservation> items)> GetBalancesObservationAsync(int take = 100, string continuation = null)
        {
            return await _balanceObservationRepository.GetAsync(take, continuation);
        }

        /// <summary>
        /// Save address balance
        /// </summary>
        /// <param name="item">Balance entity</param>
        /// <returns>true if created, false if existed before</returns>
        public async Task<bool> AddBalance(TBalance item)
        {
            return await _addressBalanceRepository.CreateIfNotExistsAsync(item);
        }

        /// <summary>
        /// Update address balance
        /// </summary>
        /// <param name="item">Balance entity</param>
        /// <returns>A Task object that represents the asynchronous operation</returns>
        public Task UpdateBalance(TBalance item)
        {
            return _addressBalanceRepository.UpdateAsync(item);
        }

        /// <summary>
        /// Check is address balance exist
        /// </summary>
        /// <param name="item">Balance entity</param>
        /// <returns>true if exist</returns>
        public async Task<bool> IsBalanceExistAsync(TBalance item)
        {
            return await _addressBalanceRepository.IsExistAsync(item);
        }

        /// <summary>
        /// Remove address balance
        /// </summary>
        /// <param name="item">Balance entity</param>
        /// <returns>A Task object that represents the asynchronous operation</returns>
        public async Task<bool> RemoveBalanceAsync(TBalance item)
        {
            return await _addressBalanceRepository.DeleteIfExistAsync(item);
        }
    }
}

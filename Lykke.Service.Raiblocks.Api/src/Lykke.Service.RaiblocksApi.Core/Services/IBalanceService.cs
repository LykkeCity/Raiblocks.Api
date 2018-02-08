using Lykke.Service.RaiblocksApi.Core.Domain.Entities.Balances;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.RaiblocksApi.Core.Services
{
    public interface IBalanceService<TBalanceObservation, TBalance>
        where TBalanceObservation: IBalanceObservation
        where TBalance : IAddressBalance
    {
        /// <summary>
        /// Observe address balance
        /// </summary>
        /// <param name="item">Balance observation entity</param>
        /// <returns>true if created, false if existed before</returns>
        Task<bool> AddBalanceObservationAsync(TBalanceObservation item);

        /// <summary>
        /// Save address balance
        /// </summary>
        /// <param name="item">Balance entity</param>
        /// <returns>true if created, false if existed before</returns>
        Task<bool> AddBalance(TBalance item);

        /// <summary>
        /// Stop observe address balance
        /// </summary>
        /// <param name="item">Balance observation entity</param>
        /// <returns>A Task object that represents the asynchronous operation.</returns>
        Task<bool> RemoveBalanceObservationAsync(TBalanceObservation item);

        /// <summary>
        /// Remove address balance
        /// </summary>
        /// <param name="item">Balance entity</param>
        /// <returns>A Task object that represents the asynchronous operation.</returns>
        Task<bool> RemoveBalanceAsync(TBalance item);

        /// <summary>
        /// Check is address balance already observed
        /// </summary>
        /// <param name="item">Balance observation entity</param>
        /// <returns>true if already observed</returns>
        Task<bool> IsBalanceObservedAsync(TBalanceObservation item);

        /// <summary>
        /// Get balances
        /// </summary>
        /// <param name="take">Amount of balances</param>
        /// <param name="continuation">continuation data</param>
        /// <returns>continuation data and balances</returns>
        Task<(string continuation, IEnumerable<TBalance> items)> GetBalancesAsync(int take = 100, string continuation = null);

        /// <summary>
        /// Get observed addresses
        /// </summary>
        /// <param name="take"></param>
        /// <param name="continuation"></param>
        /// <returns>A Task object that represents the asynchronous operation.</returns>
        Task<(string continuation, IEnumerable<TBalanceObservation> items)> GetBalancesObservationAsync(int take = 100, string continuation = null);

        /// <summary>
        /// Update address balance
        /// </summary>
        /// <param name="item">Balance entity</param>
        /// <returns>A Task object that represents the asynchronous operation.</returns>
        Task UpdateBalance(TBalance item);

        /// <summary>
        /// Check is address balance exist
        /// </summary>
        /// <param name="item">Balance entity</param>
        /// <returns>true if exist</returns>
        Task<bool> IsBalanceExistAsync(TBalance item);
    }
}

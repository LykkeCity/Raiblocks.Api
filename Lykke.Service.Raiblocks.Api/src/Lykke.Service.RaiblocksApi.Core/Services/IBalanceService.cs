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
        /// <param name="item"></param>
        /// <returns></returns>
        Task<bool> AddBalanceObservationAsync(TBalanceObservation item);

        /// <summary>
        /// Save address balance
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        Task<bool> AddBalance(TBalance item);

        /// <summary>
        /// Stop observe address balance
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        Task<bool> RemoveBalanceObservationAsync(TBalanceObservation item);

        /// <summary>
        /// Remove address balance
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        Task<bool> RemoveBalancenAsync(TBalance item);

        /// <summary>
        /// Check is address balance already observed
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        Task<bool> IsBalanceObservedAsync(TBalanceObservation item);

        /// <summary>
        /// Get balances
        /// </summary>
        /// <param name="take"></param>
        /// <param name="continuation"></param>
        /// <returns></returns>
        Task<(string continuation, IEnumerable<TBalance> items)> GetBalancesAsync(int take = 100, string continuation = null);

        /// <summary>
        /// Get observed addresses
        /// </summary>
        /// <param name="take"></param>
        /// <param name="continuation"></param>
        /// <returns></returns>
        Task<(string continuation, IEnumerable<TBalanceObservation> items)> GetBalancesObservationAsync(int take = 100, string continuation = null);

        /// <summary>
        /// Update address balance
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        Task UpdateBalance(TBalance item);

        /// <summary>
        /// Check is address balance exist
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        Task<bool> IsBalanceExistAsync(TBalance item);
    }
}

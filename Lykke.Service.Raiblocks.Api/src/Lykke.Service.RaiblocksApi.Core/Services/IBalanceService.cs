using Lykke.Service.RaiblocksApi.Core.Domain.Entities.Balances;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.RaiblocksApi.Core.Services
{
    public interface IBalanceService<BalanceObservation, Balance>
        where BalanceObservation: IBalanceObservation
        where Balance : IAddressBalance
    {
        /// <summary>
        /// Observe address balance
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        Task<bool> AddBalanceObservation(BalanceObservation item);

        /// <summary>
        /// Save address balance
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        Task<bool> AddBalance(Balance item);

        /// <summary>
        /// Stop observe address balance
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        Task<bool> RemoveBalanceObservation(BalanceObservation item);

        /// <summary>
        /// Remove address balance
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        Task<bool> RemoveBalancenAsync(Balance item);

        /// <summary>
        /// Check is address balance already observed
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        Task<bool> IsBalanceObserved(BalanceObservation item);

        /// <summary>
        /// Get balances
        /// </summary>
        /// <param name="take"></param>
        /// <param name="continuation"></param>
        /// <returns></returns>
        Task<(string continuation, IEnumerable<Balance> items)> GetBalances(int take = 100, string continuation = null);

        /// <summary>
        /// Get observed addresses
        /// </summary>
        /// <param name="take"></param>
        /// <param name="continuation"></param>
        /// <returns></returns>
        Task<(string continuation, IEnumerable<BalanceObservation> items)> GetBalancesObservation(int take = 100, string continuation = null);

        /// <summary>
        /// Update address balance
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        Task UpdateBalance(Balance item);

        /// <summary>
        /// Check is address balance exist
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        Task<bool> IsBalanceExist(Balance item);
    }
}

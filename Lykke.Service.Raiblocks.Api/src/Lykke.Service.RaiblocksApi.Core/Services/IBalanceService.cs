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
        Task<bool> AddBalanceObservation(BalanceObservation item);

        Task<bool> AddBalance(Balance item);

        Task<bool> RemoveBalanceObservation(BalanceObservation item);

        Task<bool> RemoveBalancenAsync(Balance item);

        Task<bool> IsBalanceObserved(BalanceObservation item);

        Task<(string continuation, IEnumerable<Balance> items)> GetBalances(int take = 100, string continuation = null);

        Task<(string continuation, IEnumerable<BalanceObservation> items)> GetBalancesObservation(int take = 100, string continuation = null);

        Task UpdateBalance(Balance item);

        Task<bool> IsBalanceExist(Balance item);
    }
}

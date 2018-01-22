using Lykke.Service.RaiblocksApi.Core.Domain.Entities.Balances;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.RaiblocksApi.Core.Services
{
    public interface IBalanceService<T, P>
    {
        Task<bool> AddBalanceObservation(T item);

        Task<bool> RemoveBalanceObservation(T item);

        Task<bool> IsBalanceObserved(T item);

        Task<(string continuation, IEnumerable<P> items)> GetBalances(int take = 100, string continuation = null);
    }
}

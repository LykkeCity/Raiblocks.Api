using Lykke.Service.RaiblocksApi.Core.Domain.Entities.Balances;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.RaiblocksApi.Core.Services
{
    public interface IBlockchainService <T, P>
        where T: IAddressBalance
        where P: IBalanceObservation
    {
        Task<IEnumerable<T>> GetAddressBalances(IEnumerable<P> balanceObservation);
    }
}

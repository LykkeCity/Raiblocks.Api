using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.RaiblocksApi.Core.Services
{
    public interface IBlockchainService
    {
        Task<bool> AddBalanceObservation(string address);

        Task<bool> RemoveBalanceObservation(string address);

        Task<bool> IsBalanceObserved(string address);
    }
}

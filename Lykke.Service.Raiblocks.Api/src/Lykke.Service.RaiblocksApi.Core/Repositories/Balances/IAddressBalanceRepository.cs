using Lykke.Service.RaiblocksApi.Core.Domain.Entities.Balances;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.RaiblocksApi.Core.Repositories.Balances
{
    public interface IAddressBalanceRepository
    {
        Task<bool> CreateIfNotExistsAsync(string address, string balance);

        Task<bool> DeleteIfExistAsync(string address);

        Task<bool> IsExistAsync(string address);

        Task<(string continuation, IEnumerable<IAddressBalance> items)> GetAllAsync(int take = 100, string continuation = null);
    }
}

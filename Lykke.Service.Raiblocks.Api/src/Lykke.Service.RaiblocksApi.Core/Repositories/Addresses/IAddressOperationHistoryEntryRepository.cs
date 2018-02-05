using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.RaiblocksApi.Core.Repositories.Addresses
{
    public interface IAddressOperationHistoryEntryRepository<T> : IRepository<T>
    {
        Task<IEnumerable<T>> GetByAddressAsync(int take, string partitionKey, string address);
    }
}

using Lykke.Service.RaiblocksApi.Core.Domain.Entities.Addresses;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.RaiblocksApi.Core.Repositories.Addresses
{
    public interface IAddressHistoryEntryRepository<HistoryEntry> : IRepository<HistoryEntry>
        where HistoryEntry : IAddressHistoryEntry
    {
        Task<IEnumerable<HistoryEntry>> GetByAddressAsync(int take, string partitionKey, string address, long afterBlockCount = 0);
    }
}

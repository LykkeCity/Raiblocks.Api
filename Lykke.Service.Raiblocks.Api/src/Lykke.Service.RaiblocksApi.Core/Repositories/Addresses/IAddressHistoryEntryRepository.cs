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
        /// <summary>
        /// Return history entries for address after specific hash
        /// </summary>
        /// <param name="take">Amount of the returned history entry</param>
        /// <param name="partitionKey">PartitionKey for azure table storage</param>
        /// <param name="address">Address</param>
        /// <param name="afterBlockCount">Block hash</param>
        /// <returns>History entries for address after specific hash</returns>
        Task<IEnumerable<HistoryEntry>> GetByAddressAsync(int take, string partitionKey, string address, long afterBlockCount = 0);
    }
}

using Common.Log;
using Lykke.Service.RaiblocksApi.AzureRepositories.Entities.Addresses;
using Lykke.Service.RaiblocksApi.Core.Repositories.Addresses;
using Lykke.SettingsReader;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.RaiblocksApi.AzureRepositories.Repositories.Addresses
{
    public class AddressHistoryEntryRepository : AzureRepository<AddressHistoryEntry>, IAddressHistoryEntryRepository<AddressHistoryEntry>
    {
        public AddressHistoryEntryRepository(IReloadingManager<string> connectionStringManager, ILog log) : base(connectionStringManager, log)
        {
        }

        public override string DefaultPartitionKey()
        {
            return nameof(AddressHistoryEntry);
        }
    }
}

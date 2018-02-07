using Lykke.AzureStorage.Tables;
using Lykke.Service.RaiblocksApi.Core.Domain.Entities.Addresses;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.RaiblocksApi.AzureRepositories.Entities.Addresses
{
    public class AddressOperationHistoryEntry : AzureTableEntity, IAddressOperationHistoryEntry
    {
        public Guid OperationId { get; set; }

        [IgnoreProperty]
        public string Hash { get => RowKey; set => RowKey = value; }

        public string Address { get; set; }

        [IgnoreProperty]
        public AddressObservationType Type
        {
            get => (AddressObservationType)Enum.Parse(typeof(AddressObservationType), PartitionKey);
            set => PartitionKey = Enum.GetName(typeof(AddressObservationType), value);
        }
    }
}

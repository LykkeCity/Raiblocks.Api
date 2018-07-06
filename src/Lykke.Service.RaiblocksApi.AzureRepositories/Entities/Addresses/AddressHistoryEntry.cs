using Lykke.AzureStorage.Tables;
using Lykke.Service.RaiblocksApi.Core.Domain.Entities.Addresses;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;
using Lykke.Service.RaiblocksApi.Core.Services;

namespace Lykke.Service.RaiblocksApi.AzureRepositories.Entities.Addresses
{
    public class AddressHistoryEntry : AzureTableEntity, IAddressHistoryEntry
    {
        public string FromAddress { get; set; }

        public string ToAddress { get; set; }

        public DateTime? TransactionTimestamp { get; set; }

        [IgnoreProperty] public string AssetId { get; set; }

        public string Amount { get; set; }

        [IgnoreProperty]
        public string Hash
        {
            get => RowKey;
            set => RowKey = value;
        }

        public long BlockCount { get; set; }

        public TransactionType TransactionType { get; set; }

        [IgnoreProperty]
        public AddressObservationType Type
        {
            get => (AddressObservationType) Enum.Parse(typeof(AddressObservationType), PartitionKey);
            set => PartitionKey = Enum.GetName(typeof(AddressObservationType), value);
        }
    }
}

using Lykke.AzureStorage.Tables;
using Lykke.Service.RaiblocksApi.Core.Domain.Entities.Addresses;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.RaiblocksApi.AzureRepositories.Entities.Addresses
{
    public class AddressHistoryEntry : AzureTableEntity, IAddressHistoryEntry
    {
        [IgnoreProperty]
        public string OperationId { get => RowKey; set => RowKey = value; }

        public string FromAddress { get; set; }

        public string ToAddress { get; set; }

        public DateTime TransactionTimestamp { get; set; }
        
        [IgnoreProperty]
        public string AssetId { get; set; }

        public string Amount { get; set; }

        public string Hash { get; set; }

        public string BlockHash { get; set; }

    }
}

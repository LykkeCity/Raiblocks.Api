using Lykke.AzureStorage.Tables;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.RaiblocksApi.AzureRepositories.Entities.Addresses
{
    public class AddressHistoryEntry : AzureTableEntity
    {
        public string OperationId { get; set; }

        public string FromAddress { get; set; }

        public string ToAddress { get; set; }

        public DateTime TransactionTimestamp { get; set; }
        
        public string AssetId { get; set; }

        public string Amount { get; set; }

        public string Hash { get; set; }

        public string BlockHash { get; set; }

    }
}

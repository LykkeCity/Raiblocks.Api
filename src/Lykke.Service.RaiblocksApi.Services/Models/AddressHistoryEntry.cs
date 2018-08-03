using System;
using Lykke.Service.RaiblocksApi.Core.Domain.Entities.Addresses;
using Lykke.Service.RaiblocksApi.Core.Services;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.RaiblocksApi.Services.Models
{
    public class AddressHistoryEntry : IAddressHistoryEntry
    {
        public string FromAddress { get; set; }

        public string ToAddress { get; set; }

        public DateTime? TransactionTimestamp { get; set; }

        public string AssetId { get; set; }

        public string Amount { get; set; }

        public string Hash { get; set; }

        public long BlockCount { get; set; }

        public TransactionType TransactionType { get; set; }

        public AddressObservationType Type { get; set; }
    }
}

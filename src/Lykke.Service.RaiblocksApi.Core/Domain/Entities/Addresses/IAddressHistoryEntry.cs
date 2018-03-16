using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.RaiblocksApi.Core.Domain.Entities.Addresses
{
    public interface IAddressHistoryEntry
    {
        string FromAddress { get; set; }

        string ToAddress { get; set; }

        DateTime? TransactionTimestamp { get; set; }

        string AssetId { get; set; }

        string Amount { get; set; }

        string Hash { get; set; }

        Int64 BlockCount { get; set; }

        AddressObservationType Type { get; set; }
    }
}

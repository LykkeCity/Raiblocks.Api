using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.RaiblocksApi.Core.Domain.Entities.Transactions
{
    public interface ITransactionMeta
    {
        string OperationId { get; set; }

        string FromAddress { get; set; }

        string ToAddress { get; set; }

        string AssetId { get; set; }

        string Amount { get; set; }

        bool IncludeFee { get; set; }

        TransactionState State { get; set; }

        string Error { get; set; }

        string Hash { get; set; }

        DateTime CreateTimestamp { get; set; }

        DateTime SignTimestamp { get; set; }

        DateTime BroadcastTimestamp { get; set; }

        DateTime CompleteTimestamp { get; set; }
    }
}

using Lykke.AzureStorage.Tables;
using Lykke.Service.RaiblocksApi.Core.Domain.Entities.Transactions;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.RaiblocksApi.AzureRepositories.Entities.Transactions
{
    public class TransactionMeta : AzureTableEntity, ITransactionMeta
    {
        [IgnoreProperty]
        public Guid OperationId { get => new Guid(RowKey); set => RowKey = value.ToString(); }

        public string FromAddress { get; set; }

        public string ToAddress { get; set; }

        [IgnoreProperty]
        public string AssetId { get; set; }

        public string Amount { get; set; }

        public bool IncludeFee { get; set; }

        public TransactionState State { get; set; }

        public string Error { get; set; }

        public string Hash { get; set; }

        public DateTime? CreateTimestamp { get; set; }

        public DateTime? SignTimestamp { get; set; }

        public DateTime? BroadcastTimestamp { get; set; }

        public DateTime? CompleteTimestamp { get; set; }

        public long BlockCount { get; set; }
    }

}

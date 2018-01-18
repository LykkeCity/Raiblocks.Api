using Lykke.AzureStorage.Tables;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.RaiblocksApi.AzureRepositories.Entities.Transactions
{
    public class TransactionMeta : AzureTableEntity
    {
        public string OperationId { get; set; }

        public string FromAddress { get; set; }

        public string ToAddress { get; set; }

        public string AssetId { get; set; }

        public string Amount { get; set; }

        public bool IncludeFee { get; set; }

        public TransactionState State { get; set; }

        public string Error { get; set; }

        public string Hash { get; set; }

        public DateTime CreateTimestamp { get; set; }

        public DateTime SignTimestamp { get; set; }

        public DateTime BroadcastTimestamp { get; set; }

        public DateTime CompleteTimestamp { get; set; }


    }

}

using Lykke.AzureStorage.Tables;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.RaiblocksApi.AzureRepositories.Entities.Transactions
{
    public class TransactionBody : AzureTableEntity
    {
        public string FromAddress { get; set; }

        public string ToAddress { get; set; }

        public long AmountSatoshi { get; set; }

        public long FeeSatoshi { get; set; }

        public string Timestamp { get; set; }

        public string Hash { get; set; }

        public string Error { get; set; }
    }
}

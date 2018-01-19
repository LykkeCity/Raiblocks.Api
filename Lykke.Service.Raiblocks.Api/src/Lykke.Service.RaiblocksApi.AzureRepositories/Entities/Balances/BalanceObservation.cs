using Lykke.AzureStorage.Tables;
using Lykke.Service.RaiblocksApi.Core.Domain.Entities.Balances;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.RaiblocksApi.AzureRepositories.Entities.Balances
{
    public class BalanceObservation : AzureTableEntity, IBalanceObservation
    {
        public BalanceObservation()
        {
        }

        public BalanceObservation(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }

        public string Address { get; set; }
    }
}

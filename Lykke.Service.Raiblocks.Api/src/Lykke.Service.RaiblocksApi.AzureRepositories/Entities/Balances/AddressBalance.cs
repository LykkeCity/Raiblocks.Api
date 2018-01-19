using Lykke.AzureStorage.Tables;
using Lykke.Service.RaiblocksApi.Core.Domain.Entities.Balances;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.RaiblocksApi.AzureRepositories.Entities.Balances
{
    public class AddressBalance : AzureTableEntity, IAddressBalance
    {
        public AddressBalance()
        {
        }

        public AddressBalance(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }

        public AddressBalance(string partitionKey, string rowKey, string balance)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
            Balance = balance;
        }

        [IgnoreProperty]
        public string Address { get => RowKey; }

        [IgnoreProperty]
        public string AssetId { get => "123"; }

        public string Balance { get; set; }
    }
}

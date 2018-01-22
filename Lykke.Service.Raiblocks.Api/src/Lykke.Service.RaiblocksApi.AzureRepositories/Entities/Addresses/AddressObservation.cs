using Lykke.AzureStorage.Tables;
using Lykke.Service.RaiblocksApi.Core.Domain.Entities.Addresses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.RaiblocksApi.AzureRepositories.Entities.Addresses
{
    public class AddressObservation : AzureTableEntity, IAddressObservation
    {
        public AddressObservation()
        {
        }

        public AddressObservation(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }

        public string Address { get; set; }

        public AddressObservationType Type { get; set; }
    }
}

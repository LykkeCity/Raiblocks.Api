using Lykke.AzureStorage.Tables;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.RaiblocksApi.AzureRepositories.Entities.Addresses
{
    public class AddressObservation : AzureTableEntity
    {
        public string Address { get; set; }

        public AddressObservationType Type { get; set; }
    }
}

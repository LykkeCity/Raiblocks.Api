using Lykke.AzureStorage.Tables;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.RaiblocksApi.AzureRepositories.Entities.Balances
{
    public class AddressBalance : AzureTableEntity
    {
        public string Address { get; set; }

        public string AssetId { get; set; }

        public string Balance { get; set; }
    }
}

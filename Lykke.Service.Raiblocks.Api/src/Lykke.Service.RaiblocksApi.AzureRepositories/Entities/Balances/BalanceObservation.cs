using Lykke.AzureStorage.Tables;
using Lykke.Service.RaiblocksApi.Core.Domain.Entities.Balances;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.RaiblocksApi.AzureRepositories.Entities.Balances
{
    public class BalanceObservation : AzureTableEntity, IBalanceObservation
    {
        [IgnoreProperty]
        public string Address { get => RowKey; set => RowKey = value; }
    }
}

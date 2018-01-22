using Lykke.AzureStorage.Tables;
using Lykke.Service.RaiblocksApi.Core.Domain.Entities.Transactions;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.RaiblocksApi.AzureRepositories.Entities.Transactions
{
    public class TransactionObservation : AzureTableEntity, ITransactionObservation
    {
        [IgnoreProperty]
        public string OperationId { get => RowKey; set => RowKey = value; }
    }
}

using Lykke.AzureStorage.Tables;
using Lykke.Service.RaiblocksApi.Core.Domain.Entities.Transactions;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.RaiblocksApi.AzureRepositories.Entities.Transactions
{
    public class TransactionBody : AzureTableEntity, ITransactionBody
    {
        [IgnoreProperty]
        public Guid OperationId { get => new Guid(RowKey); set => RowKey = value.ToString(); }
        public string UnsignedTransaction { get; set; }
        public string SignedTransaction { get; set; }
    }
}

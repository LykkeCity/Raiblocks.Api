using System;
using Lykke.Service.RaiblocksApi.Core.Domain.Entities.Transactions;
using Lykke.Service.RaiblocksApi.Core.Services;

namespace Lykke.Service.RaiblocksApi.Services.Models
{
    public class TransactionMeta : ITransactionMeta
    {
        public Guid OperationId { get; set; }
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }
        public string AssetId { get; set; }
        public string Amount { get; set; }
        public bool IncludeFee { get; set; }
        public TransactionState State { get; set; }
        public string Error { get; set; }
        public string Hash { get; set; }
        public long BlockCount { get; set; }
        public TransactionType TransactionType { get; set; }
        public DateTime? CreateTimestamp { get; set; }
        public DateTime? SignTimestamp { get; set; }
        public DateTime? BroadcastTimestamp { get; set; }
        public DateTime? CompleteTimestamp { get; set; }
    }
}

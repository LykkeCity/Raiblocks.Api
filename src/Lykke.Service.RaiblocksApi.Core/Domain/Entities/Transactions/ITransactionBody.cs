using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.RaiblocksApi.Core.Domain.Entities.Transactions
{
    public interface ITransactionBody
    {
        Guid OperationId { get; set; }

        string UnsignedTransaction { get; set; }

        string SignedTransaction { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.RaiblocksApi.Core.Domain.Entities.Transactions
{
    public interface ITransactionObservation
    {
        string OperationId { get; set; }
    }
}

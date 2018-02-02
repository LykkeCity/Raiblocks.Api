using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.RaiblocksApi.Core.Domain.Entities.Transactions
{
    public interface ITransactionObservation
    {
        Guid OperationId { get; set; }
    }
}

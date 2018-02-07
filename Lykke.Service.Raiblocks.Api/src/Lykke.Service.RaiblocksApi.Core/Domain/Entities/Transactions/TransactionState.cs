using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.RaiblocksApi.Core.Domain.Entities.Transactions
{
    public enum TransactionState
    {
        NotSigned = 1,
        Signed = 2,
        Broadcasted = 3,
        Confirmed = 4,
        Failed = 5,
        BlockChainFailed = 6
    }
}

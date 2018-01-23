using Lykke.Service.RaiblocksApi.Core.Domain.Entities.Transactions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.RaiblocksApi.Core.Services
{
    public interface ITransactionService<T, P, Q> 
        where T: ITransactionBody
        where P: ITransactionMeta
        where Q: ITransactionObservation
    {
        Task<P> GetTransactionMeta(string id);
        Task<bool> SaveTransactionMeta(P transactionMeta);
    }
}

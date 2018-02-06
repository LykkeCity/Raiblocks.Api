﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.RaiblocksApi.Core.Repositories.Addresses
{
    public interface IAddressHistoryEntryRepository<TransactionBody> : IRepository<TransactionBody>
    {
        Task<IEnumerable<TransactionBody>> GetByAddressAsync(int take, string partitionKey, string address, long afterBlockCount = 0);
    }
}

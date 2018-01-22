using Common.Log;
using Lykke.Service.RaiblocksApi.AzureRepositories.Entities.Transactions;
using Lykke.Service.RaiblocksApi.Core.Repositories.Transactions;
using Lykke.SettingsReader;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.RaiblocksApi.AzureRepositories.Repositories.Transactions
{
    public class TransactionBodyRepository : AzureRepository<TransactionBody>, ITransactionBodyRepository<TransactionBody>
    {
        public TransactionBodyRepository(IReloadingManager<string> connectionStringManager, ILog log) : base(connectionStringManager, log)
        {
        }

        public override string DefaultPartitionKey()
        {
            return nameof(TransactionBody);
        }
    }
}

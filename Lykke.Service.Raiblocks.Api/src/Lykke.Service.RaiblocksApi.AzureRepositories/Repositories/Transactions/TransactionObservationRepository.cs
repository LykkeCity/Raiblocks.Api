using Common.Log;
using Lykke.Service.RaiblocksApi.AzureRepositories.Entities.Transactions;
using Lykke.Service.RaiblocksApi.Core.Repositories.Transactions;
using Lykke.SettingsReader;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.RaiblocksApi.AzureRepositories.Repositories.Transactions
{
    public class TransactionObservationRepository : AzureRepository<TransactionObservation>, ITransactionObservationRepository<TransactionObservation>
    {
        public TransactionObservationRepository(IReloadingManager<string> connectionStringManager, ILog log) : base(connectionStringManager, log)
        {
        }

        public override string DefaultPartitionKey()
        {
            return nameof(TransactionObservation);
        }
    }
}

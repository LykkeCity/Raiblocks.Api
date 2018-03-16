using AzureStorage;
using AzureStorage.Tables;
using Common.Log;
using Lykke.AzureStorage.Tables.Paging;
using Lykke.Service.RaiblocksApi.AzureRepositories.Entities.Balances;
using Lykke.Service.RaiblocksApi.Core.Domain.Entities.Balances;
using Lykke.Service.RaiblocksApi.Core.Repositories;
using Lykke.Service.RaiblocksApi.Core.Repositories.Balances;
using Lykke.SettingsReader;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.RaiblocksApi.AzureRepositories.Repositories.Balances
{
    public class BalanceObservationRepository : AzureRepository<BalanceObservation>, IBalanceObservationRepository<BalanceObservation>
    {
        public BalanceObservationRepository(IReloadingManager<string> connectionStringManager, ILog log) : base(connectionStringManager, log)
        {
        }

        public override string DefaultPartitionKey()
        {
            return nameof(BalanceObservation);
        }
    }
}

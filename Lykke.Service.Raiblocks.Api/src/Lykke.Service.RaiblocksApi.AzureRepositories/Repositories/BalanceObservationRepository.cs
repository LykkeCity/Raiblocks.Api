using AzureStorage;
using AzureStorage.Tables;
using Common.Log;
using Lykke.Service.RaiblocksApi.AzureRepositories.Entities.Balances;
using Lykke.Service.RaiblocksApi.Core.Repositories;
using Lykke.SettingsReader;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.RaiblocksApi.AzureRepositories.Repositories
{
    public class BalanceObservationRepository: IBalanceObservationRepository
    {
        private INoSQLTableStorage<BalanceObservation> _tableStorage;

        private static string GetPartitionKey() => nameof(BalanceObservation);
        private static string GetRowKey(string address) => address;

        public BalanceObservationRepository(IReloadingManager<string> connectionStringManager, ILog log)
        {
            _tableStorage = AzureTableStorage<BalanceObservation>.Create(connectionStringManager, "RaiblocksBalanceObservation", log);
        }

        public async Task<bool> CreateIfNotExistsAsync(string address)
        {
            var partitionKey = GetPartitionKey();
            var rowKey = GetRowKey(address);

            return await _tableStorage.CreateIfNotExistsAsync(new BalanceObservation(partitionKey, rowKey));
        }

    }
}

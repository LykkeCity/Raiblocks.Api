using AzureStorage;
using AzureStorage.Tables;
using Common.Log;
using Lykke.AzureStorage.Tables.Paging;
using Lykke.Service.RaiblocksApi.AzureRepositories.Entities.Balances;
using Lykke.Service.RaiblocksApi.Core.Domain.Entities.Balances;
using Lykke.Service.RaiblocksApi.Core.Repositories.Balances;
using Lykke.SettingsReader;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.RaiblocksApi.AzureRepositories.Repositories.Balances
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

        public async Task<bool> DeleteIfExistAsync(string address)
        {
            var partitionKey = GetPartitionKey();
            var rowKey = GetRowKey(address);

            return await _tableStorage.DeleteIfExistAsync(partitionKey, rowKey);
        }

        public async Task<bool> IsExistAsync(string address)
        {
            var partitionKey = GetPartitionKey();
            var rowKey = GetRowKey(address);

            return await _tableStorage.RecordExistsAsync(new BalanceObservation(partitionKey, rowKey));
        }

        public async Task<(string continuation, IEnumerable<IBalanceObservation> items)> GetAllAsync(int take = 100, string continuation = null)
        {
            var pagingInfo = new PagingInfo { ElementCount = take };

            pagingInfo.Decode(continuation);
            var query = new TableQuery<BalanceObservation>();
            var items = await _tableStorage.ExecuteQueryWithPaginationAsync(query, pagingInfo);

            return (items.PagingInfo.Encode(), items);
        }
    }
}

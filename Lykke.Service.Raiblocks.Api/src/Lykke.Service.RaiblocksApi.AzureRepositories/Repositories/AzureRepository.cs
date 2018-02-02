using AzureStorage;
using AzureStorage.Tables;
using Common.Log;
using Lykke.AzureStorage.Tables;
using Lykke.AzureStorage.Tables.Paging;
using Lykke.Service.RaiblocksApi.AzureRepositories.Entities;
using Lykke.Service.RaiblocksApi.Core.Repositories;
using Lykke.SettingsReader;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.RaiblocksApi.AzureRepositories.Repositories
{
    public abstract class AzureRepository<T> : IRepository<T> 
        where T : AzureTableEntity, new()
    {
        private INoSQLTableStorage<T> _tableStorage;

        public AzureRepository(IReloadingManager<string> connectionStringManager, ILog log)
        {
            _tableStorage = AzureTableStorage<T>.Create(connectionStringManager, this.GetType().Name, log);
        }

        public Task UpdateAsync(T item)
        {
            if (item.PartitionKey == null)
            {
                item.PartitionKey = DefaultPartitionKey();
            }
            return _tableStorage.InsertOrReplaceAsync(item);
        }

        public async Task<bool> CreateIfNotExistsAsync(T item)
        {
            if (item.PartitionKey == null)
            {
                item.PartitionKey = DefaultPartitionKey();
            }
            return await _tableStorage.CreateIfNotExistsAsync(item);
        }

        public async Task<bool> DeleteIfExistAsync(T item)
        {
            if (item.PartitionKey == null)
            {
                item.PartitionKey = DefaultPartitionKey();
            }
            return await _tableStorage.DeleteIfExistAsync(item.PartitionKey, item.RowKey);
        }


        public async Task<(string continuation, IEnumerable<T> items)> GetAsync(int take = 100, string continuation = null)
        {
            var result = await _tableStorage.GetDataWithContinuationTokenAsync(DefaultPartitionKey(), take, continuation);
            return (result.ContinuationToken, result.Entities);
        }

        public async Task<bool> IsExistAsync(T item)
        {
            if (item.PartitionKey == null)
            {
                item.PartitionKey = DefaultPartitionKey();
            }
            return await _tableStorage.RecordExistsAsync(item);
        }

        public abstract string DefaultPartitionKey();

        public async Task<T> GetAsync(string id)
        {
            return await _tableStorage.GetDataAsync(DefaultPartitionKey(), id);
        }
    }
}

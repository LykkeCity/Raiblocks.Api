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
    public class AddressBalanceRepository : IAddressBalanceRepository
    {
        private INoSQLTableStorage<AddressBalance> _tableStorage;

        private static string GetPartitionKey() => nameof(AddressBalance);
        private static string GetRowKey(string address) => address;

        public AddressBalanceRepository(IReloadingManager<string> connectionStringManager, ILog log)
        {
            _tableStorage = AzureTableStorage<AddressBalance>.Create(connectionStringManager, "RaiblocksAddressBalance", log);
        }

        public async Task<bool> CreateIfNotExistsAsync(string address, string balance)
        {
            var partitionKey = GetPartitionKey();
            var rowKey = GetRowKey(address);

            return await _tableStorage.CreateIfNotExistsAsync(new AddressBalance(partitionKey, rowKey, balance));
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

            return await _tableStorage.RecordExistsAsync(new AddressBalance(partitionKey, rowKey));
        }

        public async Task<(string continuation, IEnumerable<IAddressBalance> items)> GetAllAsync(int take = 100, string continuation = null)
        {
            var pagingInfo = new PagingInfo { ElementCount = take };

            pagingInfo.Decode(continuation);
            var query = new TableQuery<AddressBalance>();
            var items = await _tableStorage.ExecuteQueryWithPaginationAsync(query, pagingInfo);

            return (items.PagingInfo.Encode(), items);
        }
    }
}

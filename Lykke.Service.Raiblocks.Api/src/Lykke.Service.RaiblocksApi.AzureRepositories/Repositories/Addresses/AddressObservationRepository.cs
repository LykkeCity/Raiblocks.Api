using AzureStorage;
using AzureStorage.Tables;
using Common.Log;
using Lykke.AzureStorage.Tables.Paging;
using Lykke.Service.RaiblocksApi.AzureRepositories.Entities.Addresses;
using Lykke.Service.RaiblocksApi.Core.Domain.Entities.Addresses;
using Lykke.Service.RaiblocksApi.Core.Repositories.Addresses;
using Lykke.SettingsReader;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.RaiblocksApi.AzureRepositories.Repositories.Addresses
{
    //public class AddressObservationRepository : IAddressObservationRepository
    //{
    //    private INoSQLTableStorage<AddressObservation> _tableStorage;

    //    private static string GetPartitionKey(AddressObservationType addressObservationType) => Enum.GetName(typeof(AddressObservationType), addressObservationType);
    //    private static string GetRowKey(string address) => address;

    //    public AddressObservationRepository(IReloadingManager<string> connectionStringManager, ILog log)
    //    {
    //        _tableStorage = AzureTableStorage<AddressObservation>.Create(connectionStringManager, "RaiblocksAddressObservation", log);
    //    }

    //    public async Task<bool> CreateIfNotExistsAsync(string address, AddressObservationType addressObservationType)
    //    {
    //        var partitionKey = GetPartitionKey(addressObservationType);
    //        var rowKey = GetRowKey(address);

    //        return await _tableStorage.CreateIfNotExistsAsync(new AddressObservation(partitionKey, rowKey));
    //    }

    //    public async Task<bool> DeleteIfExistAsync(string address, AddressObservationType addressObservationType)
    //    {
    //        var partitionKey = GetPartitionKey(addressObservationType);
    //        var rowKey = GetRowKey(address);

    //        return await _tableStorage.DeleteIfExistAsync(partitionKey, rowKey);
    //    }

    //    public async Task<bool> IsExistAsync(string address, AddressObservationType addressObservationType)
    //    {
    //        var partitionKey = GetPartitionKey(addressObservationType);
    //        var rowKey = GetRowKey(address);

    //        return await _tableStorage.RecordExistsAsync(new AddressObservation(partitionKey, rowKey));
    //    }

    //    public async Task<(string continuation, IEnumerable<IAddressObservation> items)> GetAllAsync(int take = 100, string continuation = null)
    //    {
    //        var pagingInfo = new PagingInfo { ElementCount = take };

    //        pagingInfo.Decode(continuation);
    //        var query = new TableQuery<AddressObservation>();
    //        var items = await _tableStorage.ExecuteQueryWithPaginationAsync(query, pagingInfo);

    //        return (items.PagingInfo.Encode(), items);
    //    }
    //}
}

using Lykke.Service.RaiblocksApi.Core.Domain.Entities.Addresses;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.RaiblocksApi.Core.Services
{
    public interface IHistoryService<AddressHistory, AddressObservation, AddressOperation>
    {
        Task<bool> IsAddressObserved(AddressObservation addressObservation);
        Task<bool> AddAddressObservation(AddressObservation addressObservation);
        Task<bool> RemoveAddressObservation(AddressObservation addressObservation);
        Task<(string continuation, IEnumerable<AddressObservation> items)> GetAddressObservationAsync(int pageSize, string continuation = null, string partitionKey = null);
        Task<IEnumerable<AddressHistory>> GetAddressHistoryAsync(int take, string partitionKey, string address, string afterHash);
        Task<(string continuation, IEnumerable<AddressHistory> items)> GetAddressHistoryAsync(int take, string continuation, string partitionKey = null);
        Task<bool> InsertAddressHistoryObservation(AddressHistory addressHistoryEntry);
        Task<IEnumerable<AddressOperation>> GetAddressOperationHistoryAsync(int take, string partitionKey, string address);
        Task<bool> AddAddressOperationHistoryAsync(AddressOperation operationHistoryEntry);
    }
}

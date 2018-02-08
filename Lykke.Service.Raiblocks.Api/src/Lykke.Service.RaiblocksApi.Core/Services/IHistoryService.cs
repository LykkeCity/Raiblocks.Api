using Lykke.Service.RaiblocksApi.Core.Domain.Entities.Addresses;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.RaiblocksApi.Core.Services
{
    public interface IHistoryService<AddressHistory, AddressObservation, AddressOperation>
    {
        /// <summary>
        /// Check is address history already observed
        /// </summary>
        /// <param name="addressObservation"></param>
        /// <returns></returns>
        Task<bool> IsAddressObservedAsync(AddressObservation addressObservation);

        /// <summary>
        /// Observe address history
        /// </summary>
        /// <param name="addressObservation"></param>
        /// <returns></returns>
        Task<bool> AddAddressObservationAsync(AddressObservation addressObservation);

        /// <summary>
        /// Stop observe address history
        /// </summary>
        /// <param name="addressObservation"></param>
        /// <returns></returns>
        Task<bool> RemoveAddressObservationAsync(AddressObservation addressObservation);

        /// <summary>
        /// Get observed addresses
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="continuation"></param>
        /// <param name="partitionKey"></param>
        /// <returns></returns>
        Task<(string continuation, IEnumerable<AddressObservation> items)> GetAddressObservationAsync(int pageSize, string continuation = null, string partitionKey = null);

        /// <summary>
        /// Get stored address history after specific hash
        /// </summary>
        /// <param name="take"></param>
        /// <param name="partitionKey"></param>
        /// <param name="address"></param>
        /// <param name="afterHash"></param>
        /// <returns></returns>
        Task<IEnumerable<AddressHistory>> GetAddressHistoryAsync(int take, string partitionKey, string address, string afterHash);
       
        /// <summary>
        /// Get stored address history
        /// </summary>
        /// <param name="take"></param>
        /// <param name="continuation"></param>
        /// <param name="partitionKey"></param>
        /// <returns></returns>
        Task<(string continuation, IEnumerable<AddressHistory> items)> GetAddressHistoryAsync(int take, string continuation, string partitionKey = null);

        /// <summary>
        /// Save address history entry
        /// </summary>
        /// <param name="addressHistoryEntry"></param>
        /// <returns></returns>
        Task<bool> InsertAddressHistoryAsync(AddressHistory addressHistoryEntry);

        /// <summary>
        /// Get operation history
        /// </summary>
        /// <param name="take"></param>
        /// <param name="partitionKey"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        Task<IEnumerable<AddressOperation>> GetAddressOperationHistoryAsync(int take, string partitionKey, string address);

        /// <summary>
        /// Save operation history entry
        /// </summary>
        /// <param name="operationHistoryEntry"></param>
        /// <returns></returns>
        Task<bool> AddAddressOperationHistoryAsync(AddressOperation operationHistoryEntry);
    }
}

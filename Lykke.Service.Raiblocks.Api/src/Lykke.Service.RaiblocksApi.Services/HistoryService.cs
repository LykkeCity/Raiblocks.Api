using Lykke.Service.RaiblocksApi.Core.Domain.Entities.Addresses;
using Lykke.Service.RaiblocksApi.Core.Repositories.Addresses;
using Lykke.Service.RaiblocksApi.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.RaiblocksApi.Services
{
    public class HistoryService<TAddressHistory, TAddressObservation, TAddressOperation> : IHistoryService<TAddressHistory, TAddressObservation, TAddressOperation>
        where TAddressHistory : IAddressHistoryEntry
        where TAddressObservation : IAddressObservation
        where TAddressOperation : IAddressOperationHistoryEntry
    {
        private readonly IAddressHistoryEntryRepository<TAddressHistory> _addressHistoryEntryRepository;
        private readonly IAddressObservationRepository<TAddressObservation> _addressObservationRepository;
        private readonly IAddressOperationHistoryEntryRepository<TAddressOperation> _addressOperationHistoryEntryRepository;

        public HistoryService(IAddressHistoryEntryRepository<TAddressHistory> addressHistoryEntryRepository, IAddressObservationRepository<TAddressObservation> addressObservationRepository, IAddressOperationHistoryEntryRepository<TAddressOperation> addressOperationHistoryEntryRepository)
        {
            _addressHistoryEntryRepository = addressHistoryEntryRepository;
            _addressObservationRepository = addressObservationRepository;
            _addressOperationHistoryEntryRepository = addressOperationHistoryEntryRepository;
        }

        /// <summary>
        /// Observe address history
        /// </summary>
        /// <param name="addressObservation">Address observation entity</param>
        /// <returns>true if created, false if existed before</returns>
        public async Task<bool> AddAddressObservationAsync(TAddressObservation addressObservation)
        {
            return await _addressObservationRepository.CreateIfNotExistsAsync(addressObservation);
        }

        /// <summary>
        /// Save address history entry
        /// </summary>
        /// <param name="addressHistoryEntry">Address history entry</param>
        /// <returns>A Task object that represents the asynchronous operation.</returns>
        public async Task<bool> InsertAddressHistoryAsync(TAddressHistory addressHistoryEntry)
        {
            return await _addressHistoryEntryRepository.CreateIfNotExistsAsync(addressHistoryEntry);
        }

        /// <summary>
        /// Get stored address history
        /// </summary>
        /// <param name="take">Amount of history entries</param>
        /// <param name="continuation">continuation data</param>
        /// <param name="partitionKey">partition key for azure storage</param>
        /// <returns>Address history</returns>
        public async Task<(string continuation, IEnumerable<TAddressHistory> items)> GetAddressHistoryAsync(int take, string continuation, string partitionKey = null)
        {
            return await _addressHistoryEntryRepository.GetAsync(take, continuation, partitionKey);

        }

        /// <summary>
        /// Get stored address history after specific hash
        /// </summary>
        /// <param name="take">Amount of history entries</param>
        /// <param name="partitionKey">partition key for azure storage</param>
        /// <param name="address">Address</param>
        /// <param name="afterHash">Block hash</param>
        /// <returns>Address history</returns>
        public async Task<IEnumerable<TAddressHistory>> GetAddressHistoryAsync(int take, string partitionKey, string address, string afterHash)
        {
            if (address != null && partitionKey != null)
            {
                if (afterHash != null)
                {
                    var afterRecord = await _addressHistoryEntryRepository.GetAsync(afterHash, partitionKey);
                    var afterBlockCount = afterRecord.BlockCount;

                    return await _addressHistoryEntryRepository.GetByAddressAsync(take, partitionKey, address, afterBlockCount);
                }
                else
                {
                    return await _addressHistoryEntryRepository.GetByAddressAsync(take, partitionKey, address);
                }
            }
            else
            {
                throw new ArgumentException();
            }

        }

        /// <summary>
        /// Get observed addresses
        /// </summary>
        /// <param name="pageSize">Amount of address observation</param>
        /// <param name="continuation">continuation data</param>
        /// <param name="partitionKey">partition key for azure storage</param>
        /// <returns>continuation data and observerd addresses</returns>
        public async Task<(string continuation, IEnumerable<TAddressObservation> items)> GetAddressObservationAsync(int take, string continuation, string partitionKey = null)
        {
            return await _addressObservationRepository.GetAsync(take, continuation, partitionKey);
        }

        /// <summary>
        /// Check is address history already observed
        /// </summary>
        /// <param name="addressObservation">Address observation entity</param>
        /// <returns>true if already observed</returns>
        public async Task<bool> IsAddressObservedAsync(TAddressObservation addressObservation)
        {
            return await _addressObservationRepository.IsExistAsync(addressObservation);
        }

        /// <summary>
        /// Stop observe address history
        /// </summary>
        /// <param name="addressObservation"></param>
        /// <returns>A Task object that represents the asynchronous operation.</returns>
        public async Task<bool> RemoveAddressObservationAsync(TAddressObservation addressObservation)
        {
            return await _addressObservationRepository.DeleteIfExistAsync(addressObservation);
        }

        /// <summary>
        /// Get operation history
        /// </summary>
        /// <param name="take"></param>
        /// <param name="partitionKey"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public async Task<IEnumerable<TAddressOperation>> GetAddressOperationHistoryAsync(int take, string partitionKey, string address)
        {
            return await _addressOperationHistoryEntryRepository.GetByAddressAsync(take, partitionKey, address);
        }

        /// <summary>
        /// Save operation history entry
        /// </summary>
        /// <param name="operationHistoryEntry">Operation history entry</param>
        /// <returns>true if created, false if existed before</returns>
        public async Task<bool> AddAddressOperationHistoryAsync(TAddressOperation operationHistoryEntry)
        {
            return await _addressOperationHistoryEntryRepository.CreateIfNotExistsAsync(operationHistoryEntry);
        }
    }
}

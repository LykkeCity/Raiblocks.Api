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

        public async Task<bool> AddAddressObservationAsync(TAddressObservation addressObservation)
        {
            return await _addressObservationRepository.CreateIfNotExistsAsync(addressObservation);
        }

        public async Task<bool> InsertAddressHistoryAsync(TAddressHistory addressHistoryEntry)
        {
            return await _addressHistoryEntryRepository.CreateIfNotExistsAsync(addressHistoryEntry);
        }

        public async Task<(string continuation, IEnumerable<TAddressHistory> items)> GetAddressHistoryAsync(int take, string continuation, string partitionKey = null)
        {
            return await _addressHistoryEntryRepository.GetAsync(take, continuation, partitionKey);

        }

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

        public async Task<(string continuation, IEnumerable<TAddressObservation> items)> GetAddressObservationAsync(int take, string continuation, string partitionKey = null)
        {
            return await _addressObservationRepository.GetAsync(take, continuation, partitionKey);
        }

        public async Task<bool> IsAddressObservedAsync(TAddressObservation addressObservation)
        {
            return await _addressObservationRepository.IsExistAsync(addressObservation);
        }

        public async Task<bool> RemoveAddressObservationAsync(TAddressObservation addressObservation)
        {
            return await _addressObservationRepository.DeleteIfExistAsync(addressObservation);
        }

        public async Task<IEnumerable<TAddressOperation>> GetAddressOperationHistoryAsync(int take, string partitionKey, string address)
        {
            return await _addressOperationHistoryEntryRepository.GetByAddressAsync(take, partitionKey, address);
        }

        public async Task<bool> AddAddressOperationHistoryAsync(TAddressOperation operationHistoryEntry)
        {
            return await _addressOperationHistoryEntryRepository.CreateIfNotExistsAsync(operationHistoryEntry);
        }
    }
}

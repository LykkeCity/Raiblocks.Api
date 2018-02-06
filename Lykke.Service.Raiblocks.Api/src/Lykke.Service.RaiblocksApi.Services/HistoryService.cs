using Lykke.Service.RaiblocksApi.Core.Domain.Entities.Addresses;
using Lykke.Service.RaiblocksApi.Core.Repositories.Addresses;
using Lykke.Service.RaiblocksApi.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.RaiblocksApi.Services
{
    public class HistoryService<AddressHistory, AddressObservation, AddressOperation> : IHistoryService<AddressHistory, AddressObservation, AddressOperation>
        where AddressHistory : IAddressHistoryEntry
        where AddressObservation : IAddressObservation
        where AddressOperation : IAddressOperationHistoryEntry
    {
        private readonly IAddressHistoryEntryRepository<AddressHistory> _addressHistoryEntryRepository;
        private readonly IAddressObservationRepository<AddressObservation> _addressObservationRepository;
        private readonly IAddressOperationHistoryEntryRepository<AddressOperation> _addressOperationHistoryEntryRepository;

        public HistoryService(IAddressHistoryEntryRepository<AddressHistory> addressHistoryEntryRepository, IAddressObservationRepository<AddressObservation> addressObservationRepository, IAddressOperationHistoryEntryRepository<AddressOperation> addressOperationHistoryEntryRepository)
        {
            _addressHistoryEntryRepository = addressHistoryEntryRepository;
            _addressObservationRepository = addressObservationRepository;
            _addressOperationHistoryEntryRepository = addressOperationHistoryEntryRepository;
        }

        public async Task<bool> AddAddressObservation(AddressObservation addressObservation)
        {
            return await _addressObservationRepository.CreateIfNotExistsAsync(addressObservation);
        }

        public async Task<bool> InsertAddressHistoryObservation(AddressHistory addressHistoryEntry)
        {
            return await _addressHistoryEntryRepository.CreateIfNotExistsAsync(addressHistoryEntry);
        }

        public async Task<(string continuation, IEnumerable<AddressHistory> items)> GetAddressHistoryAsync(int take, string continuation, string partitionKey = null)
        {
            return await _addressHistoryEntryRepository.GetAsync(take, continuation, partitionKey);

        }

        public async Task<IEnumerable<AddressHistory>> GetAddressHistoryAsync(int take, string partitionKey, string address, string afterHash)
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

        public async Task<(string continuation, IEnumerable<AddressObservation> items)> GetAddressObservationAsync(int take, string continuation, string partitionKey = null)
        {
            return await _addressObservationRepository.GetAsync(take, continuation, partitionKey);
        }

        public async Task<bool> IsAddressObserved(AddressObservation addressObservation)
        {
            return await _addressObservationRepository.IsExistAsync(addressObservation);
        }

        public async Task<bool> RemoveAddressObservation(AddressObservation addressObservation)
        {
            return await _addressObservationRepository.DeleteIfExistAsync(addressObservation);
        }

        public async Task<IEnumerable<AddressOperation>> GetAddressOperationHistoryAsync(int take, string partitionKey, string address)
        {
            return await _addressOperationHistoryEntryRepository.GetByAddressAsync(take, partitionKey, address);
        }

        public async Task<bool> AddAddressOperationHistoryAsync(AddressOperation operationHistoryEntry)
        {
            return await _addressOperationHistoryEntryRepository.CreateIfNotExistsAsync(operationHistoryEntry);
        }
    }
}

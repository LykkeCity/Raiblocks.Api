using Lykke.Service.RaiblocksApi.Core.Domain.Entities.Addresses;
using Lykke.Service.RaiblocksApi.Core.Repositories.Addresses;
using Lykke.Service.RaiblocksApi.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.RaiblocksApi.Services
{
    public class HistoryService<T, P> : IHistoryService<T, P>
    {
        private readonly IAddressHistoryEntryRepository<T> _addressHistoryEntryRepository;
        private readonly IAddressObservationRepository<P> _addressObservationRepository;

        public HistoryService(IAddressHistoryEntryRepository<T> addressHistoryEntryRepository, IAddressObservationRepository<P> addressObservationRepository)
        {
            _addressHistoryEntryRepository = addressHistoryEntryRepository;
            _addressObservationRepository = addressObservationRepository;
        }

        public async Task<bool> AddAddressObservation(P addressObservation)
        {
            return await _addressObservationRepository.CreateIfNotExistsAsync(addressObservation);
        }

        public async Task<bool> IsAddressObserved(P addressObservation)
        {
            return await _addressObservationRepository.IsExistAsync(addressObservation);
        }

        public async Task<bool> RemoveAddressObservation(P addressObservation)
        {
            return await _addressObservationRepository.DeleteIfExistAsync(addressObservation);
        }
    }
}

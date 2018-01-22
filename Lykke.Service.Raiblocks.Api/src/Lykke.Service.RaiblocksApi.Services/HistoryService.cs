using Lykke.Service.RaiblocksApi.Core.Repositories.Addresses;
using Lykke.Service.RaiblocksApi.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;

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
    }
}

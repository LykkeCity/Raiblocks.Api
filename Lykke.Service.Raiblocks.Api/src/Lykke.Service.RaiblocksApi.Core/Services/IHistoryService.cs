using Lykke.Service.RaiblocksApi.Core.Domain.Entities.Addresses;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.RaiblocksApi.Core.Services
{
    public interface IHistoryService<T, P>
    {
        Task<bool> IsAddressObserved(P addressObservation);
        Task<bool> AddAddressObservation(P addressObservation);
        Task<bool> RemoveAddressObservation(P addressObservation);
    }
}

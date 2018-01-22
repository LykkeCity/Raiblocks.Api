using Lykke.Service.RaiblocksApi.Core.Domain.Entities.Addresses;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.RaiblocksApi.Core.Repositories.Addresses
{
    public interface IAddressObservationRepository
    {
        Task<bool> CreateIfNotExistsAsync(string address);

        Task<bool> DeleteIfExistAsync(string address);

        Task<bool> IsExistAsync(string address);

        Task<(string continuation, IEnumerable<IAddressObservation> items)> GetAllAsync(int take = 100, string continuation = null);
    }
}

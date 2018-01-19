using Lykke.Service.RaiblocksApi.Core.Domain.Entities.Balances;
using Lykke.Service.RaiblocksApi.Core.Repositories;
using Lykke.Service.RaiblocksApi.Core.Repositories.Balances;
using Lykke.Service.RaiblocksApi.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.RaiblocksApi.Services
{
    public class BlockchainService : IBlockchainService
    {
        private readonly IBalanceObservationRepository _balanceObservationRepository;
        private readonly IAddressBalanceRepository _addressBalanceRepository;

        public BlockchainService(IBalanceObservationRepository balanceObservationRepository, IAddressBalanceRepository addressBalanceRepository)
        {
            this._balanceObservationRepository = balanceObservationRepository;
            this._addressBalanceRepository = addressBalanceRepository;
        }
        public async Task<bool> AddBalanceObservation(string address)
        {
            return await _balanceObservationRepository.CreateIfNotExistsAsync(address);
        }

        public async Task<bool> RemoveBalanceObservation(string address)
        {
            return await _balanceObservationRepository.DeleteIfExistAsync(address);
        }

        public async Task<bool> IsBalanceObserved(string address)
        {
            return await _balanceObservationRepository.IsExistAsync(address);
        }

        public async Task<(string continuation, IEnumerable<IAddressBalance> items)> GetBalances(int take = 100, string continuation = null)
        {
            return await _addressBalanceRepository.GetAllAsync(take, continuation);
        }
    }
}

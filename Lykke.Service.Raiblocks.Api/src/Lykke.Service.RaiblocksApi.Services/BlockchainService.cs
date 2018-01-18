using Lykke.Service.RaiblocksApi.Core.Repositories;
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

        //public BlockchainService()
        //{
        //}

        public BlockchainService(IBalanceObservationRepository balanceObservationRepository)
        {
            this._balanceObservationRepository = balanceObservationRepository;
        }
        public async Task<bool> AddBalanceObservation(string address)
        {
            return await _balanceObservationRepository.CreateIfNotExistsAsync(address);
            //return false;
        }
    }
}

using Lykke.Service.RaiblocksApi.Core.Domain.Entities.Balances;
using Lykke.Service.RaiblocksApi.Core.Repositories;
using Lykke.Service.RaiblocksApi.Core.Repositories.Balances;
using Lykke.Service.RaiblocksApi.Core.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RaiBlocks;
using RaiBlocks.Actions;
using RaiBlocks.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.RaiblocksApi.Services
{
    public class RaiBlockchainService : IBlockchainService
    {

        private readonly RaiBlocksRpc _raiBlocksRpc;

        public RaiBlockchainService(RaiBlocksRpc raiBlocksRpc)
        {
            _raiBlocksRpc = raiBlocksRpc;
        }

        public async Task<string> CreateUnsignSendTransaction(string address, string destination, string amount)
        {
            var raiAddress = new RaiAddress(address);
            var raiDestination = new RaiAddress(destination);
            var accountInfo = await _raiBlocksRpc.GetAccountInformationAsync(raiAddress);

            return await Task.Run(() => JsonConvert.SerializeObject(new BlockCreate {
                Type = BlockCreate.BlockCreateType.send,
                AccountNumber = raiAddress,
                Destination = raiDestination,
                Balance = accountInfo.Balance,
                Amount = new RaiUnits.RaiRaw(amount),
                Previous = accountInfo.Frontier
            }));
        }

        public async Task<Dictionary<string, string>> GetAddressBalances(IEnumerable<string> balanceObservation)
        {
            IEnumerable<RaiAddress> accounts = balanceObservation.Select(x => new RaiAddress(x));
            var result = await _raiBlocksRpc.GetBalancesAsync(accounts);
            return result.Balances.ToDictionary(x => x.Key, x => x.Value.ToString());
        }

        public async Task<string> GetAddresBalance(string address)
        {
            var result = await _raiBlocksRpc.GetBalanceAsync(new RaiAddress(address));
            return result.Balance.ToString();
        }

        public async Task<bool> IsAddressValidAsync(string address)
        {
            try
            {
                var result = await _raiBlocksRpc.ValidateAccount(new RaiAddress(address));
                return result.IsValid();
            }
            catch (ArgumentException e)
            {
                return false;
            }

        }
    }
}

using Lykke.Service.RaiblocksApi.Core.Domain.Entities.Addresses;
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

            return await Task.Run(async () =>
            {
                var txContext = JObject.FromObject(new BlockCreate
                {
                    Type = BlockType.send,
                    AccountNumber = raiAddress,
                    Destination = raiDestination,
                    Balance = accountInfo.Balance,
                    Amount = new RaiUnits.RaiRaw(amount),
                    Previous = accountInfo.Frontier
                });
                var work = await _raiBlocksRpc.GetWorkAsync(accountInfo.Frontier);

                txContext.Add("work", work.Work);

                return txContext.ToString();
            });
        }

        public async Task<Dictionary<string, string>> GetAddressBalances(IEnumerable<string> balanceObservation)
        {
            IEnumerable<RaiAddress> accounts = balanceObservation.Select(x => new RaiAddress(x));
            var result = await _raiBlocksRpc.GetBalancesAsync(accounts);
            return result.Balances.ToDictionary(x => x.Key, x => x.Value.Balance.ToString());
        }

        public async Task<string> GetAddressBalanceAsync(string address)
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

        public async Task<long> GetAddressBlockCountAsync(string address)
        {
            var result = await _raiBlocksRpc.GetAccountBlockCountAsync(new RaiAddress(address));
            return result.BlockCount;
        }

        public async Task<(string, string)> BroadcastSignedTransactionAsync(string signedTransaction)
        {
            var result = await _raiBlocksRpc.ProcessBlockAsync(signedTransaction);
            return (result.Hash, result.Error);
        }

        public async Task<IEnumerable<(string from, string to, BigInteger amount, string hash)>> GetAddressHistoryAsync(string address, int take)
        {
            var result = await _raiBlocksRpc.GetAccountHistoryAsync(new RaiAddress(address), take);
            return result.Entries.Select(x => {
                if (x.Type == BlockType.send)
                {
                    return (address, x.RepresentativeBlock, x.Amount.Value, x.Frontier);
                } else if (x.Type == BlockType.receive) {
                    return (x.RepresentativeBlock, address, x.Amount.Value, x.Frontier);
                } else
                {
                    throw new Exception("Unknown history type");
                }
            });
        }

        public async Task<(string frontier, long blockCount)> GetAddressInfoAsync(string address)
        {
            var accountInfo = await _raiBlocksRpc.GetAccountInformationAsync(new RaiAddress(address));
            return (accountInfo.Frontier, accountInfo.BlockCount);
        }
    }
}

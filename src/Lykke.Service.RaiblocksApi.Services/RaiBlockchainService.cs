﻿using Lykke.Service.RaiblocksApi.Core.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Polly;
using RaiBlocks;
using RaiBlocks.Actions;
using RaiBlocks.ValueObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Numerics;
using System.Threading.Tasks;
using Lykke.Service.RaiblocksApi.Core.Domain.Entities.Addresses;
using Lykke.Service.RaiblocksApi.Core.Domain.Entities.Transactions;
using Lykke.Service.RaiblocksApi.Services.Models;

namespace Lykke.Service.RaiblocksApi.Services
{
    public class RaiBlockchainService : IBlockchainService
    {
        private readonly RaiBlocksRpc _raiBlocksRpc;
        private readonly IAssetService _assetService;

        private const int retryCount = 4;

        private const int retryTimeout = 1;

        private Policy policy = null;

        public RaiBlockchainService(RaiBlocksRpc raiBlocksRpc, IAssetService assetService)
        {
            _raiBlocksRpc = raiBlocksRpc;
            _assetService = assetService;

            policy = Policy
                .Handle<HttpRequestException>()
                .Or<TaskCanceledException>()
                .WaitAndRetryAsync(retryCount, retryAttempt => TimeSpan.FromSeconds(retryTimeout));
        }

        /// <summary>
        /// Simple check validity for signed transaction
        /// </summary>
        /// <param name="signedTransaction"></param>
        /// <returns>Validity</returns>
        public bool IsSignedTransactionValid(string signedTransaction)
        {
            try
            {
                // TODO: make better
                var a = JObject.Parse(signedTransaction);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<string> CreateUnsignSendTransactionAsync(string address, string destination, string amount)
        {
            var policyResult = policy.ExecuteAsync(async () =>
            {
                var raiAddress = new RaiAddress(address);
                var raiDestination = new RaiAddress(destination);
                var accountInfo = await _raiBlocksRpc.GetAccountInformationAsync(raiAddress, true);

                return await Task.Run(async () =>
                {
                    var txContext = JObject.FromObject(new UniversalBlockCreate()
                    {
                        AccountNumber = raiAddress,
                        RepresentativeNumber = accountInfo.Representative,
                        Link = raiDestination,
                        Balance = accountInfo.Balance - new RaiUnits.RaiRaw(amount),
                        Previous = accountInfo.Frontier,
                        Work = (await _raiBlocksRpc.GetWorkAsync(accountInfo.Frontier))?.Work
                    });

                    return txContext.ToString();
                });
            });

            return await policyResult;
        }


        public async Task<string> CreateUnsignReceiveTransactionAsync(string sendTransactionHash)
        {
            var policyResult = policy.ExecuteAsync(async () =>
            {
                var txMeta = await GetTransactionMetaAsync(sendTransactionHash);
                return await CreateUnsignReceiveTransactionAsync(txMeta);
            });

            return await policyResult;
        }

        public async Task<string> CreateUnsignReceiveTransactionAsync(ITransactionMeta txMeta)
        {
            var policyResult = policy.ExecuteAsync(async () =>
            {                
                var raiDestination = new RaiAddress(txMeta.ToAddress);
                
                var accountInfo = await _raiBlocksRpc.GetAccountInformationAsync(raiDestination, true);

                if (accountInfo.Error != null &&
                    accountInfo.Error.Equals("Account not found", StringComparison.InvariantCultureIgnoreCase))
                {
                    return await Task.Run(async () =>
                    {
                        var txContext = JObject.FromObject(new UniversalBlockCreate
                        {
                            AccountNumber = txMeta.ToAddress,
                            RepresentativeNumber =  txMeta.ToAddress,
                            Link = txMeta.Hash,
                            Balance = new RaiUnits.RaiRaw(txMeta.Amount),
                            Previous = "0",
                            Work = ( await _raiBlocksRpc.GetWorkAsync(
                                (await _raiBlocksRpc.GetAccountKeyAsync(raiDestination)).Key)).Work
                        });

                        return txContext.ToString();
                    });
                }
                else
                {
                    return await Task.Run(async () =>
                    {
                        var txContext = JObject.FromObject(new UniversalBlockCreate
                        {
                            AccountNumber = txMeta.ToAddress,
                            RepresentativeNumber =  accountInfo.Representative,
                            Link = txMeta.Hash,
                            Balance = accountInfo.Balance + new RaiUnits.RaiRaw(txMeta.Amount),
                            Previous = accountInfo.Frontier,
                            Work = (await _raiBlocksRpc.GetWorkAsync(accountInfo.Frontier)).Work
                            
                        });

                        return txContext.ToString();
                    });
                }
            });

            return await policyResult;
        }
        
        public async Task<Dictionary<string, string>> GetAddressBalancesAsync(IEnumerable<string> balanceObservation)
        {
            var policyResult = policy.ExecuteAsync(async () =>
            {
                IEnumerable<RaiAddress> accounts = balanceObservation.Select(x => new RaiAddress(x));
                var result = await _raiBlocksRpc.GetBalancesAsync(accounts);
                return result.Balances.ToDictionary(x => x.Key, x => x.Value.Balance.ToString());
            });

            return await policyResult;
        }

        public async Task<string> GetAddressBalanceAsync(string address)
        {
            var policyResult = policy.ExecuteAsync(async () =>
            {
                var result = await _raiBlocksRpc.GetBalanceAsync(new RaiAddress(address));
                return result.Balance.ToString();
            });

            return await policyResult;
        }

        public async Task<bool> IsAddressValidAsync(string address)
        {
            try
            {
                var policyResult = policy.ExecuteAsync(async () =>
                {
                    var result = await _raiBlocksRpc.ValidateAccountAsync(new RaiAddress(address));
                    return result.IsValid();
                });

                return await policyResult;
            }
            catch (ArgumentException e)
            {
                return false;
            }
        }

        public async Task<long> GetAddressBlockCountAsync(string address)
        {
            var policyResult = policy.ExecuteAsync(async () =>
            {
                var result = await _raiBlocksRpc.GetAccountBlockCountAsync(new RaiAddress(address));
                return result.BlockCount;
            });

            return await policyResult;
        }

        public async Task<(string hash, string error)> BroadcastSignedTransactionAsync(string signedTransaction)
        {
            var policyResult = policy.ExecuteAsync(async () =>
            {
                var result = await _raiBlocksRpc.ProcessBlockAsync(signedTransaction);
                return (result.Hash, result.Error);
            });

            return await policyResult;
        }

        public async Task<IEnumerable<(string from, string to, BigInteger amount, string hash, TransactionType type)>>
            GetAddressHistoryAsync(string address, int take)
        {
            var policyResult = policy.ExecuteAsync(async () =>
            {
                var result = await _raiBlocksRpc.GetAccountHistoryAsync(new RaiAddress(address), take);
                return result.Entries.Select(x =>
                {
                    if (x.Type == BlockType.send)
                    {
                        return (address, x.Account, x.Amount.Value, x.Frontier, TransactionType.send);
                    }
                    else if (x.Type == BlockType.receive)
                    {
                        return (x.Account, address, x.Amount.Value, x.Frontier, TransactionType.receive);
                    }
                    else
                    {
                        throw new Exception("Unknown history type");
                    }
                });
            });

            return await policyResult;
        }

        public async Task<(string frontier, long blockCount)> GetAddressInfoAsync(string address)
        {
            var policyResult = policy.ExecuteAsync(async () =>
            {
                var accountInfo = await _raiBlocksRpc.GetAccountInformationAsync(new RaiAddress(address));
                return (accountInfo.Frontier, accountInfo.BlockCount);
            });

            return await policyResult;
        }

        public async Task<ITransactionMeta> GetTransactionMetaAsync(string hash)
        {
            var policyResult = policy.ExecuteAsync(async () =>
            {
                var account = (await _raiBlocksRpc.GetBlockAccountAsync(hash)).Account;
                
                var raiSource = new RaiAddress(account);
                
                var accountHistory = (await _raiBlocksRpc.GetAccountHistoryAsync(raiSource, 1, hash)).Entries.First();
                
                var destination = accountHistory.Account;
                                
                return new TransactionMeta
                {
                    Amount = accountHistory.Amount.ToString(),
                    AssetId = _assetService.AssetId,
                    FromAddress = account,
                    ToAddress = destination,
                    Hash = hash,
                    IncludeFee = false,
                    TransactionType =
                        accountHistory.Type == BlockType.send ? TransactionType.send : TransactionType.receive
                };
            });

            return await policyResult;
        }

        public async Task<List<string>> GetPreviousBlocksAsync(string hash, long count)
        {
            var policyResult = policy.ExecuteAsync(async () =>
            {
                var chain =
                    (await _raiBlocksRpc.GetChainAsync(hash, count));
                return chain.Blocks;
            });

            return await policyResult;
        }

        public async Task<List<IAddressHistoryEntry>>
            GetAccountsPendingAsync(List<string> accounts, long count, bool source)
        {
            var policyResult = policy.ExecuteAsync(async () =>
            {
                var result = new List<IAddressHistoryEntry>();
                if (accounts.Count != 0)
                {
                    var pendingInfo =
                        (await _raiBlocksRpc.GetAccountsPendingAsync(accounts, count, source));


                    foreach (var account in pendingInfo.Blocks)
                    {
                        if (account.Value != null)
                        {
                            foreach (var block in account.Value)
                            {
                                result.Add(new AddressHistoryEntry
                                {
                                    FromAddress = block.Value.Source,
                                    ToAddress = account.Key,
                                    Amount = block.Value.Amount.ToString(),
                                    Hash = block.Key,
                                    Type = AddressObservationType.To,
                                    BlockCount = long.MaxValue,
                                    TransactionType = TransactionType.send
                                });
                            }
                        }
                    }
                }

                return result;
            });

            return await policyResult;
        }

        public bool IsAddressValidOfflineAsync(string address)
        {
            try
            {
                var raiAddress = new RaiAddress(address);
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }
    }
}

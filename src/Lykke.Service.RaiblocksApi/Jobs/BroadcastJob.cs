using Common.Log;
using Lykke.JobTriggers.Triggers.Attributes;
using Lykke.Service.RaiblocksApi.AzureRepositories.Entities.Addresses;
using Lykke.Service.RaiblocksApi.AzureRepositories.Entities.Transactions;
using Lykke.Service.RaiblocksApi.Core.Domain.Entities.Transactions;
using Lykke.Service.RaiblocksApi.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Lykke.Service.RaiblocksApi.Core.Domain.Entities.Addresses;

namespace Lykke.Service.RaiblocksApi.Jobs
{
    public class BroadcastJob
    {
        private readonly ILog _log;
        private readonly IBlockchainService _blockchainService;
        private readonly ITransactionService<TransactionBody, TransactionMeta, TransactionObservation> _transactionService;
        private readonly IHistoryService<AddressHistoryEntry, AddressObservation, AddressOperationHistoryEntry> _historyService;

        private const int pageSize = 10;

        public BroadcastJob(ILog log, IBlockchainService blockchainService, ITransactionService<TransactionBody, TransactionMeta, TransactionObservation> transactionService, IHistoryService<AddressHistoryEntry, AddressObservation, AddressOperationHistoryEntry> historyService)
        {
            _log = log;
            _blockchainService = blockchainService;
            _transactionService = transactionService;
            _historyService = historyService;
        }

        /// <summary>
        /// Job for publish signed transaction
        /// </summary>
        /// <returns></returns>
        [TimerTrigger("00:00:10")]
        public async Task BroadcastTransactionsAsync()
        {
            await _log.WriteInfoAsync(nameof(HistoryRefreshJob), $"Env: {Program.EnvInfo}", "Broadcast start", DateTime.Now);

            (string continuation, IEnumerable<TransactionObservation> items) transactionObservations;
            string continuation = null;

            do
            {
                transactionObservations = await _transactionService.GetTransactionObservationAsync(pageSize, continuation);

                foreach (var transactionObservation in transactionObservations.items)
                {
                    var txMeta = await _transactionService.GetTransactionMetaAsync(transactionObservation.OperationId.ToString());

                    if (txMeta.State == TransactionState.Signed)
                    {
                        var txBody = await _transactionService.GetTransactionBodyByIdAsync(transactionObservation.OperationId);
                        var broadcactResult = await _blockchainService.BroadcastSignedTransactionAsync(txBody.SignedTransaction);

                        if (broadcactResult.error != null) {
                            txMeta.Error = broadcactResult.error;
                            txMeta.State = TransactionState.Failed;
                        } else
                        {
                            var address = txMeta.TransactionType == TransactionType.send
                                ? txMeta.FromAddress
                                : txMeta.ToAddress;
                            
                            var blockHeigth = await GetBlockHeigthAsync(address, broadcactResult.hash);

                            if (blockHeigth != null)
                            {
                                txMeta.State = TransactionState.Confirmed;
                                txMeta.Hash = broadcactResult.hash;
                                AddressOperationHistoryEntry operationHistoryEntry = new AddressOperationHistoryEntry
                                {
                                    OperationId = transactionObservation.OperationId,
                                    Hash = broadcactResult.hash,  
                                    Address = address,
                                    Type = txMeta.TransactionType == TransactionType.send ? AddressObservationType.From : AddressObservationType.To,
                                    TransactionTimestamp = DateTime.Now
                                };
                                txMeta.CompleteTimestamp = operationHistoryEntry.TransactionTimestamp;
                                txMeta.BlockCount = blockHeigth.Value;
                                await _historyService.AddAddressOperationHistoryAsync(operationHistoryEntry);
                            }
                            else
                            {
                                txMeta.Error = "Unknown block number/height";
                                txMeta.State = TransactionState.Failed;
                            }                      
                        }
                        await _transactionService.UpdateTransactionMeta(txMeta);
                    }
                };

                continuation = transactionObservations.continuation;
            } while (continuation != null);

            await _log.WriteInfoAsync(nameof(HistoryRefreshJob), $"Env: {Program.EnvInfo}", "Broadcasts finished", DateTime.Now);
        }

        public async Task<long?> GetBlockHeigthAsync(string address, string hash)
        {
            var addressInfo = await _blockchainService.GetAddressInfoAsync(address);
            if (addressInfo.frontier.Equals(hash))
            {
                return addressInfo.blockCount;
            }
            else
            {
                return (await _blockchainService.GetPreviousBlocksAsync(hash, -1))?.Count;
            }
        }
    }
}

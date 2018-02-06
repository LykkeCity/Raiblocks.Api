using Common.Log;
using Lykke.JobTriggers.Triggers.Attributes;
using Lykke.Service.RaiblocksApi.AzureRepositories.Entities.Addresses;
using Lykke.Service.RaiblocksApi.Core.Domain.Entities.Addresses;
using Lykke.Service.RaiblocksApi.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lykke.Service.RaiblocksApi.Jobs
{
    public class HistoryRefreshJob
    {
        private readonly ILog _log;
        private readonly IBlockchainService _blockchainService;
        private readonly IHistoryService<AddressHistoryEntry, AddressObservation, AddressOperationHistoryEntry> _historyService;

        private const int pageSize = 10;

        public HistoryRefreshJob(ILog log, IBlockchainService blockchainService, IHistoryService<AddressHistoryEntry, AddressObservation, AddressOperationHistoryEntry> historyService)
        {
            _log = log;
            _blockchainService = blockchainService;
            _historyService = historyService;
        }

        [TimerTrigger("00:00:10")]
        public async Task RefreshHistory()
        {
            await _log.WriteInfoAsync(nameof(HistoryRefreshJob), $"Env: {Program.EnvInfo}", "History balances start", DateTime.Now);

            (string continuation, IEnumerable<AddressObservation> items) addressObservations;
            string continuation = null;

            do
            {
                addressObservations = await _historyService.GetAddressObservationAsync(pageSize, continuation, Enum.GetName(typeof(AddressObservationType),AddressObservationType.From));

                continuation = addressObservations.continuation;

                foreach (var addressObservation in addressObservations.items)
                {
                    var addressInfo = await _blockchainService.GetAddressInfoAsync(addressObservation.Address);

                    var addressBlockCount = (int)addressInfo.blockCount;

                    if (addressBlockCount > 0)
                    {
                        var currentHistoryTo = await _historyService.GetAddressHistoryAsync(addressBlockCount, null, Enum.GetName(typeof(AddressObservationType), AddressObservationType.To));
                        var currentHistoryFrom = await _historyService.GetAddressHistoryAsync(addressBlockCount, null, Enum.GetName(typeof(AddressObservationType), AddressObservationType.From));

                        if (currentHistoryTo.continuation != null || currentHistoryFrom.continuation != null)
                        {
                            await _log.WriteErrorAsync(nameof(HistoryRefreshJob), $"Env: {Program.EnvInfo}", $"Block count less then db records. Address: {addressObservation.Address}", null);
                        }

                        var latestBlockNum = currentHistoryTo.items.Count() + currentHistoryFrom.items.Count();

                        if (addressBlockCount > latestBlockNum)
                        {
                            var history = await _blockchainService.GetAddressHistoryAsync(addressObservation.Address, addressBlockCount - latestBlockNum);

                            var addressHistoryEntries = history.Reverse().Select((x, index) => new AddressHistoryEntry
                            {
                                FromAddress = x.from,
                                ToAddress = x.to,
                                Amount = x.amount.ToString(),
                                Hash = x.hash,
                                Type = x.from == addressObservation.Address ? AddressObservationType.From : AddressObservationType.To,
                                BlockCount = index + latestBlockNum + 1
                            });

                            foreach (var addressHistoryEntry in addressHistoryEntries)
                            {
                                var result = _historyService.InsertAddressHistoryObservation(addressHistoryEntry);
                            }

                        }
                        else
                        {
                            await _log.WriteInfoAsync(nameof(HistoryRefreshJob), $"Env: {Program.EnvInfo}", "All history is already sync");
                        }
                    }
                }
                
            } while (continuation != null);

            await _log.WriteInfoAsync(nameof(HistoryRefreshJob), $"Env: {Program.EnvInfo}", "History balances finished", DateTime.Now);
        }
    }
}

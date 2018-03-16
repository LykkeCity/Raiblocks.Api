using Common.Log;
using Lykke.JobTriggers.Triggers.Attributes;
using Lykke.Service.RaiblocksApi.AzureRepositories.Entities.Addresses;
using Lykke.Service.RaiblocksApi.Core.Domain.Entities.Addresses;
using Lykke.Service.RaiblocksApi.Core.Services;
using Newtonsoft.Json.Linq;
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

        private async Task RefreshHistory(AddressObservationType type)
        {
            await _log.WriteInfoAsync(nameof(HistoryRefreshJob), $"Env: {Program.EnvInfo}", $"History job {Enum.GetName(typeof(AddressObservationType), type)} start", DateTime.Now);

            (string continuation, IEnumerable<AddressObservation> items) addressObservations;
            string continuation = null;

            do
            {
                addressObservations = await _historyService.GetAddressObservationAsync(pageSize, continuation, Enum.GetName(typeof(AddressObservationType), type));

                continuation = addressObservations.continuation;

                foreach (var addressObservation in addressObservations.items)
                {
                    var addressInfo = await _blockchainService.GetAddressInfoAsync(addressObservation.Address);

                    var addressBlockCount = (int)addressInfo.blockCount;

                    if (addressBlockCount > 0)
                    {
                        var currentHistoryTo = (await _historyService.GetAddressHistoryAsync(addressBlockCount, Enum.GetName(typeof(AddressObservationType), AddressObservationType.To), addressObservation.Address)).items;

                        var currentHistoryFrom = (await _historyService.GetAddressHistoryAsync(addressBlockCount, Enum.GetName(typeof(AddressObservationType), AddressObservationType.From), addressObservation.Address)).items;

                        var latestBlockNum = currentHistoryTo.Count() + currentHistoryFrom.Count();

                        if (addressBlockCount > latestBlockNum)
                        {
                            var history = await _blockchainService.GetAddressHistoryAsync(addressObservation.Address, addressBlockCount - latestBlockNum);

                            var addressHistoryEntries = history.Select((x, index) => new AddressHistoryEntry
                            {
                                FromAddress = x.from,
                                ToAddress = x.to,
                                Amount = x.amount.ToString(),
                                Hash = x.hash,
                                Type = x.type == TransactionType.send ? AddressObservationType.From : AddressObservationType.To,
                                BlockCount = addressBlockCount - index
                            });

                            foreach (var addressHistoryEntry in addressHistoryEntries)
                            {
                                await _historyService.InsertAddressHistoryAsync(addressHistoryEntry);
                            }
                            await _log.WriteInfoAsync(nameof(HistoryRefreshJob), JArray.FromObject(addressHistoryEntries).ToString(), $"History {Enum.GetName(typeof(AddressObservationType), type)}  {addressObservation.Address} sync");

                        }
                        else if (addressBlockCount == latestBlockNum)
                        {
                            await _log.WriteInfoAsync(nameof(HistoryRefreshJob), $"Env: {Program.EnvInfo}", $"All history {Enum.GetName(typeof(AddressObservationType), type)} {addressObservation.Address} is already sync ");
                        }
                        else
                        {
                            await _log.WriteErrorAsync(nameof(HistoryRefreshJob), $"Env: {Program.EnvInfo}", $"Block count less then db records. Address: {addressObservation.Address}", null);
                        }
                    }
                }

            } while (continuation != null);

            await _log.WriteInfoAsync(nameof(HistoryRefreshJob), $"Env: {Program.EnvInfo}", $"History job {Enum.GetName(typeof(AddressObservationType), type)} finished", DateTime.Now);
        }

        /// <summary>
        /// Job for update history from addresses
        /// </summary>
        /// <returns></returns>
        [TimerTrigger("00:00:10")]
        public async Task RefreshHistoryAsync()
        {
            await RefreshHistory(AddressObservationType.From);
            await RefreshHistory(AddressObservationType.To);
        }
    }
}

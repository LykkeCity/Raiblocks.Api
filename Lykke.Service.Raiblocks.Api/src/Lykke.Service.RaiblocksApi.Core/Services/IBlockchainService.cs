using Lykke.Service.RaiblocksApi.Core.Domain.Entities.Balances;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.RaiblocksApi.Core.Services
{
    public interface IBlockchainService
    {
        Task<Dictionary<string, string>> GetAddressBalances(IEnumerable<string> addresses);

        Task<bool> IsAddressValidAsync(string address);

        Task<string> CreateUnsignSendTransaction(string address, string destination, string amount);
        Task<string> GetAddressBalanceAsync(string address);
        Task<Int64> GetAddressBlockCountAsync(string address);
        Task<(string hash, string error)> BroadcastSignedTransactionAsync(string signedTransaction);
    }
}

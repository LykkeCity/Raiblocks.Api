using Lykke.Service.RaiblocksApi.Core.Domain.Entities.Balances;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.RaiblocksApi.Core.Services
{
    public interface IBlockchainService
    {
        /// <summary>
        /// Get balances for addresses
        /// </summary>
        /// <param name="addresses">Addresses</param>
        /// <returns>Balances for addresses</returns>
        Task<Dictionary<string, string>> GetAddressBalancesAsync(IEnumerable<string> addresses);

        /// <summary>
        /// Check validity for address
        /// </summary>
        /// <param name="address">Address</param>
        /// <returns>Validity</returns>
        Task<bool> IsAddressValidAsync(string address);

        /// <summary>
        /// Check validity for signed transaction
        /// </summary>
        /// <param name="signedTransaction"></param>
        /// <returns>Validity</returns>
        bool IsSignedTransactionValid(string signedTransaction);

        /// <summary>
        /// Check validity for address offline
        /// </summary>
        /// <param name="address">Address</param>
        /// <returns>Validity</returns>
        bool IsAddressValidOfflineAsync(string address);

        /// <summary>
        /// Build unsined transaction
        /// </summary>
        /// <param name="address">Address from</param>
        /// <param name="destination">Address to</param>
        /// <param name="amount">Amount</param>
        /// <returns>Unsined transaction</returns>
        Task<string> CreateUnsignSendTransactionAsync(string address, string destination, string amount);

        /// <summary>
        /// Get balance for address
        /// </summary>
        /// <param name="address">Address</param>
        /// <returns>Balance</returns>
        Task<string> GetAddressBalanceAsync(string address);

        /// <summary>
        /// Get address chain height
        /// </summary>
        /// <param name="address">Address</param>
        /// <returns>Block count</returns>
        Task<Int64> GetAddressBlockCountAsync(string address);

        /// <summary>
        /// Broadcast transaction to network
        /// </summary>
        /// <param name="signedTransaction">Signed transaction</param>
        /// <returns>Broadcast result (hash or error)</returns>
        Task<(string hash, string error)> BroadcastSignedTransactionAsync(string signedTransaction);

        /// <summary>
        /// Get address history
        /// </summary>
        /// <param name="address">Address</param>
        /// <param name="take">Amount of the returned history entries</param>
        /// <returns>Account history</returns>
        Task<IEnumerable<(string from, string to, BigInteger amount, string hash, TransactionType type)>> GetAddressHistoryAsync(string address, int take);

        /// <summary>
        /// Get address info
        /// </summary>
        /// <param name="address">Address</param>
        /// <returns>Return frontier and block count</returns>
        Task<(string frontier, long blockCount)> GetAddressInfoAsync(string address);
    }

    public enum TransactionType
    {
        open,
        receive,
        send,
        change
    }
}

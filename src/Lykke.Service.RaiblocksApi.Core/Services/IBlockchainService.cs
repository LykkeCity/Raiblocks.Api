﻿using Lykke.Service.RaiblocksApi.Core.Domain.Entities.Balances;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Lykke.Service.RaiblocksApi.Core.Domain.Entities.Addresses;
using Lykke.Service.RaiblocksApi.Core.Domain.Entities.Transactions;

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
        /// Build unsined send transaction
        /// </summary>
        /// <param name="address">Address from</param>
        /// <param name="destination">Address to</param>
        /// <param name="amount">Amount</param>
        /// <returns>Unsined transaction</returns>
        Task<string> CreateUnsignSendTransactionAsync(string address, string destination, string amount);

        /// <summary>
        /// Build unsined receive transaction
        /// </summary>
        /// <param name="sendTransactionHash">Send transaction hash</param>
        /// <returns>Unsined transaction</returns>
        Task<string> CreateUnsignReceiveTransactionAsync(string sendTransactionHash);

        /// <summary>
        /// Build unsined receive transaction
        /// </summary>
        /// <param name="txMeta">Transaction meta from node</param>
        /// <returns>Unsined transaction</returns>
        Task<string> CreateUnsignReceiveTransactionAsync(ITransactionMeta txMeta);

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
        Task<IEnumerable<(string from, string to, BigInteger amount, string hash, TransactionType type)>>
            GetAddressHistoryAsync(string address, int take);

        /// <summary>
        /// Get address info
        /// </summary>
        /// <param name="address">Address</param>
        /// <returns>Return frontier and block count</returns>
        Task<(string frontier, long blockCount)> GetAddressInfoAsync(string address);

        /// <summary>
        /// Get transaction meta from node
        /// </summary>
        /// <param name="hash">Transaction hash.</param>
        /// <returns></returns>
        Task<ITransactionMeta> GetTransactionMetaAsync(string hash);
        
        /// <summary>
        /// Get previous blocks in chain
        /// </summary>
        /// <param name="hash">Block hash</param>
        /// <param name="count">Count</param>
        /// <returns>List of hashes</returns>
        Task<List<string>> GetPreviousBlocksAsync(string hash, long count);
        
        /// <summary>
        /// Get accounts pending info
        /// </summary>
        /// <param name="accounts">List of accounts</param>
        /// <param name="count">Count</param>
        /// <param name="source">Is source needed</param>
        /// <returns>Accounts pending info</returns>
        Task<List<IAddressHistoryEntry>> GetAccountsPendingAsync(List<string> accounts, long count, bool source);
    }

    public enum TransactionType
    {
        open,
        receive,
        send,
        change
    }
}

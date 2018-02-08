using Lykke.Service.RaiblocksApi.Core.Domain.Entities.Transactions;
using Lykke.Service.RaiblocksApi.Core.Repositories.Transactions;
using Lykke.Service.RaiblocksApi.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.RaiblocksApi.Services
{
    public class TransactionService<TTransactionBody, TTransactionMeta, TTransactionObservation> : ITransactionService<TTransactionBody, TTransactionMeta, TTransactionObservation>
        where TTransactionBody : ITransactionBody, new()
        where TTransactionMeta : ITransactionMeta, new()
        where TTransactionObservation : ITransactionObservation, new()
    {
        private readonly ITransactionBodyRepository<TTransactionBody> _transactionBodyRepository;
        private readonly ITransactionMetaRepository<TTransactionMeta> _transactionMetaRepository;
        private readonly ITransactionObservationRepository<TTransactionObservation> _transactionObservationRepository;
        private readonly IBlockchainService _blockchainService;

        public TransactionService(ITransactionBodyRepository<TTransactionBody> transactionBodyRepository, ITransactionMetaRepository<TTransactionMeta> transactionMetaRepository, ITransactionObservationRepository<TTransactionObservation> transactionObservationRepository, IBlockchainService blockchainService)
        {
            _transactionBodyRepository = transactionBodyRepository;
            _transactionMetaRepository = transactionMetaRepository;
            _transactionObservationRepository = transactionObservationRepository;
            _blockchainService = blockchainService;
        }

        /// <summary>
        /// Observe tansaction
        /// </summary>
        /// <param name="transactionObservation">Transaction observation</param>
        /// <returns>true if created, false if existed before</returns>
        public async Task<bool> CreateObservationAsync(TTransactionObservation transactionObservation)
        {
            return await _transactionObservationRepository.CreateIfNotExistsAsync(transactionObservation);
        }

        /// <summary>
        /// Get transaction body by operation id
        /// </summary>
        /// <param name="operationId">Operation Id</param>
        /// <returns>Transaction body</returns>
        public async Task<TTransactionBody> GetTransactionBodyByIdAsync(Guid operationId)
        {
            return await _transactionBodyRepository.GetAsync(operationId.ToString());
        }

        /// <summary>
        /// Get transaction meta by operation Id
        /// </summary>
        /// <param name="id">Operation Id</param>
        /// <returns>Transaction meta</returns>
        public async Task<TTransactionMeta> GetTransactionMetaAsync(string id)
        {
            return await _transactionMetaRepository.GetAsync(id);
        }

        /// <summary>
        /// Get observed transaction
        /// </summary>
        /// <param name="pageSize">Abount of transaction observation</param>
        /// <param name="continuation">ontinuation data</param>
        /// <returns>ontinuation data and transaction observation</returns>
        public async Task<(string continuation, IEnumerable<TTransactionObservation> items)> GetTransactionObservationAsync(int pageSize, string continuation)
        {
            return await _transactionObservationRepository.GetAsync(pageSize, continuation);
        }

        /// <summary>
        /// Get new or exist unsigned transaction
        /// </summary>
        /// <param name="operationId">Operation Id</param>
        /// <param name="fromAddress">Address from</param>
        /// <param name="toAddress">Address to</param>
        /// <param name="amount">Amount</param>
        /// <param name="assetId">Asset Id</param>
        /// <param name="includeFee">Include fee</param>
        /// <returns>Unsigned transaction context</returns>
        public async Task<string> GetUnsignSendTransactionAsync(Guid operationId, string fromAddress, string toAddress, string amount, string assetId = "XBR", bool includeFee = false)
        {
            TTransactionBody transactionBody = await GetTransactionBodyByIdAsync(operationId);

            if(transactionBody == null)
            {
                TTransactionMeta transactionMeta = new TTransactionMeta
                {
                    OperationId = operationId,
                    FromAddress = fromAddress,
                    ToAddress = toAddress,
                    AssetId = assetId,
                    Amount = amount,
                    IncludeFee = includeFee,
                    State = TransactionState.NotSigned,
                    CreateTimestamp = DateTime.Now
                };

                await SaveTransactionMetaAsync(transactionMeta);
                
                var unsignTransaction = await _blockchainService.CreateUnsignSendTransactionAsync(transactionMeta.FromAddress, transactionMeta.ToAddress, transactionMeta.Amount);
                
                transactionBody = new TTransactionBody
                {
                    OperationId = operationId,
                    UnsignedTransaction = unsignTransaction
                };

                await SaveTransactionBodyAsync(transactionBody);
            }

            return transactionBody.UnsignedTransaction;
        }

        /// <summary>
        /// Publish signed transaction to network
        /// </summary>
        /// <param name="operationId">Operation Id</param>
        /// <param name="signedTransaction">Signed transaction</param>
        /// <returns>true if publish, false if already publish</returns>
        public async Task<bool> BroadcastSignedTransactionAsync(Guid operationId, string signedTransaction)
        {
            TTransactionBody transactionBody = await GetTransactionBodyByIdAsync(operationId);
            if (transactionBody == null)
            {
                transactionBody = new TTransactionBody
                {
                    OperationId = operationId
                };
            }

            transactionBody.SignedTransaction = signedTransaction;

            await UpdateTransactionBodyAsync(transactionBody);

            var txMeta = await GetTransactionMetaAsync(operationId.ToString());

            if (txMeta.State == TransactionState.Broadcasted 
                || txMeta.State == TransactionState.Confirmed 
                || txMeta.State == TransactionState.Failed
                || txMeta.State == TransactionState.BlockChainFailed)
            {
                return false;
            }

            txMeta.State = TransactionState.Signed;
            txMeta.BroadcastTimestamp = DateTime.Now;
            await UpdateTransactionMeta(txMeta);

            TTransactionObservation transactionObservation = new TTransactionObservation
            {
                OperationId = operationId
            };

            await CreateObservationAsync(transactionObservation);

            return true;
        }

        /// <summary>
        /// Check is transaction already observed
        /// </summary>
        /// <param name="transactionObservation">Transaction observation</param>
        /// <returns>true if already observed</returns>
        public async Task<bool> IsTransactionObservedAsync(TTransactionObservation transactionObservation)
        {
            return await _transactionObservationRepository.IsExistAsync(transactionObservation);
        }

        /// <summary>
        /// Stop observe trancaction
        /// </summary>
        /// <param name="transactionObservation"></param>
        /// <returns>A Task object that represents the asynchronous operation</returns>
        public async Task<bool> RemoveTransactionObservationAsync(TTransactionObservation transactionObservation)
        {
            return await _transactionObservationRepository.DeleteIfExistAsync(transactionObservation);
        }

        /// <summary>
        /// Save transaction body
        /// </summary>
        /// <param name="transactionBody">Transaction body</param>
        /// <returns>true if created, false if existed before</returns>
        public async Task<bool> SaveTransactionBodyAsync(TTransactionBody transactionBody)
        {
            return await _transactionBodyRepository.CreateIfNotExistsAsync(transactionBody);
        }

        /// <summary>
        /// Save transaction meta
        /// </summary>
        /// <param name="transactionMeta">Transaction meta</param>
        /// <returns>true if created, false if existed before</returns>
        public async Task<bool> SaveTransactionMetaAsync(TTransactionMeta transactionMeta)
        {
            return await _transactionMetaRepository.CreateIfNotExistsAsync(transactionMeta);
        }

        /// <summary>
        /// Update transaction body
        /// </summary>
        /// <param name="transactionBody">Transaction body</param>
        /// <returns>A Task object that represents the asynchronous operation</returns>
        public Task UpdateTransactionBodyAsync(TTransactionBody transactionBody)
        {
            return _transactionBodyRepository.UpdateAsync(transactionBody);
        }

        /// <summary>
        /// Update transaction meta
        /// </summary>
        /// <param name="transactionMeta">Transaction body</param>
        /// <returns>A Task object that represents the asynchronous operation</returns>
        public Task UpdateTransactionMeta(TTransactionMeta transactionMeta)
        {
            return _transactionMetaRepository.UpdateAsync(transactionMeta);
        }

    }
}

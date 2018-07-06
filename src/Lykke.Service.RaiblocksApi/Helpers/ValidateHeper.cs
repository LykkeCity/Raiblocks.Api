using Common;
using Lykke.Service.BlockchainApi.Contract.Transactions;
using Lykke.Service.RaiblocksApi.Core.Services;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;


namespace Lykke.Service.RaiblocksApi.Helpers
{
    public class ValidateHeper
    {
        public static bool IsContinuationValid(string continuation)
        {
            if (string.IsNullOrEmpty(continuation))
            {
                return true;
            }

            try
            {
                JsonConvert.DeserializeObject<TableContinuationToken>(Utils.HexToString(continuation));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsBuildSingleTransactionRequestValid(BuildSingleTransactionRequest buildSingleTransaction, IBlockchainService blockchainService)
        {
            if(buildSingleTransaction.OperationId == null || buildSingleTransaction.OperationId == System.Guid.Empty)
            {
                return false;
            }

            if (buildSingleTransaction.ToAddress == null || !blockchainService.IsAddressValidOfflineAsync(buildSingleTransaction.ToAddress))
            {
                return false;
            }

            if (buildSingleTransaction.FromAddress == null || !blockchainService.IsAddressValidOfflineAsync(buildSingleTransaction.FromAddress))
            {
                return false;
            }

            return true;
        }
        
        
        public static bool IsBuildSingleReciveTransactionRequestValid(BuildSingleReceiveTransactionRequest buildSingleReceiveTransactionRequest, IBlockchainService blockchainService)
        {
            if(buildSingleReceiveTransactionRequest.OperationId == null || buildSingleReceiveTransactionRequest.OperationId == System.Guid.Empty)
            {
                return false;
            }

            if (buildSingleReceiveTransactionRequest.SendTransactionHash == null)
            {
                return false;
            }

            return true;
        }
    }
}

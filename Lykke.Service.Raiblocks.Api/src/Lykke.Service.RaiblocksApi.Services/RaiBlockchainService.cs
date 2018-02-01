using Lykke.Service.RaiblocksApi.Core.Domain.Entities.Balances;
using Lykke.Service.RaiblocksApi.Core.Repositories;
using Lykke.Service.RaiblocksApi.Core.Repositories.Balances;
using Lykke.Service.RaiblocksApi.Core.Services;
using Newtonsoft.Json.Linq;
using RaiBlocks;
using RaiBlocks.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.RaiblocksApi.Services
{
    public class RaiBlockchainService<T, P> : IBlockchainService<T, P>
        where T : IAddressBalance, new()
        where P : IBalanceObservation
    {

        private readonly RaiBlocksRpc _raiBlocksRpc;

        public RaiBlockchainService(RaiBlocksRpc raiBlocksRpc)
        {
            _raiBlocksRpc = raiBlocksRpc;
        }

        public async Task<IEnumerable<T>> GetAddressBalances(IEnumerable<P> balanceObservation)
        {
            IEnumerable<RaiAddress> accounts = balanceObservation.Select(x => new RaiAddress(x.Address));
            var result = await _raiBlocksRpc.GetBalancesAsync(accounts);
            return result.Balances.Select(x => new T
            {
                Address = x.Key,
                Balance = x.Value.ToString()
            });
        }

        public async Task<bool> IsAddressValidAsync(string address)
        {
            //JObject jObject = JObject.FromObject(new
            //{
            //    action = "validate_account_number",
            //    account = address
            //});
            //var requestContent = new StringContent(jObject.ToString(), Encoding.UTF8, "application/json");

            //using (HttpClient client = new HttpClient())
            //using (HttpResponseMessage response = await client.PostAsync(_publicNodeURL, requestContent))
            //using (HttpContent content = response.Content)
            //{
            //    var result = JObject.Parse(await content.ReadAsStringAsync());
            //    return result["valid"].ToString().Equals("1") ? true : false;
            //}
            return false;
        }
    }
}

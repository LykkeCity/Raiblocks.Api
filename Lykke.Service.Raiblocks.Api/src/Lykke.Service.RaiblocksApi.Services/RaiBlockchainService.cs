using Lykke.Service.RaiblocksApi.Core.Domain.Entities.Balances;
using Lykke.Service.RaiblocksApi.Core.Repositories;
using Lykke.Service.RaiblocksApi.Core.Repositories.Balances;
using Lykke.Service.RaiblocksApi.Core.Services;
using Newtonsoft.Json.Linq;
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

        private readonly string _privateNodeURL;

        private readonly string _publicNodeURL;

        public RaiBlockchainService(string privateNodeURL, string publicNodeURL)
        {
            _privateNodeURL = privateNodeURL;
            _publicNodeURL = publicNodeURL;
        }

        public async Task<IEnumerable<T>> GetAddressBalances(IEnumerable<P> balanceObservation)
        {
            JObject jObject = JObject.FromObject(new
            {
                action = "accounts_balances",
                accounts = balanceObservation.Select(x => x.Address)
            });
            var requestContent = new StringContent(jObject.ToString(), Encoding.UTF8, "application/json");

            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.PostAsync(_publicNodeURL, requestContent))
            using (HttpContent content = response.Content)
            {
                var result = JObject.Parse(await content.ReadAsStringAsync());
                var addressBalances = new List<T>();
                foreach (var x in (JObject)result.Values().FirstOrDefault())
                {
                    string name = x.Key;
                    JObject value = (JObject)x.Value;

                    addressBalances.Add(new T {
                        Address = name,
                        Balance = value["balance"]?.ToString()
                    });
                }

                return addressBalances;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.RaiblocksApi.Core.Repositories
{
    public interface IRepository<T>
    {
        Task<bool> CreateIfNotExistsAsync(T item);

        Task<bool> DeleteIfExistAsync(T item);

        Task<bool> IsExistAsync(T item);

        Task<(string continuation, IEnumerable<T> items)> GetAsync(int take = 100, string continuation = null);

        Task<T> GetAsync(string id);
    }
}

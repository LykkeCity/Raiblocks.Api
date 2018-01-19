using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.RaiblocksApi.Core.Domain.Entities.Balances
{
    public interface IAddressBalance
    {
        string Address { get; }

        string AssetId { get; }

        string Balance { get; set; }
    }
}

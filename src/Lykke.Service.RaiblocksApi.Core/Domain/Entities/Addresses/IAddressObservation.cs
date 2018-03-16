using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.RaiblocksApi.Core.Domain.Entities.Addresses
{
    public interface IAddressObservation
    {
        string Address { get; set; }

        AddressObservationType Type { get; set; }
    }
}

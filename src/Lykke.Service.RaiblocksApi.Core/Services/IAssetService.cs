using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.RaiblocksApi.Core.Services
{
    public interface IAssetService
    {
        string AssetId { get; }


        string Name { get; }

        int Accuracy { get; }
    }
}

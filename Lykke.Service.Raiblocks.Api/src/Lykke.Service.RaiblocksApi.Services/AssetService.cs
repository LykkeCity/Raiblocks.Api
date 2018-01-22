using Lykke.Service.RaiblocksApi.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.RaiblocksApi.Services
{
    public class AssetService: IAssetService
    {
        public string AssetId { get => "XRB"; }


        public string Name { get => "RaiBlocks"; }

        public int Accuracy { get => 6; }
    }
}

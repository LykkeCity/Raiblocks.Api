using Lykke.Service.RaiblocksApi.Core.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Swashbuckle.AspNetCore.SwaggerGen;
using Lykke.Service.BlockchainApi.Contract.Assets;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Service.BlockchainApi.Contract;

namespace Lykke.Service.RaiblocksApi.Controllers
{
    [Route("api/[controller]")]
    public class AssetsController : Controller
    {
        private readonly IHealthService _healthService;

        public AssetsController(IHealthService healthService)
        {
            _healthService = healthService;
        }

        /// <summary>
        ///  Get batch blockchain assets (coins, tags)
        /// </summary>
        /// <param name="take">Amount of the returned assets should not exceed take</param>
        /// <param name="continuation">Optional continuation contains context of the previous request</param>
        /// <returns>Batch blockchain assets (coins, tags)</returns>
        [HttpGet]
        [SwaggerOperation("GetAssets")]
        [ProducesResponseType(typeof(PaginationResponse<AssetContract>), (int)HttpStatusCode.OK)]
        public PaginationResponse<AssetContract> GetAssets([FromQuery]int take = 100, [FromQuery]string continuation = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get specified asset (coin, tag)
        /// </summary>
        /// <param name="assetId">Asset id</param>
        /// <returns>Specified asset (coin, tag)</returns>
        [HttpGet("{assetId}")]
        [SwaggerOperation("GetAsset")]
        [ProducesResponseType(typeof(AssetContract), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NoContent)]
        public IActionResult GetAsset(string assetId)
        {
            throw new NotImplementedException();
        }
    }
}

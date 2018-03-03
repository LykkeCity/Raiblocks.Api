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
using System.Threading.Tasks;
using Lykke.Service.RaiblocksApi.Helpers;

namespace Lykke.Service.RaiblocksApi.Controllers
{
    [Route("api/[controller]")]
    public class AssetsController : Controller
    {
        private readonly IHealthService _healthService;
        private readonly IAssetService _assetService;

        public AssetsController(IHealthService healthService, IAssetService assetService)
        {
            _healthService = healthService;
            _assetService = assetService;
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
        public IActionResult GetAssets([FromQuery]string take = "100", [FromQuery]string continuation = null)
        {
            if (int.TryParse(take, out var takeParsed) && take != null && ValidateHeper.IsContinuationValid(continuation))
            {
                return StatusCode((int)HttpStatusCode.OK, 
                    PaginationResponse.From(
                        null,
                        new List<AssetContract> {
                            new AssetContract
                            {
                                AssetId = _assetService.AssetId,
                                Name = _assetService.Name,
                                Accuracy = _assetService.Accuracy
                            }
                        }
                        ));
            } else
            {
                return StatusCode((int)HttpStatusCode.BadRequest, ErrorResponse.Create("Invalid params"));
            }
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
            if (assetId.Equals(_assetService.AssetId))
            {
                return Ok(new AssetContract
                {
                    AssetId = _assetService.AssetId,
                    Name = _assetService.Name,
                    Accuracy = _assetService.Accuracy
                });
            }
            return StatusCode((int)HttpStatusCode.NoContent);
        }
    }
}

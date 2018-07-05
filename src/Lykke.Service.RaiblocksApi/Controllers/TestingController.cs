using System.Net;
using Lykke.Service.BlockchainApi.Contract.Common;
using Lykke.Service.BlockchainApi.Contract.Testing;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Service.RaiblocksApi.Controllers
{
    [Route("api/[controller]")]
    public class TestingController : Controller
    {
        /// <summary>
        ///  Transfer funds from the fromAddress to the toAddress addresses.
        /// </summary>
        /// <param name="testingTransferRequest">Testing transfer request <see cref="TestingTransferRequest"/>.</param>
        /// <returns>Http status code <see cref="IActionResult"/>.</returns>
        [HttpPost("transfers")]
        [SwaggerOperation("Constants")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotImplemented)]
        public IActionResult PostTransfer([FromBody] TestingTransferRequest testingTransferRequest)
        {
            return StatusCode((int)HttpStatusCode.NotImplemented);
        }
    }
}

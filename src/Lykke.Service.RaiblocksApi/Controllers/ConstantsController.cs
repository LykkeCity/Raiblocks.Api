using System.Net;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Service.BlockchainApi.Contract.Common;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Service.RaiblocksApi.Controllers
{
    [Route("api/[controller]")]
    public class ConstantsController : Controller
    {
        private readonly ILog _log;

        public ConstantsController(ILog log)
        {
            _log = log;
        }
        
        /// <summary>
        /// Get blockchain integration constants.
        /// </summary>
        /// <returns>Blockchain integration constants <see cref="ConstantsResponse"/>.</returns>
        [HttpGet]
        [SwaggerOperation("Constants")]
        [ProducesResponseType(typeof(ConstantsResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotImplemented)]
        public IActionResult GetConstants()
        {
            return StatusCode((int)HttpStatusCode.NotImplemented);
        }
    }
}

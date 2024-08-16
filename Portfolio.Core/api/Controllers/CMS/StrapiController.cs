using Microsoft.AspNetCore.Mvc;
using Portfolio.Core.CMS.Strapi;
using Portfolio.Core.Exceptions;
using Portfolio.Core.Types.DTOs.Resources;

namespace Portfolio.Core.Controllers.CMS
{
    [Route("api/[controller]")]
    [ApiController]
    public class StrapiController : ControllerBase
    {
        private readonly IStrapiService _strapiService;

        public StrapiController(IStrapiService strapiService)
        {
            _strapiService = strapiService;
        }

        #region Get Http Calls
        [HttpPost("sync")]
        public async Task<bool> SyncDBAndCMSAsync()
        {
            return await DefaultException.ExceptionControllerHandler(async () => { return await _strapiService?.SyncDBAndCMSAsync(); });
        }

        [HttpPost("list")]
        public async Task<IEnumerable<ResourceDTO>> FetchAllAsync([FromBody] BaseStrapiRequest request)
        {
            return await DefaultException.ExceptionControllerHandler(async () => { return await _strapiService?.FetchAllAsync(request?.RenderPath, request?.EnSync == true); });
        }

        [HttpPost("find")]
        public async Task<ResourceDTO> FindAsync([FromBody] BaseStrapiRequest request)
        {
            return await DefaultException.ExceptionControllerHandler(async () => { return await _strapiService?.FindAsync(request?.Title, request?.RenderPath, request?.EnSync == true); });
        }
        #endregion
    }
}
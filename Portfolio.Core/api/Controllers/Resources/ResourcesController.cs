using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Core.Exceptions;
using Portfolio.Core.Interfaces.Resources;
using Portfolio.Core.Types.DTOs.Requests.Resources;
using Portfolio.Core.Types.DTOs.Resources;
using Portfolio.Core.Types.DTOs.Responses;

namespace Portfolio.Core.Controllers.Resources
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResourcesController : ControllerBase
    {
        private readonly IResourcesService _resourcesService;

        public ResourcesController(IResourcesService resourcesService)
        {
            _resourcesService = resourcesService;
        }

        [HttpPost("getAllResourcesByTagsAsync")]
        public async Task<IEnumerable<ResourceDTO>> GetAllResourcesByTagsAsync([FromBody] ResourceDTO request)
        {
            return await DefaultException.ExceptionControllerHandler(async () => { 
                    return await _resourcesService?.GetAllResourcesByTagsAsync(request);
                });              
        }

        [HttpPost("list")]
        public async Task<IEnumerable<ResourceDTO>> GetAllResourcesByRenderPathAsync([FromBody] ResourceRequest request)
        {
            return await DefaultException.ExceptionControllerHandler(async () => { 
                    return await _resourcesService?.GetAllResourcesByRenderPathAsync(request);
                });              
        }

        [HttpPost("find")]
        public async Task<ResourceDTO> GetResourceByFullPathAsync([FromBody] ResourceRequest request)
        {
            return await DefaultException.ExceptionControllerHandler(async () => { 
                    return await _resourcesService?.GetResourceByFullPathAsync(request);
                });            
        }

        [HttpPost("insert")]
        [Authorize(Policy = "Admin")]
        public async Task<BaseResponse> AddResourceAsync([FromBody] ResourceDTO request)
        {
            return await DefaultException.ExceptionControllerHandler(async () => { 
                    return await _resourcesService?.AddResourceAsync(request);
                });            
        }

        [HttpPost("edit")]
        [Authorize(Policy = "Admin")]
        public async Task<BaseResponse> UpdateResourceAsync(ResourceDTO request)
        {
            return await DefaultException.ExceptionControllerHandler(async () => { 
                    return await _resourcesService?.UpdateResourceAsync(request);
                });                
        }

        [HttpPost("delete")]
        [Authorize(Policy = "Admin")]
        public async Task<BaseResponse> DeleteResourceAsync(ResourceRequest request)
        {
            return await DefaultException.ExceptionControllerHandler(async () => { 
                    return await _resourcesService?.DeleteResourceAsync(request);
                });             
        }
    }
}
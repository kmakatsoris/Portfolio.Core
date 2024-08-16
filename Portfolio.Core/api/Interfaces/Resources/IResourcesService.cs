using Portfolio.Core.Types.DTOs.Requests.Resources;
using Portfolio.Core.Types.DTOs.Resources;
using Portfolio.Core.Types.DTOs.Responses;

namespace Portfolio.Core.Interfaces.Resources
{
    public interface IResourcesService
    {
        Task<IEnumerable<ResourceDTO>> GetAllResourcesByRenderPathAsync(ResourceRequest request);
        Task<ResourceDTO> GetResourceByFullPathAsync(ResourceRequest request);
        Task<IEnumerable<ResourceDTO>> GetAllResourcesByTagsAsync(ResourceDTO request);
        Task<BaseResponse> AddResourceAsync(ResourceDTO request);
        Task<BaseResponse> UpdateResourceAsync(ResourceDTO request);
        Task<BaseResponse> DeleteResourceAsync(ResourceRequest request);
    }
}
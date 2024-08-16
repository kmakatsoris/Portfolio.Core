using Portfolio.Core.Types.Models.Resources;

namespace Portfolio.Core.Interfaces.Context.Resources
{
    public interface IResourceRepository
    {
        Task<IEnumerable<Resource>> GetAllResourcesByRenderPathAsync(string renderPath);
        Task<Resource> GetResourceByFullPathAsync(string path, string title);
        Task<IEnumerable<Resource>> GetAllResourcesByTagsAsync(List<string> tags);
        Task<bool> AddResourceAsync(Resource resource);
        Task<bool> UpdateResourceAsync(Resource resource);
        Task<bool> DeleteResourceAsync(string path, string title);
    }
}
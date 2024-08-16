using Portfolio.Core.Types.DTOs.Resources;

namespace Portfolio.Core.CMS.Strapi
{
    public interface IStrapiService
    {
        Task<bool> SyncDBAndCMSAsync();
        Task<IEnumerable<ResourceDTO>> FetchAllAsync(string renderPath, bool enSync = true, bool enThrow = false);
        Task<ResourceDTO> FindAsync(string title, string renderPath, bool enSync = true, bool enThrow = false);
    }
}
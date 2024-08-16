using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Portfolio.Core.CMS.Strapi;
using Portfolio.Core.Types.DTOs.Resources;
using Portfolio.Core.Interfaces.Context.Resources;
using Portfolio.Core.Types.Context;
using ILogger = NLog.ILogger;
using Microsoft.Extensions.Options;
using Portfolio.Core.Utils.Mapping;
using Newtonsoft.Json;
using Portfolio.Core.Types.Enums.Resources;
using Portfolio.Core.Utils.DefaultUtils;
using Portfolio.Core.Exceptions;

namespace Portfolio.Core.CMS.Strapi
{
    public class StrapiService : IStrapiService
    {
        private readonly IOptions<AppSettings> _appSettings;
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;
        private readonly IResourceRepository _resourceRepository;

        public StrapiService(
            IOptions<AppSettings> appSettings,
            ILogger logger,
            IHttpClientFactory clientFactory,
            IResourceRepository resourceRepository
            )
        {
            _appSettings = appSettings;
            _logger = logger;
            _httpClient = clientFactory.CreateClient("StrapiClient");
            _resourceRepository = resourceRepository;
        }

        #region Get Http Calls
        public async Task<bool> SyncDBAndCMSAsync()
        {
            List<ResourceDTO> result = new List<ResourceDTO>();
            result = AddSyncResult(result, await FetchAllAsync("homepage", true, false));
            result = AddSyncResult(result, await FetchAllAsync("homepage", true, true));

            result = AddSyncResult(result, await FetchAllAsync("biopage", true, false));
            result = AddSyncResult(result, await FetchAllAsync("biopage", true, true));

            result = AddSyncResult(result, await FetchAllAsync("skillspage", true, false));
            result = AddSyncResult(result, await FetchAllAsync("skillspage", true, true));

            result = AddSyncResult(result, await FetchAllAsync("projectspage", true, false));
            result = AddSyncResult(result, await FetchAllAsync("projectspage", true, true));

            result = AddSyncResult(result, await FetchAllAsync("apispage", true, false));
            result = AddSyncResult(result, await FetchAllAsync("apispage", true, true));

            result = AddSyncResult(result, await FetchAllAsync("contactspage", true, false));
            result = AddSyncResult(result, await FetchAllAsync("contactspage", true, true));
            return true;
        }

        public async Task<ResourceDTO> FindAsync(string title, string renderPath, bool enSync = true, bool enThrow = false)
        {
            ResourceDTO resourceDTO = ResourcesMapping.ToMap(await _resourceRepository?.GetResourceByFullPathAsync(ValidateRenderPathForDB(renderPath), title));

            if (string.IsNullOrEmpty(resourceDTO?.StoragePath))
            {
                _logger.Error(DefaultException.WarningLogTag + $"Error in StrapiService/FindAsync with Message: Not Found StoragePath to be linked with render path and title attribute. \n\nTitle:{title}\nRenderPath:{renderPath}");
                return new ResourceDTO();
            }

            SingleStrapiDataResponse strapiResp = await _httpClient.GetFromJsonAsync<SingleStrapiDataResponse>(_httpClient?.BaseAddress + ExtractRenderPath(renderPath) + "/" + (enSync ? resourceDTO?.StoragePath : title) + "?populate=*");
            return ToPerform(await ExecuteSync(resourceDTO, strapiResp, renderPath, enSync, enThrow));
        }

        public async Task<IEnumerable<ResourceDTO>> FetchAllAsync(string renderPath, bool enSync = true, bool enThrow = false)
        {
            try
            {
                _logger.Error($"@TEST: HEllo World!");
                IEnumerable<ResourceDTO> resourceDTOs = ResourcesMapping.ToMap(await _resourceRepository?.GetAllResourcesByRenderPathAsync(ValidateRenderPathForDB(renderPath)));

                if ((resourceDTOs == null || resourceDTOs?.Count() <= 0 || resourceDTOs?.Any(r => string.IsNullOrEmpty(r?.StoragePath)) == true) && string.IsNullOrEmpty(ExtractRenderPath(renderPath)))
                {
                    _logger.Error(DefaultException.WarningLogTag + $"Error in StrapiService/FetchAll with Message: Not Found StoragePath to be linked with render path attribute. \n\nRenderPath:{renderPath}");
                    return new List<ResourceDTO>();
                }

                MultipleStrapiDataResponse strapiResps = await _httpClient.GetFromJsonAsync<MultipleStrapiDataResponse>(_httpClient?.BaseAddress + ExtractRenderPath(renderPath) + "?populate=*");

                List<ResourceDTO> result = new List<ResourceDTO>();
                foreach (var s in strapiResps?.Data)
                {
                    bool isAny = false;
                    foreach (var r in resourceDTOs)
                    {
                        if (s?.Id.ToString()?.ToLower().Equals(r?.StoragePath?.ToLower()) == true)
                        {
                            var t = await ExecuteSync(r, new SingleStrapiDataResponse { Data = s }, renderPath, enSync, enThrow); // Update
                            result.Add(t);
                            isAny = true;
                        }
                    }
                    if (isAny == false)
                    {
                        var t = await ExecuteSync(new ResourceDTO(), new SingleStrapiDataResponse { Data = s }, renderPath, enSync, enThrow);
                        if (!string.IsNullOrEmpty(t?.Title)) result.Add(t); // Insert
                    }
                }
                return ToPerform(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex?.Message);
                throw new Exception(ex?.Message);
            }

        }
        #endregion

        #region Private Methods        
        private List<ResourceDTO> AddSyncResult(List<ResourceDTO> result, IEnumerable<ResourceDTO> newResources)
        {
            foreach (var r in newResources)
            {
                result.Add(r);
            }
            return result;
        }
        private string ValidateRenderPathForDB(string renderPath)
        {
            string r = renderPath?.ToLower();
            bool c = new List<string>(){
                "homepage", "biopage", "skillspage", "projectspage", "apispage", "contactspage"
            }.Contains(r);
            return c ? r : "";
        }
        private ResourceDTO ToPerform(ResourceDTO r)
        {
            ResourceDTO result = r;
            result.MetaDataID = Guid.Empty;
            result.StoragePath = "*****";
            return result;
        }

        private IEnumerable<ResourceDTO> ToPerform(IEnumerable<ResourceDTO> r)
        {
            List<ResourceDTO> result = new List<ResourceDTO>();
            foreach (var resource in r)
            {
                result.Add(ToPerform(resource));
            }
            return result;
        }

        // @CMS_Strapi_RenderPathsAndCollections
        private string ExtractRenderPath(string rp)
        {
            string result = "";
            if (string.IsNullOrEmpty(rp)) return result;
            switch (rp?.ToLower())
            {
                case "homepage":
                    result = _appSettings?.Value?.CMS_Strapi_RenderPathsAndCollections?.HomePage;
                    break;
                case "biopage":
                    result = _appSettings?.Value?.CMS_Strapi_RenderPathsAndCollections?.BioPage;
                    break;
                case "skillspage":
                    result = _appSettings?.Value?.CMS_Strapi_RenderPathsAndCollections?.SkillsPage;
                    break;
                case "projectspage":
                    result = _appSettings?.Value?.CMS_Strapi_RenderPathsAndCollections?.ProjectsPage;
                    break;
                case "apispage":
                    result = _appSettings?.Value?.CMS_Strapi_RenderPathsAndCollections?.APIsPage;
                    break;
                case "contactspage":
                    result = _appSettings?.Value?.CMS_Strapi_RenderPathsAndCollections?.ContactsPage;
                    break;
                default:
                    result = "";
                    break;
            }
            return result ?? "";
        }

        private async Task<ResourceDTO> ExecuteSync(ResourceDTO dbItem, SingleStrapiDataResponse strapiItem, string validRenderPathToSync, bool enSync = true, bool enThrow = false)
        {
            if (strapiItem?.Data?.Attributes.FullPath?.Split(';') == null || strapiItem?.Data?.Attributes.FullPath?.Split(';')?.Length <= 0)
                throw new Exception("Error fetching the FullPath from strapi.");

            string title = strapiItem?.Data?.Attributes?.FullPath?.Split(';')?.GetValue<string>(1);
            string renderPath = strapiItem?.Data?.Attributes?.FullPath?.Split(';')?.GetValue<string>(0);
            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(renderPath))
            {
                _logger.Error(DefaultException.WarningLogTag + "There are Strapi Items which dont have Title or Render Path. \n\n" + JsonConvert.SerializeObject(strapiItem));
                return new ResourceDTO();
            }
            if (renderPath?.ToLower()?.Equals(validRenderPathToSync?.ToLower()) != true) return new ResourceDTO();

            ExtensiveDescriptionType[] extensiveDesc = JsonConvert.DeserializeObject<ExtensiveDescriptionType[]>(strapiItem?.Data?.Attributes?.MetaData?.ExtensiveDescription ?? "") ?? [];
            string tags = strapiItem?.Data?.Attributes?.Tags ?? "";
            var previewDataDTO = new Portfolio.Core.Types.DataTypes.Resources.PreviewData
            {
                Url = strapiItem?.Data?.Attributes?.PreviewData?.Data?.Attributes?.Url ?? "",
                Caption = strapiItem?.Data?.Attributes?.PreviewData?.Data?.Attributes?.Caption?.ToString() ?? "",
                Height = strapiItem?.Data?.Attributes?.PreviewData?.Data?.Attributes?.Height.ToString() ?? "",
                Width = strapiItem?.Data?.Attributes?.PreviewData?.Data?.Attributes?.Width.ToString() ?? "",
            };

            ResourceDTO result = new ResourceDTO
            {
                Title = title ?? "",
                BriefDescription = strapiItem?.Data?.Attributes?.MetaData?.BriefDescription ?? "",
                ExtensiveDescriptionDTO = extensiveDesc,
                MetaDataID = dbItem?.MetaDataID ?? Guid.Empty,
                MetaData = JsonConvert.SerializeObject(strapiItem?.Data?.Attributes?.MetaData) ?? "",
                RenderPath = ValidateRenderPathForDB(renderPath) ?? "",
                StoragePath = strapiItem?.Data?.Id.ToString() ?? "",
                PreviewDataDTO = previewDataDTO,
                Dimension = strapiItem?.Data?.Attributes?.PreviewData?.Data?.Attributes?.Width.ToString() + "X" + strapiItem?.Data?.Attributes?.PreviewData?.Data?.Attributes?.Height.ToString(),
                Tags = tags,
                TypeEnum = SelectMediaType(strapiItem?.Data?.Attributes?.PreviewData?.Data?.Attributes?.Mime),
                CreatedAt = strapiItem?.Data?.Attributes?.CreatedAt ?? DateTime.UtcNow,
                UpdatedAt = strapiItem?.Data?.Attributes?.UpdatedAt ?? DateTime.UtcNow,
            };

            if (enSync)
            {
                bool isSync = CheckSync(dbItem, strapiItem);
                if (enThrow && !isSync) throw new Exception($"Error on sync the dbItem: {dbItem?.Title} with strapiItem: {strapiItem?.Data?.Attributes?.FullPath}");
                bool isValid = isSync ? true : (dbItem?.Title?.ToLower()?.Equals(title?.ToLower()) == true && dbItem?.RenderPath?.ToLower()?.Equals(renderPath?.ToLower()) == true ?
                             await _resourceRepository?.UpdateResourceAsync(result?.ToMap()) :
                             await _resourceRepository?.AddResourceAsync(result?.ToMap()));
                return isValid ? result : new ResourceDTO();
            }

            return result;
        }

        private ResourcesTypesEnum SelectMediaType(string mime)
        {
            if (string.IsNullOrEmpty(mime)) return ResourcesTypesEnum.Default;
            else if (mime.Contains("image")) return ResourcesTypesEnum.ImagesPNG;
            else return ResourcesTypesEnum.Default;
        }

        private bool CheckSync(ResourceDTO dbItem, SingleStrapiDataResponse strapiItem)
        {
            string title = strapiItem?.Data?.Attributes?.FullPath?.Split(';')?.GetValue<string>(1);
            string renderPath = strapiItem?.Data?.Attributes?.FullPath?.Split(';')?.GetValue<string>(0);
            IEnumerable<ExtensiveDescriptionType> extensiveDesc = JsonConvert.DeserializeObject<IEnumerable<ExtensiveDescriptionType>>(strapiItem?.Data?.Attributes?.MetaData?.ExtensiveDescription ?? "") ?? [];
            string tags = strapiItem?.Data?.Attributes?.Tags ?? "";
            var previewDataDTO = new Portfolio.Core.Types.DataTypes.Resources.PreviewData
            {
                Url = strapiItem?.Data?.Attributes?.PreviewData?.Data?.Attributes?.Url ?? "",
                Caption = strapiItem?.Data?.Attributes?.PreviewData?.Data?.Attributes?.Caption?.ToString() ?? "",
                Height = strapiItem?.Data?.Attributes?.PreviewData?.Data?.Attributes?.Height.ToString() ?? "",
                Width = strapiItem?.Data?.Attributes?.PreviewData?.Data?.Attributes?.Width.ToString() ?? "",
            };

            return dbItem?.Title.MyEqual(title) == true &&
                dbItem?.RenderPath.MyEqual(renderPath) == true &&
                DefaultUtils.ArraysContainSameElements(dbItem?.ExtensiveDescriptionDTO, extensiveDesc) &&
                dbItem?.Tags.MyEqual(tags) == true &&
                dbItem?.BriefDescription.MyEqual(strapiItem?.Data?.Attributes?.MetaData?.BriefDescription) == true &&
                dbItem?.StoragePath.MyEqual(strapiItem?.Data?.Id.ToString()) == true &&
                (
                    dbItem?.PreviewDataDTO != null && previewDataDTO != null &&
                    dbItem?.PreviewDataDTO?.Url.MyEqual(previewDataDTO?.Url) == true &&
                    dbItem?.PreviewDataDTO?.Caption.MyEqual(previewDataDTO?.Caption) == true &&
                    dbItem?.PreviewDataDTO?.Height.MyEqual(previewDataDTO?.Height) == true &&
                    dbItem?.PreviewDataDTO?.Width.MyEqual(previewDataDTO?.Width) == true) &&
                dbItem?.Dimension.MyEqual(strapiItem?.Data?.Attributes?.PreviewData?.Data?.Attributes?.Width.ToString() + "X" + strapiItem?.Data?.Attributes?.PreviewData?.Data?.Attributes?.Height.ToString()) == true &&
                DefaultUtils.ArraysContainSameElements(dbItem?.Tags, tags);
        }
        #endregion

    }
}

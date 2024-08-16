using NLog;
using Portfolio.Core.Interfaces.Context.MetaData;
using Portfolio.Core.Interfaces.Context.Resources;
using Portfolio.Core.Interfaces.Identity;
using Portfolio.Core.Interfaces.Resources;
using Portfolio.Core.Types.DTOs;
using Portfolio.Core.Types.DTOs.Requests;
using Portfolio.Core.Types.DTOs.Requests.Resources;
using Portfolio.Core.Types.DTOs.Resources;
using Portfolio.Core.Types.DTOs.Responses;
using Portfolio.Core.Types.Models.MetaData;
using Portfolio.Core.Utils;
using Portfolio.Core.Utils.DefaultUtils;
using Portfolio.Core.Utils.Mapping;
using ILogger = NLog.ILogger;

namespace Portfolio.Core.Implementation.Resources
{
    public partial class ResourcesService : IResourcesService
    {
        private readonly ILogger _logger;
        private readonly IResourceRepository _resourcesRepository;
        private readonly IMetaDataRepository _metaDataRepository;
        private readonly IOAuthAuthorizationService _oauthService;

        public ResourcesService
        (
            ILogger logger,
            IResourceRepository resourcesRepository,
            IMetaDataRepository metaDataRepository,
            IOAuthAuthorizationService oauthService
        )
        {
            _logger = logger;
            _resourcesRepository = resourcesRepository;
            _metaDataRepository = metaDataRepository;
            _oauthService = oauthService;
        }

        public async Task<IEnumerable<ResourceDTO>> GetAllResourcesByRenderPathAsync(ResourceRequest request)
        {
            if (string.IsNullOrEmpty(request?.Path)) throw new Exception("Bad Request");
            return ResourcesMapping.ToMap(await _resourcesRepository?.GetAllResourcesByRenderPathAsync(request?.Path)) ?? new List<ResourceDTO>();
        }

        public async Task<ResourceDTO> GetResourceByFullPathAsync(ResourceRequest request)
        {
            if (string.IsNullOrEmpty(request?.Path) || string.IsNullOrEmpty(request?.Title)) throw new Exception("Bad Request");
            return ResourcesMapping.ToMap(await _resourcesRepository?.GetResourceByFullPathAsync(request?.Path, request?.Title)) ?? new ResourceDTO();
        }

        public async Task<IEnumerable<ResourceDTO>> GetAllResourcesByTagsAsync(ResourceDTO request)
        {
            if (request == null || string.IsNullOrEmpty(request?.Tags)) throw new Exception("Bad Request");
            return ResourcesMapping.ToMap(await _resourcesRepository?.GetAllResourcesByTagsAsync(request?.Tags?.Trim()?.Split(',').ToList())) ?? new List<ResourceDTO>();
        }

        public async Task<BaseResponse> AddResourceAsync(ResourceDTO request)
        {
            if (request == null || string.IsNullOrEmpty(request?.Tags)) throw new Exception("Bad Request");
            if (!request.IsBasicResourceValid()) throw new Exception("The Resource is not Valid");
            request.Tags = request?.Tags?.ToPerformTagsRequest() ?? "";
            return new BaseResponse { Success = await _resourcesRepository?.AddResourceAsync(ResourcesMapping.ToMap(request)) };
        }

        public async Task<BaseResponse> UpdateResourceAsync(ResourceDTO request)
        {
            if (request == null || string.IsNullOrEmpty(request?.Tags)) throw new Exception("Bad Request");
            if (!request.IsBasicResourceValid()) throw new Exception("The Resource is not Valid");
            request.Tags = request?.Tags?.ToPerformTagsRequest() ?? "";
            return new BaseResponse { Success = await _resourcesRepository?.UpdateResourceAsync(ResourcesMapping.ToMap(request)) };
        }

        public async Task<BaseResponse> DeleteResourceAsync(ResourceRequest request)
        {
            if (string.IsNullOrEmpty(request?.Path) || string.IsNullOrEmpty(request?.Title)) throw new Exception("Bad Request");
            return new BaseResponse { Success = await _resourcesRepository?.DeleteResourceAsync(request?.Path, request?.Title) };
        }

        #region MetaData
        public async Task<MetaDataDTO<MetaDataModel>> GetResourceMetaData<T>(BaseIdRequest request)
        {
            if (request?.Id == null) return new MetaDataDTO<MetaDataModel>();
            return MetaDataMapping.ToMap<MetaDataModel>(await _metaDataRepository?.GetMetaDataAsyncById(request?.Id ?? Guid.Empty)) ?? new MetaDataDTO<MetaDataModel>();
        }

        public async Task<BaseResponse> InsertResourceMetaData(string token, MetaDataDTO<MetaDataModel> metaDataDTO)
        {
            if (_oauthService?.GetEmailFromToken(token)?.ToLower()?.Equals(metaDataDTO?.Email?.ToLower()) != true) throw new Exception("Sorry, but you do not have permission to ADD other people's reviews.");
            return new BaseResponse { Success = await _metaDataRepository?.AddMetaDataAsync(MetaDataMapping.ToMap<MetaDataModel>(metaDataDTO) as MetaDataModel) };
        }

        public async Task<BaseResponse> UpdateResourceMetaData(string token, MetaDataDTO<MetaDataModel> metaDataDTO)
        {
            if (_oauthService?.GetEmailFromToken(token)?.ToLower()?.Equals(metaDataDTO?.Email?.ToLower()) != true) throw new Exception("Sorry, but you do not have permission to EDIT other people's reviews.");
            return new BaseResponse { Success = await _metaDataRepository?.UpdateMetaDataAsync(MetaDataMapping.ToMap<MetaDataModel>(metaDataDTO) as MetaDataModel) };
        }

        public async Task<BaseResponse> DeleteResourceMetaData(string token, BaseRequest request)
        {
            if (_oauthService?.GetEmailFromToken(token)?.ToLower()?.Equals(request?.Email?.ToLower()) != true) throw new Exception("Sorry, but you do not have permission to DELETE other people's reviews.");
            return new BaseResponse { Success = await _metaDataRepository?.DeleteMetaDataAsync(request?.Email) };
        }
        #endregion
    }
}
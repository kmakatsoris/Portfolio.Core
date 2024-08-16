using Newtonsoft.Json;
using Portfolio.Core.Types.DataTypes.Resources;
using Portfolio.Core.Types.DTOs.Resources;
using Portfolio.Core.Types.Enums.Resources;
using Portfolio.Core.Types.Models.Resources;
using Portfolio.Core.Utils.DefaultUtils;

namespace Portfolio.Core.Utils.Mapping
{
    public static class ResourcesMapping
    {
        #region Map Review To ReviewDTO
        public static ResourceDTO ToMap(this Resource dto)
        {
            if (!dto.IsBasicResourceValid()) return new ResourceDTO();
            var t = new ResourceDTO
            {
                Title = dto?.Title ?? "",
                BriefDescription = dto?.BriefDescription ?? "",
                ExtensiveDescriptionDTO = dto?.ExtensiveDescription == null ? [] : JsonConvert.DeserializeObject<IEnumerable<ExtensiveDescriptionType>>(dto?.ExtensiveDescription),
                MetaDataID = dto?.MetaDataID ?? Guid.Empty,
                RenderPath = dto?.RenderPath ?? "",
                StoragePath = dto?.StoragePath ?? "",
                PreviewDataDTO = dto?.PreviewData == null ? new PreviewData() : JsonConvert.DeserializeObject<PreviewData>(dto?.PreviewData),
                Dimension = dto?.Dimension,
                Tags = dto?.Tags == null ? "" : dto?.Tags,
                TypeEnum = Enum.TryParse(dto?.Type, true, out ResourcesTypesEnum typeEnum) == true ? typeEnum : ResourcesTypesEnum.Default,
                CreatedAt = dto?.CreatedAt ?? DateTime.UtcNow,
                UpdatedAt = dto?.UpdatedAt ?? DateTime.UtcNow,
            };
            return t;
        }

        public static IEnumerable<ResourceDTO> ToMap(this IEnumerable<Resource> dtos)
        {
            if (dtos == null || dtos?.Count() <= 0) return new List<ResourceDTO>();
            List<ResourceDTO> result = new List<ResourceDTO>();
            foreach (Resource dto in dtos)
            {
                if (dto.IsBasicResourceValid())
                    result.Add(dto?.ToMap());
            }
            return result;
        }
        #endregion

        #region Map ReviewDTO To Review
        public static Resource ToMap(this ResourceDTO dto)
        {
            if (!dto.IsBasicResourceValid()) return new Resource();
            var t = new Resource
            {
                Title = dto?.Title ?? "",
                BriefDescription = dto?.BriefDescription ?? "",
                ExtensiveDescription = dto?.ExtensiveDescription ?? "",
                MetaDataID = dto?.MetaDataID ?? Guid.Empty,
                RenderPath = dto?.RenderPath ?? "",
                StoragePath = dto?.StoragePath ?? "",
                PreviewData = dto?.PreviewData ?? "",
                Dimension = dto?.Dimension,
                Tags = dto?.Tags ?? "",
                Type = dto?.Type ?? ResourcesTypesEnum.Default.GetStringValue(),
                CreatedAt = dto?.CreatedAt ?? DateTime.UtcNow,
                UpdatedAt = dto?.UpdatedAt ?? DateTime.UtcNow,
            };
            return t;
        }
        #endregion

    }
}
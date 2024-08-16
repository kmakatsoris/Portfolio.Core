

using Portfolio.Core.Interfaces.Context.Resources;
using Portfolio.Core.Types.Enums.Resources;
using Portfolio.Core.Types.Models.Resources;
using Portfolio.Core.Utils.DefaultUtils;

namespace Portfolio.Core.Utils
{
    public static class ResourcesUtils
    {
        public static bool IsBasicResourceValid(this IResourceProperties resource) => resource != null &&
                                                     !string.IsNullOrEmpty(resource?.Title) &&
                                                     resource?.CreatedAt != null && resource?.CreatedAt != DateTime.MinValue && resource?.CreatedAt >= new DateTime(2024, 5, 1) &&
                                                     resource?.UpdatedAt != null && resource?.UpdatedAt != DateTime.MinValue && resource?.UpdatedAt >= new DateTime(2024, 5, 1) &&
                                                     !string.IsNullOrEmpty(resource?.RenderPath) &&
                                                     !string.IsNullOrEmpty(resource?.StoragePath) &&
                                                     Enum.IsDefined(typeof(ResourcesTypesEnum), resource?.Type);

        public static Resource ToPerformDB(this Resource dto)
        {
            if (!dto.IsBasicResourceValid()) throw new Exception("The Resource is not Valid");
            var t = new Resource
            {
                Title = dto?.Title ?? "",
                BriefDescription = dto?.BriefDescription ?? "",
                ExtensiveDescription = dto?.ExtensiveDescription == null ? "" : dto?.ExtensiveDescription,
                MetaDataID = dto?.MetaDataID ?? Guid.Empty,
                RenderPath = dto?.RenderPath ?? "",
                StoragePath = dto?.StoragePath ?? "",
                PreviewData = dto?.PreviewData == null ? "" : dto?.PreviewData,
                Dimension = dto?.Dimension,
                Tags = dto?.Tags == null ? "" : dto?.Tags,
                Type = dto?.Type == null ? ResourcesTypesEnum.Default.GetStringValue() : dto?.Type,
                UpdatedAt = DateTime.UtcNow,
            };
            return t;
        }

        #region Delegate Definition        
        public static bool IsResourceAllMetaDataDataValid<T>(T data) where T : class => true;
        #endregion
    }
}
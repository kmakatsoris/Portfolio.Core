using Portfolio.Core.Interfaces.Context;
using Portfolio.Core.Types.DTOs;
using Portfolio.Core.Types.Models.MetaData;

namespace Portfolio.Core.Utils.MetaDatasUtils
{
    public static class MetaDataUtils
    {
        public static bool IsMetaDataValid(this IMetaDataProperties MetaData) => MetaData != null &&
                                                     !string.IsNullOrEmpty(MetaData?.Email) &&
                                                     MetaData?.CreatedAt != null && MetaData?.CreatedAt != DateTime.MinValue && MetaData?.CreatedAt >= new DateTime(2024, 5, 1) &&
                                                     MetaData?.UpdatedAt != null && MetaData?.UpdatedAt != DateTime.MinValue && MetaData?.UpdatedAt >= new DateTime(2024, 5, 1) &&
                                                     !string.IsNullOrEmpty(MetaData?.Data);

        public delegate bool IsMetaDataAllDataValidDelegateType<T>(T data) where T : class; // Delegate Function Definition.

        // @TODO: Check
        public static bool IsMetaDataDTOValid<T>(this IMetaDataPropertiesDTO<T> metaDataDTO, IsMetaDataAllDataValidDelegateType<T> isMetaDataAllDataValidDelegate = null) where T : class
        {
            return metaDataDTO != null &&
                   !string.IsNullOrEmpty(metaDataDTO?.Email) &&
                   metaDataDTO?.CreatedAt != null && metaDataDTO.CreatedAt != DateTime.MinValue && metaDataDTO.CreatedAt >= new DateTime(2024, 5, 1) &&
                   metaDataDTO?.UpdatedAt != null && metaDataDTO.UpdatedAt != DateTime.MinValue && metaDataDTO.UpdatedAt >= new DateTime(2024, 5, 1) &&
                   isMetaDataAllDataValidDelegate != null && isMetaDataAllDataValidDelegate(metaDataDTO?.DataJSON) == true &&
                   !string.IsNullOrEmpty(metaDataDTO?.Data);
        }


    }
}
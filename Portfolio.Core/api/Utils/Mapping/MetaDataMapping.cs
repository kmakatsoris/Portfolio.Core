using Newtonsoft.Json;
using Portfolio.Core.Interfaces.Context;
using Portfolio.Core.Types.DTOs;
using Portfolio.Core.Types.Models.MetaData;
using Portfolio.Core.Utils.MetaDatasUtils;
using static Portfolio.Core.Utils.MetaDatasUtils.MetaDataUtils;

namespace Portfolio.Core.Utils.Mapping
{
    public static class MetaDataMapping
    {
        #region Map MetaData To MetaDataDTO
        public static MetaDataDTO<T> ToMap<T>(this IMetaDataProperties dto)
        {
            if (!MetaDataUtils.IsMetaDataValid(dto)) return new MetaDataDTO<T>();
            var t = new MetaDataDTO<T>
            {
                Email = dto?.Email,
                CreatedAt = dto?.CreatedAt ?? DateTime.UtcNow,
                UpdatedAt = dto?.UpdatedAt ?? DateTime.UtcNow,
                DataJSON = JsonConvert.DeserializeObject<T>(dto?.Data)
            };
            return t;
        }

        public static IEnumerable<MetaDataDTO<T>> ToMap<T>(this IEnumerable<IMetaDataProperties> dtos)
        {
            if (dtos == null || dtos?.Count() <= 0) return new List<MetaDataDTO<T>>();
            List<MetaDataDTO<T>> result = new List<MetaDataDTO<T>>();
            foreach (MetaDataModel dto in dtos)
            {
                if (MetaDataUtils.IsMetaDataValid(dto))
                    result.Add(dto?.ToMap<T>());
            }
            return result;
        }
        #endregion

        #region Map MetaDataDTO To MetaData
        public static MetaDataModel ToMap<T>(this MetaDataDTO<T> dto, IsMetaDataAllDataValidDelegateType<T> isMetaDataAllDataValidDelegate = null) where T : class
        {
            if (!MetaDataUtils.IsMetaDataDTOValid(dto, isMetaDataAllDataValidDelegate)) return new MetaDataModel();
            return new MetaDataModel
            {
                Email = dto?.Email,
                CreatedAt = dto?.CreatedAt ?? DateTime.UtcNow,
                UpdatedAt = dto?.UpdatedAt ?? DateTime.UtcNow,
                Data = dto?.Data
            };
        }
        #endregion
    }
}
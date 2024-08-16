using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Portfolio.Core.Interfaces.Context;
using Portfolio.Core.Types.Models.MetaData;

namespace Portfolio.Core.Types.Models.Reviews
{
    [Table("Reviews")]
    public class Review : MetaDataModel, IMetaDataProperties
    {
        public Review(MetaDataModel metaData)
        {
            Id = metaData.Id;
            Email = metaData.Email;
            CreatedAt = metaData.CreatedAt;
            UpdatedAt = metaData.UpdatedAt;
            Data = metaData.Data;
        }

        public Review() { }
    }
}
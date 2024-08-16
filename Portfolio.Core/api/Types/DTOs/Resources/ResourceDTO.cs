using System.Runtime.Serialization;
using Microsoft.AspNetCore.Html;
using Newtonsoft.Json;
using Portfolio.Core.CMS;
using Portfolio.Core.Interfaces.Context.Resources;
using Portfolio.Core.Types.DataTypes.Resources;
using Portfolio.Core.Types.Enums.Resources;
using Portfolio.Core.Utils.DefaultUtils;

namespace Portfolio.Core.Types.DTOs.Resources
{
    [DataContract]
    public class ResourceDTO : IResourceProperties
    {
        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "briefDescription")]
        public string BriefDescription { get; set; }

        [DataMember(Name = "extensiveDescription")]
        public IEnumerable<ExtensiveDescriptionType> ExtensiveDescriptionDTO { get; set; }
        public string ExtensiveDescription => JsonConvert.SerializeObject(ExtensiveDescriptionDTO);

        [DataMember(Name = "metaDataID")]
        public Guid MetaDataID { get; set; }

        [DataMember(Name = "metaData")]
        public string MetaData { get; set; }

        [DataMember(Name = "renderPath")]
        public string RenderPath { get; set; }

        [DataMember(Name = "storagePath")]
        public string StoragePath { get; set; }

        [DataMember(Name = "previewData")]
        public PreviewData PreviewDataDTO { get; set; }
        public string PreviewData => JsonConvert.SerializeObject(PreviewDataDTO);

        [DataMember(Name = "dimension")]
        public string Dimension { get; set; }

        [DataMember(Name = "tags")]
        public string Tags { get; set; }

        [DataMember(Name = "type")]
        public ResourcesTypesEnum TypeEnum { get; set; }
        public string Type => TypeEnum.GetStringValue();

        [DataMember(Name = "createdAt")]
        public DateTime CreatedAt { get; set; }

        [DataMember(Name = "updatedAt")]
        public DateTime UpdatedAt { get; set; }

    }

    [DataContract]
    public class ExtensiveDescriptionType
    {
        [DataMember(Name = "text")]
        public string Text { get; set; }

        [DataMember(Name = "color")]
        public string Color { get; set; }

        [DataMember(Name = "location")]
        public ResourcesExtensiveDescriptionLocationEnum Location { get; set; }
    }
}
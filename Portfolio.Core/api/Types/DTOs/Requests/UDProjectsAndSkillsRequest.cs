using System.Runtime.Serialization;

namespace Portfolio.Core.Types.DTOs.Requests
{
    [DataContract]
    public class UDProjectsAndSkillsRequest : BaseRequest
    {
        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "resourceIntroId")]
        public Guid ResourceIntroId { get; set; }

        [DataMember(Name = "category")]
        public string Category { get; set; }

        [DataMember(Name = "resourceMainId")]
        public Guid ResourceMainId { get; set; }

        [DataMember(Name = "tagName")]
        public string[] TagNames { get; set; }

        [DataMember(Name = "comments")]
        public string Comments { get; set; }
    }
}
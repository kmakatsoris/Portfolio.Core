using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Portfolio.Core.Types.Contracts
{
    [DataContract]
    public class BasicSkillsAndProjectsDTOType<G, C>
    {
        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "briefDescription")]
        public string BriefDescription { get; set; }

        [DataMember(Name = "extensiveDescription")]
        public IEnumerable<string> ExtensiveDescriptionDTO { get; set; }
        public string ExtensiveDescription => JsonConvert.SerializeObject(ExtensiveDescriptionDTO);

        [DataMember(Name = "resourceIntroId")]
        public Guid ResourceIntroId { get; set; }

        [DataMember(Name = "category")]
        public string Category { get; set; }

        [DataMember(Name = "createdAt")]
        public DateTime CreatedAt { get; set; }

        [DataMember(Name = "resourceMainId")]
        public Guid ResourceMainId { get; set; }

        [DataMember(Name = "tags")]
        public IEnumerable<string> TagsDTO { get; set; }
        public string Tags => JsonConvert.SerializeObject(TagsDTO);

        public G GradesDTO { get; set; } // Mine Grade
        [DataMember(Name = "grades")]
        public string Grades => JsonConvert.SerializeObject(GradesDTO);

        [DataMember(Name = "comment")]
        public IEnumerable<C> CommentsDTO { get; set; } // Others Review & Grades
        public string Comments => JsonConvert.SerializeObject(CommentsDTO);

    }
}
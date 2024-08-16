using System.Runtime.Serialization;

namespace Portfolio.Core.Types.DTOs.Requests.Resources
{
    [DataContract]
    public class ResourceRequest
    {
        [DataMember(Name = "path")]
        public string Path { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }
    }
}
using System.Runtime.Serialization;
using Portfolio.Core.Types.DTOs.Projects;

namespace Portfolio.Core.Types.DTOs.Requests
{
    [DataContract]
    public class IUProjectRequest : BaseRequest
    {
        [DataMember(Name = "projects")]
        public ProjectsDTO Projects { get; set; }
    }
}
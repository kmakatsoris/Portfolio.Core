using System.Runtime.Serialization;
using Portfolio.Core.Types.DTOs.Requests.Identity;
using Portfolio.Core.Types.DTOs.Skills;

namespace Portfolio.Core.Types.DTOs.Requests
{
    [DataContract]
    public class IUSkillRequest : BaseRequest
    {
        [DataMember(Name = "skills")]
        public SkillsDTO Skills { get; set; }
    }
}
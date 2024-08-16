using System.Runtime.Serialization;

namespace Portfolio.Core.Types.DTOs.Requests
{
    [DataContract]
    public class BaseIdRequest
    {
        [DataMember(Name = "id")]
        public Guid Id { get; set; }
    }
}
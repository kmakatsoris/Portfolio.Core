using System.Runtime.Serialization;
using Portfolio.Core.Interfaces.Contract;
using Portfolio.Core.Types.DTOs;
using Portfolio.Core.Types.DTOs.Reviews;

namespace Portfolio.Core.Types.Contracts
{
    [DataContract]
    public abstract class BasicCommentType<G, R> : IBasicCommentTypeContract<G, R>
    {
        // Abstract property that must be implemented by derived classes
        [DataMember(Name = "grade")]
        public abstract G Grade { get; set; }

        [DataMember(Name = "review")]
        public MetaDataDTO<R> Review { get; set; }
    }
}
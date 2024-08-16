using Portfolio.Core.Types.DTOs;
using Portfolio.Core.Types.DTOs.Reviews;

namespace Portfolio.Core.Interfaces.Contract
{
    public interface IBasicCommentTypeContract<G, R>
    {
        public MetaDataDTO<R> Review { get; set; }
        public abstract G Grade { get; set; }
    }
}
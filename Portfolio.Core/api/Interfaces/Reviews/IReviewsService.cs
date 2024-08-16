using Portfolio.Core.Types.DTOs;
using Portfolio.Core.Types.DTOs.Requests;
using Portfolio.Core.Types.DTOs.Responses;
using Portfolio.Core.Types.DTOs.Reviews;

namespace Portfolio.Core.Interfaces.Reviews
{
    public interface IReviewsService
    {
        Task<IEnumerable<ReviewDTO>> GetAllReviews();
        Task<ReviewDTO> GetReview(BaseRequest request);
        Task<BaseResponse> InsertReview(string token, ReviewDTO reviewDTO);
        Task<BaseResponse> UpdateReview(string token, ReviewDTO reviewDTO);
        Task<BaseResponse> AppendDataJson(string token, MetaDataDTO<ReviewData> reviewDTO);
        Task<BaseResponse> AlterKeyDataJson(string token, MetaDataDTO<ReviewData> reviewDTO, string key);
        Task<BaseResponse> DeleteReview(string token, BaseRequest request);
    }
}
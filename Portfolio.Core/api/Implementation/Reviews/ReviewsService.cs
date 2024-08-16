using NLog;
using Portfolio.Core.Interfaces.Context.Reviews;
using Portfolio.Core.Interfaces.Identity;
using Portfolio.Core.Interfaces.Reviews;
using Portfolio.Core.Types.DTOs;
using Portfolio.Core.Types.DTOs.Requests;
using Portfolio.Core.Types.DTOs.Responses;
using Portfolio.Core.Types.DTOs.Reviews;
using Portfolio.Core.Types.Models.Reviews;
using Portfolio.Core.Utils.Mapping;
using ILogger = NLog.ILogger;

namespace Portfolio.Core.Implementation.Reviews
{
    public partial class ReviewsService : IReviewsService
    {
        private readonly ILogger _logger;
        private readonly IReviewRepository _reviewRepository;
        private readonly IOAuthAuthorizationService _oauthService;

        public ReviewsService
        (
            ILogger logger,
            IReviewRepository reviewRepository,
            IOAuthAuthorizationService oauthService
        )
        {
            _logger = logger;
            _reviewRepository = reviewRepository;
            _oauthService = oauthService;
        }

        public async Task<IEnumerable<ReviewDTO>> GetAllReviews()
        {
            return ReviewsMapping.ToMap(await _reviewRepository?.GetAllReviewsAsync()) ?? new List<ReviewDTO>();
        }

        public async Task<ReviewDTO> GetReview(BaseRequest request)
        {
            if (string.IsNullOrEmpty(request?.Email)) return new ReviewDTO();
            return ReviewsMapping.ToMap(await _reviewRepository?.GetReviewAsync(request?.Email)) ?? new ReviewDTO();
        }

        public async Task<BaseResponse> InsertReview(string token, ReviewDTO reviewDTO)
        {
            if (_oauthService?.GetEmailFromToken(token)?.ToLower()?.Equals(reviewDTO?.MetaData?.Email?.ToLower()) != true) throw new Exception("Sorry, but you do not have permission to ADD other people's reviews.");
            return new BaseResponse { Success = await _reviewRepository?.AddReviewAsync(ReviewsMapping.ToMap(reviewDTO)) };
        }

        public async Task<BaseResponse> UpdateReview(string token, ReviewDTO reviewDTO)
        {
            if (_oauthService?.GetEmailFromToken(token)?.ToLower()?.Equals(reviewDTO?.MetaData?.Email?.ToLower()) != true) throw new Exception("Sorry, but you do not have permission to EDIT other people's reviews.");
            return new BaseResponse { Success = await _reviewRepository?.UpdateReviewAsync(ReviewsMapping.ToMap(reviewDTO)) };
        }
        public async Task<BaseResponse> AppendDataJson(string token, MetaDataDTO<ReviewData> reviewDTO)
        {
            if (_oauthService?.GetEmailFromToken(token)?.ToLower()?.Equals(reviewDTO?.Email?.ToLower()) != true) throw new Exception("Sorry, but you do not have permission to APPEND other people's reviews.");
            return new BaseResponse { Success = await _reviewRepository?.AppendDataJson(reviewDTO?.Email, reviewDTO?.DataJSON) };
        }
        public async Task<BaseResponse> AlterKeyDataJson(string token, MetaDataDTO<ReviewData> reviewDTO, string key)
        {
            if (_oauthService?.GetEmailFromToken(token)?.ToLower()?.Equals(reviewDTO?.Email?.ToLower()) != true) throw new Exception("Sorry, but you do not have permission to ALTER KEY other people's reviews.");
            return new BaseResponse { Success = await _reviewRepository?.AlterKeyDataJson(reviewDTO?.Email, reviewDTO?.DataJSON, key) };
        }

        public async Task<BaseResponse> DeleteReview(string token, BaseRequest request)
        {
            if (_oauthService?.GetEmailFromToken(token)?.ToLower()?.Equals(request?.Email?.ToLower()) != true) throw new Exception("Sorry, but you do not have permission to DELETE other people's reviews.");
            return new BaseResponse { Success = await _reviewRepository?.DeleteReviewAsync(request?.Email) };
        }
    }
}
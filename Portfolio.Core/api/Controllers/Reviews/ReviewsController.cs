using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Core.Exceptions;
using Portfolio.Core.Interfaces.Reviews;
using Portfolio.Core.Types.DTOs.Requests;
using Portfolio.Core.Types.DTOs.Requests.Identity;
using Portfolio.Core.Types.DTOs.Responses;
using Portfolio.Core.Types.DTOs.Reviews;
using Portfolio.Core.Utils.DefaultUtils;
using Portfolio.Core.Utils.UsersUtils;

namespace Portfolio.Core.Controllers.Reviews
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewsService _reviewsService;

        public ReviewsController(IReviewsService reviewsService)
        {
            _reviewsService = reviewsService;
        }

        [HttpPost("list")]
        public async Task<IEnumerable<ReviewDTO>> GetAllReviews()
        {
            return await DefaultException.ExceptionControllerHandler(async () => { 
                    return await _reviewsService?.GetAllReviews();
                });               
        }

        [HttpPost("find")]
        public async Task<ReviewDTO> GetReview([FromBody] BaseRequest request)
        {
            return await DefaultException.ExceptionControllerHandler(async () => { 
                    return await _reviewsService?.GetReview(request);
                });             
        }

        [HttpPost("insert")]
        [Authorize(Policy = "AdminOrUser")]
        public async Task<BaseResponse> InsertReview([FromBody] IUReviewRequest request)
        {
            return await DefaultException.ExceptionControllerHandler(async () => { 
                    return await _reviewsService?.InsertReview(UsersUtils.ExtractToken(Request?.Headers), request?.Review);
                });                  
        }

        [HttpPost("edit")]
        [Authorize(Policy = "AdminOrUser")]
        public async Task<BaseResponse> UpdateReview([FromBody] IUReviewRequest request)
        {
            return await DefaultException.ExceptionControllerHandler(async () => { 
                    return await _reviewsService?.UpdateReview(UsersUtils.ExtractToken(Request?.Headers), request?.Review);
                });               
        }

        /*
        [HttpPost("append")]
        [Authorize(Policy = "AdminOrUser")]
        public async Task<BaseResponse> AppendReview([FromBody] IUReviewSingleDataRequest request)
        {
            return await _reviewsService?.AppendDataJson(UsersUtils.ExtractToken(Request?.Headers), request?.Review);
        }
        [HttpPost("alterKey")]
        [Authorize(Policy = "AdminOrUser")]
        public async Task<BaseResponse> AlterKey([FromBody] IUReviewSingleDataRequest request)
        {
            return await _reviewsService?.AlterKeyDataJson(UsersUtils.ExtractToken(Request?.Headers), request?.Review, DefaultUtils.GetStringValue(request?.Review?.DataJSON?.Title));
        }
        */

        [HttpPost("delete")]
        [Authorize(Policy = "AdminOrUser")]
        public async Task<BaseResponse> DeleteReview([FromBody] IdentityBaseRequest request)
        {
            return await DefaultException.ExceptionControllerHandler(async () => { 
                    return await _reviewsService?.DeleteReview(UsersUtils.ExtractToken(Request?.Headers), new BaseRequest { Email = request?.Email });
                });             
        }
    }
}
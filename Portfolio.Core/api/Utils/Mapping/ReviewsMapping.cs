using Newtonsoft.Json;
using Portfolio.Core.Types.DTOs;
using Portfolio.Core.Types.DTOs.Reviews;
using Portfolio.Core.Types.Models.Reviews;
using Portfolio.Core.Utils.ReviewsUtils;

namespace Portfolio.Core.Utils.Mapping
{
    public static class ReviewsMapping
    {
        #region Map Review To ReviewDTO
        public static ReviewDTO ToMap(this Review dto)
        {
            var t = new ReviewDTO
            {
                MetaData = dto?.ToMap<ReviewAllData>()
            };
            return t;
        }

        public static IEnumerable<ReviewDTO> ToMap(this IEnumerable<Review> dtos)
        {
            return dtos?.ToMap<ReviewAllData>() as IEnumerable<ReviewDTO>; // Not throwing exception.
        }
        #endregion

        #region Map ReviewDTO To Review
        public static Review ToMap(this ReviewDTO dto)
        {
            return new Review(dto?.MetaData?.ToMap<ReviewAllData>(data => data != null && data.IsReviewAllDataValid()));
        }
        #endregion
    }
}
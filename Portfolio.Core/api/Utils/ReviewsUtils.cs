using Portfolio.Core.Types.DTOs;
using Portfolio.Core.Types.DTOs.Reviews;
using Portfolio.Core.Types.Enums.Reviews;
using Portfolio.Core.Types.Models.Reviews;
using Portfolio.Core.Utils.MetaDatasUtils;

namespace Portfolio.Core.Utils.ReviewsUtils
{
    public static class ReviewUtils
    {
        public static bool IsReviewValid(this Review review) => review?.IsMetaDataValid() == true;

        public static bool IsReviewDTOValid(this ReviewDTO reviewDTO) => reviewDTO != null &&
                                                                         reviewDTO.MetaData != null &&
                                                                         reviewDTO.MetaData.IsMetaDataDTOValid<ReviewAllData>(data => data != null && data.IsReviewAllDataValid());

        #region Delegate Definition
        private static bool IsReviewDataValid(this ReviewData reviewData) => reviewData != null &&
                                                                 !string.IsNullOrEmpty(reviewData?.Characteristics) &&
                                                                 !string.IsNullOrEmpty(reviewData?.Highlights) &&
                                                                 reviewData?.Grade != null &&
                                                                 Enum.IsDefined(typeof(ReviewCategoryTitleEnum), reviewData?.Title);

        public static bool IsReviewAllDataValid(this ReviewAllData reviewAllData) => reviewAllData != null &&
                                                                                      reviewAllData?.CommunicationSkillsData?.IsReviewDataValid() == true &&
                                                                                      reviewAllData?.CollaborationSkillsData?.IsReviewDataValid() == true &&
                                                                                      reviewAllData?.ProblemSolvingSkillsData?.IsReviewDataValid() == true &&
                                                                                      reviewAllData?.AdaptabilitySkillsData?.IsReviewDataValid() == true &&
                                                                                      reviewAllData?.TimeManagementSkillsData?.IsReviewDataValid() == true &&
                                                                                      reviewAllData?.LeadershipSkillsData?.IsReviewDataValid() == true &&
                                                                                      reviewAllData?.EmotionalIntelligenceSkillsData?.IsReviewDataValid() == true &&
                                                                                      reviewAllData?.TechnicalSkillsData?.IsReviewDataValid() == true &&
                                                                                      reviewAllData?.CriticalThinkingSkillsData?.IsReviewDataValid() == true &&
                                                                                      reviewAllData?.ContinuousLearningSkillsData?.IsReviewDataValid() == true;
        #endregion
    }
}
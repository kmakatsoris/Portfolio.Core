using System.Runtime.Serialization;
using Portfolio.Core.Types.Enums.Reviews;

namespace Portfolio.Core.Types.DTOs.Reviews
{
    // @TAG: ReviewCategoryTitle
    [DataContract]
    public class ReviewAllData
    {
        [DataMember(Name = "communicationSkillsData")]
        public ReviewData CommunicationSkillsData { get; set; } = new ReviewData(ReviewCategoryTitleEnum.CommunicationSkills);

        [DataMember(Name = "collaborationSkillsData")]
        public ReviewData CollaborationSkillsData { get; set; } = new ReviewData(ReviewCategoryTitleEnum.CollaborationSkills);

        [DataMember(Name = "problemSolvingSkillsData")]
        public ReviewData ProblemSolvingSkillsData { get; set; } = new ReviewData(ReviewCategoryTitleEnum.ProblemSolvingSkills);

        [DataMember(Name = "adaptabilitySkillsData")]
        public ReviewData AdaptabilitySkillsData { get; set; } = new ReviewData(ReviewCategoryTitleEnum.AdaptabilitySkills);

        [DataMember(Name = "timeManagementSkillsData")]
        public ReviewData TimeManagementSkillsData { get; set; } = new ReviewData(ReviewCategoryTitleEnum.TimeManagementSkills);

        [DataMember(Name = "leadershipSkillsData")]
        public ReviewData LeadershipSkillsData { get; set; } = new ReviewData(ReviewCategoryTitleEnum.LeadershipSkills);

        [DataMember(Name = "emotionalIntelligenceSkillsData")]
        public ReviewData EmotionalIntelligenceSkillsData { get; set; } = new ReviewData(ReviewCategoryTitleEnum.EmotionalIntelligenceSkills);

        [DataMember(Name = "technicalSkillsData")]
        public ReviewData TechnicalSkillsData { get; set; } = new ReviewData(ReviewCategoryTitleEnum.TechnicalSkills);

        [DataMember(Name = "criticalThinkingSkillsData")]
        public ReviewData CriticalThinkingSkillsData { get; set; } = new ReviewData(ReviewCategoryTitleEnum.CriticalThinkingSkills);

        [DataMember(Name = "continuousLearningSkillsData")]
        public ReviewData ContinuousLearningSkillsData { get; set; } = new ReviewData(ReviewCategoryTitleEnum.ContinuousLearningSkills);
    }

}
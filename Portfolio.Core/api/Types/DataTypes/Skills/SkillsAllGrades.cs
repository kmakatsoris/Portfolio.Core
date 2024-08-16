using System.Runtime.Serialization;
using Portfolio.Core.Types.Contracts;
using Portfolio.Core.Types.Enums.Skills;

namespace Portfolio.Core.Types.DataTypes.Skills
{
    // @TAG: SkillsGradeTitle
    [DataContract]
    public class SkillsAllGrades
    {
        [DataMember(Name = "knowledgeLevelSkillsGrade")]
        public ProjectsOrSkillsGrade<SkillsGradeTitleEnum> KnowledgeLevel { get; set; } = new ProjectsOrSkillsGrade<SkillsGradeTitleEnum>(SkillsGradeTitleEnum.KnowledgeLevel);

        [DataMember(Name = "yearsOfExperienceSkillsGrade")]
        public ProjectsOrSkillsGrade<SkillsGradeTitleEnum> YearsOfExperience { get; set; } = new ProjectsOrSkillsGrade<SkillsGradeTitleEnum>(SkillsGradeTitleEnum.YearsOfExperience);

        [DataMember(Name = "problemSolvingAbilitySkillsGrade")]
        public ProjectsOrSkillsGrade<SkillsGradeTitleEnum> ProblemSolvingAbility { get; set; } = new ProjectsOrSkillsGrade<SkillsGradeTitleEnum>(SkillsGradeTitleEnum.ProblemSolvingAbility);

        [DataMember(Name = "adaptabilitySkillsGrade")]
        public ProjectsOrSkillsGrade<SkillsGradeTitleEnum> Adaptability { get; set; } = new ProjectsOrSkillsGrade<SkillsGradeTitleEnum>(SkillsGradeTitleEnum.Adaptability);

        [DataMember(Name = "CollaborationSkillsData")]
        public ProjectsOrSkillsGrade<SkillsGradeTitleEnum> CollaborationSkills { get; set; } = new ProjectsOrSkillsGrade<SkillsGradeTitleEnum>(SkillsGradeTitleEnum.CollaborationSkills);
    }
}
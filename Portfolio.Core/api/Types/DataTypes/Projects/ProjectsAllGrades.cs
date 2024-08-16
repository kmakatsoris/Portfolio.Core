using System.Runtime.Serialization;
using Portfolio.Core.Types.Contracts;
using Portfolio.Core.Types.Enums.Projects;

namespace Portfolio.Core.Types.DataTypes.Projects
{
    // @TAG: ProjectsGradeTitle
    [DataContract]
    public class ProjectsAllGrades
    {
        [DataMember(Name = "timeAssociation")]
        public ProjectsOrSkillsGrade<ProjectsGradeTitleEnum> TimeAssociation { get; set; } = new ProjectsOrSkillsGrade<ProjectsGradeTitleEnum>(ProjectsGradeTitleEnum.TimeAssociation);

        [DataMember(Name = "projectSize")]
        public ProjectsOrSkillsGrade<ProjectsGradeTitleEnum> ProjectSize { get; set; } = new ProjectsOrSkillsGrade<ProjectsGradeTitleEnum>(ProjectsGradeTitleEnum.ProjectSize);

        [DataMember(Name = "projectImpact")]
        public ProjectsOrSkillsGrade<ProjectsGradeTitleEnum> ProjectImpact { get; set; } = new ProjectsOrSkillsGrade<ProjectsGradeTitleEnum>(ProjectsGradeTitleEnum.ProjectImpact);

        [DataMember(Name = "technologicalComplexity")]
        public ProjectsOrSkillsGrade<ProjectsGradeTitleEnum> TechnologicalComplexity { get; set; } = new ProjectsOrSkillsGrade<ProjectsGradeTitleEnum>(ProjectsGradeTitleEnum.TechnologicalComplexity);

        [DataMember(Name = "stakeholderSatisfaction")]
        public ProjectsOrSkillsGrade<ProjectsGradeTitleEnum> StakeholderSatisfaction { get; set; } = new ProjectsOrSkillsGrade<ProjectsGradeTitleEnum>(ProjectsGradeTitleEnum.StakeholderSatisfaction);

        [DataMember(Name = "userAdoption")]
        public ProjectsOrSkillsGrade<ProjectsGradeTitleEnum> UserAdoption { get; set; } = new ProjectsOrSkillsGrade<ProjectsGradeTitleEnum>(ProjectsGradeTitleEnum.UserAdoption);
    }
}
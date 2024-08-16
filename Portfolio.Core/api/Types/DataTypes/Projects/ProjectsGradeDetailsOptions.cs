using Portfolio.Core.Types.Enums.Projects;

namespace Portfolio.Core.Types.DataTypes.Projects
{
    // @TAG: ProjectsGradeTitle
    public class ProjectsGradeDetailsOptions
    {
        // The total time from start to finish of the project. Example: 6 months, 1 year, 2 years
        public Dictionary<ProjectsGradeTitleEnum, List<string>> TimeAssociation = new Dictionary<ProjectsGradeTitleEnum, List<string>>
        {
            { ProjectsGradeTitleEnum.TimeAssociation, new List<string>()
            {
                "Grade 1: Simple - Interaction less than 3 MONTHS",
                "Grade 2: Intermediate - Interaction 3-6 MONTHS",
                "Grade 3: Advanced - Interaction 6-12 MONTHS",
                "Grade 4: Expert - Interaction 1-2 YEARS",
                "Grade 5: Innovative - Interaction more than 2 YEARS"
            } }
        };

        // The complexity of your role in the project. Example: 1 (Basic), 2 (Intermediate), 3 (Advanced)
        public Dictionary<ProjectsGradeTitleEnum, List<string>> ProjectSize = new Dictionary<ProjectsGradeTitleEnum, List<string>>
        {
            { ProjectsGradeTitleEnum.ProjectSize, new List<string>()
            {
                "Grade 1: Basic - Complexity of your role in the project.",
                "Grade 2: Intermediate - Complexity of your role in the project.",
                "Grade 3: Advanced - Complexity of your role in the project.",
                "Grade 4: Expert - Complexity of your role in the project.",
                "Grade 5: Innovative - Complexity of your role in the project."
            } }
        };

        // The scale or size of the project in terms of team members, budget, or scope. Example: 1 (Small), 2 (Medium), 3 (Large)
        public Dictionary<ProjectsGradeTitleEnum, List<string>> ProjectImpact = new Dictionary<ProjectsGradeTitleEnum, List<string>>
        {
            { ProjectsGradeTitleEnum.ProjectImpact, new List<string>()
            {
                "Grade 1: Small - Team, budget, or scope",
                "Grade 2: Medium - Team, budget, or scope.",
                "Grade 3: Large - Team, budget, or scope",
                "Grade 4: Extreme - Team, budget, or scope"
            } }
        };

        // The complexity of the technology stack used in the project. Example: 1 (Low), 2 (Medium), 3 (High)
        public Dictionary<ProjectsGradeTitleEnum, List<string>> TechnologicalComplexity = new Dictionary<ProjectsGradeTitleEnum, List<string>>
        {
            { ProjectsGradeTitleEnum.TechnologicalComplexity, new List<string>()
            {
                "Grade 1: Small - The complexity of the technology stack used in the project.",
                "Grade 2: Medium - The complexity of the technology stack used in the project.",
                "Grade 3: Large - The complexity of the technology stack used in the project.",
                "Grade 4: Extreme - The complexity of the technology stack used in the project."
            } }
        };

        // Feedback or rating from stakeholders. Example: 1 (Poor), 2 (Average), 3 (Excellent)
        public Dictionary<ProjectsGradeTitleEnum, List<string>> StakeholderSatisfaction = new Dictionary<ProjectsGradeTitleEnum, List<string>>
        {
            { ProjectsGradeTitleEnum.StakeholderSatisfaction, new List<string>()
            {
                "Grade 1: Poor - Feedback or rating from stakeholders.",
                "Grade 2: Average - Feedback or rating from stakeholders.",
                "Grade 3: Excellent - Feedback or rating from stakeholders.",
                "Grade 4: Extremely Satisfied - Feedback or rating from stakeholders."
            } }
        };

        // The number of users or clients adopting the project deliverables. Example: 100 users, 1000 users, etc.
        public Dictionary<ProjectsGradeTitleEnum, List<string>> UserAdoption = new Dictionary<ProjectsGradeTitleEnum, List<string>>
        {
            { ProjectsGradeTitleEnum.UserAdoption, new List<string>()
            {
                "Grade 1: Less Than 100 - Nnumber of users or clients adopting/using the project deliverables.",
                "Grade 2: 100-1K - Number of users or clients adopting/using the project deliverables.",
                "Grade 3: 1K-5K - Number of users or clients adopting/using the project deliverables.",
                "Grade 4: 5K-10K - Number of users or clients adopting/using the project deliverables.",
                "Grade 5: 10K+ - Number of users or clients adopting/using the project deliverables."
            } }
        };
    }
}
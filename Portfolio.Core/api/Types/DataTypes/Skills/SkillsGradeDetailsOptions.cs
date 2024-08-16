using Portfolio.Core.Types.Enums.Skills;

namespace Portfolio.Core.Types.DataTypes.Skills
{
    // @TAG: SkillsGradeTitle
    public class SkillsGradeDetailsOptions
    {
        public Dictionary<SkillsGradeTitleEnum, List<string>> KnowledgeLevel = new Dictionary<SkillsGradeTitleEnum, List<string>>
        {
            { SkillsGradeTitleEnum.KnowledgeLevel, new List<string>()
            {
                "Grade 1: Novice - Limited or no experience with the skill.",
                "Grade 2: Beginner - Basic understanding of concepts, with some practical experience.",
                "Grade 3: Intermediate - Comfortable with the skill, able to work independently on most tasks.",
                "Grade 4: Advanced - Strong proficiency in the skill, capable of solving complex problems and mentoring others."
            } }
        };
        public Dictionary<SkillsGradeTitleEnum, List<string>> YearsOfExperience = new Dictionary<SkillsGradeTitleEnum, List<string>>
        {
            { SkillsGradeTitleEnum.YearsOfExperience, new List<string>()
            {
                "Grade 1: Less than 1 year - New to the skill, with minimal practical experience.",
                "Grade 2: 1-3 years - Has gained some experience and familiarity with the skill over time.",
                "Grade 3: 3-5 years - Moderately experienced, with several years of practical usage.",
                "Grade 4: 5+ years - Highly experienced, with a long history of using the skill in various projects and contexts."
            } }
        };
        public Dictionary<SkillsGradeTitleEnum, List<string>> ProblemSolvingAbility = new Dictionary<SkillsGradeTitleEnum, List<string>>
        {
            { SkillsGradeTitleEnum.ProblemSolvingAbility, new List<string>()
            {
                "Grade 1: Limited - Struggles to identify and solve problems effectively.",
                "Grade 2: Competent - Can solve straightforward problems with some assistance.",
                "Grade 3: Skilled - Proficient in problem-solving, able to tackle complex issues independently.",
                "Grade 4: Expert - Excels in problem-solving, able to devise innovative solutions to challenging problems."
            } }
        };
        public Dictionary<SkillsGradeTitleEnum, List<string>> Adaptability = new Dictionary<SkillsGradeTitleEnum, List<string>>
        {
            { SkillsGradeTitleEnum.ProblemSolvingAbility, new List<string>()
            {
                "Grade 1: Resistant to Change - Prefers sticking to familiar methods and technologies.",
                "Grade 2: Accepts Change - Willing to try new approaches but may require some guidance.",
                "Grade 3: Adaptable - Quickly adjusts to new technologies and methodologies.",
                "Grade 4: Agile - Thrives in dynamic environments, adept at learning and applying new skills rapidly."
            } }
        };

        public Dictionary<SkillsGradeTitleEnum, List<string>> CollaborationSkills = new Dictionary<SkillsGradeTitleEnum, List<string>>
        {
            { SkillsGradeTitleEnum.ProblemSolvingAbility, new List<string>()
            {
                "Grade 1: Limited - Struggles to work effectively in team settings.",
                "Grade 2: Cooperative - Able to collaborate with others but may require supervision.",
                "Grade 3: Team Player - Works well in teams, communicates effectively, and contributes positively to group efforts.",
                "Grade 4: Leader - Takes on leadership roles, fosters teamwork, and drives successful outcomes."
            } }
        };
    }
}
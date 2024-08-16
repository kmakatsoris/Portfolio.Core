using Portfolio.Core.Interfaces.Contract;

namespace Portfolio.Core.Interfaces.Context.Projects
{
    public interface IProjectsProperties : IBasicProjectsAndSkillsPropertiesContract
    {
        public string MemberLabels { get; } // IEnumarable<string>                
        public DateTime UpdatedAt { get; set; }
    }
}
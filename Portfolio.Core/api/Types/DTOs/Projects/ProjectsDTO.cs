using System.Runtime.Serialization;
using Newtonsoft.Json;
using Portfolio.Core.Interfaces.Context.Projects;
using Portfolio.Core.Types.Contracts;
using Portfolio.Core.Types.DataTypes.Projects;

namespace Portfolio.Core.Types.DTOs.Projects
{
    [DataContract]
    public class ProjectsDTO : BasicSkillsAndProjectsDTOType<ProjectsAllGrades, ProjectsComment>, IProjectsProperties
    {
        [DataMember(Name = "updatedAt")]
        public DateTime UpdatedAt { get; set; }

        [DataMember(Name = "memberLabels")]
        public string MemberLabels { get; set; }
    }
}
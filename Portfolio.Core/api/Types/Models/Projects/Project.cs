using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Portfolio.Core.Interfaces.Context.Projects;
using Portfolio.Core.Types.Contracts;

namespace Portfolio.Core.Types.Models.Projects
{
    [Table("Projects")]
    public class ProjectModel : BasicSkillsAndProjectsType, IProjectsProperties
    {

    }
}
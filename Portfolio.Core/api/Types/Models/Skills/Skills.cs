using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Portfolio.Core.Interfaces.Context.Skills;
using Portfolio.Core.Types.Contracts;

namespace Portfolio.Core.Types.Models.Skills
{
    [Table("Skills")]
    public class SkillsModel : BasicSkillsAndProjectsType, ISkillsProperties
    {
        
    }
}
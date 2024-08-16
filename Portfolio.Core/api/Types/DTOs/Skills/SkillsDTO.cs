using System.Runtime.Serialization;
using Newtonsoft.Json;
using Portfolio.Core.Interfaces.Context.Skills;
using Portfolio.Core.Interfaces.Contract;
using Portfolio.Core.Types.Contracts;
using Portfolio.Core.Types.DataTypes.Skills;

namespace Portfolio.Core.Types.DTOs.Skills
{
    [DataContract]
    public class SkillsDTO : BasicSkillsAndProjectsDTOType<SkillsAllGrades, SkillsComment>, ISkillsProperties
    {

    }
}
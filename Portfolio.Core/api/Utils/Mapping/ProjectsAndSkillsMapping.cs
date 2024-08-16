using Newtonsoft.Json;
using Portfolio.Core.Interfaces.Contract;
using Portfolio.Core.Types.Contracts;
using Portfolio.Core.Utils.ProjectsUtils;

namespace Portfolio.Core.Utils.Mapping
{
    public static class ProjectsAndSkillsMapping
    {
        #region Map Skills/Projects To  SkillsDTO/ProjectsDTO
        public static IBasicProjectsAndSkillsDTOPropertiesContract<G, C> ToMap<G, C>(this BasicSkillsAndProjectsType dto)
        {
            if (!ProjectsAndSkillsUtils.IsProject_OR_SkillValid(dto)) return (IBasicProjectsAndSkillsDTOPropertiesContract<G, C>)new BasicSkillsAndProjectsDTOType<G, C>();
            var t = new BasicSkillsAndProjectsDTOType<G, C>
            {
                Title = dto?.Title,
                BriefDescription = dto?.BriefDescription,
                ExtensiveDescriptionDTO = JsonConvert.DeserializeObject<IEnumerable<string>>(dto?.ExtensiveDescription),
                ResourceIntroId = dto?.ResourceIntroId ?? Guid.Empty,
                Category = dto?.Category,
                ResourceMainId = dto?.ResourceMainId ?? Guid.Empty,
                TagsDTO = JsonConvert.DeserializeObject<IEnumerable<string>>(dto?.Tags),
                GradesDTO = JsonConvert.DeserializeObject<G>(dto?.Grades),
                CommentsDTO = JsonConvert.DeserializeObject<IEnumerable<C>>(dto?.Comments),
            };
            return (IBasicProjectsAndSkillsDTOPropertiesContract<G, C>)t;
        }

        // <SkillsAllGrades, SkillsComment> || <ProjectsAllGrades, ProjectsComment>
        public static IEnumerable<BasicSkillsAndProjectsDTOType<G, C>> ToMap<G, C>(this IEnumerable<IBasicProjectsAndSkillsPropertiesContract> dtos)
        {
            if (dtos == null || dtos?.Count() <= 0) return new List<BasicSkillsAndProjectsDTOType<G, C>>();
            List<BasicSkillsAndProjectsDTOType<G, C>> result = new List<BasicSkillsAndProjectsDTOType<G, C>>();
            foreach (BasicSkillsAndProjectsType dto in dtos)
            {
                if (ProjectsAndSkillsUtils.IsProject_OR_SkillValid(dto))
                    result.Add((BasicSkillsAndProjectsDTOType<G, C>)(dto?.ToMap<G, C>()));
            }
            return result;
        }
        #endregion

        #region Map  SkillsDTO/ProjectsDTO To  Skills/Projects
        public static BasicSkillsAndProjectsType ToMap<G, C>(this BasicSkillsAndProjectsDTOType<G, C> dto)
        {
            if (!ProjectsAndSkillsUtils.IsProject_OR_SkillDTOValid<G, C>((IBasicProjectsAndSkillsDTOPropertiesContract<G, C>)dto)) return new BasicSkillsAndProjectsType();
            return new BasicSkillsAndProjectsType
            {
                Title = dto?.Title,
                BriefDescription = dto?.BriefDescription,
                ExtensiveDescription = dto?.ExtensiveDescription,
                ResourceIntroId = dto?.ResourceIntroId ?? Guid.Empty,
                Category = dto?.Category,
                ResourceMainId = dto?.ResourceMainId ?? Guid.Empty,
                Tags = dto?.Tags,
                Grades = dto?.Grades,
                Comments = dto?.Comments,
            };
        }
        #endregion
    }
}
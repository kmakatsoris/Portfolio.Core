

using Portfolio.Core.Types.DTOs.Requests;
using Portfolio.Core.Types.Models.Skills;

namespace Portfolio.Core.Interfaces.Context.Skills
{
    public interface ISkillsRepository
    {
        #region Fetch Methods [R from CRUD]        
        Task<IEnumerable<SkillsModel>> GetAllSkills();
        Task<IEnumerable<SkillsModel>> GetSkillsByCategory(UDProjectsAndSkillsRequest request);
        Task<SkillsModel> GetSkillsByTitle(UDProjectsAndSkillsRequest request);
        Task<IEnumerable<SkillsModel>> GetSkillsByTags(UDProjectsAndSkillsRequest request);
        Task<IEnumerable<SkillsModel>> GetAllSkillsByTagsAsync(List<string> tags);
        #endregion

        #region Create Methods [C from CRUD]        
        Task<bool> InsertSkill(SkillsModel skill);
        #endregion

        #region Update Methods [U from CRUD]        
        Task<bool> UpdateSkill(SkillsModel skill);
        Task<bool> UpdateSkillComment(UDProjectsAndSkillsRequest request);
        #endregion

        #region Delete Methods [D from CRUD]        
        Task<bool> DeleteSkill(UDProjectsAndSkillsRequest request);
        #endregion
    }
}
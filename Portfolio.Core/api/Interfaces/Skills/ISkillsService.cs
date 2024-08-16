using Portfolio.Core.Types.DTOs.Requests;
using Portfolio.Core.Types.DTOs.Requests.Identity;
using Portfolio.Core.Types.DTOs.Responses;
using Portfolio.Core.Types.DTOs.Skills;

namespace Portfolio.Core.Interfaces.Skills
{
    public interface ISkillsService
    {
        #region R from CRUD
        Task<IEnumerable<SkillsDTO>> GetAllSkills();
        Task<IEnumerable<SkillsDTO>> GetSkillsByCategory(UDProjectsAndSkillsRequest request);
        Task<IEnumerable<SkillsDTO>> GetSkillsByTags(UDProjectsAndSkillsRequest request);
        Task<IEnumerable<SkillsDTO>> GetSkillsCommentedByEmail(BaseRequest request);
        Task<IEnumerable<SkillsDTO>> GetAllSkillsByTagsAsync(UDProjectsAndSkillsRequest request);
        #endregion

        #region C from CRUD
        Task<BaseResponse> InsertSkill(IUSkillRequest request);
        #endregion

        #region U from CRUD
        Task<BaseResponse> UpdateSkill(IUSkillRequest request);
        Task<BaseResponse> UpdateSkillComment(string token, IUSkillRequest request);
        #endregion

        #region D from CRUD
        Task<BaseResponse> DeleteSkill(UDProjectsAndSkillsRequest request);
        Task<BaseResponse> DeleteSkillComment(string token, IUSkillRequest request);
        #endregion
    }
}
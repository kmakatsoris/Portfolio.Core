using Portfolio.Core.Types.DTOs.Projects;
using Portfolio.Core.Types.DTOs.Requests;
using Portfolio.Core.Types.DTOs.Responses;

namespace Portfolio.Core.Interfaces.Projects
{
    public interface IProjectsService
    {
        #region R from CRUD
        Task<IEnumerable<ProjectsDTO>> GetAllProjects();
        Task<IEnumerable<ProjectsDTO>> GetProjectsByCategory(UDProjectsAndSkillsRequest request);
        Task<IEnumerable<ProjectsDTO>> GetProjectsByTags(UDProjectsAndSkillsRequest request);
        Task<IEnumerable<ProjectsDTO>> GetProjectsCommentedByEmail(BaseRequest request);
        Task<IEnumerable<ProjectsDTO>> GetAllProjectsByTagsAsync(UDProjectsAndSkillsRequest request);
        #endregion

        #region C from CRUD
        Task<BaseResponse> InsertProject(IUProjectRequest request);
        #endregion

        #region U from CRUD
        Task<BaseResponse> UpdateProject(IUProjectRequest request);
        Task<BaseResponse> UpdateProjectComment(string token, IUProjectRequest request);
        #endregion

        #region D from CRUD
        Task<BaseResponse> DeleteProject(UDProjectsAndSkillsRequest request);
        Task<BaseResponse> DeleteProjectComment(string token, IUProjectRequest request);
        #endregion
    }
}
using Portfolio.Core.Types.DTOs.Requests;
using Portfolio.Core.Types.Models.Projects;

namespace Portfolio.Core.Interfaces.Context.Projects
{
    public interface IProjectsRepository
    {
        #region Fetch Methods [R from CRUD]        
        Task<IEnumerable<ProjectModel>> GetAllProjects();
        Task<IEnumerable<ProjectModel>> GetProjectsByCategory(UDProjectsAndSkillsRequest request);
        Task<ProjectModel> GetProjectsByTitle(UDProjectsAndSkillsRequest request);
        Task<IEnumerable<ProjectModel>> GetProjectsByTags(UDProjectsAndSkillsRequest request);
        Task<IEnumerable<ProjectModel>> GetAllProjectsByTagsAsync(List<string> tags);
        #endregion

        #region Create Methods [C from CRUD]        
        Task<bool> InsertProject(ProjectModel Project);
        #endregion

        #region Update Methods [U from CRUD]        
        Task<bool> UpdateProject(ProjectModel Project);
        Task<bool> UpdateProjectComment(UDProjectsAndSkillsRequest request);
        #endregion

        #region Delete Methods [D from CRUD]        
        Task<bool> DeleteProject(UDProjectsAndSkillsRequest request);
        #endregion
    }
}
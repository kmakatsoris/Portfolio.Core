using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Core.Interfaces.Projects;
using Portfolio.Core.Types.DTOs.Requests;
using Portfolio.Core.Types.DTOs.Responses;
using Portfolio.Core.Types.DTOs.Projects;
using Portfolio.Core.Utils.UsersUtils;
using Portfolio.Core.Exceptions;

namespace Portfolio.Core.Controllers.Projects
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectsService _ProjectsService;

        public ProjectsController(IProjectsService ProjectsService)
        {
            _ProjectsService = ProjectsService;
        }

        #region R from CRUD
        [HttpPost("getAllProjects")]
        public async Task<IEnumerable<ProjectsDTO>> GetAllProjects()
        {
            return await DefaultException.ExceptionControllerHandler(async () => { return await _ProjectsService?.GetAllProjects(); });
        }

        [HttpPost("getProjectsByCategory")]
        public async Task<IEnumerable<ProjectsDTO>> GetProjectsByCategory([FromBody] UDProjectsAndSkillsRequest request)
        {
            return await DefaultException.ExceptionControllerHandler(async () => { return await _ProjectsService?.GetProjectsByCategory(request); });        
        }

        // Use GetAllProjectsByTagsAsync Instead of this.
        [HttpPost("getProjectsByTags")]
        public async Task<IEnumerable<ProjectsDTO>> GetProjectsByTags([FromBody] UDProjectsAndSkillsRequest request)
        {
            return await DefaultException.ExceptionControllerHandler(async () => { return await _ProjectsService?.GetProjectsByTags(request); });         
        }

        // Use this Instead of GetProjectsByTags.
        [HttpPost("getAllProjectsByTagsAsync")]
        public async Task<IEnumerable<ProjectsDTO>> GetAllProjectsByTagsAsync([FromBody] UDProjectsAndSkillsRequest request)
        {
            return await DefaultException.ExceptionControllerHandler(async () => { return await _ProjectsService?.GetAllProjectsByTagsAsync(request); });
        }

        [HttpPost("getProjectsCommentedByEmail")]
        public async Task<IEnumerable<ProjectsDTO>> GetProjectsCommentedByEmail([FromBody] BaseRequest request)
        {
            return await DefaultException.ExceptionControllerHandler(async () => { return await _ProjectsService?.GetProjectsCommentedByEmail(request); });      
        }
        #endregion

        #region C from CRUD
        [HttpPost("insertProject")]
        [Authorize(Policy = "Admin")]
        public async Task<BaseResponse> InsertProject([FromBody] IUProjectRequest request)
        {
            return await DefaultException.ExceptionControllerHandler(async () => { return await _ProjectsService?.InsertProject(request); });            
        }
        #endregion


        #region U from CRUD
        [HttpPost("updateProject")]
        [Authorize(Policy = "Admin")]
        public async Task<BaseResponse> UpdateProject([FromBody] IUProjectRequest request)
        {
            return await DefaultException.ExceptionControllerHandler(async () => { return await _ProjectsService?.UpdateProject(request); });            
        }

        [HttpPost("updateProjectComment")]
        [Authorize(Policy = "AdminOrUser")]
        public async Task<BaseResponse> UpdateProjectComment([FromBody] IUProjectRequest request)
        {
            return await DefaultException.ExceptionControllerHandler(async () => { return await _ProjectsService?.UpdateProjectComment(UsersUtils.ExtractToken(Request?.Headers), request); });            
        }
        #endregion

        #region D from CRUD
        [HttpPost("deleteProject")]
        [Authorize(Policy = "Admin")]
        public async Task<BaseResponse> DeleteProject([FromBody] UDProjectsAndSkillsRequest request)
        {
            return await DefaultException.ExceptionControllerHandler(async () => { return await _ProjectsService?.DeleteProject(request); });                        
        }

        [HttpPost("deleteProjectComment")]
        [Authorize(Policy = "AdminOrUser")]
        public async Task<BaseResponse> DeleteProjectComment([FromBody] IUProjectRequest request)
        {
            return await DefaultException.ExceptionControllerHandler(async () => { return await _ProjectsService?.DeleteProjectComment(UsersUtils.ExtractToken(Request?.Headers), request); });                 
        }
        #endregion
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Core.Exceptions;
using Portfolio.Core.Interfaces.Skills;
using Portfolio.Core.Types.DTOs.Requests;
using Portfolio.Core.Types.DTOs.Requests.Identity;
using Portfolio.Core.Types.DTOs.Responses;
using Portfolio.Core.Types.DTOs.Skills;
using Portfolio.Core.Utils.UsersUtils;

namespace Portfolio.Core.Controllers.Skills
{
    [Route("api/[controller]")]
    [ApiController]
    public class SkillsController : ControllerBase
    {
        private readonly ISkillsService _skillsService;

        public SkillsController(ISkillsService skillsService)
        {
            _skillsService = skillsService;
        }

        #region R from CRUD
        [HttpPost("getAllSkills")]
        public async Task<IEnumerable<SkillsDTO>> GetAllSkills()
        {
            return await DefaultException.ExceptionControllerHandler(async () => { 
                    return await _skillsService?.GetAllSkills();
                });             
        }

        [HttpPost("getSkillsByCategory")]
        public async Task<IEnumerable<SkillsDTO>> GetSkillsByCategory([FromBody] UDProjectsAndSkillsRequest request)
        {
            return await DefaultException.ExceptionControllerHandler(async () => { 
                    return await _skillsService?.GetSkillsByCategory(request);
                });                
        }

        // Use getAllSkillsByTagsAsync Instead of this.
        [HttpPost("getSkillsByTags")]
        public async Task<IEnumerable<SkillsDTO>> GetSkillsByTags([FromBody] UDProjectsAndSkillsRequest request)
        {
            return await DefaultException.ExceptionControllerHandler(async () => { 
                    return await _skillsService?.GetSkillsByTags(request);
                });              
        }

        // Use this Instead of getSkillsByTags.
        [HttpPost("getAllSkillsByTagsAsync")]
        public async Task<IEnumerable<SkillsDTO>> GetAllSkillsByTagsAsync([FromBody] UDProjectsAndSkillsRequest request)
        {
            return await DefaultException.ExceptionControllerHandler(async () => { 
                    return await _skillsService?.GetAllSkillsByTagsAsync(request);
                });             
        }

        [HttpPost("getSkillsCommentedByEmail")]
        public async Task<IEnumerable<SkillsDTO>> GetSkillsCommentedByEmail([FromBody] BaseRequest request)
        {
            return await DefaultException.ExceptionControllerHandler(async () => { 
                    return await _skillsService?.GetSkillsCommentedByEmail(request);
                });            
        }
        #endregion

        #region C from CRUD
        [HttpPost("insertSkill")]
        [Authorize(Policy = "Admin")]
        public async Task<BaseResponse> InsertSkill([FromBody] IUSkillRequest request)
        {
            return await DefaultException.ExceptionControllerHandler(async () => { 
                    return await _skillsService?.InsertSkill(request);
                });             
        }
        #endregion


        #region U from CRUD
        [HttpPost("updateSkill")]
        [Authorize(Policy = "Admin")]
        public async Task<BaseResponse> UpdateSkill([FromBody] IUSkillRequest request)
        {
            return await DefaultException.ExceptionControllerHandler(async () => { 
                    return await _skillsService?.UpdateSkill(request);
                });             
        }

        [HttpPost("updateSkillComment")]
        [Authorize(Policy = "AdminOrUser")]
        public async Task<BaseResponse> UpdateSkillComment([FromBody] IUSkillRequest request)
        {
            return await DefaultException.ExceptionControllerHandler(async () => { 
                    return await _skillsService?.UpdateSkillComment(UsersUtils.ExtractToken(Request?.Headers), request);
                });            
        }
        #endregion

        #region D from CRUD
        [HttpPost("deleteSkill")]
        [Authorize(Policy = "Admin")]
        public async Task<BaseResponse> DeleteSkill([FromBody] UDProjectsAndSkillsRequest request)
        {
            return await DefaultException.ExceptionControllerHandler(async () => { 
                    return await _skillsService?.DeleteSkill(request);
                });              
        }

        [HttpPost("deleteSkillComment")]
        [Authorize(Policy = "AdminOrUser")]
        public async Task<BaseResponse> DeleteSkillComment([FromBody] IUSkillRequest request)
        {
            return await DefaultException.ExceptionControllerHandler(async () => { 
                    return await _skillsService?.DeleteSkillComment(UsersUtils.ExtractToken(Request?.Headers), request);
                });                 
        }
        #endregion
    }
}
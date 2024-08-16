using Newtonsoft.Json;
using Portfolio.Core.Interfaces.Context.Projects;
using Portfolio.Core.Interfaces.Identity;
using Portfolio.Core.Interfaces.Projects;
using Portfolio.Core.Types.DataTypes.Projects;
using Portfolio.Core.Types.DTOs.Requests;
using Portfolio.Core.Types.DTOs.Responses;
using Portfolio.Core.Types.DTOs.Projects;
using Portfolio.Core.Types.Models.Projects;
using Portfolio.Core.Utils.Mapping;
using Portfolio.Core.Utils.ProjectsUtils;
using ILogger = NLog.ILogger;
using Portfolio.Core.Types.Contracts;

namespace Portfolio.Core.Implementation.Projects
{
    public partial class ProjectsService : IProjectsService
    {
        private readonly ILogger _logger;
        private readonly IProjectsRepository _ProjectsRepository;
        private readonly IOAuthAuthorizationService _oauthService;

        public ProjectsService
        (
            ILogger logger,
            IProjectsRepository ProjectsRepository,
            IOAuthAuthorizationService oauthService
        )
        {
            _logger = logger;
            _ProjectsRepository = ProjectsRepository;
            _oauthService = oauthService;
        }

        #region R from CRUD
        public async Task<IEnumerable<ProjectsDTO>> GetAllProjects()
        {
            return (IEnumerable<ProjectsDTO>)(ProjectsAndSkillsMapping.ToMap<ProjectsAllGrades, ProjectsComment>(await _ProjectsRepository?.GetAllProjects()) ?? new List<ProjectsDTO>());
        }

        public async Task<IEnumerable<ProjectsDTO>> GetProjectsByCategory(UDProjectsAndSkillsRequest request)
        {
            if (string.IsNullOrEmpty(request?.Email)) return new List<ProjectsDTO>();
            return (IEnumerable<ProjectsDTO>)(ProjectsAndSkillsMapping.ToMap<ProjectsAllGrades, ProjectsComment>(await _ProjectsRepository?.GetProjectsByCategory(request)) ?? new List<ProjectsDTO>());
        }

        public async Task<IEnumerable<ProjectsDTO>> GetProjectsByTags(UDProjectsAndSkillsRequest request)
        {
            if (string.IsNullOrEmpty(request?.Email)) return new List<ProjectsDTO>();
            return (IEnumerable<ProjectsDTO>)(ProjectsAndSkillsMapping.ToMap<ProjectsAllGrades, ProjectsComment>(await _ProjectsRepository?.GetProjectsByTags(request)) ?? new List<ProjectsDTO>());
        }

        public async Task<IEnumerable<ProjectsDTO>> GetAllProjectsByTagsAsync(UDProjectsAndSkillsRequest request)
        {
            if (string.IsNullOrEmpty(request?.Email)) return new List<ProjectsDTO>();
            return (IEnumerable<ProjectsDTO>)(ProjectsAndSkillsMapping.ToMap<ProjectsAllGrades, ProjectsComment>(await _ProjectsRepository?.GetAllProjectsByTagsAsync(request?.TagNames?.ToList())) ?? new List<ProjectsDTO>());
        }

        public async Task<IEnumerable<ProjectsDTO>> GetProjectsCommentedByEmail(BaseRequest request)
        {
            IEnumerable<ProjectsDTO> allProjects = (IEnumerable<ProjectsDTO>)ProjectsAndSkillsMapping.ToMap<ProjectsAllGrades, ProjectsComment>(await _ProjectsRepository?.GetAllProjects());
            return allProjects?.Where(s => s?.CommentsDTO != null && s.CommentsDTO.Any(c => c?.Review != null && c?.Review?.Email?.ToLower()?.Equals(request?.Email?.ToLower()) == true)).ToList();
        }
        #endregion

        #region C from CRUD
        public async Task<BaseResponse> InsertProject(IUProjectRequest request)
        {
            // @TOCARE: If let other than me to insert Projects need to look the emails to add and ....more Validations            
            return new BaseResponse { Success = await _ProjectsRepository?.InsertProject((ProjectModel)ProjectsAndSkillsMapping.ToMap<ProjectsAllGrades, ProjectsComment>(request?.Projects)) };
        }
        #endregion


        #region U from CRUD
        public async Task<BaseResponse> UpdateProject(IUProjectRequest request)
        {
            // @TOCARE: If let other than me to update Projects need to look the emails to add and ....more Validations
            return new BaseResponse { Success = await _ProjectsRepository?.UpdateProject((ProjectModel)ProjectsAndSkillsMapping.ToMap<ProjectsAllGrades, ProjectsComment>(request?.Projects)) };
        }

        public async Task<BaseResponse> UpdateProjectComment(string token, IUProjectRequest request)
        {
            if (request?.Projects?.CommentsDTO?.Count() != 1)
                throw new Exception("There are more than one comments in the request.");

            if (_oauthService?.GetEmailFromToken(token)?.ToLower()?.Equals(request?.Email?.ToLower()) != true || ProjectsAndSkillsUtils.ExtractAllEmailsFromProject_OR_Skills<ProjectsComment, ProjectsAllGrades, ProjectsReviewsMetaData>(request?.Projects)?.Any(e => e?.ToLower()?.Equals(request?.Email?.ToLower()) != true) == true)
                throw new Exception("Sorry, but you do not have permission to UPDATE other people's Project.");

            ProjectsDTO project = (ProjectsDTO)ProjectsAndSkillsMapping.ToMap<ProjectsAllGrades, ProjectsComment>(
                (IEnumerable<Interfaces.Contract.IBasicProjectsAndSkillsPropertiesContract>)await _ProjectsRepository?.GetProjectsByTitle(new UDProjectsAndSkillsRequest { Title = request?.Projects?.Title })
                );
            FetchUserCommentsBySkillOrProject<ProjectsComment> userCommentsByProject = await ProjectsAndSkillsUtils.FetchUserCommentsByProject_OR_Skills<ProjectsDTO, ProjectsAllGrades, ProjectsComment>(project, request?.Email, request?.Projects?.CommentsDTO);

            bool success = false;
            ProjectsComment comment = request?.Projects?.CommentsDTO?.FirstOrDefault();
            if (userCommentsByProject?.IsExist == true)
            {
                userCommentsByProject.CommentsToStore.Add(comment);
                success = await _ProjectsRepository?.UpdateProjectComment(new UDProjectsAndSkillsRequest { Title = request?.Projects?.Title, Comments = JsonConvert.SerializeObject(userCommentsByProject.CommentsToStore) });
            }
            else
            {
                success = await _ProjectsRepository?.UpdateProjectComment(new UDProjectsAndSkillsRequest { Title = request?.Projects?.Title, Comments = JsonConvert.SerializeObject(new List<ProjectsComment> { comment }) });
            }

            return new BaseResponse { Success = success };
        }
        #endregion

        #region D from CRUD
        public async Task<BaseResponse> DeleteProject(UDProjectsAndSkillsRequest request)
        {
            return new BaseResponse { Success = await _ProjectsRepository?.DeleteProject(request) };
        }

        public async Task<BaseResponse> DeleteProjectComment(string token, IUProjectRequest request)
        {
            if (request?.Projects?.CommentsDTO?.Count() != 1)
                throw new Exception("There are more than one comments in the request.");

            if (_oauthService?.GetEmailFromToken(token)?.ToLower()?.Equals(request?.Email?.ToLower()) != true || ProjectsAndSkillsUtils.ExtractAllEmailsFromProject_OR_Skills<ProjectsComment, ProjectsAllGrades, ProjectsReviewsMetaData>(request?.Projects)?.Any(e => e?.ToLower()?.Equals(request?.Email?.ToLower()) != true) == true)
                throw new Exception("Sorry, but you do not have permission to UPDATE other people's Project.");

            ProjectsDTO project = (ProjectsDTO)ProjectsAndSkillsMapping.ToMap<ProjectsAllGrades, ProjectsComment>(
                (IEnumerable<Interfaces.Contract.IBasicProjectsAndSkillsPropertiesContract>)await _ProjectsRepository?.GetProjectsByTitle(new UDProjectsAndSkillsRequest { Title = request?.Projects?.Title })
                );
            FetchUserCommentsBySkillOrProject<ProjectsComment> userCommentsByProject = await ProjectsAndSkillsUtils.FetchUserCommentsByProject_OR_Skills<ProjectsDTO, ProjectsAllGrades, ProjectsComment>(project, request?.Email, request?.Projects?.CommentsDTO);

            bool success = true;
            if (userCommentsByProject?.IsExist == true)
            {
                success = await _ProjectsRepository?.UpdateProjectComment(new UDProjectsAndSkillsRequest { Title = request?.Projects?.Title, Comments = JsonConvert.SerializeObject(userCommentsByProject.CommentsToStore) });
            }

            return new BaseResponse { Success = success };
        }
        #endregion

    }
}
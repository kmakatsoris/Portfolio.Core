using Newtonsoft.Json;
using Portfolio.Core.Interfaces.Context.Skills;
using Portfolio.Core.Interfaces.Identity;
using Portfolio.Core.Interfaces.Skills;
using Portfolio.Core.Types.Contracts;
using Portfolio.Core.Types.DataTypes.Skills;
using Portfolio.Core.Types.DTOs.Requests;
using Portfolio.Core.Types.DTOs.Responses;
using Portfolio.Core.Types.DTOs.Skills;
using Portfolio.Core.Types.Models.Skills;
using Portfolio.Core.Utils.Mapping;
using Portfolio.Core.Utils.ProjectsUtils;
using ILogger = NLog.ILogger;

namespace Portfolio.Core.Implementation.Skills
{
    public partial class SkillsService : ISkillsService
    {
        private readonly ILogger _logger;
        private readonly ISkillsRepository _skillsRepository;
        private readonly IOAuthAuthorizationService _oauthService;

        public SkillsService
        (
            ILogger logger,
            ISkillsRepository skillsRepository,
            IOAuthAuthorizationService oauthService
        )
        {
            _logger = logger;
            _skillsRepository = skillsRepository;
            _oauthService = oauthService;
        }

        #region R from CRUD
        public async Task<IEnumerable<SkillsDTO>> GetAllSkills()
        {
            return (IEnumerable<SkillsDTO>)(ProjectsAndSkillsMapping.ToMap<SkillsAllGrades, SkillsComment>(await _skillsRepository?.GetAllSkills()) ?? new List<SkillsDTO>());
        }

        public async Task<IEnumerable<SkillsDTO>> GetSkillsByCategory(UDProjectsAndSkillsRequest request)
        {
            if (string.IsNullOrEmpty(request?.Email)) return new List<SkillsDTO>();
            return (IEnumerable<SkillsDTO>)(ProjectsAndSkillsMapping.ToMap<SkillsAllGrades, SkillsComment>(await _skillsRepository?.GetSkillsByCategory(request)) ?? new List<SkillsDTO>());
        }

        public async Task<IEnumerable<SkillsDTO>> GetSkillsByTags(UDProjectsAndSkillsRequest request)
        {
            if (string.IsNullOrEmpty(request?.Email)) return new List<SkillsDTO>();
            return (IEnumerable<SkillsDTO>)(ProjectsAndSkillsMapping.ToMap<SkillsAllGrades, SkillsComment>(await _skillsRepository?.GetSkillsByTags(request)) ?? new List<SkillsDTO>());
        }

        public async Task<IEnumerable<SkillsDTO>> GetAllSkillsByTagsAsync(UDProjectsAndSkillsRequest request)
        {
            if (string.IsNullOrEmpty(request?.Email)) return new List<SkillsDTO>();
            return (IEnumerable<SkillsDTO>)(ProjectsAndSkillsMapping.ToMap<SkillsAllGrades, SkillsComment>(await _skillsRepository?.GetAllSkillsByTagsAsync(request?.TagNames?.ToList())) ?? new List<SkillsDTO>());
        }

        public async Task<IEnumerable<SkillsDTO>> GetSkillsCommentedByEmail(BaseRequest request)
        {
            IEnumerable<SkillsDTO> allSkills = (IEnumerable<SkillsDTO>)ProjectsAndSkillsMapping.ToMap<SkillsAllGrades, SkillsComment>(await _skillsRepository?.GetAllSkills());
            return allSkills?.Where(s => s?.CommentsDTO != null && s.CommentsDTO.Any(c => c?.Review != null && c?.Review?.Email?.ToLower()?.Equals(request?.Email?.ToLower()) == true)).ToList();
        }
        #endregion

        #region C from CRUD
        public async Task<BaseResponse> InsertSkill(IUSkillRequest request)
        {
            // @TOCARE: If let other than me to insert skills need to look the emails to add and ....more Validations                        
            return new BaseResponse { Success = await _skillsRepository?.InsertSkill((SkillsModel)ProjectsAndSkillsMapping.ToMap<SkillsAllGrades, SkillsComment>(request?.Skills)) };
        }
        #endregion


        #region U from CRUD
        public async Task<BaseResponse> UpdateSkill(IUSkillRequest request)
        {
            // @TOCARE: If let other than me to update skills need to look the emails to add and ....more Validations
            return new BaseResponse { Success = await _skillsRepository?.UpdateSkill((SkillsModel)ProjectsAndSkillsMapping.ToMap<SkillsAllGrades, SkillsComment>(request?.Skills)) };
        }

        public async Task<BaseResponse> UpdateSkillComment(string token, IUSkillRequest request)
        {
            if (request?.Skills?.CommentsDTO?.Count() != 1)
                throw new Exception("There are more than one comments in the request.");

            if (_oauthService?.GetEmailFromToken(token)?.ToLower()?.Equals(request?.Email?.ToLower()) != true || ProjectsAndSkillsUtils.ExtractAllEmailsFromProject_OR_Skills<SkillsComment, SkillsAllGrades, SkillsReviewsMetaData>(request?.Skills)?.Any(e => e?.ToLower()?.Equals(request?.Email?.ToLower()) != true) == true)
                throw new Exception("Sorry, but you do not have permission to UPDATE other people's skill.");

            SkillsDTO skill = (SkillsDTO)ProjectsAndSkillsMapping.ToMap<SkillsAllGrades, SkillsComment>(
                (IEnumerable<Interfaces.Contract.IBasicProjectsAndSkillsPropertiesContract>)await _skillsRepository?.GetSkillsByTitle(new UDProjectsAndSkillsRequest { Title = request?.Skills?.Title })
                );
            FetchUserCommentsBySkillOrProject<SkillsComment> userCommentsBySkill = await ProjectsAndSkillsUtils.FetchUserCommentsByProject_OR_Skills<SkillsDTO, SkillsAllGrades, SkillsComment>(skill, request?.Email, request?.Skills?.CommentsDTO);

            bool success = false;
            SkillsComment comment = request?.Skills?.CommentsDTO?.FirstOrDefault();
            if (userCommentsBySkill?.IsExist == true)
            {
                userCommentsBySkill.CommentsToStore.Add(comment);
                success = await _skillsRepository?.UpdateSkillComment(new UDProjectsAndSkillsRequest { Title = request?.Skills?.Title, Comments = JsonConvert.SerializeObject(userCommentsBySkill.CommentsToStore) });
            }
            else
            {
                success = await _skillsRepository?.UpdateSkillComment(new UDProjectsAndSkillsRequest { Title = request?.Skills?.Title, Comments = JsonConvert.SerializeObject(new List<SkillsComment> { comment }) });
            }

            return new BaseResponse { Success = success };
        }
        #endregion

        #region D from CRUD
        public async Task<BaseResponse> DeleteSkill(UDProjectsAndSkillsRequest request)
        {
            return new BaseResponse { Success = await _skillsRepository?.DeleteSkill(request) };
        }

        public async Task<BaseResponse> DeleteSkillComment(string token, IUSkillRequest request)
        {
            if (request?.Skills?.CommentsDTO?.Count() != 1)
                throw new Exception("There are more than one comments in the request.");

            if (_oauthService?.GetEmailFromToken(token)?.ToLower()?.Equals(request?.Email?.ToLower()) != true || ProjectsAndSkillsUtils.ExtractAllEmailsFromProject_OR_Skills<SkillsComment, SkillsAllGrades, SkillsReviewsMetaData>(request?.Skills)?.Any(e => e?.ToLower()?.Equals(request?.Email?.ToLower()) != true) == true)
                throw new Exception("Sorry, but you do not have permission to UPDATE other people's skill.");

            SkillsDTO skill = (SkillsDTO)ProjectsAndSkillsMapping.ToMap<SkillsAllGrades, SkillsComment>(
                (IEnumerable<Interfaces.Contract.IBasicProjectsAndSkillsPropertiesContract>)await _skillsRepository?.GetSkillsByTitle(new UDProjectsAndSkillsRequest { Title = request?.Skills?.Title })
                );
            FetchUserCommentsBySkillOrProject<SkillsComment> userCommentsBySkill = await ProjectsAndSkillsUtils.FetchUserCommentsByProject_OR_Skills<SkillsDTO, SkillsAllGrades, SkillsComment>(skill, request?.Email, request?.Skills?.CommentsDTO);

            bool success = true;
            if (userCommentsBySkill?.IsExist == true)
            {
                success = await _skillsRepository?.UpdateSkillComment(new UDProjectsAndSkillsRequest { Title = request?.Skills?.Title, Comments = JsonConvert.SerializeObject(userCommentsBySkill.CommentsToStore) });
            }

            return new BaseResponse { Success = success };
        }
        #endregion
    }
}
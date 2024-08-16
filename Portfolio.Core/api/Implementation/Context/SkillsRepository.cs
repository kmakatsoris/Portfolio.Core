using System.Text.RegularExpressions;
using Dapper;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using Portfolio.Core.Interfaces.Context.Skills;
using Portfolio.Core.Types.Context;
using Portfolio.Core.Types.DTOs.Requests;
using Portfolio.Core.Types.Models.Skills;
using Portfolio.Core.Utils.DefaultUtils;
using ILogger = NLog.ILogger;

namespace Portfolio.Core.Services.Context
{
    public class SkillsRepository : ISkillsRepository
    {
        private readonly IOptions<AppSettings> _appSettings;
        private readonly ILogger _logger;

        public SkillsRepository
        (
            IOptions<AppSettings> appSettings,
            ILogger logger
        )
        {
            _appSettings = appSettings;
            _logger = logger;
        }

        #region Fetch Methods [R from CRUD]
        // Fetch All Skills,
        public async Task<IEnumerable<SkillsModel>> GetAllSkills()
        {
            var sql = "SELECT * FROM Skills";
            try
            {
                using (var connection = new MySqlConnection(_appSettings?.Value?.ConnectionStrings?.IdentityConnection ?? ""))
                {
                    await connection.OpenAsync();
                    var result = await connection.QueryAsync<SkillsModel>(sql);
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error getting skills with Exception:\n\n{ex?.Message}. @GetAllSkills");
                throw new Exception(ex?.Message);
            }
        }

        // Fetch Specific Skill Based on Given Category
        public async Task<IEnumerable<SkillsModel>> GetSkillsByCategory(UDProjectsAndSkillsRequest request)
        {
            var sql = "SELECT * FROM Skills WHERE Category = @Category";
            try
            {
                using (var connection = new MySqlConnection(_appSettings?.Value?.ConnectionStrings?.IdentityConnection ?? ""))
                {
                    await connection.OpenAsync();
                    var result = await connection.QueryAsync<SkillsModel>(sql, new { Category = request?.Category });
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error getting skills by Category with Exception:\n\n{ex?.Message}. @GetSkillsByCategory");
                throw new Exception(ex?.Message);
            }
        }

        public async Task<SkillsModel> GetSkillsByTitle(UDProjectsAndSkillsRequest request)
        {
            var sql = "SELECT * FROM Skills WHERE Title = @Title";
            try
            {
                using (var connection = new MySqlConnection(_appSettings?.Value?.ConnectionStrings?.IdentityConnection ?? ""))
                {
                    await connection.OpenAsync();
                    var result = await connection.QueryAsync<SkillsModel>(sql, new { Title = request?.Title });
                    return SelectSingleSkillFromList(result);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error getting skills by Title with Exception:\n\n{ex?.Message}. @GetSkillsByTitle");
                throw new Exception(ex?.Message);
            }
        }

        // Fetch Specific Skill Based on Given Tags Array
        public async Task<IEnumerable<SkillsModel>> GetSkillsByTags(UDProjectsAndSkillsRequest request)
        {
            if (request == null) return new List<SkillsModel>();
            request.TagNames = ToPerformTagsRequest(request?.TagNames);
            if (request?.TagNames?.Count() <= 0) return new List<SkillsModel>();
            // SQL query to match against a comma-separated list of tags in the "Tags" column            
            var conditions = string.Join(" OR ", request.TagNames.Select(tag => $"FIND_IN_SET('{tag.Replace("'", "\\'")}', Tags) > 0"));
            var sql = $@"SELECT * FROM Skills WHERE {conditions};"; // @TODO: Change to have 'WHERE Tags ...(sth)...

            try
            {
                using (var connection = new MySqlConnection(_appSettings?.Value?.ConnectionStrings?.IdentityConnection ?? ""))
                {
                    await connection.OpenAsync();
                    var result = await connection.QueryAsync<SkillsModel>(sql, new { Tags = request?.TagNames });
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error getting skills by Tags with Exception:\n\n" + ex.Message);
                throw new Exception(ex?.Message);
            }
        }

        public async Task<IEnumerable<SkillsModel>> GetAllSkillsByTagsAsync(List<string> tags)
        {
            string conds = tags?.GenerateTagsClauseCondition();
            if (string.IsNullOrEmpty(conds)) return new List<SkillsModel>();
            var sql = "select * from Skills where match(Tags) against(" + conds + " IN BOOLEAN MODE);";
            try
            {
                using (var connection = new MySqlConnection(_appSettings?.Value?.ConnectionStrings?.IdentityConnection ?? ""))
                {
                    await connection.OpenAsync();
                    var result = await connection.QueryAsync<SkillsModel>(sql);
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error getting Skills with Exception:\n\n{ex?.Message}. @GetAllSkillsByTagsAsync");
                throw new Exception(ex?.Message);
            }
        }
        #endregion

        #region Create Methods [C from CRUD]
        // Add Skill,
        public async Task<bool> InsertSkill(SkillsModel skill)
        {
            if (skill == null) return false;
            var sql = @"
            INSERT INTO Skills
            (Title, BriefDescription, ExtensiveDescription, ResourceIntroId, Category, ResourceMainId, Tags, Grades, Comments)
            VALUES
            (@Title, @BriefDescription, @ExtensiveDescription, @ResourceIntroId, @Category, @ResourceMainId, @Tags, @Grades, @Comments);
            ";

            try
            {
                using (var connection = new MySqlConnection(_appSettings?.Value?.ConnectionStrings?.IdentityConnection ?? ""))
                {
                    await connection.OpenAsync();
                    var parameters = new
                    {
                        skill?.Title,
                        skill?.BriefDescription,
                        ExtensiveDescription = skill?.ExtensiveDescription,
                        skill?.ResourceIntroId,
                        skill?.Category,
                        skill?.ResourceMainId,
                        Tags = skill?.Tags,
                        Grades = skill?.Grades,
                        Comments = skill?.Comments
                    };
                    var result = await connection.ExecuteAsync(sql, parameters);
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error adding new skill. @SkillsDTO");
                throw new Exception(ex?.Message);
            }
        }

        #endregion

        #region Update Methods [U from CRUD]
        // Update Review
        public async Task<bool> UpdateSkill(SkillsModel skill)
        {
            if (skill == null) return false;
            var sql = @"
            UPDATE Skills
            SET
                Title = @Title,
                BriefDescription = @BriefDescription,
                ExtensiveDescription = @ExtensiveDescription,
                ResourceIntroId = @ResourceIntroId,
                Category = @Category,
                ResourceMainId = @ResourceMainId,
                Tags = @Tags,
                Grades = @Grades,
                Comments = @Comments
            WHERE Title = @Title;
            ";

            try
            {
                using (var connection = new MySqlConnection(_appSettings?.Value?.ConnectionStrings?.IdentityConnection ?? ""))
                {
                    await connection.OpenAsync();
                    var result = await connection.ExecuteAsync(sql, new
                    {
                        skill.Title,
                        skill.BriefDescription,
                        skill?.ExtensiveDescription,
                        skill.ResourceIntroId,
                        skill.Category,
                        skill.ResourceMainId,
                        skill?.Tags,
                        skill?.Grades,
                        skill?.Comments
                    });
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error updating skill. @UpdateSkill");
                throw new Exception(ex?.Message);
            }
        }

        public async Task<bool> UpdateSkillComment(UDProjectsAndSkillsRequest request)
        {
            if (request == null) return false;
            var sql = "UPDATE Skills SET Comments = @Comments WHERE Title = @Title";

            try
            {
                using (var connection = new MySqlConnection(_appSettings?.Value?.ConnectionStrings?.IdentityConnection ?? ""))
                {
                    await connection.OpenAsync();
                    var result = await connection.ExecuteAsync(sql, new
                    {
                        request?.Title,
                        request?.Comments
                    });
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error updating skill comments. @UpdateSkillComment");
                throw new Exception(ex?.Message);
            }
        }
        #endregion

        #region Delete Methods [D from CRUD]
        // Delete Skill
        public async Task<bool> DeleteSkill(UDProjectsAndSkillsRequest request)
        {
            var sql = "DELETE FROM Skills WHERE Title = @Title";
            try
            {
                using (var connection = new MySqlConnection(_appSettings?.Value?.ConnectionStrings?.IdentityConnection ?? ""))
                {
                    await connection.OpenAsync();
                    var result = await connection.ExecuteAsync(sql, new { Title = request?.Title });
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error deleting skill. @DeleteSkill");
                throw new Exception(ex?.Message);
            }
        }

        #endregion

        #region Private Methods,
        private string[] ToPerformTagsRequest(IEnumerable<string> tags)
        {
            if (tags == null) return [];
            string pattern = @"^#[A-Za-z0-9._]+$";
            return tags.Where(tag => tag != null && Regex.IsMatch(tag, pattern)).ToArray();
        }

        private SkillsModel SelectSingleSkillFromList(IEnumerable<SkillsModel> skills)
        {
            if (skills == null || skills?.Count() <= 0)
                return new SkillsModel();
            else if (skills?.Count() > 1)
                throw new Exception("More than one review with same email");
            else
                return skills.FirstOrDefault();
        }
        #endregion
    }
}
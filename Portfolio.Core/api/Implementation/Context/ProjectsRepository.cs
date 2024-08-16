using System.Text.RegularExpressions;
using Dapper;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using Portfolio.Core.Interfaces.Context.Projects;
using Portfolio.Core.Types.Context;
using Portfolio.Core.Types.DTOs.Requests;
using Portfolio.Core.Types.Models.Projects;
using Portfolio.Core.Utils.DefaultUtils;
using ILogger = NLog.ILogger;

namespace Portfolio.Core.Services.Context
{
    public class ProjectsRepository : IProjectsRepository
    {
        private readonly IOptions<AppSettings> _appSettings;
        private readonly ILogger _logger;

        public ProjectsRepository
        (
            IOptions<AppSettings> appSettings,
            ILogger logger
        )
        {
            _appSettings = appSettings;
            _logger = logger;
        }

        #region Fetch Methods [R from CRUD]
        // Fetch All Projects,
        public async Task<IEnumerable<ProjectModel>> GetAllProjects()
        {
            var sql = "SELECT * FROM Projects";
            try
            {
                using (var connection = new MySqlConnection(_appSettings?.Value?.ConnectionStrings?.IdentityConnection ?? ""))
                {
                    await connection.OpenAsync();
                    var result = await connection.QueryAsync<ProjectModel>(sql);
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error getting Projects with Exception:\n\n{ex?.Message}. @GetAllProjects");
                throw new Exception(ex?.Message);
            }
        }

        // Fetch Specific Project Based on Given Category
        public async Task<IEnumerable<ProjectModel>> GetProjectsByCategory(UDProjectsAndSkillsRequest request)
        {
            var sql = "SELECT * FROM Projects WHERE Category = @Category";
            try
            {
                using (var connection = new MySqlConnection(_appSettings?.Value?.ConnectionStrings?.IdentityConnection ?? ""))
                {
                    await connection.OpenAsync();
                    var result = await connection.QueryAsync<ProjectModel>(sql, new { Category = request?.Category });
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error getting Projects by Category with Exception:\n\n{ex?.Message}. @GetProjectsByCategory");
                throw new Exception(ex?.Message);
            }
        }

        public async Task<ProjectModel> GetProjectsByTitle(UDProjectsAndSkillsRequest request)
        {
            var sql = "SELECT * FROM Projects WHERE Title = @Title";
            try
            {
                using (var connection = new MySqlConnection(_appSettings?.Value?.ConnectionStrings?.IdentityConnection ?? ""))
                {
                    await connection.OpenAsync();
                    var result = await connection.QueryAsync<ProjectModel>(sql, new { Title = request?.Title });
                    return SelectSingleProjectFromList(result);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error getting Projects by Title with Exception:\n\n{ex?.Message}. @GetProjectsByTitle");
                throw new Exception(ex?.Message);
            }
        }

        // Fetch Specific Project Based on Given Tags Array
        public async Task<IEnumerable<ProjectModel>> GetProjectsByTags(UDProjectsAndSkillsRequest request)
        {
            if (request == null) return new List<ProjectModel>();
            request.TagNames = ToPerformTagsRequest(request?.TagNames);
            if (request?.TagNames?.Count() <= 0) return new List<ProjectModel>();
            // SQL query to match against a comma-separated list of tags in the "Tags" column            
            var conditions = string.Join(" OR ", request.TagNames.Select(tag => $"FIND_IN_SET('{tag.Replace("'", "\\'")}', Tags) > 0"));
            var sql = $@"SELECT * FROM Projects WHERE {conditions};"; // @TODO: Change to have 'WHERE Tags ...(sth)...

            try
            {
                using (var connection = new MySqlConnection(_appSettings?.Value?.ConnectionStrings?.IdentityConnection ?? ""))
                {
                    await connection.OpenAsync();
                    var result = await connection.QueryAsync<ProjectModel>(sql, new { Tags = request?.TagNames });
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error getting Projects by Tags with Exception:\n\n" + ex.Message);
                throw new Exception(ex?.Message);
            }
        }

        public async Task<IEnumerable<ProjectModel>> GetAllProjectsByTagsAsync(List<string> tags)
        {
            string conds = tags?.GenerateTagsClauseCondition();
            if (string.IsNullOrEmpty(conds)) return new List<ProjectModel>();
            var sql = "select * from Projects where match(Tags) against(" + conds + " IN BOOLEAN MODE);";
            try
            {
                using (var connection = new MySqlConnection(_appSettings?.Value?.ConnectionStrings?.IdentityConnection ?? ""))
                {
                    await connection.OpenAsync();
                    var result = await connection.QueryAsync<ProjectModel>(sql);
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error getting Projects with Exception:\n\n{ex?.Message}. @GetAllProjectsByTagsAsync");
                throw new Exception(ex?.Message);
            }
        }
        #endregion

        #region Create Methods [C from CRUD]
        // Add Project,
        public async Task<bool> InsertProject(ProjectModel Project)
        {
            if (Project == null) return false;
            var sql = @"
            INSERT INTO Projects
            (Title, BriefDescription, ExtensiveDescription, ResourceIntroId, Category, ResourceMainId, Tags, Grades, Comments, MemberLabels)
            VALUES
            (@Title, @BriefDescription, @ExtensiveDescription, @ResourceIntroId, @Category, @ResourceMainId, @Tags, @Grades, @Comments, @MemberLabels);
            ";

            try
            {
                using (var connection = new MySqlConnection(_appSettings?.Value?.ConnectionStrings?.IdentityConnection ?? ""))
                {
                    await connection.OpenAsync();
                    var parameters = new
                    {
                        Project?.Title,
                        Project?.BriefDescription,
                        ExtensiveDescription = Project?.ExtensiveDescription,
                        Project?.ResourceIntroId,
                        Project?.Category,
                        Project?.ResourceMainId,
                        Tags = Project?.Tags,
                        Grades = Project?.Grades,
                        Comments = Project?.Comments,
                        Project?.MemberLabels
                    };
                    var result = await connection.ExecuteAsync(sql, parameters);
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error adding new Project.");
                throw new Exception(ex?.Message);
            }
        }

        #endregion

        #region Update Methods [U from CRUD]
        // Update Review
        public async Task<bool> UpdateProject(ProjectModel Project)
        {
            if (Project == null) return false;
            var sql = @"
            UPDATE Projects
            SET
                Title = @Title,
                BriefDescription = @BriefDescription,
                ExtensiveDescription = @ExtensiveDescription,
                ResourceIntroId = @ResourceIntroId,
                Category = @Category,
                ResourceMainId = @ResourceMainId,
                Tags = @Tags,
                Grades = @Grades,
                Comments = @Comments,
                MemberLabels = @MemberLabels
            WHERE Title = @Title;
            ";

            try
            {
                using (var connection = new MySqlConnection(_appSettings?.Value?.ConnectionStrings?.IdentityConnection ?? ""))
                {
                    await connection.OpenAsync();
                    var result = await connection.ExecuteAsync(sql, new
                    {
                        Project.Title,
                        Project.BriefDescription,
                        Project?.ExtensiveDescription,
                        Project.ResourceIntroId,
                        Project.Category,
                        Project.ResourceMainId,
                        Project?.Tags,
                        Project?.Grades,
                        Project?.Comments,
                        Project?.MemberLabels
                    });
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error updating Project. @UpdateProject");
                throw new Exception(ex?.Message);
            }
        }

        public async Task<bool> UpdateProjectComment(UDProjectsAndSkillsRequest request)
        {
            if (request == null) return false;
            var sql = "UPDATE Projects SET Comments = @Comments WHERE Title = @Title";

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
                _logger.Error(ex, "Error updating Project comments. @UpdateProjectComment");
                throw new Exception(ex?.Message);
            }
        }
        #endregion

        #region Delete Methods [D from CRUD]
        // Delete Project
        public async Task<bool> DeleteProject(UDProjectsAndSkillsRequest request)
        {
            var sql = "DELETE FROM Projects WHERE Title = @Title";
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
                _logger.Error(ex, "Error deleting Project. @DeleteProject");
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

        private ProjectModel SelectSingleProjectFromList(IEnumerable<ProjectModel> Projects)
        {
            if (Projects == null || Projects?.Count() <= 0)
                return new ProjectModel();
            else if (Projects?.Count() > 1)
                throw new Exception("More than one review with same email");
            else
                return Projects.FirstOrDefault();
        }
        #endregion
    }
}
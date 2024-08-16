using System.Text.RegularExpressions;
using Dapper;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using Portfolio.Core.Interfaces.Context.Resources;
using Portfolio.Core.Types.Context;
using Portfolio.Core.Types.Models.Resources;
using Portfolio.Core.Utils;
using Portfolio.Core.Utils.DefaultUtils;
using ILogger = NLog.ILogger;

namespace Portfolio.Core.Services.Context
{
    public class ResourcesRepository : IResourceRepository
    {
        private readonly IOptions<AppSettings> _appSettings;
        private readonly ILogger _logger;

        public ResourcesRepository
        (
            IOptions<AppSettings> appSettings,
            ILogger logger
        )
        {
            _appSettings = appSettings;
            _logger = logger;
        }

        #region Fetch Methods [R from CRUD]
        // Fetch All Resources By Render Path,
        public async Task<IEnumerable<Resource>> GetAllResourcesByRenderPathAsync(string renderPath)
        {
            var sql = "SELECT * FROM Resources where RenderPath = @RenderPath";
            try
            {
                using (var connection = new MySqlConnection(_appSettings?.Value?.ConnectionStrings?.IdentityConnection ?? ""))
                {
                    await connection.OpenAsync();
                    var result = await connection.QueryAsync<Resource>(sql, new { RenderPath = renderPath });
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error getting RESOURCES with Exception:\n\n{ex?.Message}. @GetAllResourcesByRenderPathAsync");
                throw new Exception(ex?.Message);
            }
        }

        // Fetch Specific Resource
        public async Task<Resource> GetResourceByFullPathAsync(string path, string title)
        {
            var sql = "SELECT * FROM Resources WHERE RenderPath = @RenderPath and Title = @Title";
            try
            {
                using (var connection = new MySqlConnection(_appSettings?.Value?.ConnectionStrings?.IdentityConnection ?? ""))
                {
                    await connection.OpenAsync();
                    var result = await connection.QueryAsync<Resource>(sql, new { RenderPath = path, Title = title });
                    return SelectSingleResourceFromList(result);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error getting RESOURCE with Exception:\n\n{ex?.Message}. @GetResourceByFullPathAsync");
                throw new Exception(ex?.Message);
            }
        }

        public async Task<IEnumerable<Resource>> GetAllResourcesByTagsAsync(List<string> tags)
        {
            string conds = tags?.GenerateTagsClauseCondition();
            if (string.IsNullOrEmpty(conds)) return new List<Resource>();
            var sql = "select * from Resources where match(Tags) against(" + conds + " IN BOOLEAN MODE);";
            try
            {
                using (var connection = new MySqlConnection(_appSettings?.Value?.ConnectionStrings?.IdentityConnection ?? ""))
                {
                    await connection.OpenAsync();
                    var result = await connection.QueryAsync<Resource>(sql);
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error getting RESOURCES with Exception:\n\n{ex?.Message}. @GetAllResourcesByTagsAsync");
                throw new Exception(ex?.Message);
            }
        }
        #endregion

        #region Create Methods [C from CRUD]
        // Add Review,
        public async Task<bool> AddResourceAsync(Resource resource)
        {
            if (resource == null) return false;
            string sql = @"
            INSERT INTO Resources (Title, BriefDescription, ExtensiveDescription, MetaDataID, RenderPath, StoragePath, PreviewData, Dimension, Tags, Type, UpdatedAt) 
            VALUES (@Title, @BriefDescription, @ExtensiveDescription, @MetaDataID, @RenderPath, @StoragePath, @PreviewData, @Dimension, @Tags, @Type, @UpdatedAt)";
            try
            {
                using (var connection = new MySqlConnection(_appSettings?.Value?.ConnectionStrings?.IdentityConnection ?? ""))
                {
                    await connection.OpenAsync();
                    var parameters = resource?.ToPerformDB();
                    var result = await connection.ExecuteAsync(sql, parameters);
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error adding new RESOURCE. @AddResourceAsync");
                throw new Exception(ex?.Message);
            }
        }
        #endregion

        #region Update Methods [U from CRUD]
        // Update Resource
        public async Task<bool> UpdateResourceAsync(Resource resource)
        {
            if (resource == null) return false;
            var sql = @"UPDATE Resources SET Title = @Title, BriefDescription = @BriefDescription, ExtensiveDescription = @ExtensiveDescription, 
                        MetaDataID = @MetaDataID, RenderPath = @RenderPath, StoragePath = @StoragePath, PreviewData = @PreviewData, Dimension = @Dimension, 
                        Tags = @Tags, Type = @Type, UpdatedAt = @UpdatedAt
                        WHERE RenderPath = @RenderPath and Title = @Title";

            try
            {
                using (var connection = new MySqlConnection(_appSettings?.Value?.ConnectionStrings?.IdentityConnection ?? ""))
                {
                    await connection.OpenAsync();
                    var parameters = resource?.ToPerformDB();
                    var result = await connection.ExecuteAsync(sql, parameters);
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error updating RESOURCE. @UpdateResourceAsync");
                throw new Exception(ex?.Message);
            }
        }
        #endregion

        #region Delete Methods [D from CRUD]
        // Delete Resource
        public async Task<bool> DeleteResourceAsync(string path, string title)
        {
            var sql = "DELETE FROM Resources WHERE RenderPath = @RenderPath and Title = @Title";
            try
            {
                using (var connection = new MySqlConnection(_appSettings?.Value?.ConnectionStrings?.IdentityConnection ?? ""))
                {
                    await connection.OpenAsync();
                    var result = await connection.ExecuteAsync(sql, new { RenderPath = path, Title = title });
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error deleting RESOURCE. @DeleteResourceAsync");
                throw new Exception(ex?.Message);
            }
        }
        #endregion

        #region Private Methods,
        private Resource SelectSingleResourceFromList(IEnumerable<Resource> resource)
        {
            if (resource == null || resource?.Count() <= 0 || !resource.Any())
                return new Resource();
            else if (resource?.Count() > 1)
                throw new Exception("More than one review with same email");
            else
                return resource.FirstOrDefault();
        }
        #endregion
    }
}
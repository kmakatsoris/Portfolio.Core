using Dapper;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using Portfolio.Core.Interfaces.Context.MetaData;
using Portfolio.Core.Types.Context;
using Portfolio.Core.Types.Models.MetaData;
using ILogger = NLog.ILogger;

namespace Portfolio.Core.Services.Context
{
    public class MetaDataRepository : IMetaDataRepository
    {
        private readonly IOptions<AppSettings> _appSettings;
        private readonly ILogger _logger;

        public MetaDataRepository
        (
            IOptions<AppSettings> appSettings,
            ILogger logger
        )
        {
            _appSettings = appSettings;
            _logger = logger;
        }

        #region Implement Basic CRUD Repository Interface,
        public Task<IEnumerable<MetaDataModel>> GetAllRecordsAsync() => GetAllMetaDataAsync();        
        public Task<MetaDataModel> GetRecordAsync(string email) => GetMetaDataAsync(email); 
        #endregion

        #region Fetch Methods [R from CRUD]               
        // Fetch All MetaData,
        public async Task<IEnumerable<MetaDataModel>> GetAllMetaDataAsync()
        {
            var sql = "SELECT * FROM MetaData";
            try
            {
                using (var connection = new MySqlConnection(_appSettings?.Value?.ConnectionStrings?.IdentityConnection ?? ""))
                {
                    await connection.OpenAsync();
                    var result = await connection.QueryAsync<MetaDataModel>(sql);
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error getting user with Exception:\n\n{ex?.Message}. @GetAllMetaDataAsync");
                throw new Exception(ex?.Message);
            }
        }        

        // Fetch Specific MetaData
        public async Task<MetaDataModel> GetMetaDataAsync(string email)
        {
            var sql = "SELECT * FROM MetaData WHERE Email = @Email";
            try
            {
                using (var connection = new MySqlConnection(_appSettings?.Value?.ConnectionStrings?.IdentityConnection ?? ""))
                {
                    await connection.OpenAsync();
                    var result = await connection.QueryAsync<MetaDataModel>(sql, new { Email = email });
                    return SelectSingleMetaDataFromList(result);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error getting user with Exception:\n\n{ex?.Message}. @GetMetaDataAsync");
                throw new Exception(ex?.Message);
            }
        }        
        // @TODO: Test
        public async Task<MetaDataModel> GetMetaDataAsyncById(Guid id)
        {
            var sql = "SELECT * FROM MetaData WHERE Id = @Id";
            try
            {
                using (var connection = new MySqlConnection(_appSettings?.Value?.ConnectionStrings?.IdentityConnection ?? ""))
                {
                    await connection.OpenAsync();
                    var result = await connection.QueryAsync<MetaDataModel>(sql, new { Id = id });
                    return SelectSingleMetaDataFromList(result);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error getting user with Exception:\n\n{ex?.Message}. @GetMetaDataAsyncById");
                throw new Exception(ex?.Message);
            }
        }
        #endregion

        #region Create Methods [C from CRUD]
        // Add MetaData,
        public async Task<bool> AddMetaDataAsync(MetaDataModel MetaData)
        {
            if (MetaData == null) return false;
            string sql = @"
            INSERT INTO MetaData (Email, UpdatedAt, Data) 
            VALUES (@Email, @UpdatedAt, @Data)";
            try
            {
                using (var connection = new MySqlConnection(_appSettings?.Value?.ConnectionStrings?.IdentityConnection ?? ""))
                {
                    await connection.OpenAsync();
                    var parameters = new
                    {
                        MetaData?.Email,
                        UpdatedAt = DateTime.UtcNow,
                        MetaData?.Data
                    };
                    var result = await connection.ExecuteAsync(sql, parameters);
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error adding new MetaData. @AddMetaDataAsync");
                throw new Exception(ex?.Message);
            }
        }
        #endregion

        #region Update Methods [U from CRUD]
        // Update MetaData
        public async Task<bool> UpdateMetaDataAsync(MetaDataModel MetaData)
        {
            if (MetaData == null) return false;
            MetaData.UpdatedAt = DateTime.UtcNow;
            var sql = "UPDATE MetaData SET UpdatedAt = @UpdatedAt, Data = @Data WHERE Email = @Email";

            try
            {
                using (var connection = new MySqlConnection(_appSettings?.Value?.ConnectionStrings?.IdentityConnection ?? ""))
                {
                    await connection.OpenAsync();
                    var result = await connection.ExecuteAsync(sql, MetaData);
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error updating user. @UpdateMetaDataAsync");
                throw new Exception(ex?.Message);
            }
        }
        #endregion

        #region Delete Methods [D from CRUD]
        // Delete MetaData
        public async Task<bool> DeleteMetaDataAsync(string email)
        {
            var sql = "DELETE FROM MetaData WHERE Email = @Email";
            try
            {
                using (var connection = new MySqlConnection(_appSettings?.Value?.ConnectionStrings?.IdentityConnection ?? ""))
                {
                    await connection.OpenAsync();
                    var result = await connection.ExecuteAsync(sql, new { Email = email });
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error deleting user. @DeleteMetaDataAsync");
                throw new Exception(ex?.Message);
            }
        }
        #endregion

        #region Private Methods,
        private MetaDataModel SelectSingleMetaDataFromList(IEnumerable<MetaDataModel> MetaData)
        {
            if (MetaData == null || MetaData?.Count() <= 0)
                return new MetaDataModel();
            else if (MetaData?.Count() > 1)
                throw new Exception("More than one MetaData with same email");
            else
                return MetaData.FirstOrDefault();
        }        
        #endregion
    }
}
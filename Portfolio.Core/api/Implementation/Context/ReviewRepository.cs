using Dapper;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Portfolio.Core.Interfaces.Context.Reviews;
using Portfolio.Core.Types.Context;
using Portfolio.Core.Types.DTOs.Reviews;
using Portfolio.Core.Types.Models.Reviews;
using ILogger = NLog.ILogger;

namespace Portfolio.Core.Services.Context
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly IOptions<AppSettings> _appSettings;
        private readonly ILogger _logger;

        public ReviewRepository
        (
            IOptions<AppSettings> appSettings,
            ILogger logger
        )
        {
            _appSettings = appSettings;
            _logger = logger;
        }

        #region Fetch Methods [R from CRUD]
        // Fetch All Reviews,
        public async Task<IEnumerable<Review>> GetAllReviewsAsync()
        {
            var sql = "SELECT * FROM Reviews";
            try
            {
                using (var connection = new MySqlConnection(_appSettings?.Value?.ConnectionStrings?.IdentityConnection ?? ""))
                {
                    await connection.OpenAsync();
                    var result = await connection.QueryAsync<Review>(sql);
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error getting user with Exception:\n\n{ex?.Message}. @GetAllReviewsAsync");
                throw new Exception(ex?.Message);
            }
        }

        // Fetch Specific Review
        public async Task<Review> GetReviewAsync(string email)
        {
            var sql = "SELECT * FROM Reviews WHERE Email = @Email";
            try
            {
                using (var connection = new MySqlConnection(_appSettings?.Value?.ConnectionStrings?.IdentityConnection ?? ""))
                {
                    await connection.OpenAsync();
                    var result = await connection.QueryAsync<Review>(sql, new { Email = email });
                    return SelectSingleReviewFromList(result);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error getting user with Exception:\n\n{ex?.Message}. @GetAllReviewsAsync");
                throw new Exception(ex?.Message);
            }
        }
        #endregion

        #region Create Methods [C from CRUD]
        // Add Review,
        public async Task<bool> AddReviewAsync(Review review)
        {
            if (review == null) return false;
            string sql = @"
            INSERT INTO Reviews (Email, Data) 
            VALUES (@Email, @Data)";
            try
            {
                using (var connection = new MySqlConnection(_appSettings?.Value?.ConnectionStrings?.IdentityConnection ?? ""))
                {
                    await connection.OpenAsync();
                    var parameters = new
                    {
                        review?.Email,
                        review?.Data
                    };
                    var result = await connection.ExecuteAsync(sql, parameters);
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error adding new review. @AddReviewAsync");
                throw new Exception(ex?.Message);
            }
        }
        #endregion

        #region Update Methods [U from CRUD]
        // Update Review
        public async Task<bool> UpdateReviewAsync(Review review)
        {
            if (review == null) return false;
            review.UpdatedAt = DateTime.UtcNow;
            var sql = "UPDATE Reviews SET UpdatedAt = @UpdatedAt, Data = @Data WHERE Email = @Email";

            try
            {
                using (var connection = new MySqlConnection(_appSettings?.Value?.ConnectionStrings?.IdentityConnection ?? ""))
                {
                    await connection.OpenAsync();
                    var result = await connection.ExecuteAsync(sql, review);
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error updating user. @UpdateReviewAsync");
                throw new Exception(ex?.Message);
            }
        }

        // @TAG: Usefull. Dont use it, unless modify it but it is working! Check them at a dummy DB
        public async Task<bool> AppendDataJson(string email, ReviewData data)
        {
            if (data == null || string.IsNullOrEmpty(email))
                return false;

            try
            {
                string jsonToAdd = JsonConvert.SerializeObject(new ReviewData(data.Title));
                var sql = "UPDATE Reviews SET Data = JSON_MERGE_PATCH(Data, @JsonToAdd) WHERE Email = @Email;";

                using (var connection = new MySqlConnection(_appSettings?.Value?.ConnectionStrings?.IdentityConnection ?? ""))
                {
                    await connection.OpenAsync();
                    var result = await connection.ExecuteAsync(sql, new { Email = email, JsonToAdd = jsonToAdd });
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error appending review. @AppendReviewDataAsync");
                throw new Exception(ex?.Message);
            }
        }

        // @TAG: Usefull. Dont use it, unless modify it but it is working! Check them at a dummy DB
        public async Task<bool> AlterKeyDataJson(string email, ReviewData data, string key)
        {
            if (data == null || string.IsNullOrEmpty(email)) return false;
            var sql = "UPDATE Reviews SET Data = JSON_SET(Data, CONCAT('$.', @Key), @Value) WHERE Email = @Email;";

            try
            {
                string dataStr = JsonConvert.SerializeObject(data);
                using (var connection = new MySqlConnection(_appSettings?.Value?.ConnectionStrings?.IdentityConnection ?? ""))
                {
                    await connection.OpenAsync();
                    var result = await connection.ExecuteAsync(sql, new { Email = email, Key = key, Value = dataStr });
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error appending review. @AlterKeyDataJson");
                throw new Exception(ex?.Message);
            }
        }
        #endregion

        #region Delete Methods [D from CRUD]
        // Delete Review
        public async Task<bool> DeleteReviewAsync(string email)
        {
            var sql = "DELETE FROM Reviews WHERE Email = @Email";
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
                _logger.Error(ex, "Error deleting user. @DeleteReviewAsync");
                throw new Exception(ex?.Message);
            }
        }
        #endregion

        #region Private Methods,
        private Review SelectSingleReviewFromList(IEnumerable<Review> reviews)
        {
            if (reviews == null || reviews?.Count() <= 0)
                return new Review();
            else if (reviews?.Count() > 1)
                throw new Exception("More than one review with same email");
            else
                return reviews.FirstOrDefault();
        }
        #endregion
    }
}
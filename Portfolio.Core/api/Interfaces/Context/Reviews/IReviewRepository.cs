using Portfolio.Core.Types.DTOs.Reviews;
using Portfolio.Core.Types.Models.Reviews;

namespace Portfolio.Core.Interfaces.Context.Reviews
{
    public interface IReviewRepository
    {
        Task<IEnumerable<Review>> GetAllReviewsAsync();
        Task<Review> GetReviewAsync(string email);
        Task<bool> AddReviewAsync(Review review);
        Task<bool> UpdateReviewAsync(Review review);
        Task<bool> AppendDataJson(string email, ReviewData data);
        Task<bool> AlterKeyDataJson(string email, ReviewData data, string key);
        Task<bool> DeleteReviewAsync(string email);
    }
}
namespace Portfolio.Core.Interfaces.Context
{
    public interface IRepository<T> where T: class
    {
        Task<IEnumerable<T>> GetAllRecordsAsync();
        Task<T> GetRecordAsync(string email);
    }
}
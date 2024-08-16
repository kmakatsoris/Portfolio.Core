using Microsoft.Extensions.Options;
using Portfolio.Core.Types.Context;
using ILogger = NLog.ILogger;

namespace Portfolio.Core.Interfaces.Context
{
    public class Repository<T> : IRepository<T> where T: class
    {
        private readonly IOptions<AppSettings> _appSettings;
        private readonly ILogger _logger;

        public Repository(IOptions<AppSettings> appSettings, ILogger logger)
        {
            _appSettings = appSettings;
            _logger = logger;
        }

        public virtual Task<IEnumerable<T>> GetAllRecordsAsync() => Task.FromResult<IEnumerable<T>>(new List<T>());

        public virtual Task<T> GetRecordAsync(string email) => Task.FromResult<T>(default(T));
    }
}
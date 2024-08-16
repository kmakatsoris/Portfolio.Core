/*
                [DESCRIPTION OF TEST CASES]
    1. GET ALL ITEMS
    1.1. Valid Connection String -> Returns all the records of the table
    1.2. Ensure returning empty list of records in case of not existing in the Table yet.
    1.3. Invalid Connection String -> Handle Exception & Returns Empty list of table's records.
    1.4. Return empty list of meta data, in case of NULL connection string
    1.5. Ensure Thread Safety with Concurrent Calls
    1.6. Giving valid timeout connection and return results. 

    2. GET SELECTED ITEM
    2.1. Giving valid argument/email and returns results.
    2.2. Giving invalid argument/email and DONT return results.
    2.3. Giving NULL argument/email and DONT return results.
    2.4. Giving EMPTY argument/email and DONT return results.
    2.5. Ensure Thread Safety with Concurrent Calls
    2.6. Giving invalid connection string and DONT return results.
    2.7. Giving null connection string and DONT return results.
    2.8. Giving valid timeout connection and return results.    
*/
using System.Data;
using Dapper;
using Microsoft.Extensions.Options;
using Moq;
using MySql.Data.MySqlClient;
using Portfolio.Core.Interfaces.Context;
using Portfolio.Core.Types.Context;
using Xunit;
using ILogger = NLog.ILogger;

namespace Portfolio.Core.UnitTesting.Implementation.Context
{
    public partial class BasicRepository__UnitTest<T> where T : class
    {
        // Define Class Variables
        private readonly string VALID_CONNECTION_STRING;
        private readonly DBBasicUtils _dBBasicUtils;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly Mock<ILogger> _loggerMock;
        private readonly Repository<T> _repository;
        private readonly string INVALID_CONNECTION_STRING;
        private readonly string VALID_EMAIL;
        private readonly string INVALID_EMAIL;
        private readonly string TABLE_NAME;
        private readonly int CONCURRENT_CALLS;
        private readonly TimeSpan VALID_TIMEOUT;

        // Define Constructor
        public BasicRepository__UnitTest(
            string validConnectionString,
            string invalidConnectionString,
            string validEmail,
            string invalidEmail,
            string tableName,
            int concurrentCalls,
            TimeSpan validTimeout
        )
        {
            VALID_CONNECTION_STRING = validConnectionString;
            INVALID_CONNECTION_STRING = invalidConnectionString;
            VALID_EMAIL = validEmail;
            INVALID_EMAIL = invalidEmail;
            TABLE_NAME = tableName;
            CONCURRENT_CALLS = concurrentCalls;
            VALID_TIMEOUT = validTimeout;

            _appSettings = Options.Create(new AppSettings
            {
                ConnectionStrings = new ConnectionStrings
                {
                    IdentityConnection = VALID_CONNECTION_STRING
                }
            });

            _dBBasicUtils = new DBBasicUtils(VALID_CONNECTION_STRING);
            _loggerMock = new Mock<ILogger>();
            _repository = new Repository<T>(_appSettings, _loggerMock.Object);
        }

        #region Private Repository Utils    
        private async Task GetUnitTestingMethod_Helper<U>(
            string connectionString,
            Func<Task<U>> tUnit,
            IEnumerable<Action<U>> assertionsPipeline,
            Action loggerVerification = null,
            bool enConcurrency = false,
            bool enTimeout = false
        ) where U : class
        {
            // Arrange
            var repository = connectionString?.ToLower()?.Equals(VALID_CONNECTION_STRING?.ToLower()) == true
                            ? _repository
                            : new Repository<T>(Options.Create(new AppSettings
                            {
                                ConnectionStrings = new ConnectionStrings
                                {
                                    IdentityConnection = INVALID_CONNECTION_STRING
                                }
                            }), _loggerMock.Object);

            var tasks = new List<Task<U>>();
            var cancellationTokenSource = new CancellationTokenSource();
            var timeoutTask = Task.Delay(VALID_TIMEOUT, cancellationTokenSource.Token);

            // Act
            U result = null;
            Task completedTask = null;
            if (enConcurrency)
            {
                for (int i = 0; i < CONCURRENT_CALLS; i++)
                {
                    tasks.Add(Task.Run(async () => await tUnit()));
                }
                await Task.WhenAll(tasks);
            }
            else if (enTimeout)
            {
                completedTask = await Task.WhenAny(tUnit(), timeoutTask);
            }
            else
            {
                result = await tUnit();
            }

            // Assert 
            loggerVerification ??= () => { }; // Default to a no-op action if no loggerVerification is provided
            if (enConcurrency)
            {
                foreach (var task in tasks)
                {
                    var r = await task; // Act
                    loggerVerification();
                    foreach (var assertion in assertionsPipeline)
                    {
                        assertion(r);
                    }
                }
            }
            else if (enTimeout)
            {
                if (completedTask == tUnit())
                {
                    result = await tUnit();
                    foreach (var assertion in assertionsPipeline)
                    {
                        assertion(result);
                    }
                }
                else
                {
                    cancellationTokenSource.Cancel();
                    loggerVerification();
                    Assert.True(false, "A Test did not complete within the specified timeout period.");
                }
            }
            else
            {
                loggerVerification();
                foreach (var assertion in assertionsPipeline)
                {
                    assertion(result);
                }
            }
        }
        #endregion  

    }
}

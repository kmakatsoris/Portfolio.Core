using Moq;
using Xunit;

namespace Portfolio.Core.UnitTesting.Implementation.Context
{            
    public partial class BasicRepository__UnitTest<T> where T : class
    {        
        [Fact]
        [Trait("Category", "GetSpecifiedRecordRepository")]
        [Trait("Subcategory", "GetRecordAsync_ValidEmail_FoundInDatabase")]
        [Trait("Attribute", "[Standard-Usage]: Giving valid email and returns results.")]
        public async Task GetRecordAsync_ValidEmail_FoundInDatabase()
        {
            // Arrange & Act & Assert
            await GetUnitTestingMethod_Helper<T>(
                VALID_CONNECTION_STRING,              
                async () => await _repository.GetRecordAsync(VALID_EMAIL),                
                new List<Action<T>>
                {
                    (result) => Assert.NotNull(result),
                    (result) => Assert.IsAssignableFrom<T>(result)                    
                }                
            );             
        }        

        [Fact]
        [Trait("Category", "GetSpecifiedRecordRepository")]
        [Trait("Subcategory", "GetRecordAsync_InvalidEmail_NotFoundInDatabase")]
        [Trait("Attribute", "[Standard-Usage]: Giving invalid email and DONT return results.")] 
        public async Task GetRecordAsync_InvalidEmail_NotFoundInDatabase()
        {
            // Arrange & Act & Assert
            await GetUnitTestingMethod_Helper<T>(
                VALID_CONNECTION_STRING,              
                async () => await _repository.GetRecordAsync(INVALID_EMAIL),                
                new List<Action<T>>
                {
                    (result) => Assert.Null(result),
                    (result) => Assert.False(result is T)                    
                }
            );          
        }

        [Fact]
        [Trait("Category", "MetaDataRepository")]
        [Trait("Subcategory", "GetRecordAsync_InvalidEmail_NotFoundInDatabase")]
        [Trait("Attribute", "[Standard-Usage]: Giving NULL email and DONT return results.")]
        public async Task GetRecordAsync_NullEmail_NotFoundInDatabase()
        {
            // Arrange & Act & Assert
            await GetUnitTestingMethod_Helper<T>(
                VALID_CONNECTION_STRING,              
                async () => await _repository.GetRecordAsync(null),                
                new List<Action<T>>
                {
                    (result) => Assert.Null(result),
                    (result) => Assert.False(result is T)                    
                }
            );           
        }        

        [Fact]
        [Trait("Category", "MetaDataRepository")]
        [Trait("Subcategory", "GetRecordAsync_InvalidEmail_NotFoundInDatabase")]
        [Trait("Attribute", "[Standard-Usage]: Giving EMPTY email and DONT return results.")]        
        public async Task GetRecordAsync_EmptyEmail_NotFoundInDatabase()
        {
            // Arrange & Act & Assert
            await GetUnitTestingMethod_Helper<T>(
                VALID_CONNECTION_STRING,              
                async () => await _repository.GetRecordAsync(""),                
                new List<Action<T>>
                {
                    (result) => Assert.Null(result),
                    (result) => Assert.False(result is T)                    
                }
            );         
        }  

        [Fact]
        [Trait("Category", "MetaDataRepository")]
        [Trait("Subcategory", "GetRecordAsync_ConcurrentCalls_ThreadSafety")]
        [Trait("Attribute", "[Standard-Usage]: Check concurrency between multiple DB executions and expect to Find Result Both of Them.")]        
        public async Task GetRecordAsync_ConcurrentCalls_ThreadSafety()
        {
            // Arrange & Act & Assert
            await GetUnitTestingMethod_Helper<T>(
                VALID_CONNECTION_STRING,              
                async () => await _repository.GetRecordAsync(VALID_EMAIL),                
                new List<Action<T>>
                {
                    (result) => Assert.NotNull(result),
                    (result) => Assert.IsAssignableFrom<T>(result)                    
                },
                enConcurrency: true
            );            
        }              

        [Fact]
        [Trait("Category", "MetaDataRepository")]
        [Trait("Subcategory", "GetRecordAsync_InvalidConnection_HandleException")]
        [Trait("Attribute", "[Standard-Usage]: Giving invalid connection string and DONT return results.")]        
        public async Task GetRecordAsync_InvalidConnection_HandleException()
        {
            // Arrange & Act & Assert
            await GetUnitTestingMethod_Helper<T>(
                INVALID_CONNECTION_STRING,              
                async () => await _repository.GetRecordAsync(VALID_EMAIL),                
                new List<Action<T>>
                {
                    (result) => Assert.NotNull(result),
                    (result) => Assert.IsAssignableFrom<T>(result)                                        
                },
                () => _loggerMock.Verify(logger => logger.Error(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once)              
            );                  
        }

        [Fact]
        [Trait("Category", "MetaDataRepository")]
        [Trait("Subcategory", "GetRecordAsync_NullConnection_HandleException")]
        [Trait("Attribute", "[Standard-Usage]: Giving null connection string and DONT return results.")]
        public async Task GetRecordAsync_NullConnection_HandleException()
        {
            // Arrange & Act & Assert
            await GetUnitTestingMethod_Helper<T>(
                null,              
                async () => await _repository.GetRecordAsync(VALID_EMAIL),                
                new List<Action<T>>
                {
                    (result) => Assert.NotNull(result),
                    (result) => Assert.IsAssignableFrom<T>(result)                                        
                },
                () => _loggerMock.Verify(logger => logger.Error(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once)              
            );                     
        }  
        
        [Fact]
        [Trait("Category", "MetaDataRepository")]
        [Trait("Subcategory", "GetRecordAsync_TimeOutConnection_FindResults")]
        [Trait("Attribute", "[Standard-Usage]: Giving valid timeout connection and return results.")]
        public async Task GetRecordAsync_TimeOutConnection_FindResults()
        {
            // Arrange & Act & Assert
            await GetUnitTestingMethod_Helper<T>(
                VALID_CONNECTION_STRING,              
                async () => await _repository.GetRecordAsync(VALID_EMAIL),                
                new List<Action<T>>
                {
                    (result) => Assert.NotNull(result),
                    (result) => Assert.IsAssignableFrom<T>(result)                                        
                },
                () => _loggerMock.Verify(logger => logger.Error(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once),
                enTimeout: true
            );
                
        }                
    }        
}
using Moq;
using Xunit;

namespace Portfolio.Core.UnitTesting.Implementation.Context
{
    public partial class BasicRepository__UnitTest<T> where T : class
    {
        [Fact]
        [Trait("Category", "Repository")]
        [Trait("Subcategory", "GetAllRecordsAsync__ReturnsData")]
        [Trait("Attribute", "[Standard-Usage]: Returns rows and the number of them is equal with the total involves in DB.")]
        public async Task GetAllRecordsAsync__ReturnsData()
        {
            // Arrange & Act & Assert
            await GetUnitTestingMethod_Helper<IEnumerable<T>>(
                VALID_CONNECTION_STRING,
                async () => await _repository.GetAllRecordsAsync(),                
                new List<Action<IEnumerable<T>>>
                {
                    (result) => Assert.NotNull(result),
                    (result) => Assert.IsAssignableFrom<IEnumerable<T>>(result),
                    async (result) => Assert.Equal(await _dBBasicUtils.GetTableRowsCount(), result.Count())
                }
            );
        }

        [Fact]
        [Trait("Category", "Repository")]
        [Trait("Subcategory", "GetAllRecordsAsync__ReturnsEmptyListWhenNoData")]
        [Trait("Attribute", "[Possible-Usage]: Ensure the TABLE can be empty.")]
        public async Task GetAllRecordsAsync__ReturnsEmptyListWhenNoData()
        {
            // Arrange & Act & Assert
            await GetUnitTestingMethod_Helper<IEnumerable<T>>(      
                VALID_CONNECTION_STRING,      
                async () => await _dBBasicUtils.ExecuteActionValidationAsync(
                    TABLE_NAME, 
                    async () => await _dBBasicUtils.ExecuteNonQueryAsync("DELETE FROM " + TABLE_NAME), 
                    async () => await _repository.GetAllRecordsAsync()),
                new List<Action<IEnumerable<T>>>
                {
                    (result) => Assert.NotNull(result),
                    (result) => Assert.IsAssignableFrom<IEnumerable<T>>(result),
                    (result) => Assert.Empty(result)
                }
            );
        }

        [Fact]
        [Trait("Category", "Repository")]
        [Trait("Subcategory", "GetAllRecordsAsync__HandlesException")]
        [Trait("Attribute", "[Handle-Exception]: Return empty list of T data, in case of INVALID connection string")]
        public async Task GetAllRecordsAsync__HandlesException()
        {        
            // Arrange & Act & Assert
            await GetUnitTestingMethod_Helper<IEnumerable<T>>(
                INVALID_CONNECTION_STRING,              
                async () => await _repository.GetAllRecordsAsync(),                
                new List<Action<IEnumerable<T>>>
                {
                    (result) => Assert.NotNull(result),
                    (result) => Assert.IsAssignableFrom<IEnumerable<T>>(result),
                    (result) => Assert.Empty(result)
                },
                () => _loggerMock.Verify(logger => logger.Error(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once)
            );             
        }

        [Fact]
        [Trait("Category", "Repository")]
        [Trait("Subcategory", "GetAllRecordsAsync__HandlesNullConnectionString")]
        [Trait("Attribute", "[Handle-Exception]: Return empty list of T data, in case of NULL connection string")]
        public async Task GetAllRecordsAsync__HandlesNullConnectionString()
        {
            // Arrange & Act & Assert
            await GetUnitTestingMethod_Helper<IEnumerable<T>>(
                null,              
                async () => await _repository.GetAllRecordsAsync(),                
                new List<Action<IEnumerable<T>>>
                {
                    (result) => Assert.NotNull(result),
                    (result) => Assert.IsAssignableFrom<IEnumerable<T>>(result),
                    (result) => Assert.Empty(result)
                },
                () => _loggerMock.Verify(logger => logger.Error(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once)
            );          
        }
    }    

}    
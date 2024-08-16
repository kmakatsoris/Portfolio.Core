using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using MySql.Data.MySqlClient;

public class DBBasicUtils
{
    private string _connectionString;
    private DataTable _lastScreenshot;

    public DBBasicUtils(string connectionString)
    {
        _connectionString = connectionString ?? "";
    }

    // Method to get a "screenshot" of the table (retrieve data)
    public async Task<DataTable> TakeScreenshotAsync (string tableName)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            string query = $"SELECT * FROM {tableName}";

            var dataTable = new DataTable();
            
            using (var reader = await connection.ExecuteReaderAsync(query))
            {
                dataTable.Load(reader);
            }

            _lastScreenshot = dataTable;
            return dataTable;
        }
    }

    // Method to restore the last "screenshot" to the table
    public async Task RestoreLastScreenshotAsync(string tableName)
    {
        if (_lastScreenshot == null)
        {
            throw new InvalidOperationException("No screenshot available to restore.");
        }

        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            using (var transaction = await connection.BeginTransactionAsync())
            {
                try
                {
                    // Clear the table before restoring
                    string clearTableQuery = $"DELETE FROM {tableName}";
                    await connection.ExecuteAsync(clearTableQuery, transaction: transaction);

                    foreach (DataRow row in _lastScreenshot.Rows)
                    {
                        var columnNames = string.Join(", ", _lastScreenshot.Columns.Cast<DataColumn>().Select(c => c.ColumnName));
                        var columnValues = string.Join(", ", _lastScreenshot.Columns.Cast<DataColumn>().Select(c => $"@{c.ColumnName}"));

                        string insertQuery = $"INSERT INTO {tableName} ({columnNames}) VALUES ({columnValues})";

                        var parameters = new DynamicParameters();
                        foreach (DataColumn column in _lastScreenshot.Columns)
                        {
                            parameters.Add($"@{column.ColumnName}", row[column]);
                        }

                        await connection.ExecuteAsync(insertQuery, parameters, transaction);
                    }

                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }
    }

    public async Task<TResult> ExecuteActionValidationAsync<TResult>(string tableName, Func<Task> action1, Func<Task<TResult>> action2)
    {
        await TakeScreenshotAsync(tableName);
        await action1();

        // Perform action given as argument
        TResult result = await action2();
        
        await RestoreLastScreenshotAsync(tableName);

        return result;
    }

    // Method to perform an action on the DB
    public async Task<IEnumerable<T>> ExecuteDbActionAsync<T>(string sql, object parameters = null)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            return await connection.QueryAsync<T>(sql, parameters);
        }
    }

    public async Task ExecuteNonQueryAsync(string sql, object parameters = null)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            await connection.ExecuteAsync(sql, parameters);
        }
    }

    // Get the number of rows from a table
    public async Task<int> GetTableRowsCount()
    {
        var sql = "SELECT COUNT(*) FROM MetaData";
        return (await ExecuteDbActionAsync<int>(sql))?.Count() ?? 0;        
    }    
}
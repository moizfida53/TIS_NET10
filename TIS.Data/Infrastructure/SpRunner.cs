using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace TIS.Data.Infrastructure;

public class SpRunner(string connectionString) : ISpRunner
{
    public async Task<IEnumerable<T>> QueryAsync<T>(string sp, object? param = null)
    {
        await using var conn = new SqlConnection(connectionString);
        return await conn.QueryAsync<T>(sp, param, commandType: CommandType.StoredProcedure);
    }

    public async Task<T?> QuerySingleOrDefaultAsync<T>(string sp, object? param = null)
    {
        await using var conn = new SqlConnection(connectionString);
        return await conn.QuerySingleOrDefaultAsync<T>(sp, param, commandType: CommandType.StoredProcedure);
    }

    public async Task<int> ExecuteAsync(string sp, object? param = null)
    {
        await using var conn = new SqlConnection(connectionString);
        return await conn.ExecuteAsync(sp, param, commandType: CommandType.StoredProcedure);
    }

    public async Task<TResult> QueryMultipleAsync<TResult>(
        string sp,
        object? param,
        Func<SqlMapper.GridReader, Task<TResult>> map)
    {
        await using var conn = new SqlConnection(connectionString);
        await conn.OpenAsync();
        using var grid = await conn.QueryMultipleAsync(sp, param, commandType: CommandType.StoredProcedure);
        return await map(grid);
    }
}

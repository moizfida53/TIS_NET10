using Dapper;

namespace TIS.Data.Infrastructure;

public interface ISpRunner
{
    Task<IEnumerable<T>> QueryAsync<T>(string sp, object? param = null);
    Task<T?> QuerySingleOrDefaultAsync<T>(string sp, object? param = null);
    Task<int> ExecuteAsync(string sp, object? param = null);

    /// <summary>
    /// Executes an SP that returns multiple result sets. The connection stays
    /// open for the duration of <paramref name="map"/>; dispose is handled internally.
    /// </summary>
    Task<TResult> QueryMultipleAsync<TResult>(
        string sp,
        object? param,
        Func<SqlMapper.GridReader, Task<TResult>> map);
}

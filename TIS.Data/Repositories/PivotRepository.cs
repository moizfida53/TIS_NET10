using TIS.Data.Infrastructure;
using TIS.Data.Models.Pivot;

namespace TIS.Data.Repositories;

public class PivotRepository(ISpRunner sp) : IPivotRepository
{
    // sp_vwSub1Pivot returns 2 result sets; pivot data is in result set [1]
    public Task<IEnumerable<PivotRowDto>> GetPivotAsync() =>
        sp.QueryMultipleAsync("sp_vwSub1Pivot", null, async grid =>
        {
            await grid.ReadAsync<dynamic>(); // skip result set [0]
            return await grid.ReadAsync<PivotRowDto>();
        });

    public Task SavePivotAsync(string pivotObject) =>
        sp.ExecuteAsync("sp_SavePivot", new { Object = pivotObject });

    public async Task<string?> RestorePivotAsync()
    {
        var result = await sp.QuerySingleOrDefaultAsync<string>("sp_RestorePivot");
        return result;
    }
}

using TIS.Data.Models.Pivot;

namespace TIS.Data.Repositories;

public interface IPivotRepository
{
    Task<IEnumerable<PivotRowDto>> GetPivotAsync();

    // sp_SavePivot must exist:  INSERT INTO tblPivot (Object, Date) VALUES (@Object, GETDATE())
    Task SavePivotAsync(string pivotObject);

    // sp_RestorePivot must exist:  SELECT TOP 1 Object FROM tblPivot ORDER BY Date DESC
    Task<string?> RestorePivotAsync();
}

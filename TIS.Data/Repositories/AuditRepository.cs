using Dapper;
using Microsoft.Data.SqlClient;
using TIS.Data.Models.Audit;

namespace TIS.Data.Repositories;

// Audit tables (TBL_AT_MASTER / TBL_AT_DETAILS / tbluser) have no stored procedures,
// so this repository uses parameterized Dapper queries directly.
public class AuditRepository(string connectionString) : IAuditRepository
{
    public async Task<IEnumerable<AuditEmployeeDto>> GetEmployeesAsync()
    {
        await using var conn = new SqlConnection(connectionString);
        const string sql = "SELECT uid, username, name, EMPLOYEENO FROM tbluser ORDER BY name";
        var rows = await conn.QueryAsync<dynamic>(sql);
        return rows.Select(r =>
        {
            var d = (IDictionary<string, object>)r;
            return new AuditEmployeeDto
            {
                Uid      = Convert.ToInt32(d["uid"] ?? 0),
                UserName = d["username"]?.ToString() ?? "",
                EmpName  = d["name"]?.ToString()     ?? "",
                EmpNo    = d["EMPLOYEENO"]?.ToString() ?? ""
            };
        });
    }

    public async Task<IEnumerable<AuditReportRowDto>> SearchAsync(AuditSearchRequest req)
    {
        await using var conn = new SqlConnection(connectionString);
        const string sql = @"
            SELECT m.ID, m.ACTION_NAME, m.RESULT,
                   ISNULL(u.NAME,'') AS [USER], m.USERID, m.DATE1, m.FORM_ID
            FROM   TBL_AT_MASTER m
            LEFT JOIN tbluser u ON u.uid = m.userid
            WHERE  m.DATE1  BETWEEN @StartDate AND @EndDate
              AND  m.form_id = @Event
              AND  m.userid  = @Uid
              AND  m.result  = @Status
            ORDER BY m.DATE1 DESC";

        var rows = await conn.QueryAsync<dynamic>(sql, new
        {
            req.StartDate,
            req.EndDate,
            req.Event,
            req.Uid,
            req.Status
        });

        return rows.Select(r =>
        {
            var d = (IDictionary<string, object>)r;
            return new AuditReportRowDto
            {
                Id         = Convert.ToInt32(d["ID"]          ?? 0),
                ActionName = d["ACTION_NAME"]?.ToString()     ?? "",
                Result     = d["RESULT"]?.ToString()          ?? "",
                User       = d["USER"]?.ToString()            ?? "",
                UserId     = d["USERID"]?.ToString()          ?? "",
                Date       = d["DATE1"]?.ToString()           ?? "",
                FormId     = Convert.ToInt32(d["FORM_ID"]     ?? 0)
            };
        });
    }

    public async Task<IEnumerable<AuditDetailDto>> GetDetailsAsync(int id)
    {
        await using var conn = new SqlConnection(connectionString);
        const string sql = @"
            SELECT ID, SNO, AT_ID, OLD_VALUE, NEW_VALUE, FIELD_NAME
            FROM   TBL_AT_DETAILS
            WHERE  AT_ID = @Id";

        var rows = await conn.QueryAsync<dynamic>(sql, new { Id = id });
        return rows.Select(r =>
        {
            var d = (IDictionary<string, object>)r;
            return new AuditDetailDto
            {
                Id        = Convert.ToInt32(d["ID"]         ?? 0),
                Sno       = Convert.ToInt32(d["SNO"]        ?? 0),
                AtId      = Convert.ToInt32(d["AT_ID"]      ?? 0),
                OldValue  = d["OLD_VALUE"]?.ToString()      ?? "",
                NewValue  = d["NEW_VALUE"]?.ToString()      ?? "",
                FieldName = d["FIELD_NAME"]?.ToString()     ?? ""
            };
        });
    }
}

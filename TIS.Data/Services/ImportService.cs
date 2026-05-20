using System.Data;
using System.Runtime.Versioning;
using System.Text;
using ExcelDataReader;
using Microsoft.Data.SqlClient;
using TIS.Data.Models.Import;
using TIS.Data.Repositories;

namespace TIS.Data.Services;

/// <summary>
/// Handles the heavy Excel-read → tblImport bulk-copy pipeline.
/// Lives in TIS.Data because it needs direct SqlBulkCopy + connection access.
/// </summary>
public class ImportService(string connectionString, IImportRepository repo)
{
    private static readonly string[] DestCols =
        ["SUB_NO", "BILLDATE", "CALLDATE", "TRANS_TYPE", "DESCRIPTION", "CALLTIME", "DURATION", "AMOUNT", "BILLNUMBER"];

    // ── Excel / Access import ─────────────────────────────────────────────

    public async Task<ImportNullResult> ImportFileAsync(
        string filePath, string sheetName, int providerId, DateTime billDate)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        var setting = await repo.GetProviderSettingAsync(providerId)
            ?? throw new InvalidOperationException($"Provider {providerId} not found.");

        DataTable data;
        if (Path.GetExtension(filePath).ToLower() is ".mde" or ".mdb")
        {
            if (!OperatingSystem.IsWindows())
                throw new PlatformNotSupportedException("Access file import requires Windows.");
#pragma warning disable CA1416
            data = ReadAccessSheet(filePath, sheetName);
#pragma warning restore CA1416
        }
        else
            data = ReadExcelSheet(filePath, sheetName);

        await BulkCopyToImportAsync(data, setting, providerId, billDate);
        return await repo.GetImportNullRowsAsync();
    }

    // ── DB-based import (external SQL via OleDb conn string) ─────────────

    [SupportedOSPlatform("windows")]
    public async Task<ImportNullResult> ImportDbAsync(
        string view, string oleDbConnStr, int month, int year, int providerId, DateTime billDate)
    {
        if (string.IsNullOrWhiteSpace(view))
            throw new ArgumentException("View name is empty.");

        var setting = await repo.GetProviderSettingAsync(providerId)
            ?? throw new InvalidOperationException($"Provider {providerId} not found.");

        string billDateStr = billDate.ToString("yyyy-MM-dd");
        string sql = $"SELECT *, '{billDateStr}' AS BillDateNew FROM [{view}] WHERE Month(CallDate)={month} AND Year(CallDate)={year}";

        var data = new DataTable();
        using (var conn = new System.Data.OleDb.OleDbConnection(oleDbConnStr))
        using (var adapter = new System.Data.OleDb.OleDbDataAdapter(sql, conn))
            adapter.Fill(data);

        await BulkCopyToImportAsync(data, setting, providerId, billDate);
        return await repo.GetImportNullRowsAsync();
    }

    // ── Get sheet names from a file ───────────────────────────────────────

    public static IEnumerable<string> GetSheetNames(string filePath)
    {
        string ext = Path.GetExtension(filePath).ToLower();
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        if (ext is ".xls" or ".xlsx")
        {
            using var stream = File.OpenRead(filePath);
            using var reader = ExcelReaderFactory.CreateReader(stream);
            var ds = reader.AsDataSet(new ExcelDataSetConfiguration
            {
                ConfigureDataTable = _ => new ExcelDataTableConfiguration { UseHeaderRow = true }
            });
            return ds.Tables.Cast<DataTable>().Select(t => t.TableName).ToList();
        }

        if (OperatingSystem.IsWindows() && ext is ".mde" or ".mdb")
            return GetAccessTableNames(filePath);

        return [];
    }

    // ── Get column names from a sheet ─────────────────────────────────────

    public static IEnumerable<string> GetColumnNames(string filePath, string sheetName)
    {
        string ext = Path.GetExtension(filePath).ToLower();
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        if (ext is ".xls" or ".xlsx")
        {
            using var stream = File.OpenRead(filePath);
            using var reader = ExcelReaderFactory.CreateReader(stream);
            var ds = reader.AsDataSet(new ExcelDataSetConfiguration
            {
                ConfigureDataTable = _ => new ExcelDataTableConfiguration { UseHeaderRow = true }
            });
            var tbl = ds.Tables.Cast<DataTable>()
                .FirstOrDefault(t => t.TableName.Equals(sheetName, StringComparison.OrdinalIgnoreCase));
            return tbl?.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToList() ?? [];
        }

        if (OperatingSystem.IsWindows() && ext is ".mde")
            return GetAccessColumnNames(filePath, sheetName);

        return [];
    }

    // ── Private helpers ───────────────────────────────────────────────────

    private async Task BulkCopyToImportAsync(
        DataTable data, ProviderSettingDto setting, int providerId, DateTime billDate)
    {
        string billDateStr = billDate.ToString("yyyy-MM-dd");

        // Inject BillDateNew column
        if (!data.Columns.Contains("BillDateNew"))
            data.Columns.Add("BillDateNew", typeof(string));
        foreach (DataRow r in data.Rows)
            r["BillDateNew"] = billDateStr;

        // Col1=SUB_NO, BillDateNew(injected)=BILLDATE, Col3-Col9 for remaining dest cols
        string[] srcCols =
        [
            setting.Col1, "BillDateNew", setting.Col3, setting.Col4,
            setting.Col5, setting.Col6, setting.Col7, setting.Col8, setting.Col9
        ];

        await using var conn = new SqlConnection(connectionString);
        await conn.OpenAsync();

        // Clear staging table and reseed
        int maxId = await new SqlCommand(
            "SELECT ISNULL(MAX(ID), 0) FROM tblcallrecord", conn).ExecuteScalarAsync() is int v ? v : 0;
        await new SqlCommand("DELETE FROM tblImport", conn).ExecuteNonQueryAsync();
        await new SqlCommand($"DBCC CHECKIDENT(tblIMPORT, RESEED, {maxId})", conn).ExecuteNonQueryAsync();

        using var bulk = new SqlBulkCopy(conn) { BulkCopyTimeout = 500, DestinationTableName = "tblImport" };
        for (int i = 0; i < 9; i++)
        {
            if (string.IsNullOrWhiteSpace(srcCols[i])) continue;
            bulk.ColumnMappings.Add(srcCols[i], DestCols[i]);
        }
        await bulk.WriteToServerAsync(data);

        // Run sp_tblImport to normalise CALLTIME and set PROVIDER
        await new SqlCommand(
            $"EXEC sp_tblImport @PROVIDER={providerId}", conn).ExecuteNonQueryAsync();
    }

    private static DataTable ReadExcelSheet(string filePath, string sheetName)
    {
        using var stream = File.OpenRead(filePath);
        using var reader = ExcelReaderFactory.CreateReader(stream);
        var ds = reader.AsDataSet(new ExcelDataSetConfiguration
        {
            ConfigureDataTable = _ => new ExcelDataTableConfiguration { UseHeaderRow = true }
        });
        return ds.Tables.Cast<DataTable>()
            .FirstOrDefault(t => t.TableName.Equals(sheetName, StringComparison.OrdinalIgnoreCase))
            ?? throw new Exception($"Sheet '{sheetName}' not found in '{filePath}'.");
    }

    [SupportedOSPlatform("windows")]
    private static DataTable ReadAccessSheet(string filePath, string tableName)
    {
        string connStr = Path.GetExtension(filePath).ToLower() == ".mde"
            ? $"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={filePath};User Id=admin;Password=;"
            : $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={filePath};Persist Security Info=False;";
        var dt = new DataTable();
        using var conn = new System.Data.OleDb.OleDbConnection(connStr);
        using var adapter = new System.Data.OleDb.OleDbDataAdapter($"SELECT * FROM [{tableName}]", conn);
        adapter.Fill(dt);
        return dt;
    }

    [SupportedOSPlatform("windows")]
    private static IEnumerable<string> GetAccessTableNames(string filePath)
    {
        string ext = Path.GetExtension(filePath).ToLower();
        string connStr = ext == ".mde"
            ? $"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={filePath};"
            : $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={filePath};Persist Security Info=False;";
        using var conn = new System.Data.OleDb.OleDbConnection(connStr);
        conn.Open();
        return conn.GetSchema("Tables").AsEnumerable()
            .Select(r => r["TABLE_NAME"]?.ToString() ?? "").Where(n => !string.IsNullOrEmpty(n)).ToList();
    }

    [SupportedOSPlatform("windows")]
    private static IEnumerable<string> GetAccessColumnNames(string filePath, string tableName)
    {
        string connStr = $"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={filePath};User Id=admin;Password=;";
        var dt = new DataTable();
        using var conn = new System.Data.OleDb.OleDbConnection(connStr);
        using var adapter = new System.Data.OleDb.OleDbDataAdapter($"SELECT * FROM [{tableName}]", conn);
        adapter.Fill(dt);
        return dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToList();
    }
}

---
name: sp-catalog-updater
description: Re-generates docs/sp_catalog.md from the live TIS_KDD_NEW database. Use when stored procedures have been added, modified, or removed since the last catalog generation.
tools: PowerShell, Write
---

You are the **SP catalog updater**.

## Method

1. Run this PowerShell to extract SPs from `TIS_KDD_NEW`:
```powershell
$sql = @"
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
SELECT 
    p.name + '|' + ISNULL(STUFF((
        SELECT '; ' + pa.name + ' ' + TYPE_NAME(pa.user_type_id) 
        FROM sys.parameters pa 
        WHERE pa.object_id = p.object_id AND pa.is_output = 0
        ORDER BY pa.parameter_id
        FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, ''), '') + '|' + CONVERT(varchar, p.modify_date, 23)
FROM sys.procedures p
WHERE p.is_ms_shipped = 0
ORDER BY p.name;
"@
$rows = sqlcmd -S '.\SQLEXPRESS' -E -d TIS_KDD_NEW -Q $sql -W -h -1
```

2. Build a markdown table with columns: SP name (code-formatted), parameters (or `_(no params)_`), last modified date.

3. Write to `F:\DAD Projects\TIS_NET10\docs\sp_catalog.md` overwriting any prior version, with this header:
```
# TIS Stored Procedure Catalog

Generated from local restore of `TIS_KDD_NEW` (source: `TIS_KDD__May.bak`).
Total user stored procedures: **N**.
```

4. Report total count and any deltas from the previous catalog (use `git diff` if the file was previously committed).

## Do not

- Do not run any other SQL or modify the database.
- Do not edit code files.
- Keep your reply under 100 words.

---
name: add-sp-repo
description: Scaffold a repository method around an existing stored procedure listed in docs/sp_catalog.md. Trigger when user types /add-sp-repo followed by an SP name, OR asks to "wrap sp_X in a repo method", "create a repo for {sp}".
---

# /add-sp-repo — Add a repository method for a known SP

## When to invoke

User: `/add-sp-repo sp_GetForceBills`, "wrap sp_GetEmail in BillRepository", "I need a repo method for API_UpdateBillByBillId".

## What to do — inline, no sub-agent needed

1. **Read** `docs/sp_catalog.md` and find the SP. If it's not listed, stop and tell the user to run `/sp-catalog-updater` first.
2. **Identify the parameters** from the catalog row. Map SQL types to C#:
   - `int` → `int`
   - `bigint` → `long`
   - `varchar`/`nvarchar` → `string`
   - `datetime` → `DateTime`
   - `bit` → `bool`
   - Table types (e.g. `BAPIData`) → flag this; needs DataTable parameter
3. **Determine the result shape**:
   - Ask the user if it returns rows or just a status (or scan the SP definition with `sqlcmd -Q "EXEC sp_helptext '{sp}'"` if necessary, but only when unclear).
   - If rows, propose a DTO class name and properties matching expected columns.
4. **Pick or create the repository**:
   - Find the closest existing repo by SP name prefix (e.g. `sp_Bill*` → `BillRepository`, `API_*` → `ApiRepository`, `BAPI_*` → `SapRepository`).
   - If no repo exists, create `TIS.Data/Repositories/I{Name}Repository.cs` + `{Name}Repository.cs` and register in `Program.cs`.
5. **Add the method** — pattern:
```csharp
public Task<IEnumerable<{Dto}>> {MethodName}Async({params}) =>
    _sp.QueryAsync<{Dto}>("{sp_name}", new { p1, p2, ... });
```
6. **Build** to confirm.

## Reply

Show the final method signature and confirm build. Under 80 words.

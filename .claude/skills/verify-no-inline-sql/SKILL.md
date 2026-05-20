---
name: verify-no-inline-sql
description: Grep the new TIS.NET10 codebase for any SQL outside TIS.Data/Repositories/. Use as a gate before declaring a module done or before committing. Trigger when user types /verify-no-inline-sql OR asks to "check for inline SQL", "verify SP-only".
---

# /verify-no-inline-sql — Enforce SP-only data access

## What to do

Run these greps against the new project (`F:\DAD Projects\TIS_NET10\`). Exclude `TIS.Data/Repositories/`, `bin/`, `obj/`, and `wwwroot/lib/`.

1. **ADO.NET surface area:**
   - `new SqlConnection`
   - `new SqlCommand`
   - `SqlDataAdapter`
   - `IDbConnection`
   - `IDbCommand`

2. **EF Core raw escape hatches:**
   - `FromSqlRaw`
   - `FromSqlInterpolated`
   - `ExecuteSqlRaw`
   - `ExecuteSqlInterpolated`

3. **Raw SQL strings in user-facing code** (regex):
   - `\bSELECT\s+`
   - `\bINSERT\s+INTO`
   - `\bUPDATE\s+\w+\s+SET`
   - `\bDELETE\s+FROM`
   - `\bEXEC\s+sp_`

## Output

For each category, report the count of hits and the file:line of the first hit. If all categories are zero, say:

> ✅ No inline SQL outside `TIS.Data/Repositories/`. Safe to commit.

Otherwise list the offenders so the user can fix them. Reply ≤ 80 words.

## Do not

- Open Edit/Write. This skill is read-only.
- Fix the offenders yourself — report only.

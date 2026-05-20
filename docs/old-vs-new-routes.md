# Old vs New Route Parity

Maintain URL parity wherever possible so bookmarks survive cutover. If a route MUST change, add a permanent redirect in `Program.cs`.

| Legacy URL | New URL | Notes |
|---|---|---|
| `/Admin/Index` | `/Admin/Index` | Same |
| `/User/Index` | `/Bill/Index` | Legacy `UserController` was actually bill list — rename for clarity |
| `/Email/Templates` | `/Email/Templates` | Same |
| `/BillReport/Index` | `/BillReport/Index` | Same |
| `/Dashboard/Index` | `/Dashboard/Index` | Same |
| `/Setting/Index` | `/Settings/Index` | Plural for consistency — add redirect |
| `/MarsaAPI/*` | `/api/marsa/*` | Move to `api/` prefix per ASP.NET Core convention — add redirects |

To be filled in as each module is ported.

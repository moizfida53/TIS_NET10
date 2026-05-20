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

## Module 1 — Admin / Users (done 2026-05-20)

| Legacy URL | New URL | Notes |
|---|---|---|
| `GET  /Admin/Index` | `GET  /Admin/Index` | Same; Admins (role 3) redirect to /BillReport |
| `GET  /Admin/GetUser` | `GET  /Admin/GetUser` | Returns `{ dtEmp, dtCC, CountryList, RoleList }` |
| `POST /Admin/AddEmployee` | `POST /Admin/AddEmployee` | JSON body `EmployeeRequest { Employee, CountryIds[] }` |
| `POST /Admin/UpdateEmployee` | `POST /Admin/UpdateEmployee` | Same shape as Add |
| `POST /Admin/DeleteEmployee` | `POST /Admin/DeleteEmployee?uid=` | Query param |
| `POST /Admin/AddCC` | `POST /Admin/AddCC` | JSON `{ UID, CCName, CCNum }` |
| `POST /Admin/UpdateCC` | `POST /Admin/UpdateCC` | Same |
| `POST /Admin/DeleteCC` | `POST /Admin/DeleteCC?uid=` | |
| `POST /Admin/AddCountry` | `POST /Admin/AddCountry` | JSON; backed by sp_ManageCountry |
| `POST /Admin/UpdateCountry` | `POST /Admin/UpdateCountry` | Same |
| `POST /Admin/DeleteCountry` | `POST /Admin/DeleteCountry?countryId=` | |
| `GET  /Admin/GetManagers` | `GET  /Admin/GetManagers` | Returns `{ dtManager }` via sp_GetManagers |
| `POST /Admin/AddManager` | `POST /Admin/AddManager` | JSON `ManagerRequest { Uid, Name, EmployeeNo }` |
| `POST /Admin/UpdateManager` | `POST /Admin/UpdateManager` | Same |
| `POST /Admin/DeleteManager` | `POST /Admin/DeleteManager?uid=` | |
| `GET  /Admin/Telephone` | `GET  /Admin/Telephone` | View: AddTelephone.cshtml |
| `GET  /Admin/GetTelData` | `GET  /Admin/GetTelData` | Returns `{ dtTel, dtAsg, dtProvider, dtEmp }` |
| `GET  /Admin/GetTelNo` | `GET  /Admin/GetTelNo` | Unassigned numbers only |
| `POST /Admin/AddTelephone` | `POST /Admin/AddTelephone` | |
| `POST /Admin/UpdateTelephone` | `POST /Admin/UpdateTelephone` | |
| `POST /Admin/DeleteTelephone` | `POST /Admin/DeleteTelephone?id=` | |
| `POST /Admin/Assign` | `POST /Admin/Assign` | |
| `POST /Admin/UpdateAssign` | `POST /Admin/UpdateAssign` | |
| `POST /Admin/DeleteAssign` | `POST /Admin/DeleteAssign?id=` | |
| `GET  /Admin/Delegate` | `GET  /Admin/Delegate` | View: DelegateBills.cshtml |
| `GET  /Admin/GetDelegate` | `GET  /Admin/GetDelegate` | Returns `{ dtSec, empList }` |
| `POST /Admin/SaveDelegate` | `POST /Admin/SaveDelegate` | Add new delegation |
| `POST /Admin/UpdateDelegate` | `POST /Admin/UpdateDelegate` | |
| `POST /Admin/DeleteDelegate` | `POST /Admin/DeleteDelegate?id=` | |
| `GET  /Admin/Package` | `GET  /Admin/Package` | View: Package.cshtml |
| `GET  /Admin/GetPkgData` | `GET  /Admin/GetPkgData` | Returns `{ dtPro, dtPkg }` |
| `GET  /Admin/GetPkgDetail` | `GET  /Admin/GetPkgDetail?pkgId=` | Returns `{ pkgDetail }` |
| `POST /Admin/AddPackage` | `POST /Admin/AddPackage` | JSON `PackageRequest { Master, Details[] }` |
| `POST /Admin/UpdatePackage` | `POST /Admin/UpdatePackage` | Same |
| `POST /Admin/DeletePackage` | `POST /Admin/DeletePackage?id=` | |
| `GET  /Admin/GetDataRoaming` | `GET  /Admin/GetDataRoaming` | Returns `{ dtCountry }` |
| `POST /Admin/AddDataRoaming` | `POST /Admin/AddDataRoaming` | |
| `POST /Admin/UpdateDataRoaming` | `POST /Admin/UpdateDataRoaming` | |
| `POST /Admin/DeleteDataRoaming` | `POST /Admin/DeleteDataRoaming?id=` | |
| `GET  /ADTest/Index` | `GET  /ADTest/Index` | Standalone diagnostic page |
| `POST /ADTest/TestConnection` | `POST /ADTest/TestConnection` | |
| `POST /ADTest/SearchUser` | `POST /ADTest/SearchUser` | |
| `POST /ADTest/UpdateMobile` | `POST /ADTest/UpdateMobile` | |

## Module 3 — Settings (done 2026-05-20)

| Legacy URL | New URL | Notes |
|---|---|---|
| `GET  /Setting/Index` | `GET  /Setting/Index` | View: Config.cshtml |
| `GET  /Setting/GetConfig` | `GET  /Setting/GetConfig` | Returns `{ dtConfig }` via sp_GetConfig |
| `POST /Setting/SaveConfig` | `POST /Setting/SaveConfig` | JSON body `ConfigDto`; backed by sp_SaveConfig |
| `POST /Setting/ApplyPolicy` | `POST /Setting/ApplyPolicy` | Runs sp_ApplyPolicy |
| `GET  /Setting/Policy` | `GET  /Setting/Policy` | View: ManageCallType.cshtml |
| `GET  /Setting/GetPolicyData` | `GET  /Setting/GetPolicyData` | Returns `{ dtProvider, dtCallType, dtEmp, dtLineType }` |
| `GET  /Setting/GetPolicies` | `GET  /Setting/GetPolicies` | Returns `{ dtPolicy }` via sp_GetPolicies |
| `GET  /Setting/GetTransTypes` | `GET  /Setting/GetTransTypes?providerId=` | Distinct trans types for a provider |
| `GET  /Setting/GetDescriptions` | `GET  /Setting/GetDescriptions?providerId=&transType=` | Distinct descriptions |
| `GET  /Setting/GetPolicyDetail` | `GET  /Setting/GetPolicyDetail?id=` | Returns `{ dtDetail }` — employee-line pairs |
| `POST /Setting/AddPolicy` | `POST /Setting/AddPolicy` | JSON body `AddPolicyRequest` |
| `POST /Setting/UpdatePolicy` | `POST /Setting/UpdatePolicy` | JSON body `UpdatePolicyRequest` |
| `POST /Setting/DeletePolicy` | `POST /Setting/DeletePolicy?id=` | |
| `GET  /Setting/Provider` | `GET  /Setting/Provider` | View: Provider.cshtml |
| `GET  /Setting/GetProviders` | `GET  /Setting/GetProviders` | Returns `{ ProviderList }` filtered by role/country |
| `POST /Setting/AddProvider` | `POST /Setting/AddProvider` | JSON body `ProviderDto` |
| `POST /Setting/UpdateProvider` | `POST /Setting/UpdateProvider` | JSON body `ProviderDto` |
| `POST /Setting/DeleteProvider` | `POST /Setting/DeleteProvider?id=` | Cascades via `DeleteProvider` SP |

## Module 4 — Import (done 2026-05-20)

| Legacy URL | New URL | Notes |
|---|---|---|
| `GET  /Import/Index` | `GET  /Import/Index` | View: ImportInvoice.cshtml |
| `GET  /Import/UnAssigned` | `GET  /Import/UnAssigned` | View: UnAssignedInvoice.cshtml |
| `GET  /Import/GetUploadHistory` | `GET  /Import/GetUploadHistory` | Returns `{ UploadList, IsDeleteButShow }` |
| `POST /Import/Upload` | `POST /Import/Upload` | Multipart; saves to `Bills/` folder |
| `GET  /Import/FillSheet` | `GET  /Import/FillSheet?fileName=` | Returns `{ dtSheet }` — sheet names |
| `GET  /Import/UploadSetting` | `GET  /Import/UploadSetting?fileName=&sheetName=` | Returns `{ dtCol }` — column names |
| `GET  /Import/UploadFile` | `POST /Import/UploadFile` | **Changed to POST** with JSON body `UploadFileRequest`; SQL injection fixed |
| `GET  /Import/ProcessBill` | `POST /Import/ProcessBill` | **Changed to POST**; stateless — body has `FileName, BillDate, ProviderID` (no Session) |
| `GET  /Import/UpdateImport` | `POST /Import/UpdateImport` | **Changed to POST**; JSON body; SQL injection fixed |
| `GET  /Import/GetSetting` | `GET  /Import/GetSetting?provider=` | Returns `{ dtCol }` or `{ dtDBCol }` |
| `POST /Import/UpdateSetting` | `POST /Import/UpdateSetting` | Handles both Excel and DB setting; `dbBased` inferred from `DbConstr` field |
| `POST /Import/UpdateDBSetting` | `POST /Import/UpdateSetting` | Merged into UpdateSetting |
| `GET  /Import/CheckProvider` | `GET  /Import/CheckProvider?provider=` | Returns `{ DbBased }` |
| `GET  /Import/DeleteBill` | `POST /Import/DeleteBill` | **Changed to POST**; SQL injection fixed |
| `GET  /Import/GetUnAssignedBill` | `GET  /Import/GetUnAssignedBill` | SQL injection fixed (countryId from claims) |
| `GET  /Import/AssignInvoice` | `POST /Import/AssignInvoice` | **Changed to POST** |

## Module 5 — Bill management (done 2026-05-20)

| Legacy URL | New URL | Notes |
|---|---|---|
| `GET  /Bill/Index`            | `GET  /Bill/Index`            | View: ForceBill.cshtml |
| `GET  /Bill/ChangeStatus`     | `GET  /Bill/ChangeStatus`     | View: ChangeStatus.cshtml |
| `GET  /Bill/ReAssignBill`     | `GET  /Bill/ReAssignBill`     | View: ReAssignBill.cshtml |
| `GET  /Bill/ReImburseBill`    | `GET  /Bill/ReImburseBill`    | View: ReImburseBill.cshtml |
| `GET  /Bill/GetForceBill`     | `GET  /Bill/GetForceBill`     | Returns `{ Bills }` via sp_GetForceBills |
| `POST /Bill/ForceBill`        | `POST /Bill/ForceBill`        | JSON body; UID from claims (not body); tmp_bill_ids parameterized |
| `GET  /Bill/GetSearchData`    | `GET  /Bill/GetSearchData?isStatus=` | Returns `{ EmpList, ProviderList, dtStatus }` via sp_SearchBill |
| `POST /Bill/Search`           | `POST /Bill/Search`           | JSON body `BillSearchRequest`; SP_ChangeBillStatus_Search |
| `POST /Bill/ChangeStatus`     | `POST /Bill/ChangeStatus?billId=` | Query param; SP_ChangeBillStatus_Update; inline UPDATE replaced |
| `POST /Bill/SearchOpenBill`   | `POST /Bill/SearchOpenBill`   | JSON body; sp_ReAssignBill_Search |
| `POST /Bill/ReAssignBill_Save`| `POST /Bill/ReAssignBill_Save`| JSON `{ BillId, Uid }`; sp_ReAssignBill_Save; inline UPDATE replaced |
| `POST /Bill/SearchCloseBill`  | `POST /Bill/SearchCloseBill`  | JSON body; **sp_SearchReimburseBill** (new SP required — inline SQL removed) |
| `POST /Bill/ReimbursingBill`  | `POST /Bill/ReimbursingBill`  | JSON `{ BillID[] }`; **sp_ReimburseBill** (new SP required — inline UPDATE removed) |

**New SPs required in DB:**
- `sp_SearchReimburseBill(@Month, @Year, @UID, @Provider)` — replaces inline SELECT from vwPendingBills_new where status=4
- `sp_ReimburseBill(@BillId)` — replaces inline UPDATE on tblBills setting status=1, reimbursement fields

To be filled in as each module is ported.

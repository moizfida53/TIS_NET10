# TIS Stored Procedure Catalog

Generated from local restore of `TIS_KDD_NEW` (source: `TIS_KDD__May.bak`).
Total user stored procedures: **143**.

**Convention:** in TIS.Data repositories, call these via `ISpRunner` only. Never inline-SQL the same logic elsewhere.

| SP | Parameters | Last Modified |
|---|---|---|
| `AllownceTypePolicy` | @AllownceID int; @IsAll int | 2023-10-08 |
| `API_ApprovalBillList` | @Username varchar | 2023-10-08 |
| `API_GetBillDetailsByBillId` | @billId int | 2023-10-08 |
| `API_GetCountofBillsByUserName` | @userName nvarchar | 2023-10-08 |
| `API_GetPendingApprovalCountByUserName` | @userName nvarchar | 2023-10-08 |
| `API_UpdateBillByBillId` | @billId int; @status int; @comments nvarchar | 2023-10-08 |
| `BAPI_ImportData` | @dtBAPIData BAPIData | 2023-10-08 |
| `BAPI_ImportData_test` | @dtBAPIData BAPIData_Test | 2023-10-08 |
| `BAPI_SyncData` | _(no params)_ | 2023-10-08 |
| `CallTypePolicy` | @ManageCallTypeID int; @IsAll int | 2023-10-08 |
| `DeleteProvider` | @Provider int | 2023-10-08 |
| `GetCallrecords` | @billid int | 2023-10-08 |
| `getdataingrid` | @Year int; @month int | 2023-10-08 |
| `getEmployee` | @Filter varchar | 2023-10-08 |
| `gettype` | _(no params)_ | 2023-10-08 |
| `PurgeDatabase` | _(no params)_ | 2026-01-17 |
| `SAP_PostZeroAmountBill` | _(no params)_ | 2023-10-08 |
| `sp_AddDataRoaming` | @Country varchar; @Operator varchar | 2023-10-08 |
| `sp_AddPackage` | @Count int; @PkgName varchar; @PkgDesc varchar; @ProviderID int; @TransID int; @DescID int; @IsAll bit; @ExpType int; @Amount float; @StartDate datetime | 2023-10-08 |
| `sp_AddPolicy` | @ProviderID int; @TransType varchar; @Description varchar; @CallTypeID int; @IsAll bit; @IsSupImp bit | 2023-10-08 |
| `sp_AddTelephone` | @SUBNO varchar; @PROVIDER int; @DESCRIPTION varchar; @ACCOUNTNO varchar; @TYPE varchar; @LINETYPE int; @ContractExpiry varchar | 2023-10-15 |
| `sp_AddUpdateContact` | @Name varchar; @Uid int; @DialledNo varchar; @ExName varchar | 2023-12-11 |
| `sp_AddUpdateGroup` | @UIDs nvarchar; @GroupName nvarchar; @GroupID int; @IsUpdated int | 2023-10-08 |
| `sp_Approve` | @xml xml; @opt int; @Uid int | 2023-10-08 |
| `sp_AssignInvoice` | _(no params)_ | 2023-10-08 |
| `sp_AssignNumber` | @UID int; @SubNoId int; @STARTDATE datetime; @ENDDATE datetime; @ALLOWANCELIMIT float; @BUSINESSLIMIT float; @LINESTATUS int; @CostCenterID int | 2026-01-13 |
| `SP_BillApprovalReminder_New` | _(no params)_ | 2023-10-08 |
| `SP_ChangeBillStatus_Search` | @Month int; @Year int; @UID int; @Status int; @Provider int | 2025-11-20 |
| `SP_ChangeBillStatus_Update` | @Bill_ID int | 2025-11-24 |
| `sp_ClearPreviousImport` | @Provider int; @BillDate datetime; @FileName varchar | 2025-11-19 |
| `sp_CloseBill` | @xml xml; @BusinessCharges float; @PersonalCharges float; @PersonalLimitCharges float; @DeductibleAmount float; @TOTALAMOUNT float; @BID int; @comment varchar; @Uid int; @Waiver float | 2026-05-19 |
| `sp_CreateException` | @Uid int; @EventName nvarchar; @EventType nvarchar; @EventMsg nvarchar; @EventSeverity nvarchar | 2025-11-19 |
| `sp_CreateSMSTemplate` | @SMSTemplateName nvarchar; @Message nvarchar; @Language int | 2023-10-08 |
| `sp_CreateTemplate` | @TemplateName nvarchar; @Subject nvarchar; @TemplateText nvarchar | 2023-10-08 |
| `sp_delegate` | @Command int; @id int; @secid int; @managerid int; @app bit; @idt bit; @sdate date; @edate date | 2023-10-08 |
| `sp_DeleteBill` | @Provider int; @BillDate datetime | 2025-11-19 |
| `sp_DeleteEmail` | @Id int | 2023-10-08 |
| `sp_DeleteEmailTemplate` | @ID int | 2023-10-08 |
| `sp_DeleteEmployee` | @UID int; @UserUid int | 2023-10-08 |
| `sp_DeleteGroup` | @GroupID int | 2023-10-08 |
| `sp_DeleteLogEmail` | @Id int | 2023-10-08 |
| `sp_DeleteLogSMS` | @ID int | 2023-10-08 |
| `sp_DeletePackage` | @ID int | 2023-10-08 |
| `sp_DeletePolicy` | @ID int | 2023-10-08 |
| `sp_DeleteSMSGroup` | @GroupID int | 2023-10-08 |
| `sp_DeleteSMSTemplate` | @ID int | 2023-10-08 |
| `sp_EmailReport` | @StartDate datetime; @EndDate datetime; @Sent int | 2023-10-08 |
| `sp_Exception` | @Exception nvarchar; @FunctionName nvarchar | 2023-10-08 |
| `sp_ForceBill` | @chkWavRtl bit; @chkwavBus bit; @chkTrain bit; @CallType int; @Status int; @UID int | 2025-11-19 |
| `sp_GetArchived` | @Uid int | 2023-10-08 |
| `sp_GetArrovalBills` | @Uid int | 2023-12-11 |
| `sp_GetBillDetails` | @Id int; @type int | 2023-12-11 |
| `sp_GetBillDetailsAppr` | @Id int; @type int | 2023-12-04 |
| `sp_GetBillReport_Details` | @Month int; @Year int; @Status int | 2026-01-19 |
| `sp_GetCC` | _(no params)_ | 2023-10-08 |
| `sp_GetCostCenter` | _(no params)_ | 2026-01-13 |
| `sp_GetDashboardChart1` | @Year int | 2023-10-08 |
| `sp_GetDashboardChart2` | @Year int; @TRANS_TYPE nvarchar | 2023-10-08 |
| `sp_GetDashboardChart3` | @Year int; @Month int | 2023-10-08 |
| `sp_GetDashboardChart4` | @Year int | 2023-10-08 |
| `sp_GetDashboardChart5` | @Year int; @calltypename nvarchar; @calltypeid int | 2023-10-08 |
| `sp_GetDashboardChart6` | @Year int; @Call_type nvarchar | 2025-04-26 |
| `sp_GetDashboardChart7` | @Year int; @OUT_COUNTRY nvarchar; @Call_type nvarchar | 2023-10-08 |
| `sp_GetDashboardData` | @Year int; @TRANS_TYPE nvarchar | 2023-10-08 |
| `sp_GetDataRoaming` | _(no params)_ | 2023-10-08 |
| `sp_getDel` | @UID int | 2023-10-08 |
| `sp_GetDelegate` | @RoleID int; @CountryID int; @Command int | 2023-10-08 |
| `sp_GetDepartmentBills` | @Uid int | 2023-10-08 |
| `sp_GetEmail` | @bid int | 2023-10-08 |
| `sp_GetEmailApprove` | _(no params)_ | 2023-10-08 |
| `sp_GetEmailPipeLine` | _(no params)_ | 2023-10-08 |
| `sp_GetEmails` | @GroupID nvarchar | 2023-10-08 |
| `sp_GetEmp` | @RoleID int; @CountryID int | 2023-10-08 |
| `sp_GetEmployee` | @Username varchar | 2026-01-20 |
| `sp_GetEmployeesForGroup` | _(no params)_ | 2023-10-08 |
| `sp_GetForceBills` | @RoleId int; @CountryId int | 2023-10-08 |
| `sp_GetGroupDetails` | @GroupID int | 2023-10-08 |
| `sp_GetGroupEmail` | @ID int | 2023-10-08 |
| `sp_GetGroups` | _(no params)_ | 2023-10-08 |
| `sp_GetGroupSMS` | @ID int | 2023-10-08 |
| `sp_GetLandingPageData` | @Uid int | 2023-10-08 |
| `sp_GetLineTypes` | _(no params)_ | 2026-01-12 |
| `sp_GetLogEmails` | _(no params)_ | 2023-10-08 |
| `sp_GetLogSMS` | _(no params)_ | 2023-10-08 |
| `sp_GetMobileNos` | @GroupID nvarchar | 2023-10-08 |
| `sp_GetNumber` | @RoleID int; @CountryID int; @Command int | 2026-05-19 |
| `sp_GetPendingEmail` | _(no params)_ | 2023-10-08 |
| `sp_GetPkgData` | _(no params)_ | 2023-10-08 |
| `sp_GetPkgDetail` | @ID int | 2023-10-08 |
| `sp_GetPolicyData` | _(no params)_ | 2023-10-08 |
| `sp_GetProvider` | @RoleID int; @CountryID int | 2023-10-08 |
| `sp_GetReminderEmail` | _(no params)_ | 2023-10-08 |
| `sp_GetReportBill` | @bid int | 2023-12-11 |
| `sp_GetReportBillArchive` | @bid int | 2026-05-19 |
| `sp_GetSalesReportFilterData` | _(no params)_ | 2023-10-15 |
| `sp_GetSAPReport` | _(no params)_ | 2026-05-19 |
| `sp_GetSendEmail` | _(no params)_ | 2026-05-19 |
| `sp_GetSettings` | @userName varchar; @UserUid int | 2023-10-15 |
| `sp_GetSMSGroupDetails` | @GroupID int | 2023-10-08 |
| `sp_GetSMSGroups` | _(no params)_ | 2023-10-08 |
| `sp_GetTelData` | @RoleID int; @CountryID int | 2026-05-19 |
| `sp_GetTemplates` | _(no params)_ | 2023-10-08 |
| `sp_GetUploadHistory` | @RoleID int; @CountryID int | 2023-10-08 |
| `sp_GetUserBills` | @Uid int | 2023-10-08 |
| `sp_GroupEmailMarkAsSent` | @ID int | 2023-10-08 |
| `sp_GroupEmailTemplate` | _(no params)_ | 2023-10-08 |
| `sp_GroupSMSTemplate` | _(no params)_ | 2023-10-08 |
| `sp_hm_closeBills` | @BusinessCharges float; @PersonalCharges float; @PersonalLimitCharges float; @DeductibleAmount float; @TOTALAMOUNT float; @BID int | 2023-10-08 |
| `sp_ImportInvoice` | @PROVIDER int; @BILLDATE datetime | 2026-05-19 |
| `sp_InsertUploadHistoryAndAudit` | @UploadFileName nvarchar; @BillDate datetime; @Provider int; @Status nvarchar; @UserID nvarchar | 2025-11-19 |
| `sp_Login` | @Username varchar | 2023-10-08 |
| `sp_LoginAs` | _(no params)_ | 2023-10-08 |
| `sp_ManageCC` | @Command int; @UID int; @NAME varchar; @EMPLOYEENO varchar | 2023-10-08 |
| `sp_ManageDelegate` | @Command int; @ID int; @secid int; @managerid int; @app bit; @idt bit | 2023-10-08 |
| `sp_MarkAsSent` | @id int | 2023-10-08 |
| `sp_ReAssignBill_Save` | @UID int; @Bill_ID int | 2025-11-19 |
| `sp_ReAssignBill_Search` | @Month int; @Year int; @UID int; @Status int; @Provider int | 2025-11-19 |
| `sp_ReportChart` | _(no params)_ | 2023-10-08 |
| `sp_SAP_MarkAsPosted` | @Username nvarchar; @Bill_ID int; @DeductibleAmount decimal | 2026-05-19 |
| `sp_Save` | @Bill_Id int; @EmailText nvarchar; @EmailTo nvarchar; @CC nvarchar | 2023-10-08 |
| `sp_SaveCloseBill` | @xml xml | 2023-10-08 |
| `sp_SaveSMSTemplate` | @SMSTemplateName nvarchar; @Message nvarchar; @SMSTemplateId int; @Language int | 2023-10-08 |
| `sp_SaveTemplate` | @TemplateName nvarchar; @Subject nvarchar; @TemplateText nvarchar; @TemplateId int | 2023-10-08 |
| `sp_SaveTemplates` | @id int; @cId int; @tId int; @text nvarchar; @EmailFrom varchar; @EmailBCC varchar | 2023-10-08 |
| `sp_SearchBill` | @IsStatus bit; @RoleID int; @CountryID int | 2023-10-08 |
| `sp_SearchBillReport` | @Month int; @Year int; @Status int; @CompanyId int | 2026-05-19 |
| `sp_SendGroupEmail` | @GroupList varchar; @Subject varchar; @TemplateID int; @Email nvarchar | 2023-10-08 |
| `sp_SetBill_ReminderNew` | _(no params)_ | 2023-10-08 |
| `sp_SetForceBill_ReminderNew` | _(no params)_ | 2023-10-08 |
| `sp_SMSAddUpdateGroup` | @SUB_NOs nvarchar; @GroupName nvarchar; @GroupID int; @IsUpdated int | 2023-10-08 |
| `sp_SMSLog` | @TemplateID int; @Message nvarchar; @SMSTo nvarchar; @IsSent int; @Language int | 2023-10-08 |
| `sp_SMSLogMarkAsSent` | @ID int; @Message nvarchar; @IsSent int; @Language int; @SMSCount int | 2023-10-08 |
| `sp_SMSReport` | @StartDate datetime; @EndDate datetime; @Sent int | 2023-10-08 |
| `sp_tblImport` | @PROVIDER int | 2023-10-08 |
| `sp_UpdateAssignNumber` | @ID int; @UID int; @SubNoId int; @STARTDATE datetime; @ENDDATE datetime; @ALLOWANCELIMIT float; @BUSINESSLIMIT float; @LINESTATUS int; @CostCenterID int | 2026-05-19 |
| `sp_UpdateDataRoaming` | @ID int; @Country varchar; @Operator varchar | 2023-10-08 |
| `sp_UpdatePackage` | @Count int; @PkgID int; @PkgName varchar; @PkgDesc varchar; @ProviderID int; @TransID int; @DescID int; @IsAll bit; @ExpType int; @Amount float; @StartDate datetime | 2023-10-08 |
| `sp_UpdateTelephone` | @ID int; @SUBNO varchar; @PROVIDER int; @DESCRIPTION varchar; @ACCOUNTNO varchar; @TYPE varchar; @LINETYPE int; @ContractExpiry varchar | 2023-10-15 |
| `sp_UploadDBSetting` | @Col1 varchar; @Col2 varchar; @Col3 varchar; @Col4 varchar; @Col5 varchar; @Col6 varchar; @Col7 varchar; @Col8 varchar; @Provider int; @dbConstr nvarchar; @dbTableName nvarchar | 2023-10-08 |
| `sp_UploadSetting` | @Col1 varchar; @Col2 varchar; @Col3 varchar; @Col4 varchar; @Col5 varchar; @Col6 varchar; @Col7 varchar; @Col8 varchar; @Provider int; @dbConstr nvarchar; @dbTableName nvarchar | 2023-10-08 |
| `sp_vwSub1Pivot` | _(no params)_ | 2026-05-19 |

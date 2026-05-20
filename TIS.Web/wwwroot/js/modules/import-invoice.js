'use strict';

// ── State ─────────────────────────────────────────────────────────────────
let importFileName  = '';   // name of file saved in Bills folder
let importBillDate  = '';   // "yyyy-MM-dd" – last day of selected month/year
let importProviderId = '';
let showDeleteBtn   = false;
let historyDt, nullRowsDt;

// ── Init ──────────────────────────────────────────────────────────────────
$(function () {
    fillYear();
    loadProviders();
    loadHistory();
    wireEvents();
    hideImportActions();
});

// ── Wire events ───────────────────────────────────────────────────────────
function wireEvents() {
    // Provider change → check if DB-based
    $('#cmbProvider').on('change', function () {
        importProviderId = $(this).val();
        if (!importProviderId) return;
        $.get('/Import/CheckProvider', { provider: importProviderId }, function (r) {
            if (r.DbBased === 'True') {
                $('#fileSection').hide();
            } else {
                $('#fileSection').show();
            }
        });
    });

    // File selected → upload to server then fill sheets
    $('#importFile').on('change', function () {
        const files = this.files;
        if (!files || !files.length) return;
        const formData = new FormData();
        formData.append('fileToUpload', files[0]);
        showLoader('Uploading file...');
        $.ajax({
            url: '/Import/Upload', type: 'POST',
            data: formData, processData: false, contentType: false,
            success: function () {
                hideLoader();
                importFileName = files[0].name;
                $('#lblFileName').text(importFileName);
                fillSheets('#cmbSheet', importFileName);
            },
            error: function () { hideLoader(); toastError('File upload failed.'); }
        });
    });

    // Submit → import
    $('#btnSubmit').on('click', submitImport);

    // Save null-row edits
    $('#btnSave').on('click', saveNullRowEdits);

    // Process bill
    $('#btnProcess').on('click', processBill);

    // Edit null row modal save
    $('#btnSaveRow').on('click', saveRowEdit);

    // Mapping tab events
    $('#cmbType').on('change', onTypeChange);
    $('#btnPrevSetting').on('click', loadPrevSetting);
    $('#btnResetMapping').on('click', resetMapping);
    $('#mappingFile').on('change', onMappingFileChange);
    $('#cmbSheet2').on('change', loadMappingColumns);
    $('#btnLoadCols').on('click', loadMappingColumns);
    $('#btnUpdateExcel').on('click', updateExcelSetting);
    $('#btnUpdateDb').on('click', updateDbSetting);
    $('#btnTestConn').on('click', testConnection);
}

// ── Year dropdown ─────────────────────────────────────────────────────────
function fillYear() {
    const $s = $('#cmbYear').empty().append('<option value="">Select Year</option>');
    const now = new Date().getFullYear();
    for (let y = now - 2; y <= now + 3; y++) {
        $s.append(`<option value="${y}">${y}</option>`);
    }
    $s.val(now);
}

// ── Providers ─────────────────────────────────────────────────────────────
function loadProviders() {
    $.get('/Setting/GetProviders', function (r) {
        const providers = r.providers || r.Providers || [];
        ['#cmbProvider', '#cmbProvider2'].forEach(id => {
            const $s = $(id).empty().append('<option value="">Select Provider</option>');
            providers.forEach(p => $s.append(`<option value="${p.id || p.ID}">${p.name || p.Name}</option>`));
        });
    });
}

// ── Upload history DataTable ──────────────────────────────────────────────
function loadHistory() {
    $.get('/Import/GetUploadHistory', function (r) {
        showDeleteBtn = r.IsDeleteButShow === 1;
        if (showDeleteBtn) $('#colDelete').show();

        const rows = (r.UploadList || []).map(u => [
            u.fileName,
            formatDate(u.billDate),
            formatDate(u.uploadDate),
            u.providerName,
            u.billAmount?.toFixed(3),
            showDeleteBtn ? `<button class="btn btn-danger btn-sm btn-del-history"
                data-date="${u.billDate}" data-provider="${u.providerId}">Delete</button>` : ''
        ]);

        if (historyDt) {
            historyDt.clear().rows.add(rows).draw();
        } else {
            historyDt = $('#tblHistory').DataTable({
                data: rows,
                columns: [
                    { title: 'File Name' },
                    { title: 'Bill Date' },
                    { title: 'Upload Date' },
                    { title: 'Provider' },
                    { title: 'Bill Amount', className: 'text-end' },
                    { title: '', visible: showDeleteBtn, orderable: false }
                ],
                pageLength: 10,
                order: [[1, 'desc']]
            });
        }
    });
}

// Delete bill from history (delegated)
$(document).on('click', '.btn-del-history', function () {
    const btn = $(this);
    const billDate = btn.data('date');
    const providerId = parseInt(btn.data('provider'));
    Swal.fire({
        title: 'Delete bill?', icon: 'warning',
        showCancelButton: true, confirmButtonColor: '#dc3545',
        confirmButtonText: 'Delete'
    }).then(res => {
        if (!res.isConfirmed) return;
        $.ajax({
            url: '/Import/DeleteBill', type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ ProviderID: providerId, BillDate: billDate }),
            success: r => {
                if (r.myMessage === 'succ') { toastSuccess('Bill deleted.'); loadHistory(); }
                else toastError('Delete failed.');
            }
        });
    });
});

// ── Submit import ─────────────────────────────────────────────────────────
function submitImport() {
    const month    = parseInt($('#cmbMonth').val());
    const year     = parseInt($('#cmbYear').val());
    const provider = $('#cmbProvider').val();
    const sheet    = $('#cmbSheet').val();

    if (!month) { toastWarn('Select a month.'); return; }
    if (!year)  { toastWarn('Select a year.');  return; }
    if (!provider) { toastWarn('Select a provider.'); return; }
    if (!$('#fileSection').is(':hidden') && !sheet) { toastWarn('Select a sheet.'); return; }
    if (!$('#fileSection').is(':hidden') && !importFileName) { toastWarn('Upload a file first.'); return; }

    // last day of month
    const d = new Date(year, month, 0);
    importBillDate = `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`;
    importProviderId = provider;

    // duplicate check against history table
    if (historyDt) {
        const data = historyDt.rows().data().toArray();
        const dup = data.some(row => {
            const billStr = row[1];   // formatted yyyy-MM-dd
            const prov    = row[3];
            return billStr && billStr.startsWith(importBillDate.substring(0, 7));
        });
        if (dup) { Swal.fire('Info', 'A bill for this month/provider already exists.', 'info'); return; }
    }

    const req = {
        FileName:   importFileName,
        SheetName:  sheet,
        Month:      month,
        Year:       year,
        ProviderID: parseInt(provider),
        DbBased:    $('#fileSection').is(':hidden')
    };

    showLoader('Importing bill data...');
    $.ajax({
        url: '/Import/UploadFile', type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(req),
        success: function (r) {
            hideLoader();
            if (r.MyMessage !== 'Success') { toastError(r.MyMessage || 'Import failed.'); return; }

            $('#lblBillAmount').text(r.BillAmount?.toFixed(3) ?? '');

            const nullRows = r.GridData || [];
            if (nullRows.length > 0) {
                fillNullRowsTable(nullRows);
                $('#importSection').show();
                $('#btnSave').show();
                $('#btnProcess').hide();
            } else {
                $('#importSection').hide();
                $('#btnSave').hide();
                $('#btnProcess').show();
            }
        },
        error: function () { hideLoader(); toastError('Import failed. Contact admin.'); }
    });
}

// ── Null rows table ───────────────────────────────────────────────────────
function fillNullRowsTable(rows) {
    const tableRows = rows.map(r => [
        r.id,
        r.subNo ?? '',
        formatDate(r.billDate),
        r.callDate ?? '',
        r.transType ?? '',
        r.description ?? '',
        r.amount ?? '',
        r.duration ?? '',
        r.callTime ?? '',
        `<button class="btn btn-sm btn-outline-primary btn-edit-row"
            data-id="${r.id}" data-subno="${r.subNo ?? ''}"
            data-amount="${r.amount ?? ''}" data-calldate="${r.callDate ?? ''}">Edit</button>`
    ]);

    if (nullRowsDt) {
        nullRowsDt.clear().rows.add(tableRows).draw();
    } else {
        nullRowsDt = $('#tblImport').DataTable({
            data: tableRows,
            columns: [
                { title: 'ID' },
                { title: 'Sub No' },
                { title: 'Bill Date' },
                { title: 'Call Date' },
                { title: 'Trans Type' },
                { title: 'Description' },
                { title: 'Amount' },
                { title: 'Duration' },
                { title: 'Call Time' },
                { title: '', orderable: false }
            ],
            pageLength: 10
        });
    }
}

// Open edit modal
$(document).on('click', '.btn-edit-row', function () {
    const btn = $(this);
    $('#editId').val(btn.data('id'));
    $('#editSubNo').val(btn.data('subno'));
    $('#editAmount').val(btn.data('amount'));
    const cd = btn.data('calldate');
    $('#editCallDate').val(cd ? cd.substring(0, 10) : '');
    new bootstrap.Modal('#modalEditRow').show();
});

// Save a single null-row edit
function saveRowEdit() {
    const req = {
        ID:       parseInt($('#editId').val()),
        Amount:   parseFloat($('#editAmount').val()) || 0,
        SubNo:    $('#editSubNo').val(),
        CallDate: $('#editCallDate').val()
    };
    $.ajax({
        url: '/Import/UpdateImport', type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(req),
        success: function (r) {
            bootstrap.Modal.getInstance('#modalEditRow')?.hide();
            if (r.Message === 'Success') {
                const remaining = r.dtImp || [];
                if (remaining.length > 0) {
                    fillNullRowsTable(remaining);
                } else {
                    $('#importSection').hide();
                    if (nullRowsDt) { nullRowsDt.destroy(); nullRowsDt = null; }
                    $('#btnSave').hide();
                    $('#btnProcess').show();
                    toastSuccess('All rows fixed. Ready to process.');
                }
            } else toastError('Save failed.');
        }
    });
}

// Bulk save (not really used with modal, but kept for compatibility)
function saveNullRowEdits() {
    toastWarn('Use the Edit button on each row to fix missing data.');
}

// ── Process bill ──────────────────────────────────────────────────────────
function processBill() {
    showLoader('Processing bill...');
    const req = {
        FileName:   importFileName,
        BillDate:   importBillDate,
        ProviderID: parseInt(importProviderId)
    };
    $.ajax({
        url: '/Import/ProcessBill', type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(req),
        success: function (r) {
            hideLoader();
            if (r.Message === 'succ') {
                showBillDetails(r.BillDetails);
                resetImportForm();
                loadHistory();
            } else {
                toastError('Process failed. Contact admin.');
            }
        },
        error: function () { hideLoader(); toastError('Process failed. Contact admin.'); }
    });
}

function showBillDetails(d) {
    const b = d?.BilledButNotInSystem         || {};
    const s = d?.InSystemButNotAssigned       || {};
    const a = d?.AssignedButOutsideValidDates || {};
    $('#bd1Count').text(b.countOfBills ?? 0);
    $('#bd1Amt').text((b.totalAmount ?? 0).toFixed(3));
    $('#bd2Count').text(s.countOfBills ?? 0);
    $('#bd2Amt').text((s.totalAmount ?? 0).toFixed(3));
    $('#bd3Count').text(a.countOfBills ?? 0);
    $('#bd3Amt').text((a.totalAmount ?? 0).toFixed(3));
    new bootstrap.Modal('#modalBillDetails').show();
}

function resetImportForm() {
    $('#cmbMonth').val('');
    $('#cmbYear').val(new Date().getFullYear());
    $('#cmbProvider').val('');
    $('#cmbSheet').empty().append('<option value="">Select Sheet</option>');
    $('#importFile').val('');
    $('#lblFileName').text('');
    $('#lblBillAmount').text('');
    importFileName = '';
    importBillDate = '';
    importProviderId = '';
    hideImportActions();
    $('#importSection').hide();
}

function hideImportActions() {
    $('#btnProcess').hide();
    $('#btnSave').hide();
}

// ── Sheet helpers ─────────────────────────────────────────────────────────
function fillSheets(selectId, fileName) {
    showLoader('Loading sheets...');
    $.get('/Import/FillSheet', { fileName }, function (r) {
        hideLoader();
        const $s = $(selectId).empty().append('<option value="">Select Sheet</option>');
        (r.dtSheet || []).forEach(sh => $s.append(`<option value="${sh.SheetName}">${sh.SheetName}</option>`));
    }).fail(() => { hideLoader(); toastError('Failed to load sheets.'); });
}

// ── Excel Mapping tab ─────────────────────────────────────────────────────
function onTypeChange() {
    const v = $(this).val();
    if (v === '2') {
        $('#excelSection').hide();
        $('#dbSection').show();
        $('#btnUpdateExcel').hide();
        $('#btnUpdateDb').show();
    } else {
        $('#excelSection').show();
        $('#dbSection').hide();
        $('#btnUpdateExcel').show();
        $('#btnUpdateDb').hide();
    }
}

function onMappingFileChange() {
    const files = this.files;
    if (!files || !files.length) return;
    const formData = new FormData();
    formData.append('fileToUpload', files[0]);
    showLoader('Uploading...');
    $.ajax({
        url: '/Import/Upload', type: 'POST',
        data: formData, processData: false, contentType: false,
        success: function () {
            hideLoader();
            const name = files[0].name;
            $('#lblMappingFile').text(name);
            fillSheets('#cmbSheet2', name);
        },
        error: function () { hideLoader(); toastError('Upload failed.'); }
    });
}

function loadMappingColumns() {
    const fileName  = $('#lblMappingFile').text();
    const sheetName = $('#cmbSheet2').val();
    if (!fileName || !sheetName) { toastWarn('Upload a file and select a sheet first.'); return; }
    $.get('/Import/UploadSetting', { fileName, sheetName }, function (r) {
        if (r.Message !== 'Success') { toastError('Failed to load columns.'); return; }
        const cols = (r.dtCol || []).map(c => c.Cols);
        bindMappingDropdowns(cols, null);
    });
}

function loadPrevSetting() {
    const provider = $('#cmbProvider2').val();
    if (!provider) { toastWarn('Select a provider first.'); return; }
    $.get('/Import/GetSetting', { provider }, function (r) {
        if (r.dtDBCol) {
            const s = r.dtDBCol;
            $('#cmbType').val('2').trigger('change');
            $('#txtConnStr').val(s.dbConstr ?? '');
            const cols = [s.col1, s.col3, s.col4, s.col5, s.col6, s.col7, s.col8, s.col9];
            bindMappingDropdowns(cols, s);
        } else if (r.dtCol) {
            const s = r.dtCol;
            $('#cmbType').val('1').trigger('change');
            const cols = [s.col1, s.col3, s.col4, s.col5, s.col6, s.col7, s.col8, s.col9];
            bindMappingDropdowns(cols, s);
        } else {
            toastWarn('No setting found for this provider.');
        }
    });
}

function bindMappingDropdowns(colOptions, savedSetting) {
    const selectors = ['#dd1', '#dd3', '#dd4', '#dd5', '#dd6', '#dd7', '#dd8', '#dd9'];
    const savedVals = savedSetting
        ? [savedSetting.col1, savedSetting.col3, savedSetting.col4,
           savedSetting.col5, savedSetting.col6, savedSetting.col7,
           savedSetting.col8, savedSetting.col9]
        : Array(8).fill(null);

    selectors.forEach((sel, i) => {
        const $dd = $(sel).empty().append('<option value="">Select Column</option>');
        colOptions.forEach(c => {
            if (c) $dd.append(`<option value="${c}">${c}</option>`);
        });
        if (savedVals[i]) $dd.val(savedVals[i]);
    });
}

function resetMapping() {
    ['#dd1', '#dd3', '#dd4', '#dd5', '#dd6', '#dd7', '#dd8', '#dd9']
        .forEach(id => $(id).empty().append('<option value="">Select Column</option>'));
    $('#cmbSheet2').empty().append('<option value="">Select Sheet</option>');
    $('#lblMappingFile').text('');
    $('#mappingFile').val('');
    $('#txtConnStr').val('');
    $('#cmbViews').empty().append('<option value="">Select View</option>');
}

function updateExcelSetting() {
    const provider = $('#cmbProvider2').val();
    if (!provider) { toastWarn('Select a provider.'); return; }
    const req = {
        Provider: parseInt(provider),
        Col1: $('#dd1').val(), Col2: '',
        Col3: $('#dd3').val(), Col4: $('#dd4').val(),
        Col5: $('#dd5').val(), Col6: $('#dd6').val(),
        Col7: $('#dd7').val(), Col8: $('#dd8').val(),
        Col9: $('#dd9').val(),
        DbConstr: '', DbTableName: ''
    };
    $.ajax({
        url: '/Import/UpdateSetting', type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(req),
        success: r => toastSuccess(r.Message || 'Settings saved.'),
        error: () => toastError('Failed to save settings.')
    });
}

function updateDbSetting() {
    const provider = $('#cmbProvider2').val();
    const connStr  = $('#txtConnStr').val();
    const view     = $('#cmbViews').val();
    if (!provider) { toastWarn('Select a provider.'); return; }
    if (!connStr)  { toastWarn('Enter a connection string.'); return; }
    if (!view)     { toastWarn('Select a view.'); return; }
    const req = {
        Provider: parseInt(provider),
        Col1: $('#dd1').val(), Col2: '',
        Col3: $('#dd3').val(), Col4: $('#dd4').val(),
        Col5: $('#dd5').val(), Col6: $('#dd6').val(),
        Col7: $('#dd7').val(), Col8: $('#dd8').val(),
        Col9: $('#dd9').val(),
        DbConstr: connStr, DbTableName: view
    };
    $.ajax({
        url: '/Import/UpdateSetting', type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(req),
        success: r => toastSuccess(r.Message || 'DB settings saved.'),
        error: () => toastError('Failed to save DB settings.')
    });
}

function testConnection() {
    const connStr = $('#txtConnStr').val();
    if (!connStr) { toastWarn('Enter a connection string.'); return; }
    // TestConn is not yet implemented server-side; just give feedback
    toastWarn('Test Connection endpoint not yet implemented.');
}

// ── Export history ────────────────────────────────────────────────────────
$('#btnExportHistory').on('click', function () {
    if (historyDt) historyDt.button('.buttons-excel')?.trigger();
    else toastWarn('No history loaded.');
});

// ── Utilities ─────────────────────────────────────────────────────────────
function formatDate(val) {
    if (!val) return '';
    const d = new Date(val);
    if (isNaN(d)) return val;
    return d.toISOString().substring(0, 10);
}

function showLoader(msg) {
    Swal.fire({ title: msg || 'Please wait...', allowOutsideClick: false, didOpen: () => Swal.showLoading() });
}
function hideLoader() { Swal.close(); }
function toastSuccess(msg) { Swal.fire({ icon: 'success', title: msg, timer: 2000, showConfirmButton: false }); }
function toastError(msg)   { Swal.fire({ icon: 'error',   title: msg }); }
function toastWarn(msg)    { Swal.fire({ icon: 'warning', title: msg }); }

'use strict';

let sapTable;
let sapData = [];

document.addEventListener('DOMContentLoaded', () => {
    initTable();
    loadReport();
    document.getElementById('chkAll').addEventListener('change', e =>
        document.querySelectorAll('#tblSapReport .row-chk').forEach(c => c.checked = e.target.checked));
});

function initTable() {
    sapTable = $('#tblSapReport').DataTable({
        columns: [
            { data: null, orderable: false, render: () => '<input type="checkbox" class="row-chk">' },
            { data: 'billDate' },
            { data: 'telephoneNumber' },
            { data: 'employeeNo' },
            { data: 'employeeName' },
            { data: 'managerName' },
            { data: 'totalAmount' },
            { data: 'businessCharges' },
            { data: 'personalCharges' },
            { data: 'deductibleAmount' },
            { data: 'costCenterName' },
            { data: 'costCenterCode' },
            { data: 'department' }
        ],
        pageLength: 15,
        order: []
    });
}

async function loadReport() {
    const res  = await fetch('/Sap/GetSapReport');
    const data = await res.json();
    sapData = data.dtbillDetails ?? [];
    document.getElementById('pendingBills').textContent = data.PendingBills ?? '0';
    sapTable.clear().rows.add(sapData).draw();
}

async function postSap() {
    showLoader('Posting to Finance…');
    const res  = await fetch('/Sap/PostSap', {
        method: 'POST',
        headers: { 'RequestVerificationToken': getAntiForgeryToken() }
    });
    hideLoader();
    const data = await res.json();
    Swal.fire({ title: data.Message, icon: 'info' });
}

async function markAsPosted() {
    const selected = sapTable.rows().nodes().toArray()
        .filter(tr => tr.querySelector('.row-chk')?.checked)
        .map(tr => sapTable.row(tr).data());

    if (!selected.length) { toastWarn('Select at least one row.'); return; }

    const ok = await Swal.fire({
        title: 'Mark selected rows as Posted?',
        icon: 'warning',
        showCancelButton: true
    });
    if (!ok.isConfirmed) return;

    showLoader('Updating…');
    await fetch('/Sap/MarkAsPosted', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json', 'RequestVerificationToken': getAntiForgeryToken() },
        body: JSON.stringify({
            value: selected.map(r => ({ billId: r.billId, deductibleAmount: r.deductibleAmount }))
        })
    });
    hideLoader();
    toastSuccess('Marked as posted.');
    document.getElementById('chkAll').checked = false;
    loadReport();
}

function exportToExcel() {
    if (!sapData.length) { toastWarn('No data to export.'); return; }

    const headers = ['Bill Date','Telephone No.','Employee No.','Employee Name','Line Manager',
                     'Total Amount','Business Charges','Personal Charges','Deductible Amount',
                     'Cost Center','Cost Center Code','Department'];
    const rows = sapData.map(r => [
        r.billDate, r.telephoneNumber, r.employeeNo, r.employeeName, r.managerName,
        r.totalAmount, r.businessCharges, r.personalCharges, r.deductibleAmount,
        r.costCenterName, r.costCenterCode, r.department
    ]);

    let csv = headers.join(',') + '\n';
    rows.forEach(r => { csv += r.map(v => `"${(v ?? '').replace(/"/g, '""')}"`).join(',') + '\n'; });

    const blob = new Blob([csv], { type: 'text/csv' });
    const a    = document.createElement('a');
    a.href     = URL.createObjectURL(blob);
    a.download = `BAPI_Report_${new Date().toISOString().slice(0,10)}.csv`;
    a.click();
    URL.revokeObjectURL(a.href);
}

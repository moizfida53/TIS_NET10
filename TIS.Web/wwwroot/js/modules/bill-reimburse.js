'use strict';

let reimburseDt = null;

document.addEventListener('DOMContentLoaded', () => {
    fillYears();
    loadSearchData();
    initTable();

    document.getElementById('btnSearch').addEventListener('click', search);
    document.getElementById('btnCancel').addEventListener('click', clearForm);
    document.getElementById('btnReimburse').addEventListener('click', reimburseSelected);
    document.getElementById('chkSelectAll').addEventListener('change', onSelectAll);
});

function fillYears() {
    const sel  = document.getElementById('cmbYear');
    sel.innerHTML = '';
    const year = new Date().getFullYear();
    for (let y = year; y >= year - 5; y--) {
        const opt = new Option(y, y);
        if (y === year) opt.selected = true;
        sel.add(opt);
    }
}

async function loadSearchData() {
    try {
        const res  = await fetch('/Bill/GetSearchData?isStatus=false');
        const data = await res.json();
        if (data.Fail) return;

        const empSel  = document.getElementById('cmbEmployee');
        const provSel = document.getElementById('cmbProvider');

        data.EmpList.forEach(e => empSel.add(new Option(`${e.empNo} — ${e.empName}`, e.empId)));
        data.ProviderList.forEach(p => provSel.add(new Option(p.name, p.id)));
    } catch {
        toastError('Failed to load search data.');
    }
}

function initTable() {
    reimburseDt = $('#tblReimburse').DataTable({
        columns: [
            {
                data: null, orderable: false, searchable: false,
                render: (_, __, row) =>
                    `<input type="checkbox" class="row-chk" data-id="${row.id}">`
            },
            { data: 'billDate',    render: d => d ? d.substring(0, 10) : '' },
            { data: 'mobile' },
            { data: 'empName' },
            { data: 'managerName' },
            { data: 'totalAmount', className: 'text-end', render: d => d.toFixed(3) },
            { data: 'statusName' }
        ],
        pageLength: 25,
        order: [[1, 'desc']]
    });

    $('#tblReimburse tbody').on('change', '.row-chk', updateBtn);
}

function onSelectAll() {
    const checked = this.checked;
    document.querySelectorAll('#tblReimburse .row-chk').forEach(cb => { cb.checked = checked; });
    updateBtn();
}

function updateBtn() {
    const count = document.querySelectorAll('#tblReimburse .row-chk:checked').length;
    const btn   = document.getElementById('btnReimburse');
    btn.disabled    = count === 0;
    btn.textContent = count > 0
        ? `Reimburse Selected (${count})`
        : 'Reimburse Selected';
}

function getSelectedIds() {
    return [...document.querySelectorAll('#tblReimburse .row-chk:checked')]
        .map(cb => parseInt(cb.dataset.id));
}

async function reimburseSelected() {
    const ids = getSelectedIds();
    if (!ids.length) return;

    const confirmed = await Swal.fire({
        title: `Reimburse ${ids.length} bill(s)?`,
        text: 'This action cannot be undone.',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#198754',
        confirmButtonText: 'Yes, reimburse'
    });
    if (!confirmed.isConfirmed) return;

    showLoader('Processing reimbursement...');
    try {
        const res  = await fetch('/Bill/ReimbursingBill', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json', 'RequestVerificationToken': getAntiForgeryToken() },
            body: JSON.stringify({ BillID: ids })
        });
        const data = await res.json();
        hideLoader();
        if (data.Message === 'Success') {
            toastSuccess('Bills reimbursed successfully.');
            document.getElementById('chkSelectAll').checked = false;
            search();
        } else {
            toastError('Reimbursement failed.');
        }
    } catch {
        hideLoader();
        toastError('Failed to reimburse bills.');
    }
}

async function search() {
    const payload = {
        Month:    parseInt(document.getElementById('cmbMonth').value),
        Year:     parseInt(document.getElementById('cmbYear').value),
        UID:      parseInt(document.getElementById('cmbEmployee').value),
        Provider: parseInt(document.getElementById('cmbProvider').value)
    };

    showLoader('Searching...');
    try {
        const res  = await fetch('/Bill/SearchCloseBill', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json', 'RequestVerificationToken': getAntiForgeryToken() },
            body: JSON.stringify(payload)
        });
        const data = await res.json();
        hideLoader();
        document.getElementById('chkSelectAll').checked = false;
        reimburseDt.clear().rows.add(data.dtData ?? []).draw();
        updateBtn();
    } catch {
        hideLoader();
        toastError('Search failed.');
    }
}

function clearForm() {
    document.getElementById('cmbMonth').value    = '0';
    document.getElementById('cmbEmployee').value = '0';
    document.getElementById('cmbProvider').value = '0';
    fillYears();
    document.getElementById('chkSelectAll').checked = false;
    reimburseDt?.clear().draw();
    updateBtn();
}

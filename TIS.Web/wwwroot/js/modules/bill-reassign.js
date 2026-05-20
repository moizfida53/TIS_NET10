'use strict';

let reassignDt  = null;
let allEmployees = [];

document.addEventListener('DOMContentLoaded', () => {
    fillYears();
    loadSearchData();
    initTable();

    document.getElementById('btnSearch').addEventListener('click', search);
    document.getElementById('btnCancel').addEventListener('click', clearForm);
    document.getElementById('txtEmpSearch').addEventListener('input', filterEmployees);
});

function fillYears() {
    const sel  = document.getElementById('cmbYear');
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

        allEmployees = data.EmpList;
        data.EmpList.forEach(e => empSel.add(new Option(`${e.empNo} — ${e.empName}`, e.empId)));
        data.ProviderList.forEach(p => provSel.add(new Option(p.name, p.id)));
    } catch {
        toastError('Failed to load search data.');
    }
}

function initTable() {
    reassignDt = $('#tblReAssign').DataTable({
        columns: [
            { data: 'billDate',    render: d => d ? d.substring(0, 10) : '' },
            { data: 'mobile' },
            { data: 'empName' },
            { data: 'managerName' },
            { data: 'totalAmount', className: 'text-end', render: d => d.toFixed(3) },
            { data: 'statusName' },
            {
                data: 'id', orderable: false, className: 'text-center',
                render: id =>
                    `<button class="btn btn-sm btn-outline-primary btn-assign" data-id="${id}" title="Re-Assign">
                        <i class="bi bi-person-fill-gear"></i> Assign
                     </button>`
            }
        ],
        pageLength: 25,
        order: [[0, 'desc']]
    });

    $('#tblReAssign tbody').on('click', '.btn-assign', function () {
        document.getElementById('hidAssignBillId').value = this.dataset.id;
        document.getElementById('txtEmpSearch').value    = '';
        renderEmployeeList(allEmployees);
        new bootstrap.Modal(document.getElementById('modalAssignEmployee')).show();
    });
}

function renderEmployeeList(emps) {
    const tbody = document.getElementById('tblEmpList');
    tbody.innerHTML = emps.map(e =>
        `<tr>
            <td>${e.empNo}</td>
            <td>${e.empName}</td>
            <td class="text-end">
                <button class="btn btn-sm btn-primary btn-pick" data-uid="${e.empId}">Select</button>
            </td>
         </tr>`
    ).join('');

    tbody.querySelectorAll('.btn-pick').forEach(btn => {
        btn.addEventListener('click', () => assignBill(parseInt(btn.dataset.uid)));
    });
}

function filterEmployees() {
    const q    = this.value.toLowerCase();
    const list = q
        ? allEmployees.filter(e =>
            e.empName.toLowerCase().includes(q) || e.empNo.toLowerCase().includes(q))
        : allEmployees;
    renderEmployeeList(list);
}

async function assignBill(uid) {
    const billId = parseInt(document.getElementById('hidAssignBillId').value);
    try {
        const res  = await fetch('/Bill/ReAssignBill_Save', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json', 'RequestVerificationToken': getAntiForgeryToken() },
            body: JSON.stringify({ BillId: billId, Uid: uid })
        });
        const data = await res.json();
        bootstrap.Modal.getInstance(document.getElementById('modalAssignEmployee'))?.hide();
        if (data.success) { toastSuccess(data.message); search(); }
        else toastError(data.message);
    } catch {
        toastError('Failed to re-assign bill.');
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
        const res  = await fetch('/Bill/SearchOpenBill', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json', 'RequestVerificationToken': getAntiForgeryToken() },
            body: JSON.stringify(payload)
        });
        const data = await res.json();
        hideLoader();
        reassignDt.clear().rows.add(data.dtData ?? []).draw();
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
    reassignDt?.clear().draw();
}

'use strict';

let changeStatusDt = null;

document.addEventListener('DOMContentLoaded', () => {
    fillYears();
    loadSearchData();
    initTable();

    document.getElementById('btnSearch').addEventListener('click', search);
    document.getElementById('btnCancel').addEventListener('click', clearForm);
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
        const res  = await fetch('/Bill/GetSearchData?isStatus=true');
        const data = await res.json();
        if (data.Fail) return;

        const empSel    = document.getElementById('cmbEmployee');
        const provSel   = document.getElementById('cmbProvider');
        const statusSel = document.getElementById('cmbStatus');

        data.EmpList.forEach(e => empSel.add(new Option(`${e.empNo} — ${e.empName}`, e.empId)));
        data.ProviderList.forEach(p => provSel.add(new Option(p.name, p.id)));
        data.dtStatus.forEach(s => statusSel.add(new Option(s.name, s.id)));
    } catch {
        toastError('Failed to load search data.');
    }
}

function initTable() {
    changeStatusDt = $('#tblChangeStatus').DataTable({
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
                    `<button class="btn btn-sm btn-outline-warning btn-change" data-id="${id}" title="Change to Open">
                        <i class="bi bi-arrow-counterclockwise"></i> To Open
                     </button>`
            }
        ],
        pageLength: 25,
        order: [[0, 'desc']]
    });

    $('#tblChangeStatus tbody').on('click', '.btn-change', async function () {
        const id = parseInt(this.dataset.id);
        const confirmed = await Swal.fire({
            title: 'Change Status?',
            text: 'This will change the bill status back to Open.',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: 'Yes, change it'
        });
        if (!confirmed.isConfirmed) return;

        try {
            const res  = await fetch(`/Bill/ChangeStatus?billId=${id}`, {
                method: 'POST',
                headers: { 'RequestVerificationToken': getAntiForgeryToken() }
            });
            const data = await res.json();
            if (data.success) { toastSuccess(data.message); search(); }
            else toastError(data.message);
        } catch {
            toastError('Failed to change status.');
        }
    });
}

async function search() {
    const payload = {
        Month:    parseInt(document.getElementById('cmbMonth').value),
        Year:     parseInt(document.getElementById('cmbYear').value),
        UID:      parseInt(document.getElementById('cmbEmployee').value),
        Status:   parseInt(document.getElementById('cmbStatus').value),
        Provider: parseInt(document.getElementById('cmbProvider').value)
    };

    showLoader('Searching...');
    try {
        const res  = await fetch('/Bill/Search', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json', 'RequestVerificationToken': getAntiForgeryToken() },
            body: JSON.stringify(payload)
        });
        const data = await res.json();
        hideLoader();
        changeStatusDt.clear().rows.add(data.dtData ?? []).draw();
    } catch {
        hideLoader();
        toastError('Search failed.');
    }
}

function clearForm() {
    document.getElementById('cmbMonth').value    = '0';
    document.getElementById('cmbEmployee').value = '0';
    document.getElementById('cmbProvider').value = '0';
    document.getElementById('cmbStatus').value   = '0';
    fillYears();
    changeStatusDt?.clear().draw();
}

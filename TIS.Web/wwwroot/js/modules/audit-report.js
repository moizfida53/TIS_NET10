'use strict';

let auditTable;
let detailsTable;

document.addEventListener('DOMContentLoaded', () => {
    initTable();
    initDetailsTable();
    loadUsers();
    initFlatpickr();
});

function initFlatpickr() {
    document.querySelectorAll('.flatpickr').forEach(el =>
        flatpickr(el, { enableTime: true, dateFormat: 'Y-m-d H:i:S', time_24hr: true })
    );
}

function initTable() {
    auditTable = $('#tblAuditReport').DataTable({
        columns: [
            { data: 'id' },
            { data: 'actionName' },
            { data: 'result' },
            { data: 'user' },
            { data: 'userId' },
            { data: 'date' },
            { data: 'formId' },
            {
                data: null, orderable: false,
                render: (_, __, row) =>
                    `<button class="btn btn-outline-primary btn-sm" onclick="viewDetails(${row.id})">View Details</button>`
            }
        ],
        pageLength: 15,
        order: []
    });
}

function initDetailsTable() {
    detailsTable = $('#tblAuditDetails').DataTable({
        columns: [
            { data: 'id' },
            { data: 'sno' },
            { data: 'atId' },
            { data: 'fieldName' },
            { data: 'oldValue' },
            { data: 'newValue' }
        ],
        pageLength: 10,
        order: [],
        destroy: true
    });
}

async function loadUsers() {
    const res  = await fetch('/AuditReport/GetEmp');
    const data = await res.json();
    const sel  = document.getElementById('cmbUser');
    (data.EmpList ?? []).forEach(e => {
        const opt = document.createElement('option');
        opt.value       = e.uid;
        opt.textContent = `${e.userName} - ${e.empNo}`;
        sel.appendChild(opt);
    });
    if (typeof $ !== 'undefined' && $.fn.select2) {
        $(sel).select2({ placeholder: '-- Select User --', allowClear: true });
    }
}

async function search() {
    const startDate = document.getElementById('startDate').value;
    const endDate   = document.getElementById('endDate').value;
    const event_    = document.getElementById('cmbEvents').value;
    const uid       = document.getElementById('cmbUser').value;
    const status    = document.getElementById('cmbStatus').value;

    if (!startDate || !endDate) { toastWarn('Please select start and end date.'); return; }
    if (new Date(startDate) > new Date(endDate)) { toastWarn('Start date cannot be after end date.'); return; }
    if (!uid)     { toastWarn('Please select a user.'); return; }
    if (!event_)  { toastWarn('Please select an event type.'); return; }

    const params = new URLSearchParams({ startDate, endDate, event: event_, uid, status });
    const res    = await fetch(`/AuditReport/Search?${params}`);
    const data   = await res.json();
    auditTable.clear().rows.add(data.dtAuditReport ?? []).draw();
}

async function viewDetails(id) {
    const res  = await fetch(`/AuditReport/Details?id=${id}`);
    const data = await res.json();

    if (detailsTable) detailsTable.destroy();
    detailsTable = $('#tblAuditDetails').DataTable({
        data: data.dtDetails ?? [],
        columns: [
            { data: 'id' },
            { data: 'sno' },
            { data: 'atId' },
            { data: 'fieldName' },
            { data: 'oldValue' },
            { data: 'newValue' }
        ],
        pageLength: 10,
        order: [],
        destroy: true
    });

    new bootstrap.Modal(document.getElementById('modalAuditDetails')).show();
}

function clearSearch() {
    document.getElementById('startDate')._flatpickr?.clear();
    document.getElementById('endDate')._flatpickr?.clear();
    document.getElementById('cmbEvents').value = '';
    document.getElementById('cmbStatus').value = '';
    if (typeof $ !== 'undefined' && $.fn.select2) {
        $('#cmbUser').val(null).trigger('change');
    }
    auditTable.clear().draw();
}

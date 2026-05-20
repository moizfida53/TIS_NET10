'use strict';

let forceBillDt = null;

document.addEventListener('DOMContentLoaded', () => {
    initTable();
    loadBills();

    document.getElementById('chkSelectAll').addEventListener('change', onSelectAll);
    document.getElementById('btnForceBillOpen').addEventListener('click', openForceModal);
    document.getElementById('btnForceBill').addEventListener('click', forceBill);
    document.getElementById('btnExport').addEventListener('click', () => forceBillDt?.button('.buttons-excel').trigger());
});

function initTable() {
    forceBillDt = $('#tblForceBill').DataTable({
        columns: [
            {
                data: null, orderable: false, searchable: false,
                render: (_, __, row) =>
                    `<input type="checkbox" class="row-chk" data-id="${row.id}">`
            },
            { data: 'billDate',    render: d => d ? d.substring(0, 10) : '' },
            { data: 'mobile' },
            { data: 'providerName' },
            { data: 'empName' },
            { data: 'managerName' },
            { data: 'department' },
            { data: 'totalAmount', className: 'text-end', render: d => d.toFixed(3) }
        ],
        dom: 'Bfrtip',
        buttons: [{ extend: 'excel', className: 'd-none' }],
        pageLength: 25,
        order: [[1, 'desc']]
    });

    $('#tblForceBill tbody').on('change', '.row-chk', updateForceBtnState);
}

async function loadBills() {
    try {
        const res = await fetch('/Bill/GetForceBill');
        const data = await res.json();
        if (data.Fail) { toastError('Failed to load bills.'); return; }
        forceBillDt.clear().rows.add(data.Bills).draw();
    } catch {
        toastError('Error loading bills.');
    }
}

function onSelectAll() {
    const checked = this.checked;
    document.querySelectorAll('#tblForceBill .row-chk').forEach(cb => { cb.checked = checked; });
    updateForceBtnState();
}

function updateForceBtnState() {
    const anyChecked = document.querySelectorAll('#tblForceBill .row-chk:checked').length > 0;
    document.getElementById('btnForceBillOpen').disabled = !anyChecked;
}

function openForceModal() {
    if (!getSelectedIds().length) { toastWarn('Select at least one bill.'); return; }
    new bootstrap.Modal(document.getElementById('modalForceBill')).show();
}

function getSelectedIds() {
    return [...document.querySelectorAll('#tblForceBill .row-chk:checked')]
        .map(cb => parseInt(cb.dataset.id));
}

async function forceBill() {
    const ids = getSelectedIds();
    if (!ids.length) { toastWarn('No bills selected.'); return; }

    const payload = {
        BillID:      ids,
        Status:      parseInt(document.getElementById('cmbStatus').value),
        CallType:    parseInt(document.getElementById('cmbCallType').value),
        WavRental:   document.getElementById('chkWavRtl').checked,
        WavBusiness: document.getElementById('chkWavBus').checked,
        Train:       document.getElementById('chkTrain').checked
    };

    showLoader('Processing bills...');
    try {
        const res  = await fetch('/Bill/ForceBill', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json', 'RequestVerificationToken': getAntiForgeryToken() },
            body: JSON.stringify(payload)
        });
        const data = await res.json();
        hideLoader();
        bootstrap.Modal.getInstance(document.getElementById('modalForceBill'))?.hide();

        if (data.Success) {
            toastSuccess(data.Message);
            loadBills();
            document.getElementById('chkSelectAll').checked = false;
        } else {
            toastError(data.Message);
        }
    } catch {
        hideLoader();
        toastError('Failed to process bills.');
    }
}

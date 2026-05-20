'use strict';

let unassignedDt;

$(function () {
    loadUnassigned();
    $('#btnAssign').on('click', assignInvoice);
    $('#btnExport').on('click', exportExcel);
});

function loadUnassigned() {
    $.get('/Import/GetUnAssignedBill', function (r) {
        const rows = (r.Bills || []).map(b => [
            formatDate(b.billDate),
            b.mobile,
            b.providerName,
            b.totalAmount?.toFixed(3)
        ]);

        if (unassignedDt) {
            unassignedDt.clear().rows.add(rows).draw();
        } else {
            unassignedDt = $('#tblUnassigned').DataTable({
                data: rows,
                columns: [
                    { title: 'Bill Date' },
                    { title: 'Mobile' },
                    { title: 'Provider' },
                    { title: 'Total Amount', className: 'text-end' }
                ],
                pageLength: 25,
                order: [[0, 'desc']]
            });
        }
    }).fail(() => Swal.fire({ icon: 'error', title: 'Failed to load unassigned bills.' }));
}

function assignInvoice() {
    Swal.fire({
        title: 'Assign all records?',
        text: 'This will assign all unassigned bills to employees.',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Assign'
    }).then(res => {
        if (!res.isConfirmed) return;
        $.ajax({
            url: '/Import/AssignInvoice', type: 'POST',
            success: function (r) {
                Swal.fire({ icon: 'success', title: r.Message || 'Done' });
                loadUnassigned();
            },
            error: () => Swal.fire({ icon: 'error', title: 'Assignment failed.' })
        });
    });
}

function exportExcel() {
    if (!unassignedDt) return;
    // Use DataTables Buttons if available, otherwise fallback
    const btn = unassignedDt.button('.buttons-excel');
    if (btn) btn.trigger();
    else Swal.fire({ icon: 'info', title: 'Export not configured.' });
}

function formatDate(val) {
    if (!val) return '';
    const d = new Date(val);
    if (isNaN(d)) return val;
    return d.toISOString().substring(0, 10);
}

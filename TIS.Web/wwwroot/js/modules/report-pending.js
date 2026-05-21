'use strict';

let reportDt = null;

document.addEventListener('DOMContentLoaded', () => {
    fillYears();
    loadFilters();
    initTable();

    document.getElementById('btnSearch').addEventListener('click', search);
    document.getElementById('btnCancel').addEventListener('click', clearForm);
    document.getElementById('btnExport').addEventListener('click', () => reportDt?.button('.buttons-excel').trigger());
});

function fillYears() {
    const sel  = document.getElementById('cmbYear');
    sel.innerHTML = '';
    const year = new Date().getFullYear();
    for (let y = year; y >= year - 10; y--) {
        const opt = new Option(y, y);
        if (y === year) opt.selected = true;
        sel.add(opt);
    }
}

async function loadFilters() {
    try {
        const res  = await fetch('/BillReport/GetReportFilters');
        const data = await res.json();
        if (data.Fail) return;

        const provSel = document.getElementById('cmbProvider');
        (data.ProviderList || []).forEach(p => provSel.add(new Option(p.name, p.id)));
    } catch {
        toastError('Failed to load providers.');
    }
}

function initTable() {
    reportDt = $('#tblReport').DataTable({
        columns: [
            { data: 'empName' },
            { data: 'mobile' },
            { data: 'billDate',    render: d => d ? d.substring(0, 10) : '' },
            { data: 'totalAmount', className: 'text-end', render: d => (+d).toFixed(3) },
            { data: 'lmEmail' }
        ],
        dom: 'Bfrtip',
        buttons: [{ extend: 'excel', className: 'd-none', title: 'Pending Bills Report' }],
        pageLength: 25,
        order: [[2, 'desc']]
    });
}

async function search() {
    const payload = {
        Month:    parseInt(document.getElementById('cmbMonth').value),
        Year:     parseInt(document.getElementById('cmbYear').value),
        Provider: parseInt(document.getElementById('cmbProvider').value),
        Status:   parseInt(document.getElementById('cmbStatus').value)
    };

    showLoader('Searching...');
    try {
        const res  = await fetch('/BillReport/SearchPendingBills', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json', 'RequestVerificationToken': getAntiForgeryToken() },
            body: JSON.stringify(payload)
        });
        const data = await res.json();
        hideLoader();
        reportDt.clear().rows.add(data.dtData ?? []).draw();
    } catch {
        hideLoader();
        toastError('Search failed.');
    }
}

function clearForm() {
    document.getElementById('cmbMonth').value    = '0';
    document.getElementById('cmbProvider').value = '0';
    document.getElementById('cmbStatus').value   = '0';
    fillYears();
    reportDt?.clear().draw();
}

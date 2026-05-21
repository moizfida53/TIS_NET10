'use strict';

let billReportDt = null;

document.addEventListener('DOMContentLoaded', () => {
    fillYears();
    loadFilters();
    initTable();

    document.getElementById('btnSearch').addEventListener('click', search);
    document.getElementById('btnCancel').addEventListener('click', clearForm);
    document.getElementById('btnExport').addEventListener('click', () => billReportDt?.button('.buttons-excel').trigger());
});

function fillYears() {
    const sel  = document.getElementById('cmbYear');
    const year = new Date().getFullYear();
    for (let y = year; y >= year - 10; y--) {
        const opt = new Option(y, y);
        if (y === year) opt.selected = true;
        sel.add(opt);
    }
}

async function loadFilters() {
    try {
        const res  = await fetch('/BillReport/GetBillReportFilters');
        const data = await res.json();
        if (data.Fail) return;

        const statusSel  = document.getElementById('cmbStatus');
        const companySel = document.getElementById('cmbCompany');

        (data.dtStatus  || []).forEach(s => statusSel.add(new Option(s.name, s.id)));
        (data.CompanyList || []).forEach(c => companySel.add(new Option(c.name, c.id)));
    } catch {
        toastError('Failed to load filters.');
    }
}

function initTable() {
    billReportDt = $('#tblBillReport').DataTable({
        scrollX: true,
        columns: [
            { data: 'empNo' },
            { data: 'empName' },
            { data: 'mobile' },
            { data: 'mobileDesc' },
            { data: 'billDate',    render: d => d ? d.substring(0, 10) : '' },
            { data: 'totalAmount',          className: 'text-end', render: d => (+d).toFixed(3) },
            { data: 'businessCharges',      className: 'text-end', render: d => (+d).toFixed(3) },
            { data: 'personalCharges',      className: 'text-end', render: d => (+d).toFixed(3) },
            { data: 'personalLimitCharges', className: 'text-end', render: d => (+d).toFixed(3) },
            { data: 'deductibleAmount',     className: 'text-end', render: d => (+d).toFixed(3) },
            { data: 'costCenter' },
            { data: 'costCenterCode' },
            { data: 'department' },
            { data: 'payrollCategory' },
            { data: 'status' },
            { data: 'company' },
            { data: 'providerName' },
            { data: 'forcedBy' },
            { data: 'forcedDate', render: d => d ? d.substring(0, 10) : '' }
        ],
        dom: 'Bfrtip',
        buttons: [{ extend: 'excel', className: 'd-none', title: 'Bill Report' }],
        pageLength: 50,
        order: [[4, 'desc']]
    });
}

async function search() {
    const payload = {
        Month:     parseInt(document.getElementById('cmbMonth').value),
        Year:      parseInt(document.getElementById('cmbYear').value),
        Status:    parseInt(document.getElementById('cmbStatus').value),
        CompanyId: parseInt(document.getElementById('cmbCompany').value)
    };

    showLoader('Searching...');
    try {
        const res  = await fetch('/BillReport/SearchBillReport', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json', 'RequestVerificationToken': getAntiForgeryToken() },
            body: JSON.stringify(payload)
        });
        const data = await res.json();
        hideLoader();
        billReportDt.clear().rows.add(data.dtData ?? []).draw();
    } catch {
        hideLoader();
        toastError('Search failed.');
    }
}

function clearForm() {
    document.getElementById('cmbMonth').value   = '0';
    document.getElementById('cmbStatus').value  = '0';
    document.getElementById('cmbCompany').value = '0';
    fillYears();
    billReportDt?.clear().draw();
}

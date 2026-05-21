'use strict';

let sendEmailTable;

document.addEventListener('DOMContentLoaded', () => {
    initTable();
    loadEmails();
});

function initTable() {
    sendEmailTable = $('#tblSendEmail').DataTable({
        columns: [
            { data: null, orderable: false, render: () => '<input type="checkbox" class="row-chk">' },
            { data: 'billDate' },
            { data: 'subject' },
            { data: 'emailFrom' },
            { data: 'emailTo' },
            { data: 'cc' },
            { data: 'sent', render: v => v ? '<span class="badge bg-success">Sent</span>' : '' },
            {
                data: null, orderable: false,
                render: (_, __, row) =>
                    `<button class="btn btn-outline-secondary btn-sm" onclick="openEdit(${row.billId},'${esc(row.emailText)}','${esc(row.emailTo)}','${esc(row.cc)}')">Edit</button>`
            }
        ],
        pageLength: 15, order: []
    });

    document.getElementById('chkAllBill').addEventListener('change', e =>
        document.querySelectorAll('#tblSendEmail .row-chk').forEach(c => c.checked = e.target.checked));
}

async function loadEmails() {
    const res  = await fetch('/EmailSms/GetEmail');
    const data = await res.json();
    sendEmailTable.clear().rows.add(data.dtSendEmail ?? []).draw();
}

function getSelectedIds() {
    return sendEmailTable.rows().nodes().toArray()
        .filter(tr => tr.querySelector('.row-chk')?.checked)
        .map(tr => sendEmailTable.row(tr).data().id);
}

async function sendSelected() {
    const ids = getSelectedIds();
    if (!ids.length) { toastWarn('Select at least one email.'); return; }
    showLoader('Sending…');
    const res  = await fetch('/EmailSms/SendBillEmail', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json', 'RequestVerificationToken': getAntiForgeryToken() },
        body: JSON.stringify({ bID: ids })
    });
    hideLoader();
    const data = await res.json();
    if (data.Message === 'Email Sent') { toastSuccess('Emails sent.'); loadEmails(); }
    else toastError('Send failed.');
}

async function deleteSelected() {
    const ids = getSelectedIds();
    if (!ids.length) { toastWarn('Select at least one row.'); return; }
    const ok = await Swal.fire({ title: 'Delete selected emails?', icon: 'warning', showCancelButton: true });
    if (!ok.isConfirmed) return;
    await fetch('/EmailSms/DeleteBillEmail', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json', 'RequestVerificationToken': getAntiForgeryToken() },
        body: JSON.stringify({ emailID: ids })
    });
    toastSuccess('Deleted.');
    loadEmails();
}

function openEdit(billId, emailText, emailTo, cc) {
    document.getElementById('editBillId').value    = billId;
    document.getElementById('editEmailText').value = emailText;
    document.getElementById('editEmailTo').value   = emailTo;
    document.getElementById('editCC').value        = cc;
    new bootstrap.Modal(document.getElementById('modalEditEmail')).show();
}

async function saveEdit() {
    const billId    = +document.getElementById('editBillId').value;
    const emailText = document.getElementById('editEmailText').value;
    const emailTo   = document.getElementById('editEmailTo').value;
    const cc        = document.getElementById('editCC').value;
    await fetch('/EmailSms/SaveBillEmail', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json', 'RequestVerificationToken': getAntiForgeryToken() },
        body: JSON.stringify({ billId, emailText, emailTo, cc })
    });
    bootstrap.Modal.getInstance(document.getElementById('modalEditEmail'))?.hide();
    toastSuccess('Saved.');
    loadEmails();
}

async function setReminder() {
    showLoader('Setting reminders…');
    await fetch('/EmailSms/SetReminder', {
        method: 'POST',
        headers: { 'RequestVerificationToken': getAntiForgeryToken() }
    });
    hideLoader();
    toastSuccess('Reminders set.');
    loadEmails();
}

async function setForceBillReminder() {
    showLoader('Setting force bill reminders…');
    await fetch('/EmailSms/SetForceBillReminder', {
        method: 'POST',
        headers: { 'RequestVerificationToken': getAntiForgeryToken() }
    });
    hideLoader();
    toastSuccess('Force bill reminders set.');
    loadEmails();
}

function esc(s) { return (s ?? '').replace(/'/g, "\\'"); }

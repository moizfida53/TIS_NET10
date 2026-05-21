'use strict';

let emailGroupsTable, emailEmployeesTable, emailGroupsListTable;
let smsGroupsTable,   smsEmployeesTable,   smsGroupsListTable;
let emailLogTable, smsLogTable;
let selectedEmailLanguage = 1;
const SMS_MAX = { 1: 160, 2: 70 };

document.addEventListener('DOMContentLoaded', () => {
    initEmailLogTable();
    loadEmailGroups();
    loadEmailTemplates();
    loadEmailLog();

    // SMS tab: load on first open
    document.querySelector('[data-bs-target="#tabSms"]').addEventListener('shown.bs.tab', () => {
        if (!smsLogTable) {
            initSmsLogTable();
            loadSmsGroups();
            loadSmsTemplates();
            loadSmsLog();
        }
    });

    // Email group modal: load employees + groups on first open
    document.getElementById('modalEmailGroup').addEventListener('show.bs.modal', () => {
        if (!emailEmployeesTable) initEmailGroupModal();
    });

    // SMS group modal: load on first open
    document.getElementById('modalSmsGroup').addEventListener('show.bs.modal', () => {
        if (!smsEmployeesTable) initSmsGroupModal();
    });

    // Group selection → fetch emails
    document.getElementById('selEmailGroups').addEventListener('change', onEmailGroupChange);
    document.getElementById('selSmsGroups').addEventListener('change', onSmsGroupChange);

    // Template selection → fill form
    document.getElementById('selTemplate').addEventListener('change', onTemplateChange);
    document.getElementById('selSmsTemplate').addEventListener('change', onSmsTemplateChange);

    // SMS counter
    document.getElementById('txtSMSTemplate').addEventListener('input', updateSmsCounter);

    // Select-all checkboxes
    document.getElementById('chkAllEmail').addEventListener('change', e =>
        document.querySelectorAll('#tblEmailLog .row-chk').forEach(c => c.checked = e.target.checked));
    document.getElementById('chkAllSms').addEventListener('change', e =>
        document.querySelectorAll('#tblSmsLog .row-chk').forEach(c => c.checked = e.target.checked));
});

// ── Email groups ───────────────────────────────────────────────────────────
async function loadEmailGroups() {
    const res  = await fetch('/EmailSms/GetGroups');
    const data = await res.json();
    const sel  = document.getElementById('selEmailGroups');
    sel.innerHTML = (data.dtGroups ?? []).map(g =>
        `<option value="${g.groupId}">${g.groupName}</option>`).join('');
}

async function onEmailGroupChange() {
    const ids = [...document.getElementById('selEmailGroups').selectedOptions].map(o => o.value).join(',');
    if (!ids) { document.getElementById('txtEmails').value = ''; return; }
    const res  = await fetch('/EmailSms/GetEmails', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json', 'RequestVerificationToken': getAntiForgeryToken() },
        body: JSON.stringify({ groupIDs: ids })
    });
    const data = await res.json();
    document.getElementById('txtEmails').value =
        (data.dtEmail ?? []).map(e => e.Emails).filter(Boolean).join(',');
}

function addEmail() {
    const addr  = document.getElementById('txtEmailTo').value.trim();
    if (!addr) return;
    const area  = document.getElementById('txtEmails');
    const curr  = area.value.replace(/^,/, '');
    area.value  = curr ? curr + ',' + addr : addr;
    document.getElementById('txtEmailTo').value = '';
}

// ── Email templates ────────────────────────────────────────────────────────
async function loadEmailTemplates() {
    const res  = await fetch('/EmailSms/GetTemplate');
    const data = await res.json();
    const sel  = document.getElementById('selTemplate');
    sel.innerHTML = '<option value="">— Select Template —</option>' +
        (data.dtTemplate ?? []).map(t =>
            `<option value="${t.templateId}" data-subject="${esc(t.subject)}" data-text="${esc(t.templateText)}">${t.templateName}</option>`
        ).join('');
}

function onTemplateChange() {
    const opt = document.getElementById('selTemplate').selectedOptions[0];
    if (!opt || !opt.value) return;
    document.getElementById('hidTemplateID').value    = opt.value;
    document.getElementById('txtSubject').value       = opt.dataset.subject  ?? '';
    document.getElementById('txtTemplate').value      = opt.dataset.text     ?? '';
    document.getElementById('txtNewTemplateName').value = opt.textContent;
}

async function createTemplate() {
    const name = document.getElementById('txtNewTemplateName').value.trim();
    const subj = document.getElementById('txtSubject').value.trim();
    const text = document.getElementById('txtTemplate').value.trim();
    if (!name || !subj || !text) { toastWarn('Fill Template Name, Subject, and Body.'); return; }
    await postJson('/EmailSms/CreateTemplate', { templateName: name, subject: subj, templateText: text });
    toastSuccess('Template created.');
    loadEmailTemplates();
}

async function saveTemplate() {
    const id   = document.getElementById('hidTemplateID').value;
    const name = document.getElementById('txtNewTemplateName').value.trim();
    const subj = document.getElementById('txtSubject').value.trim();
    const text = document.getElementById('txtTemplate').value.trim();
    if (!id) { toastWarn('Select a template first.'); return; }
    await postJson('/EmailSms/SaveTemplate', { templateId: +id, templateName: name, subject: subj, templateText: text });
    toastSuccess('Template saved.');
    loadEmailTemplates();
}

// ── Send email ──────────────────────────────────────────────────────────────
async function sendGroupEmail() {
    const groups  = [...document.getElementById('selEmailGroups').selectedOptions].map(o => o.value).join(',');
    const subject = document.getElementById('txtSubject').value.trim();
    const tmplId  = +document.getElementById('hidTemplateID').value || 0;
    const emails  = document.getElementById('txtEmails').value.trim();
    if (!emails) { toastWarn('No recipients selected.'); return; }

    showLoader('Sending emails…');
    const res  = await fetch('/EmailSms/SendGroupEmail', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json', 'RequestVerificationToken': getAntiForgeryToken() },
        body: JSON.stringify({ checkedGroupList: groups, subject, templateID: tmplId, emails })
    });
    hideLoader();
    const data = await res.json();
    if (data.Message === 'Success') { toastSuccess('Emails sent.'); loadEmailLog(); }
    else toastError('Send failed.');
}

// ── Email log ───────────────────────────────────────────────────────────────
function initEmailLogTable() {
    emailLogTable = $('#tblEmailLog').DataTable({
        columns: [
            { data: null, orderable: false, render: () => '<input type="checkbox" class="row-chk">' },
            { data: 'templateName' },
            { data: 'subject' },
            { data: 'emailFrom' },
            { data: 'emailTo' },
            { data: 'isSent', render: v => v ? '✓' : '' },
            { data: 'sentOn' }
        ],
        pageLength: 10, order: []
    });
}

async function loadEmailLog() {
    const res  = await fetch('/EmailSms/GetLogEmails');
    const data = await res.json();
    emailLogTable.clear().rows.add(data.dtSendEmail ?? []).draw();
}

function getCheckedEmailIds() {
    return emailLogTable.rows().nodes().toArray()
        .filter(tr => tr.querySelector('.row-chk')?.checked)
        .map(tr => emailLogTable.row(tr).data().id);
}

async function resendEmails() {
    const ids = getCheckedEmailIds();
    if (!ids.length) { toastWarn('Select at least one email.'); return; }
    showLoader('Sending…');
    const res  = await fetch('/EmailSms/Send', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json', 'RequestVerificationToken': getAntiForgeryToken() },
        body: JSON.stringify({ emailID: ids })
    });
    hideLoader();
    const data = await res.json();
    if (data.Message === 'Email Sent') { toastSuccess('Emails sent.'); loadEmailLog(); }
    else toastError('Send failed.');
}

async function deleteEmails() {
    const ids = getCheckedEmailIds();
    if (!ids.length) { toastWarn('Select at least one email.'); return; }
    const ok = await Swal.fire({ title: 'Delete?', icon: 'warning', showCancelButton: true });
    if (!ok.isConfirmed) return;
    await postJson('/EmailSms/DeleteEmail', { emailID: ids });
    toastSuccess('Deleted.');
    loadEmailLog();
}

// ── Email group modal ───────────────────────────────────────────────────────
async function initEmailGroupModal() {
    const res  = await fetch('/EmailSms/GetEmployees');
    const data = await res.json();

    emailEmployeesTable = $('#tblEmailEmployees').DataTable({
        data: data.dtEmpList ?? [],
        columns: [
            { data: null, orderable: false, render: () => '<input type="checkbox" class="emp-chk">' },
            { data: 'username' },
            { data: 'email' },
            { data: 'org' }
        ],
        pageLength: 10, order: []
    });

    document.getElementById('chkAllEmpEmail').addEventListener('change', e =>
        document.querySelectorAll('#tblEmailEmployees .emp-chk').forEach(c => c.checked = e.target.checked));

    emailGroupsListTable = $('#tblEmailGroups').DataTable({
        columns: [{ data: 'groupName' }],
        pageLength: 5, order: []
    });
    reloadGroupList();

    $('#tblEmailGroups tbody').on('click', 'tr', async function() {
        const row = emailGroupsListTable.row(this).data();
        document.getElementById('hidGroupID').value    = row.groupId;
        document.getElementById('txtGroupName').value  = row.groupName;
        document.getElementById('btnUpdateGroup').classList.remove('d-none');
        document.getElementById('btnDeleteGroup').classList.remove('d-none');
        document.getElementById('btnAddGroup').classList.add('d-none');

        // Pre-select employees in this group
        const res2  = await fetch('/EmailSms/GetGroupDetails', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json', 'RequestVerificationToken': getAntiForgeryToken() },
            body: JSON.stringify({ groupID: row.groupId })
        });
        const d2    = await res2.json();
        const uids  = new Set((d2.dtUIDs ?? []).map(u => u.UID));
        emailEmployeesTable.rows().nodes().toArray().forEach(tr => {
            const uid = emailEmployeesTable.row(tr).data()?.uid;
            const chk = tr.querySelector('.emp-chk');
            if (chk) chk.checked = uids.has(uid);
        });
    });
}

async function reloadGroupList() {
    const res  = await fetch('/EmailSms/GetGroups');
    const data = await res.json();
    emailGroupsListTable.clear().rows.add(data.dtGroups ?? []).draw();
    loadEmailGroups();
}

function getSelectedEmpUids() {
    return emailEmployeesTable.rows().nodes().toArray()
        .filter(tr => tr.querySelector('.emp-chk')?.checked)
        .map(tr => emailEmployeesTable.row(tr).data().uid);
}

async function addGroup() {
    const name = document.getElementById('txtGroupName').value.trim();
    if (!name) { toastWarn('Enter a group name.'); return; }
    const uids = getSelectedEmpUids();
    await postJson('/EmailSms/AddUpdateGroup', { emp: uids, groupName: name, groupID: 0, isUpdated: 0 });
    toastSuccess('Group added.'); clearGroup(); reloadGroupList();
}

async function updateGroup() {
    const id   = +document.getElementById('hidGroupID').value;
    const name = document.getElementById('txtGroupName').value.trim();
    const uids = getSelectedEmpUids();
    await postJson('/EmailSms/AddUpdateGroup', { emp: uids, groupName: name, groupID: id, isUpdated: 1 });
    toastSuccess('Group updated.'); clearGroup(); reloadGroupList();
}

async function deleteGroup() {
    const id = +document.getElementById('hidGroupID').value;
    const ok = await Swal.fire({ title: 'Delete group?', icon: 'warning', showCancelButton: true });
    if (!ok.isConfirmed) return;
    await postJson('/EmailSms/DeleteGroup', { groupID: id });
    toastSuccess('Deleted.'); clearGroup(); reloadGroupList();
}

function clearGroup() {
    document.getElementById('txtGroupName').value = '';
    document.getElementById('hidGroupID').value   = '';
    document.getElementById('btnAddGroup').classList.remove('d-none');
    document.getElementById('btnUpdateGroup').classList.add('d-none');
    document.getElementById('btnDeleteGroup').classList.add('d-none');
    document.querySelectorAll('#tblEmailEmployees .emp-chk').forEach(c => c.checked = false);
}

// ── SMS groups ──────────────────────────────────────────────────────────────
async function loadSmsGroups() {
    const res  = await fetch('/EmailSms/GetSMSGroups');
    const data = await res.json();
    const sel  = document.getElementById('selSmsGroups');
    sel.innerHTML = (data.dtSMSGroups ?? []).map(g =>
        `<option value="${g.groupId}">${g.groupName}</option>`).join('');
}

async function onSmsGroupChange() {
    const ids = [...document.getElementById('selSmsGroups').selectedOptions].map(o => o.value).join(',');
    if (!ids) { document.getElementById('txtMobileNos').value = ''; return; }
    const res  = await fetch('/EmailSms/GetMobileNo', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json', 'RequestVerificationToken': getAntiForgeryToken() },
        body: JSON.stringify({ groupIDs: ids })
    });
    const data = await res.json();
    document.getElementById('txtMobileNos').value =
        (data.dtMobile ?? []).map(m => m.MobileNo).filter(Boolean).join(',');
}

function addMobile() {
    const num  = document.getElementById('txtSMSTo').value.trim();
    if (num.length < 8) { toastWarn('Enter a valid mobile number.'); return; }
    const area = document.getElementById('txtMobileNos');
    const curr = area.value.replace(/^,/, '');
    area.value = curr ? curr + ',' + num : num;
    document.getElementById('txtSMSTo').value = '';
}

// ── SMS templates ───────────────────────────────────────────────────────────
async function loadSmsTemplates() {
    const res  = await fetch('/EmailSms/GetSMSTemplate');
    const data = await res.json();
    const sel  = document.getElementById('selSmsTemplate');
    sel.innerHTML = '<option value="">— Select Template —</option>' +
        (data.dtTemplate ?? []).map(t =>
            `<option value="${t.smsTemplateId}" data-msg="${esc(t.message)}" data-lang="${t.language}" data-name="${esc(t.smsTemplateName)}">${t.smsTemplateName}</option>`
        ).join('');
}

function onSmsTemplateChange() {
    const opt = document.getElementById('selSmsTemplate').selectedOptions[0];
    if (!opt || !opt.value) return;
    document.getElementById('hidSMSTemplateID').value      = opt.value;
    document.getElementById('txtSMSTemplate').value        = opt.dataset.msg  ?? '';
    document.getElementById('txtNewSMSTemplateName').value = opt.dataset.name ?? '';
    const lang = +(opt.dataset.lang ?? 1);
    onLangChange(lang);
    document.getElementById(lang === 2 ? 'chkArabic' : 'chkEnglish').checked = true;
    updateSmsCounter();
}

function onLangChange(lang) {
    selectedEmailLanguage = lang;
    const area = document.getElementById('txtSMSTemplate');
    area.disabled = false;
    area.focus();
    updateSmsCounter();
}

function updateSmsCounter() {
    const len = document.getElementById('txtSMSTemplate').value.length;
    const max = SMS_MAX[selectedEmailLanguage] ?? 160;
    document.getElementById('smsCounter').textContent = `${len} / ${max}`;
}

async function createSmsTemplate() {
    const name = document.getElementById('txtNewSMSTemplateName').value.trim();
    const msg  = document.getElementById('txtSMSTemplate').value.trim();
    if (!name || !msg) { toastWarn('Fill Template Name and Message.'); return; }
    await postJson('/EmailSms/CreateSMSTemplate',
        { smsTemplateName: name, message: msg, language: selectedEmailLanguage });
    toastSuccess('SMS template created.');
    loadSmsTemplates();
}

async function saveSmsTemplate() {
    const id   = +document.getElementById('hidSMSTemplateID').value;
    const name = document.getElementById('txtNewSMSTemplateName').value.trim();
    const msg  = document.getElementById('txtSMSTemplate').value.trim();
    if (!id) { toastWarn('Select a template first.'); return; }
    await postJson('/EmailSms/SaveSMSTemplate',
        { smsTemplateId: id, smsTemplateName: name, message: msg, language: selectedEmailLanguage });
    toastSuccess('SMS template saved.');
    loadSmsTemplates();
}

// ── Send SMS ────────────────────────────────────────────────────────────────
async function sendSms() {
    const mobiles = document.getElementById('txtMobileNos').value.trim();
    const msg     = document.getElementById('txtSMSTemplate').value.trim();
    if (!mobiles) { toastWarn('No mobile numbers.'); return; }
    if (!msg)     { toastWarn('No message.'); return; }
    showLoader('Sending SMS…');
    const res  = await fetch('/EmailSms/SendSMS', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json', 'RequestVerificationToken': getAntiForgeryToken() },
        body: JSON.stringify({
            templateID: +document.getElementById('hidSMSTemplateID').value || 0,
            mobileNos: mobiles, sMS: msg, language: selectedEmailLanguage
        })
    });
    hideLoader();
    const data = await res.json();
    if (data.Message === 'Success') { toastSuccess('SMS sent.'); loadSmsLog(); }
    else toastError('SMS send failed.');
}

// ── SMS log ─────────────────────────────────────────────────────────────────
function initSmsLogTable() {
    smsLogTable = $('#tblSmsLog').DataTable({
        columns: [
            { data: null, orderable: false, render: () => '<input type="checkbox" class="row-chk">' },
            { data: 'smsTemplateName' },
            { data: 'smsTo' },
            { data: 'message' }
        ],
        pageLength: 10, order: []
    });
}

async function loadSmsLog() {
    const res  = await fetch('/EmailSms/GetLogSMS');
    const data = await res.json();
    document.getElementById('lblBalance').textContent = data.SMSBalance ?? '—';
    smsLogTable.clear().rows.add(data.dtSendSMS ?? []).draw();
}

function getCheckedSmsIds() {
    return smsLogTable.rows().nodes().toArray()
        .filter(tr => tr.querySelector('.row-chk')?.checked)
        .map(tr => smsLogTable.row(tr).data().id);
}

async function resendSms() {
    const ids = getCheckedSmsIds();
    if (!ids.length) { toastWarn('Select at least one SMS.'); return; }
    showLoader('Sending…');
    const res  = await fetch('/EmailSms/SendSMS2', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json', 'RequestVerificationToken': getAntiForgeryToken() },
        body: JSON.stringify({ smsID: ids })
    });
    hideLoader();
    const data = await res.json();
    if (data.Message === 'Success') { toastSuccess('SMS sent.'); loadSmsLog(); }
    else toastError('Send failed.');
}

async function deleteSms() {
    const ids = getCheckedSmsIds();
    if (!ids.length) { toastWarn('Select at least one SMS.'); return; }
    const ok = await Swal.fire({ title: 'Delete?', icon: 'warning', showCancelButton: true });
    if (!ok.isConfirmed) return;
    await postJson('/EmailSms/DeleteSMS', { smsID: ids });
    toastSuccess('Deleted.');
    loadSmsLog();
}

// ── SMS group modal ─────────────────────────────────────────────────────────
async function initSmsGroupModal() {
    const res  = await fetch('/EmailSms/GetEmployees');
    const data = await res.json();

    smsEmployeesTable = $('#tblSmsEmployees').DataTable({
        data: data.dtEmpList1 ?? [],
        columns: [
            { data: null, orderable: false, render: () => '<input type="checkbox" class="sms-emp-chk">' },
            { data: 'username' },
            { data: 'subNo' },
            { data: 'org' }
        ],
        pageLength: 10, order: []
    });

    document.getElementById('chkAllEmpSms').addEventListener('change', e =>
        document.querySelectorAll('#tblSmsEmployees .sms-emp-chk').forEach(c => c.checked = e.target.checked));

    smsGroupsListTable = $('#tblSmsGroups').DataTable({
        columns: [{ data: 'groupName' }],
        pageLength: 5, order: []
    });
    reloadSmsGroupList();

    $('#tblSmsGroups tbody').on('click', 'tr', async function() {
        const row = smsGroupsListTable.row(this).data();
        document.getElementById('hidSmsGroupID').value   = row.groupId;
        document.getElementById('txtSmsGroupName').value = row.groupName;
        document.getElementById('btnUpdateSmsGroup').classList.remove('d-none');
        document.getElementById('btnDeleteSmsGroup').classList.remove('d-none');
        document.getElementById('btnAddSmsGroup').classList.add('d-none');

        const res2  = await fetch('/EmailSms/GetSMSGroupDetails', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json', 'RequestVerificationToken': getAntiForgeryToken() },
            body: JSON.stringify({ groupID: row.groupId })
        });
        const d2      = await res2.json();
        const subNos  = new Set((d2.dtSUB_NOs ?? []).map(s => s.SUB_NO));
        smsEmployeesTable.rows().nodes().toArray().forEach(tr => {
            const sub = smsEmployeesTable.row(tr).data()?.subNo;
            const chk = tr.querySelector('.sms-emp-chk');
            if (chk) chk.checked = subNos.has(sub);
        });
    });
}

async function reloadSmsGroupList() {
    const res  = await fetch('/EmailSms/GetSMSGroups');
    const data = await res.json();
    smsGroupsListTable.clear().rows.add(data.dtSMSGroups ?? []).draw();
    loadSmsGroups();
}

function getSelectedSmsNos() {
    return smsEmployeesTable.rows().nodes().toArray()
        .filter(tr => tr.querySelector('.sms-emp-chk')?.checked)
        .map(tr => smsEmployeesTable.row(tr).data().subNo);
}

async function addSmsGroup() {
    const name = document.getElementById('txtSmsGroupName').value.trim();
    if (!name) { toastWarn('Enter a group name.'); return; }
    const subs = getSelectedSmsNos();
    await postJson('/EmailSms/SMSAddUpdateGroup',
        { suB_NOs: subs, groupName: name, groupID: 0, isUpdated: 0 });
    toastSuccess('SMS group added.'); clearSmsGroup(); reloadSmsGroupList();
}

async function updateSmsGroup() {
    const id   = +document.getElementById('hidSmsGroupID').value;
    const name = document.getElementById('txtSmsGroupName').value.trim();
    const subs = getSelectedSmsNos();
    await postJson('/EmailSms/SMSAddUpdateGroup',
        { suB_NOs: subs, groupName: name, groupID: id, isUpdated: 1 });
    toastSuccess('SMS group updated.'); clearSmsGroup(); reloadSmsGroupList();
}

async function deleteSmsGroup() {
    const id = +document.getElementById('hidSmsGroupID').value;
    const ok = await Swal.fire({ title: 'Delete group?', icon: 'warning', showCancelButton: true });
    if (!ok.isConfirmed) return;
    await postJson('/EmailSms/DeleteSMSGroup', { groupID: id });
    toastSuccess('Deleted.'); clearSmsGroup(); reloadSmsGroupList();
}

function clearSmsGroup() {
    document.getElementById('txtSmsGroupName').value = '';
    document.getElementById('hidSmsGroupID').value   = '';
    document.getElementById('btnAddSmsGroup').classList.remove('d-none');
    document.getElementById('btnUpdateSmsGroup').classList.add('d-none');
    document.getElementById('btnDeleteSmsGroup').classList.add('d-none');
    document.querySelectorAll('#tblSmsEmployees .sms-emp-chk').forEach(c => c.checked = false);
}

// ── Helpers ──────────────────────────────────────────────────────────────────
function esc(s) { return (s ?? '').replace(/"/g, '&quot;'); }

async function postJson(url, body) {
    return fetch(url, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json', 'RequestVerificationToken': getAntiForgeryToken() },
        body: JSON.stringify(body)
    }).then(r => r.json());
}

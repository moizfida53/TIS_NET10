/* admin-employee.js — ManageEmployee page */
(function () {
    'use strict';

    let dtEmp, dtCC, dtCountry, dtManager;
    let pageData = {};   // { dtEmp, RoleList, CountryList, dtCC }
    let managerList = [];
    let editMode = false;

    // ── Bootstrap modals ──────────────────────────────────────────────────────
    const modalEmp    = new bootstrap.Modal('#modal-emp');
    const modalEmpDel = new bootstrap.Modal('#modal-emp-del');
    const modalCC     = new bootstrap.Modal('#modal-cc');
    const modalCCDel  = new bootstrap.Modal('#modal-cc-del');
    const modalCnt    = new bootstrap.Modal('#modal-country');
    const modalCntDel = new bootstrap.Modal('#modal-cnt-del');
    const modalMgr    = new bootstrap.Modal('#modal-manager');
    const modalMgrDel = new bootstrap.Modal('#modal-mgr-del');

    // ── Init ──────────────────────────────────────────────────────────────────
    $(function () {
        loadData();
        loadManagers();
        bindButtons();
    });

    function loadData() {
        $.get('/Admin/GetUser').done(function (res) {
            pageData = res;
            renderEmployeeTable(res.dtEmp);
            renderCCTable(res.dtCC);
            renderCountryTable(res.countryList);
            populateSelects(res);
        }).fail(function () { TIS.toast('error', 'Failed to load employee data'); });
    }

    function loadManagers() {
        $.get('/Admin/GetManagers').done(function (res) {
            managerList = res.dtManager || [];
            renderManagerTable(managerList);
            populateManagerDropdown(managerList);
        });
    }

    // ── DataTables ────────────────────────────────────────────────────────────
    function renderEmployeeTable(data) {
        if (dtEmp) { dtEmp.clear().rows.add(data).draw(); return; }
        dtEmp = $('#dt-emp').DataTable({
            data: data,
            columns: [
                { data: 'nAME' },
                { data: 'eMPLOYEENO' },
                { data: 'eMAIL' },
                { data: 'uSERNAME' },
                { data: 'rOLENAME' },
                { data: 'cOUNTRYNAME' },
                { data: 'gRADE' },
                { data: 'isActive', render: v => v ? '<span class="badge bg-success">Yes</span>' : '<span class="badge bg-secondary">No</span>' },
                {
                    data: null, orderable: false, searchable: false,
                    render: function (_, __, row) {
                        return `<button class="btn btn-xs btn-outline-primary me-1 btn-emp-edit" data-uid="${row.uID}">Edit</button>
                                <button class="btn btn-xs btn-outline-danger btn-emp-del" data-uid="${row.uID}" data-name="${row.nAME}">Del</button>`;
                    }
                }
            ],
            pageLength: 25
        });
        $('#dt-emp').on('click', '.btn-emp-edit', function () { openEmpEdit($(this).data('uid')); });
        $('#dt-emp').on('click', '.btn-emp-del', function () { openEmpDel($(this).data('uid'), $(this).data('name')); });
    }

    function renderCCTable(data) {
        if (dtCC) { dtCC.clear().rows.add(data).draw(); return; }
        dtCC = $('#dt-cc').DataTable({
            data: data,
            columns: [
                { data: 'cCName' },
                { data: 'cCNum' },
                {
                    data: null, orderable: false,
                    render: (_, __, row) =>
                        `<button class="btn btn-xs btn-outline-primary me-1 btn-cc-edit" data-uid="${row.uID}" data-name="${row.cCName}" data-num="${row.cCNum}">Edit</button>
                         <button class="btn btn-xs btn-outline-danger btn-cc-del" data-uid="${row.uID}">Del</button>`
                }
            ]
        });
        $('#dt-cc').on('click', '.btn-cc-edit', function () {
            editMode = true;
            $('#cc-uid').val($(this).data('uid'));
            $('#cc-name').val($(this).data('name'));
            $('#cc-num').val($(this).data('num'));
            $('#modal-cc-title').text('Edit Cost Center');
            modalCC.show();
        });
        $('#dt-cc').on('click', '.btn-cc-del', function () {
            $('#del-cc-uid').val($(this).data('uid'));
            modalCCDel.show();
        });
    }

    function renderCountryTable(data) {
        if (dtCountry) { dtCountry.clear().rows.add(data).draw(); return; }
        dtCountry = $('#dt-country').DataTable({
            data: data,
            columns: [
                { data: 'cOUNTRYNAME' },
                { data: 'cOUNTRYCODE' },
                { data: 'sHAYACODE' },
                { data: 'eXCHANGERATE' },
                { data: 'cURRENCY' },
                {
                    data: null, orderable: false,
                    render: (_, __, row) =>
                        `<button class="btn btn-xs btn-outline-primary me-1 btn-cnt-edit" data-id="${row.cOUNTRYID}" data-name="${row.cOUNTRYNAME}" data-code="${row.cOUNTRYCODE}" data-shaya="${row.sHAYACODE}" data-rate="${row.eXCHANGERATE}" data-currency="${row.cURRENCY}">Edit</button>
                         <button class="btn btn-xs btn-outline-danger btn-cnt-del" data-id="${row.cOUNTRYID}">Del</button>`
                }
            ]
        });
        $('#dt-country').on('click', '.btn-cnt-edit', function () {
            const d = $(this).data();
            editMode = true;
            $('#cnt-id').val(d.id); $('#cnt-name').val(d.name); $('#cnt-code').val(d.code);
            $('#cnt-shaya').val(d.shaya); $('#cnt-rate').val(d.rate); $('#cnt-currency').val(d.currency);
            $('#modal-country-title').text('Edit Country'); modalCnt.show();
        });
        $('#dt-country').on('click', '.btn-cnt-del', function () {
            $('#del-cnt-id').val($(this).data('id')); modalCntDel.show();
        });
    }

    function renderManagerTable(data) {
        if (dtManager) { dtManager.clear().rows.add(data).draw(); return; }
        dtManager = $('#dt-manager').DataTable({
            data: data,
            columns: [
                { data: 'managerName' },
                { data: 'eMPLOYEENO' },
                {
                    data: null, orderable: false,
                    render: (_, __, row) =>
                        `<button class="btn btn-xs btn-outline-primary me-1 btn-mgr-edit" data-uid="${row.uID}" data-name="${row.managerName}" data-empno="${row.eMPLOYEENO}">Edit</button>
                         <button class="btn btn-xs btn-outline-danger btn-mgr-del" data-uid="${row.uID}">Del</button>`
                }
            ]
        });
        $('#dt-manager').on('click', '.btn-mgr-edit', function () {
            editMode = true;
            $('#mgr-uid').val($(this).data('uid')); $('#mgr-name').val($(this).data('name')); $('#mgr-empno').val($(this).data('empno'));
            $('#modal-mgr-title').text('Edit Manager'); modalMgr.show();
        });
        $('#dt-manager').on('click', '.btn-mgr-del', function () {
            $('#del-mgr-uid').val($(this).data('uid')); modalMgrDel.show();
        });
    }

    // ── Selects ───────────────────────────────────────────────────────────────
    function populateSelects(res) {
        $('#emp-role').empty().append('<option value="">-- Select --</option>');
        (res.roleList || []).forEach(r => $('#emp-role').append(`<option value="${r.role_ID}">${r.roleName}</option>`));

        $('#emp-countries').empty();
        (res.countryList || []).forEach(c => $('#emp-countries').append(`<option value="${c.cOUNTRYID}">${c.cOUNTRYNAME}</option>`));
    }

    function populateManagerDropdown(managers) {
        $('#emp-manager').empty().append('<option value="0">-- None --</option>');
        managers.forEach(m => $('#emp-manager').append(`<option value="${m.uID}">${m.managerName}</option>`));
    }

    // ── Employee modal open ───────────────────────────────────────────────────
    function openEmpEdit(uid) {
        const emp = (pageData.dtEmp || []).find(e => e.uID == uid);
        if (!emp) return;
        editMode = true;
        $('#emp-uid').val(emp.uID); $('#emp-name').val(emp.nAME); $('#emp-no').val(emp.eMPLOYEENO);
        $('#emp-username').val(emp.uSERNAME); $('#emp-email').val(emp.eMAIL); $('#emp-org').val(emp.oRG);
        $('#emp-desc').val(emp.dESCRIPTION); $('#emp-grade').val(emp.gRADE); $('#emp-ext').val(emp.eXTENSION);
        $('#emp-payroll').val(emp.pAYROLL); $('#emp-ccno').val(emp.cCNO); $('#emp-company').val(emp.cOMPANYID);
        $('#emp-role').val(emp.rOLEID); $('#emp-manager').val(emp.mANAGERID);
        $('#emp-active').prop('checked', emp.isActive);
        // Pre-select the employee's country
        $('#emp-countries option').prop('selected', false);
        $('#emp-countries option[value="' + emp.cOUNTRYID + '"]').prop('selected', true);
        $('#modal-emp-title').text('Edit Employee'); modalEmp.show();
    }

    function openEmpDel(uid, name) {
        $('#del-emp-uid').val(uid); $('#del-emp-name').text(name); modalEmpDel.show();
    }

    // ── Button bindings ───────────────────────────────────────────────────────
    function bindButtons() {

        // ── Employee
        $('#btn-emp-add').on('click', function () {
            editMode = false;
            $('#emp-uid').val(''); $('#emp-name,#emp-no,#emp-username,#emp-email,#emp-org').val('');
            $('#emp-desc,#emp-grade,#emp-ext,#emp-payroll,#emp-ccno,#emp-company').val('');
            $('#emp-role').val(''); $('#emp-manager').val('0');
            $('#emp-countries option').prop('selected', false);
            $('#emp-active').prop('checked', true);
            $('#modal-emp-title').text('Add Employee'); modalEmp.show();
        });

        $('#btn-emp-save').on('click', function () {
            const cids = $('#emp-countries').val();
            if (!$('#emp-name').val() || !cids || !cids.length) {
                TIS.toast('warning', 'Name and at least one country are required'); return;
            }
            const payload = {
                Employee: {
                    UID: parseInt($('#emp-uid').val()) || 0,
                    NAME: $('#emp-name').val(), EMPLOYEENO: $('#emp-no').val(),
                    USERNAME: $('#emp-username').val(), EMAIL: $('#emp-email').val(),
                    ORG: $('#emp-org').val(), DESCRIPTION: $('#emp-desc').val(),
                    GRADE: $('#emp-grade').val(), EXTENSION: $('#emp-ext').val(),
                    PAYROLL: $('#emp-payroll').val(), CCNO: $('#emp-ccno').val(),
                    COMPANYID: $('#emp-company').val(),
                    ROLEID: parseInt($('#emp-role').val()) || 0,
                    MANAGERID: parseInt($('#emp-manager').val()) || 0,
                    IsActive: $('#emp-active').is(':checked')
                },
                CountryIds: cids.map(Number)
            };
            const url = editMode ? '/Admin/UpdateEmployee' : '/Admin/AddEmployee';
            $.post({ url, data: JSON.stringify(payload), contentType: 'application/json' })
                .done(function (res) {
                    if (res.myMessage === 'succ') {
                        modalEmp.hide(); TIS.toast('success', editMode ? 'Employee updated' : 'Employee added');
                        loadData();
                    } else { TIS.toast('error', res.myMessage); }
                }).fail(() => TIS.toast('error', 'Save failed'));
        });

        $('#btn-emp-del-confirm').on('click', function () {
            $.post('/Admin/DeleteEmployee?uid=' + $('#del-emp-uid').val())
                .done(function (res) {
                    if (res.myMessage === 'succ') { modalEmpDel.hide(); TIS.toast('success', 'Employee deleted'); loadData(); }
                    else TIS.toast('error', res.myMessage);
                }).fail(() => TIS.toast('error', 'Delete failed'));
        });

        // ── Cost Center
        $('#btn-cc-add').on('click', function () {
            editMode = false; $('#cc-uid').val(''); $('#cc-name,#cc-num').val('');
            $('#modal-cc-title').text('Add Cost Center'); modalCC.show();
        });
        $('#btn-cc-save').on('click', function () {
            const payload = { UID: parseInt($('#cc-uid').val()) || 0, CCName: $('#cc-name').val(), CCNum: $('#cc-num').val() };
            const url = editMode ? '/Admin/UpdateCC' : '/Admin/AddCC';
            $.post({ url, data: JSON.stringify(payload), contentType: 'application/json' })
                .done(function (res) {
                    if (res.myMessage && res.myMessage.startsWith('Error')) TIS.toast('error', res.myMessage);
                    else { modalCC.hide(); TIS.toast('success', 'Saved'); loadData(); }
                });
        });
        $('#btn-cc-del-confirm').on('click', function () {
            $.post('/Admin/DeleteCC?uid=' + $('#del-cc-uid').val())
                .done(function (res) {
                    if (res.myMessage && res.myMessage.startsWith('Error')) TIS.toast('error', res.myMessage);
                    else { modalCCDel.hide(); TIS.toast('success', 'Deleted'); loadData(); }
                });
        });

        // ── Country
        $('#btn-cnt-add').on('click', function () {
            editMode = false; $('#cnt-id').val(''); $('#cnt-name,#cnt-code,#cnt-shaya,#cnt-rate,#cnt-currency').val('');
            $('#modal-country-title').text('Add Country'); modalCnt.show();
        });
        $('#btn-cnt-save').on('click', function () {
            const payload = {
                COUNTRYID: parseInt($('#cnt-id').val()) || 0, COUNTRYNAME: $('#cnt-name').val(),
                COUNTRYCODE: $('#cnt-code').val(), SHAYACODE: $('#cnt-shaya').val(),
                EXCHANGERATE: parseFloat($('#cnt-rate').val()) || 0, CURRENCY: $('#cnt-currency').val()
            };
            const url = editMode ? '/Admin/UpdateCountry' : '/Admin/AddCountry';
            $.post({ url, data: JSON.stringify(payload), contentType: 'application/json' })
                .done(function (res) {
                    if (res.myMessage) TIS.toast('error', res.myMessage);
                    else { modalCnt.hide(); TIS.toast('success', 'Saved'); loadData(); }
                });
        });
        $('#btn-cnt-del-confirm').on('click', function () {
            $.post('/Admin/DeleteCountry?countryId=' + $('#del-cnt-id').val())
                .done(function (res) {
                    if (res.myMessage) TIS.toast('error', res.myMessage);
                    else { modalCntDel.hide(); TIS.toast('success', 'Deleted'); loadData(); }
                });
        });

        // ── Manager
        $('#btn-mgr-add').on('click', function () {
            editMode = false; $('#mgr-uid').val(''); $('#mgr-name,#mgr-empno').val('');
            $('#modal-mgr-title').text('Add Manager'); modalMgr.show();
        });
        $('#btn-mgr-save').on('click', function () {
            const uid = parseInt($('#mgr-uid').val()) || null;
            const payload = { Uid: uid, Name: $('#mgr-name').val(), EmployeeNo: $('#mgr-empno').val() };
            const url = editMode ? '/Admin/UpdateManager' : '/Admin/AddManager';
            $.post({ url, data: JSON.stringify(payload), contentType: 'application/json' })
                .done(function (res) {
                    if (res.myMessage) TIS.toast('error', res.myMessage);
                    else { modalMgr.hide(); TIS.toast('success', 'Saved'); loadManagers(); }
                });
        });
        $('#btn-mgr-del-confirm').on('click', function () {
            $.post('/Admin/DeleteManager?uid=' + $('#del-mgr-uid').val())
                .done(function (res) {
                    if (res.myMessage) TIS.toast('error', res.myMessage);
                    else { modalMgrDel.hide(); TIS.toast('success', 'Deleted'); loadManagers(); }
                });
        });
    }

})();

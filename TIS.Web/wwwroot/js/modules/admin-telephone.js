/* admin-telephone.js — AddTelephone page */
(function () {
    'use strict';

    let dtTel, dtAsg;
    let pageData = {};
    let telEditMode = false, asgEditMode = false;

    const modalTel    = new bootstrap.Modal('#modal-tel');
    const modalTelDel = new bootstrap.Modal('#modal-tel-del');
    const modalAsg    = new bootstrap.Modal('#modal-asg');
    const modalAsgDel = new bootstrap.Modal('#modal-asg-del');

    // flatpickr date pickers
    flatpickr('#asg-start', { dateFormat: 'Y-m-d' });
    flatpickr('#asg-end',   { dateFormat: 'Y-m-d' });

    $(function () { loadData(); bindButtons(); });

    function loadData() {
        $.when(
            $.get('/Admin/GetTelData'),
            $.get('/Admin/GetCC')
        ).done(function (telRes, ccRes) {
            const res = telRes[0];
            pageData = res;
            renderTelTable(res.dtTel);
            renderAsgTable(res.dtAsg);
            populateProviders(res.dtProvider);
            populateSubNoDropdown(res.dtTel);
            populateEmpDropdown(res.dtEmp);
            populateCCDropdown(ccRes[0].dtCC);
        }).fail(() => TIS.toast('error', 'Failed to load telephone data'));
    }

    function renderTelTable(data) {
        if (dtTel) { dtTel.clear().rows.add(data || []).draw(); return; }
        dtTel = $('#dt-tel').DataTable({
            data: data || [],
            columns: [
                { data: 'subNo' },
                { data: 'providerName' },
                { data: 'description' },
                { data: 'accountNo' },
                { data: 'lineType' },
                { data: 'isAssigned', render: v => v ? '<span class="badge bg-warning text-dark">Yes</span>' : '<span class="badge bg-success">No</span>' },
                {
                    data: null, orderable: false,
                    render: (_, __, row) =>
                        `<button class="btn btn-xs btn-outline-primary me-1 btn-tel-edit" data-id="${row.subNoId}" data-subno="${row.subNo}" data-pid="${row.providerID}" data-acct="${row.accountNo}" data-desc="${row.description}" data-lt="${row.lineType}">Edit</button>
                         <button class="btn btn-xs btn-outline-danger btn-tel-del" data-id="${row.subNoId}" data-subno="${row.subNo}">Del</button>`
                }
            ],
            pageLength: 25
        });
        $('#dt-tel').on('click', '.btn-tel-edit', function () {
            const d = $(this).data();
            telEditMode = true; $('#tel-id').val(d.id); $('#tel-subno').val(d.subno);
            $('#tel-provider').val(d.pid); $('#tel-account').val(d.acct);
            $('#tel-desc').val(d.desc); $('#tel-linetype').val(d.lt);
            $('#modal-tel-title').text('Edit Telephone'); modalTel.show();
        });
        $('#dt-tel').on('click', '.btn-tel-del', function () {
            $('#del-tel-id').val($(this).data('id'));
            $('#del-tel-subno').text($(this).data('subno'));
            modalTelDel.show();
        });
    }

    function renderAsgTable(data) {
        if (dtAsg) { dtAsg.clear().rows.add(data || []).draw(); return; }
        dtAsg = $('#dt-asg').DataTable({
            data: data || [],
            columns: [
                { data: 'subNo' },
                { data: 'employeeName' },
                { data: 'employeeNo' },
                { data: 'costCenterName' },
                { data: 'businessLimit', render: v => Number(v).toLocaleString('en', { minimumFractionDigits: 2 }) },
                { data: 'allowanceLimit', render: v => Number(v).toLocaleString('en', { minimumFractionDigits: 2 }) },
                { data: 'lineStatus' },
                { data: 'startDate', render: v => v ? v.substring(0,10) : '' },
                { data: 'endDate',   render: v => v ? v.substring(0,10) : '' },
                {
                    data: null, orderable: false,
                    render: (_, __, row) =>
                        `<button class="btn btn-xs btn-outline-primary me-1 btn-asg-edit" data-id="${row.assignID}" data-snid="${row.subNoId}" data-uid="${row.uID}" data-blimit="${row.businessLimit}" data-alimit="${row.allowanceLimit}" data-status="${row.lineStatus}" data-ccid="${row.costCenterID}" data-start="${row.startDate||''}" data-end="${row.endDate||''}">Edit</button>
                         <button class="btn btn-xs btn-outline-danger btn-asg-del" data-id="${row.assignID}" data-name="${row.employeeName}">Del</button>`
                }
            ],
            pageLength: 25
        });
        $('#dt-asg').on('click', '.btn-asg-edit', function () {
            const d = $(this).data();
            asgEditMode = true; $('#asg-id').val(d.id); $('#asg-subno').val(d.snid); $('#asg-emp').val(d.uid);
            $('#asg-cc').val(d.ccid); $('#asg-status').val(d.status);
            $('#asg-blimit').val(d.blimit); $('#asg-alimit').val(d.alimit);
            document.getElementById('asg-start')._flatpickr.setDate(d.start ? d.start.substring(0,10) : null);
            document.getElementById('asg-end')._flatpickr.setDate(d.end   ? d.end.substring(0,10)   : null);
            $('#modal-asg-title').text('Edit Assignment'); modalAsg.show();
        });
        $('#dt-asg').on('click', '.btn-asg-del', function () {
            $('#del-asg-id').val($(this).data('id'));
            $('#del-asg-name').text($(this).data('name'));
            modalAsgDel.show();
        });
    }

    function populateProviders(providers) {
        $('#tel-provider').empty();
        (providers || []).forEach(p => $('#tel-provider').append(`<option value="${p.providerID}">${p.providerName}</option>`));
    }

    function populateCCDropdown(costCenters) {
        $('#asg-cc').empty().append('<option value="">-- None --</option>');
        (costCenters || []).forEach(c => $('#asg-cc').append(`<option value="${c.uID}">${c.cCName}</option>`));
    }

    function populateSubNoDropdown(tels) {
        $('#asg-subno').empty().append('<option value="">-- Select Number --</option>');
        (tels || []).filter(t => !t.isAssigned).forEach(t => $('#asg-subno').append(`<option value="${t.subNoId}">${t.subNo}</option>`));
    }

    function populateEmpDropdown(emps) {
        $('#asg-emp').empty().append('<option value="">-- Select Employee --</option>');
        (emps || []).forEach(e => $('#asg-emp').append(`<option value="${e.uID}">${e.nAME} (${e.eMPLOYEENO})</option>`));
    }

    function bindButtons() {
        $('#btn-tel-add').on('click', function () {
            telEditMode = false; $('#tel-id').val(''); $('#tel-subno,#tel-account,#tel-desc,#tel-linetype').val(''); $('#tel-provider').val('');
            $('#modal-tel-title').text('Add Telephone'); modalTel.show();
        });

        $('#btn-tel-save').on('click', function () {
            const payload = {
                SubNoId: parseInt($('#tel-id').val()) || 0,
                SubNo: $('#tel-subno').val(), ProviderID: parseInt($('#tel-provider').val()),
                AccountNo: $('#tel-account').val(), Description: $('#tel-desc').val(), LineType: $('#tel-linetype').val()
            };
            const url = telEditMode ? '/Admin/UpdateTelephone' : '/Admin/AddTelephone';
            $.post({ url, data: JSON.stringify(payload), contentType: 'application/json' })
                .done(function (res) {
                    if (res.myMessage === 'succ') { modalTel.hide(); TIS.toast('success', 'Saved'); loadData(); }
                    else TIS.toast('error', res.myMessage);
                });
        });

        $('#btn-tel-del-confirm').on('click', function () {
            $.post('/Admin/DeleteTelephone?id=' + $('#del-tel-id').val())
                .done(function (res) {
                    if (res.myMessage === 'succ') { modalTelDel.hide(); TIS.toast('success', 'Deleted'); loadData(); }
                    else TIS.toast('error', res.myMessage);
                });
        });

        $('#btn-asg-add').on('click', function () {
            asgEditMode = false; $('#asg-id').val(''); $('#asg-blimit,#asg-alimit').val(0);
            document.getElementById('asg-start')._flatpickr.clear();
            document.getElementById('asg-end')._flatpickr.clear();
            $('#asg-subno,#asg-emp,#asg-cc').val(''); $('#asg-status').val('Active');
            // Reload unassigned numbers
            $.get('/Admin/GetTelNo').done(r => {
                $('#asg-subno').empty().append('<option value="">-- Select Number --</option>');
                (r.dtTel || []).forEach(t => $('#asg-subno').append(`<option value="${t.subNoId}">${t.subNo}</option>`));
            });
            $('#modal-asg-title').text('Assign Number'); modalAsg.show();
        });

        $('#btn-asg-save').on('click', function () {
            const payload = {
                AssignID: parseInt($('#asg-id').val()) || 0,
                SubNoId: parseInt($('#asg-subno').val()), UID: parseInt($('#asg-emp').val()),
                CostCenterID: parseInt($('#asg-cc').val()) || 0,
                LineStatus: $('#asg-status').val(),
                BusinessLimit: parseFloat($('#asg-blimit').val()) || 0,
                AllowanceLimit: parseFloat($('#asg-alimit').val()) || 0,
                StartDate: $('#asg-start').val() || null, EndDate: $('#asg-end').val() || null
            };
            const url = asgEditMode ? '/Admin/UpdateAssign' : '/Admin/Assign';
            $.post({ url, data: JSON.stringify(payload), contentType: 'application/json' })
                .done(function (res) {
                    if (res.myMessage === 'succ') { modalAsg.hide(); TIS.toast('success', 'Saved'); loadData(); }
                    else TIS.toast('error', res.myMessage);
                });
        });

        $('#btn-asg-del-confirm').on('click', function () {
            $.post('/Admin/DeleteAssign?id=' + $('#del-asg-id').val())
                .done(function (res) {
                    if (res.myMessage === 'succ') { modalAsgDel.hide(); TIS.toast('success', 'Removed'); loadData(); }
                    else TIS.toast('error', res.myMessage);
                });
        });
    }

})();

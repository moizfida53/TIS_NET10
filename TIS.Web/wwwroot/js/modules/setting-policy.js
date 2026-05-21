/* setting-policy.js — ManageCallType page */
(function () {
    'use strict';

    let dtPol;
    let pageData = { providers: [], callTypes: [], employees: [], lineTypes: [] };
    let editMode = false;

    const modalPol    = new bootstrap.Modal('#modal-pol');
    const modalPolDel = new bootstrap.Modal('#modal-pol-del');

    $(function () { loadPageData(); bindButtons(); });

    function loadPageData() {
        $.get('/Setting/GetPolicyData').done(function (res) {
            pageData.providers  = res.dtProvider  || [];
            pageData.callTypes  = res.dtCallType  || [];
            pageData.employees  = res.dtEmp       || [];
            pageData.lineTypes  = res.dtLineType  || [];
            populateStaticDropdowns();
            loadPolicies();
        }).fail(() => TIS.toast('error', 'Failed to load policy data'));
    }

    function loadPolicies() {
        $.get('/Setting/GetPolicies').done(function (res) {
            renderTable(res.dtPolicy || []);
        });
    }

    function renderTable(data) {
        if (dtPol) { dtPol.clear().rows.add(data).draw(); return; }
        dtPol = $('#dt-pol').DataTable({
            data: data,
            columns: [
                { data: 'providerName' },
                { data: 'providerTypeDesc' },
                { data: 'destinationDesc', render: v => v || '<em class="text-muted">All</em>' },
                { data: 'callTypeDesc' },
                { data: 'lineTypeName' },
                { data: 'isAll', render: v => v ? '<span class="badge bg-success">Yes</span>' : 'No' },
                { data: 'superimposeTrain', render: v => v ? '<span class="badge bg-info">Yes</span>' : 'No' },
                {
                    data: null, orderable: false,
                    render: (_, __, row) =>
                        `<button class="btn btn-xs btn-outline-primary me-1 btn-pol-edit" data-id="${row.id}" data-isall="${row.isAll}" data-sup="${row.superimposeTrain}">Edit</button>
                         <button class="btn btn-xs btn-outline-danger btn-pol-del" data-id="${row.id}">Del</button>`
                }
            ],
            pageLength: 25
        });

        $('#dt-pol').on('click', '.btn-pol-edit', function () {
            const d = $(this).data();
            editMode = true;
            $('#pol-id').val(d.id);
            $('#pol-isall').prop('checked', d.isall === true || d.isall === 'True');
            $('#pol-superimpose').prop('checked', d.sup === true || d.sup === 'True');
            toggleEmpSection();

            // Load existing employees for this policy
            $.get('/Setting/GetPolicyDetail?id=' + d.id).done(function (res) {
                const detail = res.dtDetail || [];
                $('#pol-emp option').prop('selected', false);
                detail.forEach(function (x) {
                    $('#pol-emp option[data-uid="' + x.uid + '"][data-snid="' + x.subNoID + '"]').prop('selected', true);
                });
            });

            $('#modal-pol-title').text('Edit Policy'); modalPol.show();
        });

        $('#dt-pol').on('click', '.btn-pol-del', function () {
            $('#del-pol-id').val($(this).data('id')); modalPolDel.show();
        });
    }

    function populateStaticDropdowns() {
        // Provider
        $('#pol-provider').empty().append('<option value="">-- Provider --</option>');
        pageData.providers.forEach(p => $('#pol-provider').append(`<option value="${p.id}">${p.name}</option>`));

        // Call Type
        $('#pol-calltype').empty().append('<option value="">-- Call Type --</option>');
        pageData.callTypes.forEach(c => $('#pol-calltype').append(`<option value="${c.id}">${c.name}</option>`));

        // Line Type
        $('#pol-linetype').empty().append('<option value="">-- Line Type --</option>');
        pageData.lineTypes.forEach(l => $('#pol-linetype').append(`<option value="${l.id}">${l.lineType}</option>`));

        // Employees
        $('#pol-emp').empty();
        pageData.employees.forEach(e => {
            const opt = document.createElement('option');
            opt.value = e.uid;
            opt.textContent = `${e.employeeName} (${e.org}) — ${e.subNo}`;
            opt.dataset.uid  = e.uid;
            opt.dataset.snid = e.subNoId;
            $('#pol-emp').append(opt);
        });
    }

    function loadTransTypes(providerId, callback) {
        $('#pol-transtype').empty().append('<option value="">-- Trans Type --</option>');
        if (!providerId) { if (callback) callback(); return; }
        $.get('/Setting/GetTransTypes?providerId=' + providerId).done(function (res) {
            (res.dtTransType || []).forEach(t => $('#pol-transtype').append(`<option value="${t.transType}">${t.transType}</option>`));
            if (callback) callback();
        });
    }

    function loadDescriptions(providerId, transType) {
        $('#pol-desc').empty();
        if (!providerId || !transType) return;
        $.get('/Setting/GetDescriptions?providerId=' + providerId + '&transType=' + encodeURIComponent(transType))
            .done(function (res) {
                (res.dtDesc || []).forEach(d => $('#pol-desc').append(`<option value="${d.description}">${d.description}</option>`));
            });
    }

    function toggleEmpSection() {
        const isAll = $('#pol-isall').is(':checked');
        $('#emp-section').toggle(!isAll);
    }

    function toggleDescSection() {
        const allDesc = $('#pol-alldesc').is(':checked');
        $('#desc-section').toggle(!allDesc);
    }

    function collectEmployees() {
        return $('#pol-emp option:selected').map(function () {
            return { UID: parseInt($(this).data('uid')), SubNoID: parseInt($(this).data('snid')) };
        }).get();
    }

    function bindButtons() {
        $('#pol-isall').on('change', toggleEmpSection);
        $('#pol-alldesc').on('change', toggleDescSection);

        $('#pol-provider').on('change', function () {
            loadTransTypes($(this).val());
            $('#pol-desc').empty();
        });

        $('#pol-transtype').on('change', function () {
            loadDescriptions($('#pol-provider').val(), $(this).val());
        });

        $('#btn-pol-add').on('click', function () {
            editMode = false; $('#pol-id').val('');
            $('#pol-provider,#pol-transtype,#pol-calltype,#pol-linetype').val('');
            $('#pol-isall,#pol-superimpose,#pol-alldesc').prop('checked', false);
            $('#pol-desc,#pol-emp').find('option').prop('selected', false);
            $('#emp-section').show(); $('#desc-section').show();
            $('#modal-pol-title').text('Add Policy'); modalPol.show();
        });

        $('#btn-pol-save').on('click', function () {
            const providerId = parseInt($('#pol-provider').val()) || 0;
            const callTypeId = parseInt($('#pol-calltype').val()) || 0;
            const lineTypeId = parseInt($('#pol-linetype').val()) || 0;
            const isAll      = $('#pol-isall').is(':checked');
            const isSupImp   = $('#pol-superimpose').is(':checked');
            const isAllDesc  = $('#pol-alldesc').is(':checked');

            if (!editMode && (!providerId || !callTypeId)) {
                TIS.toast('warning', 'Provider and Call Type are required'); return;
            }

            if (editMode) {
                const payload = {
                    ID: parseInt($('#pol-id').val()),
                    IsAll: isAll, IsSupImp: isSupImp,
                    Employees: isAll ? [] : collectEmployees()
                };
                $.post({ url: '/Setting/UpdatePolicy', data: JSON.stringify(payload), contentType: 'application/json' })
                    .done(function (res) {
                        if (res.myMessage === 'succ') { modalPol.hide(); TIS.toast('success', 'Updated'); loadPolicies(); }
                        else TIS.toast('error', res.myMessage);
                    }).fail(() => TIS.toast('error', 'Update failed'));
            } else {
                const descriptions = isAllDesc ? [] : $('#pol-desc option:selected').map(function () { return $(this).val(); }).get();
                const payload = {
                    ProviderID: providerId,
                    TransType:  $('#pol-transtype').val(),
                    CallTypeID: callTypeId,
                    LineTypeID: lineTypeId,
                    IsAll: isAll, IsSupImp: isSupImp, IsAllDesc: isAllDesc,
                    Descriptions: descriptions,
                    Employees: isAll ? [] : collectEmployees()
                };
                $.post({ url: '/Setting/AddPolicy', data: JSON.stringify(payload), contentType: 'application/json' })
                    .done(function (res) {
                        if (res.myMessage === 'succ') { modalPol.hide(); TIS.toast('success', 'Added'); loadPolicies(); }
                        else TIS.toast('error', res.myMessage);
                    }).fail(() => TIS.toast('error', 'Add failed'));
            }
        });

        $('#btn-pol-del-confirm').on('click', function () {
            $.post('/Setting/DeletePolicy?id=' + $('#del-pol-id').val())
                .done(function (res) {
                    if (res.myMessage === 'succ') { modalPolDel.hide(); TIS.toast('success', 'Deleted'); loadPolicies(); }
                    else TIS.toast('error', res.myMessage);
                });
        });
    }

})();

/* admin-delegate.js — DelegateBills page */
(function () {
    'use strict';

    let dtDlg;
    let empList = [];
    let editMode = false;

    const modalDlg    = new bootstrap.Modal('#modal-dlg');
    const modalDlgDel = new bootstrap.Modal('#modal-dlg-del');

    flatpickr('#dlg-start', { dateFormat: 'Y-m-d' });
    flatpickr('#dlg-end',   { dateFormat: 'Y-m-d' });

    $(function () { loadData(); bindButtons(); });

    function loadData() {
        $.get('/Admin/GetDelegate').done(function (res) {
            empList = res.empList || [];
            renderDelegateTable(res.dtSec || []);
            populateDropdowns(empList);
        }).fail(() => TIS.toast('error', 'Failed to load delegation data'));
    }

    function renderDelegateTable(data) {
        if (dtDlg) { dtDlg.clear().rows.add(data).draw(); return; }
        dtDlg = $('#dt-dlg').DataTable({
            data: data,
            columns: [
                { data: 'managerName' },
                { data: 'secretaryName' },
                { data: 'canIdentify', render: v => v ? '<span class="badge bg-success">Yes</span>' : '<span class="badge bg-secondary">No</span>' },
                { data: 'canApprove',  render: v => v ? '<span class="badge bg-success">Yes</span>' : '<span class="badge bg-secondary">No</span>' },
                { data: 'startDate', render: v => v ? v.substring(0,10) : '' },
                { data: 'endDate',   render: v => v ? v.substring(0,10) : '' },
                {
                    data: null, orderable: false,
                    render: (_, __, row) =>
                        `<button class="btn btn-xs btn-outline-primary me-1 btn-dlg-edit"
                            data-id="${row.delegateID}" data-mid="${row.managerID}" data-sid="${row.secretaryID}"
                            data-idt="${row.canIdentify}" data-app="${row.canApprove}"
                            data-start="${row.startDate||''}" data-end="${row.endDate||''}">Edit</button>
                         <button class="btn btn-xs btn-outline-danger btn-dlg-del" data-id="${row.delegateID}">Del</button>`
                }
            ],
            pageLength: 25
        });
        $('#dt-dlg').on('click', '.btn-dlg-edit', function () {
            const d = $(this).data();
            editMode = true; $('#dlg-id').val(d.id);
            $('#dlg-manager').val(d.mid); $('#dlg-secretary').val(d.sid);
            $('#dlg-identify').prop('checked', d.idt === true || d.idt === 'True');
            $('#dlg-approve').prop('checked',  d.app === true || d.app === 'True');
            document.getElementById('dlg-start')._flatpickr.setDate(d.start ? d.start.substring(0,10) : null);
            document.getElementById('dlg-end')._flatpickr.setDate(d.end   ? d.end.substring(0,10)   : null);
            $('#modal-dlg-title').text('Edit Delegation'); modalDlg.show();
        });
        $('#dt-dlg').on('click', '.btn-dlg-del', function () {
            $('#del-dlg-id').val($(this).data('id')); modalDlgDel.show();
        });
    }

    function populateDropdowns(emps) {
        $('#dlg-manager,#dlg-secretary').empty().append('<option value="">-- Select --</option>');
        emps.forEach(e => {
            const opt = `<option value="${e.uID}">${e.nAME} (${e.eMPLOYEENO})</option>`;
            $('#dlg-manager,#dlg-secretary').append(opt);
        });
    }

    function buildPayload() {
        return {
            DelegateID:  parseInt($('#dlg-id').val()) || 0,
            ManagerID:   parseInt($('#dlg-manager').val()),
            SecretaryID: parseInt($('#dlg-secretary').val()),
            CanIdentify: $('#dlg-identify').is(':checked'),
            CanApprove:  $('#dlg-approve').is(':checked'),
            StartDate:   $('#dlg-start').val() || null,
            EndDate:     $('#dlg-end').val()   || null
        };
    }

    function bindButtons() {
        $('#btn-dlg-add').on('click', function () {
            editMode = false; $('#dlg-id').val('');
            $('#dlg-manager,#dlg-secretary').val('');
            $('#dlg-identify,#dlg-approve').prop('checked', false);
            document.getElementById('dlg-start')._flatpickr.clear();
            document.getElementById('dlg-end')._flatpickr.clear();
            $('#modal-dlg-title').text('Add Delegation'); modalDlg.show();
        });

        $('#btn-dlg-save').on('click', function () {
            const payload = buildPayload();
            if (!payload.ManagerID || !payload.SecretaryID) { TIS.toast('warning', 'Manager and Secretary are required'); return; }
            const url = editMode ? '/Admin/UpdateDelegate' : '/Admin/SaveDelegate';
            $.post({ url, data: JSON.stringify(payload), contentType: 'application/json' })
                .done(function (res) {
                    if (res.myMessage && res.myMessage.startsWith('Error')) TIS.toast('error', res.myMessage);
                    else { modalDlg.hide(); TIS.toast('success', 'Saved'); loadData(); }
                }).fail(() => TIS.toast('error', 'Save failed'));
        });

        $('#btn-dlg-del-confirm').on('click', function () {
            $.post('/Admin/DeleteDelegate?id=' + $('#del-dlg-id').val())
                .done(function (res) {
                    if (res.myMessage && res.myMessage.startsWith('Error')) TIS.toast('error', res.myMessage);
                    else { modalDlgDel.hide(); TIS.toast('success', 'Removed'); loadData(); }
                });
        });
    }

})();

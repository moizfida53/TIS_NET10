/* setting-provider.js — Provider page */
(function () {
    'use strict';

    let dtProv;
    let editMode = false;

    const modalProv    = new bootstrap.Modal('#modal-prov');
    const modalProvDel = new bootstrap.Modal('#modal-prov-del');

    $(function () { loadData(); bindButtons(); });

    function loadData() {
        $.get('/Setting/GetProviders').done(function (res) {
            renderTable(res.providerList || []);
        }).fail(() => TIS.toast('error', 'Failed to load providers'));
    }

    function renderTable(data) {
        if (dtProv) { dtProv.clear().rows.add(data).draw(); return; }
        dtProv = $('#dt-prov').DataTable({
            data: data,
            columns: [
                { data: 'name' },
                { data: 'isVoip', render: v => v ? '<span class="badge bg-info">Yes</span>' : 'No' },
                { data: 'countryID' },
                {
                    data: null, orderable: false,
                    render: (_, __, row) =>
                        `<button class="btn btn-xs btn-outline-primary me-1 btn-prov-edit" data-id="${row.id}" data-name="${row.name}" data-voip="${row.isVoip}" data-country="${row.countryID}">Edit</button>
                         <button class="btn btn-xs btn-outline-danger btn-prov-del" data-id="${row.id}" data-name="${row.name}">Del</button>`
                }
            ],
            pageLength: 25
        });

        $('#dt-prov').on('click', '.btn-prov-edit', function () {
            const d = $(this).data();
            editMode = true;
            $('#prov-id').val(d.id); $('#prov-name').val(d.name);
            $('#prov-voip').prop('checked', d.voip === true || d.voip === 'True');
            $('#prov-country').val(d.country || 0);
            $('#modal-prov-title').text('Edit Provider'); modalProv.show();
        });

        $('#dt-prov').on('click', '.btn-prov-del', function () {
            $('#del-prov-id').val($(this).data('id'));
            $('#del-prov-name').text($(this).data('name'));
            modalProvDel.show();
        });
    }

    function bindButtons() {
        $('#btn-prov-add').on('click', function () {
            editMode = false; $('#prov-id').val(''); $('#prov-name').val('');
            $('#prov-voip').prop('checked', false); $('#prov-country').val(0);
            $('#modal-prov-title').text('Add Provider'); modalProv.show();
        });

        $('#btn-prov-save').on('click', function () {
            const name = $('#prov-name').val().trim();
            if (!name) { TIS.toast('warning', 'Provider name is required'); return; }
            const payload = {
                ID:        parseInt($('#prov-id').val()) || 0,
                Name:      name,
                IsVoip:    $('#prov-voip').is(':checked'),
                CountryID: parseInt($('#prov-country').val()) || 0
            };
            const url = editMode ? '/Setting/UpdateProvider' : '/Setting/AddProvider';
            $.post({ url, data: JSON.stringify(payload), contentType: 'application/json' })
                .done(function (res) {
                    if (res.myMessage === 'succ') { modalProv.hide(); TIS.toast('success', 'Saved'); loadData(); }
                    else TIS.toast('error', res.myMessage);
                }).fail(() => TIS.toast('error', 'Save failed'));
        });

        $('#btn-prov-del-confirm').on('click', function () {
            $.post('/Setting/DeleteProvider?id=' + $('#del-prov-id').val())
                .done(function (res) {
                    if (res.myMessage === 'succ') { modalProvDel.hide(); TIS.toast('success', 'Deleted'); loadData(); }
                    else TIS.toast('error', res.myMessage);
                });
        });
    }

})();

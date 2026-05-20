/* admin-package.js — Package page */
(function () {
    'use strict';

    let dtPkg, dtRoam;
    let providers = [], packages = [];
    let pkgEditMode = false, roamEditMode = false;
    let detailRows = [];

    const modalPkg    = new bootstrap.Modal('#modal-pkg');
    const modalPkgDel = new bootstrap.Modal('#modal-pkg-del');
    const modalRoam   = new bootstrap.Modal('#modal-roam');
    const modalRoamDel = new bootstrap.Modal('#modal-roam-del');

    flatpickr('#pkg-start', { dateFormat: 'Y-m-d' });

    $(function () { loadData(); loadRoaming(); bindButtons(); });

    function loadData() {
        $.get('/Admin/GetPkgData').done(function (res) {
            providers = res.dtPro || [];
            packages  = res.dtPkg || [];
            renderPkgTable(packages);
            populateProviders(providers);
        }).fail(() => TIS.toast('error', 'Failed to load package data'));
    }

    function loadRoaming() {
        $.get('/Admin/GetDataRoaming').done(function (res) {
            renderRoamTable(res.dtCountry || []);
        });
    }

    function renderPkgTable(data) {
        if (dtPkg) { dtPkg.clear().rows.add(data).draw(); return; }
        dtPkg = $('#dt-pkg').DataTable({
            data: data,
            columns: [
                { data: 'description' },
                { data: 'providerName' },
                { data: 'transType' },
                { data: 'startDate', render: v => v ? v.substring(0,10) : '' },
                { data: 'makeAllUnexpected', render: v => v ? '<span class="badge bg-warning text-dark">Yes</span>' : 'No' },
                {
                    data: null, orderable: false,
                    render: (_, __, row) =>
                        `<button class="btn btn-xs btn-outline-primary me-1 btn-pkg-edit" data-id="${row.pkgID}">Edit</button>
                         <button class="btn btn-xs btn-outline-danger btn-pkg-del" data-id="${row.pkgID}">Del</button>`
                }
            ],
            pageLength: 25
        });
        $('#dt-pkg').on('click', '.btn-pkg-edit', function () { openPkgEdit($(this).data('id')); });
        $('#dt-pkg').on('click', '.btn-pkg-del', function () { $('#del-pkg-id').val($(this).data('id')); modalPkgDel.show(); });
    }

    function renderRoamTable(data) {
        if (dtRoam) { dtRoam.clear().rows.add(data).draw(); return; }
        dtRoam = $('#dt-roam').DataTable({
            data: data,
            columns: [
                { data: 'countryName' },
                { data: 'operatorName' },
                {
                    data: null, orderable: false,
                    render: (_, __, row) =>
                        `<button class="btn btn-xs btn-outline-primary me-1 btn-roam-edit" data-id="${row.roamingID}" data-country="${row.countryName}" data-op="${row.operatorName}" data-pid="${row.providerID}" data-pkgid="${row.pkgID}">Edit</button>
                         <button class="btn btn-xs btn-outline-danger btn-roam-del" data-id="${row.roamingID}">Del</button>`
                }
            ]
        });
        $('#dt-roam').on('click', '.btn-roam-edit', function () {
            const d = $(this).data();
            roamEditMode = true; $('#roam-id').val(d.id); $('#roam-country').val(d.country); $('#roam-operator').val(d.op);
            $('#roam-provider').val(d.pid); $('#roam-pkg').val(d.pkgid);
            $('#modal-roam-title').text('Edit Roaming'); modalRoam.show();
        });
        $('#dt-roam').on('click', '.btn-roam-del', function () { $('#del-roam-id').val($(this).data('id')); modalRoamDel.show(); });
    }

    function populateProviders(pvs) {
        $('#pkg-provider,#roam-provider').empty().append('<option value="">-- Select Provider --</option>');
        pvs.forEach(p => {
            $('#pkg-provider,#roam-provider').append(`<option value="${p.providerID}">${p.providerName}</option>`);
        });
    }

    function openPkgEdit(pkgId) {
        pkgEditMode = true;
        const pkg = packages.find(p => p.pkgID == pkgId);
        if (!pkg) return;
        $('#pkg-id').val(pkgId);
        $('#pkg-provider').val(pkg.providerID);
        $('#pkg-unexpected').prop('checked', pkg.makeAllUnexpected);
        if (pkg.startDate) document.getElementById('pkg-start')._flatpickr.setDate(pkg.startDate.substring(0,10));
        detailRows = [];
        $('#pkg-detail-body').empty();
        // Load detail rows
        $.get('/Admin/GetPkgDetail?pkgId=' + pkgId).done(function (res) {
            (res.pkgDetail || []).forEach(d => addDetailRow({ transType: d.transType, description: d.description, expectedType: d.expectedType, amountLimit: d.amountLimit, detailID: d.detailID }));
            $('#modal-pkg-title').text('Edit Package'); modalPkg.show();
        });
    }

    function addDetailRow(data) {
        data = data || {};
        const idx = detailRows.length;
        detailRows.push(data);
        const row = `
        <tr data-idx="${idx}">
            <td><input type="text" class="form-control form-control-sm pkg-transtype" value="${data.transType||''}" /></td>
            <td><input type="text" class="form-control form-control-sm pkg-desc" value="${data.description||''}" /></td>
            <td>
                <select class="form-select form-select-sm pkg-exptype">
                    <option value="Expected" ${data.expectedType==='Expected'?'selected':''}>Expected</option>
                    <option value="Unexpected" ${data.expectedType==='Unexpected'?'selected':''}>Unexpected</option>
                </select>
            </td>
            <td><input type="number" step="0.01" class="form-control form-control-sm pkg-amt" value="${data.amountLimit||0}" /></td>
            <td><button type="button" class="btn btn-xs btn-outline-danger btn-rm-row">x</button></td>
        </tr>`;
        $('#pkg-detail-body').append(row);
        $('#pkg-detail-body').on('click', '.btn-rm-row', function () { $(this).closest('tr').remove(); });
    }

    function collectDetails() {
        return $('#pkg-detail-body tr').map(function () {
            return {
                TransType:    $(this).find('.pkg-transtype').val(),
                Description:  $(this).find('.pkg-desc').val(),
                ExpectedType: $(this).find('.pkg-exptype').val(),
                AmountLimit:  parseFloat($(this).find('.pkg-amt').val()) || 0
            };
        }).get();
    }

    function bindButtons() {
        $('#btn-pkg-add').on('click', function () {
            pkgEditMode = false; $('#pkg-id').val(''); $('#pkg-provider').val(''); $('#pkg-unexpected').prop('checked', false);
            document.getElementById('pkg-start')._flatpickr.clear();
            detailRows = []; $('#pkg-detail-body').empty();
            addDetailRow();
            $('#modal-pkg-title').text('Add Package'); modalPkg.show();
        });

        $('#btn-pkg-add-row').on('click', function () { addDetailRow(); });

        $('#btn-pkg-save').on('click', function () {
            const details = collectDetails();
            if (!$('#pkg-provider').val() || !details.length) { TIS.toast('warning', 'Provider and at least one detail row required'); return; }
            const payload = {
                Master: {
                    PkgID: parseInt($('#pkg-id').val()) || 0,
                    ProviderID: parseInt($('#pkg-provider').val()),
                    MakeAllUnexpected: $('#pkg-unexpected').is(':checked'),
                    StartDate: $('#pkg-start').val() || null,
                    CountryID: 0
                },
                Details: details
            };
            const url = pkgEditMode ? '/Admin/UpdatePackage' : '/Admin/AddPackage';
            $.post({ url, data: JSON.stringify(payload), contentType: 'application/json' })
                .done(function (res) {
                    if (res.myMessage && res.myMessage.startsWith('Error')) TIS.toast('error', res.myMessage);
                    else { modalPkg.hide(); TIS.toast('success', 'Saved'); loadData(); }
                });
        });

        $('#btn-pkg-del-confirm').on('click', function () {
            $.post('/Admin/DeletePackage?id=' + $('#del-pkg-id').val())
                .done(function (res) {
                    if (res.myMessage) TIS.toast('error', res.myMessage);
                    else { modalPkgDel.hide(); TIS.toast('success', 'Deleted'); loadData(); }
                });
        });

        // Roaming
        $('#btn-roam-add').on('click', function () {
            roamEditMode = false; $('#roam-id').val(''); $('#roam-country,#roam-operator').val('');
            $('#roam-provider,#roam-pkg').val('');
            // Populate pkg dropdown
            $('#roam-pkg').empty().append('<option value="">-- Select Package --</option>');
            packages.forEach(p => $('#roam-pkg').append(`<option value="${p.pkgID}">${p.description}</option>`));
            $('#modal-roam-title').text('Add Roaming Entry'); modalRoam.show();
        });

        $('#btn-roam-save').on('click', function () {
            const payload = {
                RoamingID: parseInt($('#roam-id').val()) || 0,
                CountryName: $('#roam-country').val(), OperatorName: $('#roam-operator').val(),
                ProviderID: parseInt($('#roam-provider').val()) || 0,
                PkgID: parseInt($('#roam-pkg').val()) || 0
            };
            const url = roamEditMode ? '/Admin/UpdateDataRoaming' : '/Admin/AddDataRoaming';
            $.post({ url, data: JSON.stringify(payload), contentType: 'application/json' })
                .done(function (res) {
                    if (res.myMessage && res.myMessage.startsWith('Error')) TIS.toast('error', res.myMessage);
                    else { modalRoam.hide(); TIS.toast('success', 'Saved'); loadRoaming(); }
                });
        });

        $('#btn-roam-del-confirm').on('click', function () {
            $.post('/Admin/DeleteDataRoaming?id=' + $('#del-roam-id').val())
                .done(function (res) {
                    if (res.myMessage && res.myMessage.startsWith('Error')) TIS.toast('error', res.myMessage);
                    else { modalRoamDel.hide(); TIS.toast('success', 'Deleted'); loadRoaming(); }
                });
        });
    }

})();

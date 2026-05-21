'use strict';

let pivotData = [];

document.addEventListener('DOMContentLoaded', () => {
    loadPivot();
    document.getElementById('btnSave').addEventListener('click', savePivot);
    document.getElementById('btnRestore').addEventListener('click', restorePivot);
});

async function loadPivot() {
    showLoader('Loading pivot data...');
    try {
        const res  = await fetch('/Pivot/GetPivot');
        const data = await res.json();
        hideLoader();
        if (data.Fail) { toastError('Failed to load pivot data.'); return; }
        pivotData = data.dtPivot ?? [];
        renderPivot(pivotData, {});
    } catch {
        hideLoader();
        toastError('Failed to load pivot data.');
    }
}

function renderPivot(data, opts) {
    const renderers = Object.assign(
        {},
        $.pivotUtilities.renderers,
        $.pivotUtilities.c3_renderers
    );

    const defaults = {
        renderers,
        vals:            ['amount'],
        rows:            ['mobile'],
        cols:            ['transType'],
        aggregatorName:  'Sum',
        rendererName:    'Table'
    };

    $('#pivotOutput').pivotUI(data, Object.assign(defaults, opts));
}

async function savePivot() {
    try {
        const config = $('#pivotOutput').data('pivotUIOptions');
        const copy   = JSON.parse(JSON.stringify(config));
        delete copy.aggregators;
        delete copy.renderers;

        const res = await fetch('/Pivot/Save', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json', 'RequestVerificationToken': getAntiForgeryToken() },
            body: JSON.stringify({ Object: JSON.stringify(copy) })
        });
        const txt = await res.json();
        if (txt === 'Success') toastSuccess('Layout saved.');
        else toastError('Save failed.');
    } catch {
        toastError('Save failed.');
    }
}

async function restorePivot() {
    try {
        const res  = await fetch('/Pivot/Restore');
        const data = await res.json();
        if (!data.dtPivot) { toastWarn('No saved layout found.'); return; }

        const opts = JSON.parse(data.dtPivot);
        renderPivot(pivotData, opts);
        toastSuccess('Layout restored.');
    } catch {
        toastError('Restore failed.');
    }
}

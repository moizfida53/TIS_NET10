'use strict';

const COLORS = [
    '#4e79a7','#f28e2b','#e15759','#76b7b2','#59a14f',
    '#edc948','#b07aa1','#ff9da7','#9c755f','#bab0ac'
];

const charts = {};
let currentYear = new Date().getFullYear();
let transTypes  = [];
let intCallTypes = [];

document.addEventListener('DOMContentLoaded', () => {
    fillYears();
    loadKpi();
    loadTransTypes();

    document.getElementById('selYear').addEventListener('change', onYearChange);
    document.getElementById('selTransType').addEventListener('change', () => loadChart2());
    document.getElementById('selIntCallType').addEventListener('change', () => loadChart6());
});

function fillYears() {
    const sel  = document.getElementById('selYear');
    const year = new Date().getFullYear();
    for (let y = year; y >= year - 7; y--) {
        const opt = new Option(y, y);
        if (y === year) opt.selected = true;
        sel.add(opt);
    }
    currentYear = year;
}

function onYearChange() {
    currentYear = parseInt(document.getElementById('selYear').value);
    loadKpi();
    loadChart1();
    loadChart2();
    loadChart4();
    loadChart6();
    loadCountryGrid();
}

async function loadKpi() {
    try {
        const res  = await fetch('/Dashboard/GetKpi');
        const data = await res.json();
        document.getElementById('kpiUnidentified').textContent = data.unidentifiedBills ?? '—';
        document.getElementById('kpiUnassigned').textContent   = data.unassignedAmount  ?? '—';
        document.getElementById('kpiApproval').textContent     = data.billsInApproval   ?? '—';
        document.getElementById('kpiSap').textContent          = data.sapAmount         ?? '—';
    } catch {
        console.error('KPI load failed');
    }
}

async function loadTransTypes() {
    try {
        const res  = await fetch('/Dashboard/GetTransTypes');
        const data = await res.json();
        transTypes = data.data ?? [];
        const sel  = document.getElementById('selTransType');
        transTypes.forEach(r => {
            const key = Object.values(r)[0];
            sel.add(new Option(key, key));
        });
    } catch { /* optional filter */ }

    loadChart1();
    loadChart2();
    loadChart4();
    loadChart6();
    loadCountryGrid();
}

// Generic: reads first column as label, rest as numeric series
function buildChartConfig(rows, type = 'bar') {
    if (!rows || rows.length === 0) return null;
    const keys     = Object.keys(rows[0]);
    const labelKey = keys[0];
    const valKeys  = keys.slice(1);

    return {
        type,
        data: {
            labels: rows.map(r => r[labelKey]),
            datasets: valKeys.map((k, i) => ({
                label: k,
                data: rows.map(r => parseFloat(r[k]) || 0),
                backgroundColor: COLORS[i % COLORS.length],
                borderColor:     COLORS[i % COLORS.length],
                fill: false,
                tension: 0.3
            }))
        },
        options: {
            responsive: true,
            plugins: { legend: { display: valKeys.length > 1 } },
            scales: type === 'bar' || type === 'line'
                ? { y: { beginAtZero: true } }
                : {}
        }
    };
}

function renderChart(id, config) {
    if (!config) return;
    if (charts[id]) charts[id].destroy();
    const ctx = document.getElementById(id)?.getContext('2d');
    if (!ctx) return;
    charts[id] = new Chart(ctx, config);
}

async function loadChart1() {
    try {
        const res  = await fetch(`/Dashboard/GetChart1?year=${currentYear}`);
        const data = await res.json();
        renderChart('chart1', buildChartConfig(data.data, 'bar'));
    } catch { }
}

async function loadChart2() {
    const transType = document.getElementById('selTransType').value;
    try {
        const res  = await fetch(`/Dashboard/GetChart2?year=${currentYear}&transType=${encodeURIComponent(transType)}`);
        const data = await res.json();
        renderChart('chart2', buildChartConfig(data.data, 'line'));
    } catch { }
}

async function loadChart4() {
    try {
        const res  = await fetch(`/Dashboard/GetChart4?year=${currentYear}`);
        const data = await res.json();
        renderChart('chart4', buildChartConfig(data.data, 'doughnut'));
    } catch { }
}

async function loadChart6() {
    const callType = document.getElementById('selIntCallType').value;
    try {
        const res  = await fetch(`/Dashboard/GetChart6?year=${currentYear}&callType=${encodeURIComponent(callType)}`);
        const data = await res.json();
        renderChart('chart6', buildChartConfig(data.data, 'bar'));
    } catch { }
}

async function loadCountryGrid() {
    try {
        const res  = await fetch(`/Dashboard/GetCountryGrid?year=${currentYear}`);
        const data = await res.json();
        const rows = data.data ?? [];
        if (!rows.length) return;

        const keys  = Object.keys(rows[0]);
        const thead = document.getElementById('tblCountryHead');
        const tbody = document.getElementById('tblCountryBody');

        thead.innerHTML = keys.map(k => `<th>${k}</th>`).join('');
        tbody.innerHTML = rows.map(r =>
            `<tr>${keys.map(k => `<td>${r[k] ?? ''}</td>`).join('')}</tr>`
        ).join('');

        if (!$.fn.DataTable.isDataTable('#tblCountry')) {
            $('#tblCountry').DataTable({ pageLength: 10, order: [] });
        }
    } catch { }
}

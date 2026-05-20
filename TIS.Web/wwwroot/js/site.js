// CSRF for all AJAX POSTs
const __token = document.querySelector('input[name="__RequestVerificationToken"]')?.value ?? '';
$.ajaxSetup({ beforeSend(xhr, s) { if (!/^(GET|HEAD|OPTIONS)$/.test(s.type)) xhr.setRequestHeader('RequestVerificationToken', __token); } });

// Global AJAX error
$(document).ajaxError(function (ev, xhr) {
    if (xhr.status === 401) { window.location.href = '/'; return; }
    Swal.fire({ toast: true, icon: 'error', position: 'top-end', timer: 3500, title: 'Error ' + xhr.status, showConfirmButton: false });
});

// Helpers
window.TIS = {
    toast: (icon, title) => Swal.fire({ toast: true, icon, position: 'top-end', timer: 2500, title, showConfirmButton: false }),
    confirm: (msg, fn) => Swal.fire({ icon: 'warning', title: 'Confirm', text: msg, showCancelButton: true, confirmButtonColor: '#dc3545', confirmButtonText: 'Yes' }).then(r => r.isConfirmed && fn())
};

// DataTables defaults
$.fn.dataTable.defaults.language = { search: '', searchPlaceholder: 'Search...' };

/* adtest.js */
document.addEventListener('DOMContentLoaded', function () {

    var btnTest         = document.getElementById('btn-test');
    var btnSearch       = document.getElementById('btn-search');
    var usernameEl      = document.getElementById('username-input');
    var btnUpdateMobile = document.getElementById('btn-update-mobile');
    var mobileUsernameEl = document.getElementById('mobile-username-input');
    var mobileNumberEl  = document.getElementById('mobile-number-input');

    btnTest.addEventListener('click', testConnection);
    btnSearch.addEventListener('click', searchUser);
    usernameEl.addEventListener('keydown', function (e) { if (e.key === 'Enter') searchUser(); });
    btnUpdateMobile.addEventListener('click', updateMobile);
    mobileNumberEl.addEventListener('keydown', function (e) { if (e.key === 'Enter') updateMobile(); });

    function setLoading(btnId, spinnerId, loading) {
        document.getElementById(btnId).disabled = loading;
        var sp = document.getElementById(spinnerId);
        sp.classList.toggle('active', loading);
    }

    function escHtml(str) {
        if (!str) return '';
        return str.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/"/g, '&quot;');
    }

    function testConnection() {
        setLoading('btn-test', 'conn-spinner', true);
        var statusEl = document.getElementById('conn-status');
        var msgEl    = document.getElementById('conn-msg');
        statusEl.className = '';

        fetch(btnTest.dataset.url, { method: 'POST', headers: { 'Content-Type': 'application/x-www-form-urlencoded', 'RequestVerificationToken': document.querySelector('meta[name="csrf-token"]')?.content || '' } })
            .then(r => r.json())
            .then(data => {
                msgEl.textContent   = data.message;
                statusEl.className  = 'visible ' + (data.success ? 'success' : 'fail');
            })
            .catch(err => {
                msgEl.textContent  = 'FAIL — Network error: ' + err.message;
                statusEl.className = 'visible fail';
            })
            .finally(() => setLoading('btn-test', 'conn-spinner', false));
    }

    function searchUser() {
        var username = usernameEl.value.trim();
        var box = document.getElementById('result-box');
        box.innerHTML = '';
        if (!username) { box.innerHTML = '<div class="error-msg">Please enter a username before searching.</div>'; return; }

        setLoading('btn-search', 'search-spinner', true);
        fetch(btnSearch.dataset.url, {
            method: 'POST',
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
            body: 'username=' + encodeURIComponent(username)
        })
            .then(r => r.json())
            .then(data => {
                if (!data.success) { box.innerHTML = '<div class="error-msg">' + escHtml(data.message) + '</div>'; return; }
                var d = data.data;
                box.innerHTML =
                    '<table class="result-table">' +
                    '<tr><td>Employee Name</td><td>' + (escHtml(d.displayName) || '<span class="no-data">&mdash;</span>') + '</td></tr>' +
                    '<tr><td>Email Address</td><td>' + (escHtml(d.email) || '<span class="no-data">&mdash;</span>') + '</td></tr>' +
                    '<tr><td>Department</td><td>' + (escHtml(d.department) || '<span class="no-data">&mdash;</span>') + '</td></tr>' +
                    '<tr><td>Employee Number</td><td>' + (escHtml(d.employeeNumber) || '<span class="no-data">&mdash;</span>') + '</td></tr>' +
                    '<tr><td>Username</td><td>' + (escHtml(d.samAccount) || '<span class="no-data">&mdash;</span>') + '</td></tr>' +
                    '</table>';
            })
            .catch(err => { box.innerHTML = '<div class="error-msg">Network error: ' + escHtml(err.message) + '</div>'; })
            .finally(() => setLoading('btn-search', 'search-spinner', false));
    }

    function updateMobile() {
        var username     = mobileUsernameEl.value.trim();
        var mobileNumber = mobileNumberEl.value.trim();
        var statusEl = document.getElementById('mobile-status');
        var msgEl    = document.getElementById('mobile-msg');
        statusEl.className = '';

        if (!username) { msgEl.textContent = 'Please enter a username.'; statusEl.className = 'visible fail'; return; }
        if (!mobileNumber) { msgEl.textContent = 'Please enter a mobile number.'; statusEl.className = 'visible fail'; return; }

        setLoading('btn-update-mobile', 'update-spinner', true);
        fetch(btnUpdateMobile.dataset.url, {
            method: 'POST',
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
            body: 'username=' + encodeURIComponent(username) + '&mobileNumber=' + encodeURIComponent(mobileNumber)
        })
            .then(r => r.json())
            .then(data => {
                msgEl.textContent  = data.message;
                statusEl.className = 'visible ' + (data.success ? 'success' : 'fail');
            })
            .catch(err => {
                msgEl.textContent  = 'FAIL — Network error: ' + escHtml(err.message);
                statusEl.className = 'visible fail';
            })
            .finally(() => setLoading('btn-update-mobile', 'update-spinner', false));
    }
});

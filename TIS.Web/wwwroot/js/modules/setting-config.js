/* setting-config.js — Config page */
(function () {
    'use strict';

    $(function () { loadConfig(); bindButtons(); });

    function loadConfig() {
        $.get('/Setting/GetConfig').done(function (res) {
            const c = res.dtConfig;
            if (!c) return;
            $('#cfg-emp-reminder').val(c.empReminder);
            $('#cfg-fb-reminder').val(c.forceBillReminder);
            $('#cfg-mgr-reminder').val(c.mgrComplaintReminder);
            $('#cfg-lm-reminder').val(c.lmReminder);
            $('#cfg-smtp').val(c.smtpSettings);
            $('#cfg-admin-email').val(c.adminEmail);
            $('#cfg-host-url').val(c.hostUrl);
            $('#cfg-super-grade').val(c.superGrade);
            $('#cfg-enable-grade').prop('checked', c.enableGrade);
            $('#cfg-dnt-send-email').prop('checked', c.notSendMail);
            $('#cfg-hide-per-calls').prop('checked', c.hidePersonalCalls);
            $('#cfg-gm-app').prop('checked', c.skipGMApproval);
            $('#cfg-enable-discrepancy').prop('checked', c.enableDiscrepancy);
            $('#cfg-skip-app-bus-zero').prop('checked', c.skipApprovalBuss);
            $('#cfg-ded-bus-charges').prop('checked', c.dedBussinessCharges);
            $('#cfg-zero-unlimited').prop('checked', c.businessZeroAsUnlimited);
            $('#cfg-alw-wav').prop('checked', c.allowWaiver);
            $('#cfg-enable-delete').prop('checked', c.deleteBut);
            $('#cfg-alw-train-fb').prop('checked', c.allowTrainForceBill);
            $('#cfg-hide-allowance').prop('checked', c.hideAllowanceLimit);
            $('#cfg-hide-personal').prop('checked', c.hidePersonalLimit);
        }).fail(() => TIS.toast('error', 'Failed to load configuration'));
    }

    function buildPayload() {
        return {
            EmpReminder:          parseInt($('#cfg-emp-reminder').val()) || 0,
            ForceBillReminder:    parseInt($('#cfg-fb-reminder').val())  || 0,
            MgrComplaintReminder: parseInt($('#cfg-mgr-reminder').val()) || 0,
            LMReminder:           parseInt($('#cfg-lm-reminder').val())  || 0,
            SMTPSettings:         $('#cfg-smtp').val(),
            AdminEmail:           $('#cfg-admin-email').val(),
            HostUrl:              $('#cfg-host-url').val(),
            SuperGrade:           parseInt($('#cfg-super-grade').val()) || 0,
            EnableGrade:          $('#cfg-enable-grade').is(':checked'),
            NotSendMail:          $('#cfg-dnt-send-email').is(':checked'),
            HidePersonalCalls:    $('#cfg-hide-per-calls').is(':checked'),
            SkipGMApproval:       $('#cfg-gm-app').is(':checked'),
            EnableDiscrepancy:    $('#cfg-enable-discrepancy').is(':checked'),
            SkipApprovalBuss:     $('#cfg-skip-app-bus-zero').is(':checked'),
            DedBussinessCharges:  $('#cfg-ded-bus-charges').is(':checked'),
            BusinessZeroAsUnlimited: $('#cfg-zero-unlimited').is(':checked'),
            AllowWaiver:          $('#cfg-alw-wav').is(':checked'),
            DeleteBut:            $('#cfg-enable-delete').is(':checked'),
            AllowTrainForceBill:  $('#cfg-alw-train-fb').is(':checked'),
            HideAllowanceLimit:   $('#cfg-hide-allowance').is(':checked'),
            HidePersonalLimit:    $('#cfg-hide-personal').is(':checked')
        };
    }

    function bindButtons() {
        $('#btn-cfg-save').on('click', function () {
            $.post({ url: '/Setting/SaveConfig', data: JSON.stringify(buildPayload()), contentType: 'application/json' })
                .done(function (res) {
                    if (res.myMessage === 'succ') TIS.toast('success', 'Configuration saved');
                    else TIS.toast('error', res.myMessage);
                }).fail(() => TIS.toast('error', 'Save failed'));
        });

        $('#btn-apply-policy').on('click', function () {
            $.post('/Setting/ApplyPolicy')
                .done(function (res) {
                    if (res.myMessage === 'succ') TIS.toast('success', 'Policy applied');
                    else TIS.toast('error', res.myMessage);
                });
        });
    }

})();

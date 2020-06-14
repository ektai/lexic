/*
 *
 * (c) Copyright Ascensio System Limited 2010-2020
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ektai Solutions LTD expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ektai Solutions LTD by email at sales@lexic.xyz
 *
 * The interactive user interfaces in modified source and object code versions of LEXIC must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original LEXIC logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by LEXIC" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/


using ASC.Notify.Model;

namespace ASC.Web.Studio.Core.Notify
{
    static class Actions
    {
        public static readonly INotifyAction AdminNotify = new NotifyAction("admin_notify", "admin notifications");
        public static readonly INotifyAction PeriodicNotify = new NotifyAction("periodic_notify", "periodic notifications");

        public static readonly INotifyAction SelfProfileUpdated = new NotifyAction("self_profile_updated", "self profile updated");
        public static readonly INotifyAction UserHasJoin = new NotifyAction("user_has_join", "user has join");
        public static readonly INotifyAction UserMessageToAdmin = new NotifyAction("for_admin_notify", "for_admin_notify");
        public static readonly INotifyAction VoipWarning = new NotifyAction("admin_voip_warning", "admin_voip_warning");
        public static readonly INotifyAction VoipBlocked = new NotifyAction("admin_voip_blocked", "admin_voip_blocked");
        public static readonly INotifyAction RequestTariff = new NotifyAction("request_tariff", "request_tariff");
        public static readonly INotifyAction RequestLicense = new NotifyAction("request_license", "request_license");

        public static readonly INotifyAction YourProfileUpdated = new NotifyAction("profile_updated", "profile updated");
        public static readonly INotifyAction JoinUsers = new NotifyAction("join", "join users");
        public static readonly INotifyAction SendWhatsNew = new NotifyAction("send_whats_new", "send whats new");
        public static readonly INotifyAction BackupCreated = new NotifyAction("backup_created", "backup created");
        public static readonly INotifyAction RestoreStarted = new NotifyAction("restore_started", "restore_started");
        public static readonly INotifyAction RestoreCompleted = new NotifyAction("restore_completed", "restore_completed");
        public static readonly INotifyAction PortalDeactivate = new NotifyAction("portal_deactivate", "portal deactivate");
        public static readonly INotifyAction PortalDelete = new NotifyAction("portal_delete", "portal delete");
        public static readonly INotifyAction PortalDeleteSuccessV10 = new NotifyAction("portal_delete_success_v10");
        
        public static readonly INotifyAction ProfileDelete = new NotifyAction("profile_delete", "profile_delete");
        public static readonly INotifyAction ProfileHasDeletedItself = new NotifyAction("profile_has_deleted_itself", "profile_has_deleted_itself");
        public static readonly INotifyAction ReassignsCompleted = new NotifyAction("reassigns_completed", "reassigns_completed");
        public static readonly INotifyAction ReassignsFailed = new NotifyAction("reassigns_failed", "reassigns_failed");
        public static readonly INotifyAction RemoveUserDataCompleted = new NotifyAction("remove_user_data_completed", "remove_user_data_completed");
        public static readonly INotifyAction RemoveUserDataCompletedCustomMode = new NotifyAction("remove_user_data_completed_custom_mode");
        public static readonly INotifyAction RemoveUserDataFailed = new NotifyAction("remove_user_data_failed", "remove_user_data_failed");
        public static readonly INotifyAction DnsChange = new NotifyAction("dns_change", "dns_change");

        public static readonly INotifyAction ConfirmOwnerChange = new NotifyAction("owner_confirm_change", "owner_confirm_change");
        public static readonly INotifyAction ActivateEmail = new NotifyAction("activate_email", "activate_email");
        public static readonly INotifyAction EmailChange = new NotifyAction("change_email", "change_email");
        public static readonly INotifyAction PasswordChange = new NotifyAction("change_password", "change_password");
        public static readonly INotifyAction PhoneChange = new NotifyAction("change_phone", "change_phone");
        public static readonly INotifyAction TfaChange = new NotifyAction("change_tfa", "change_tfa");
        public static readonly INotifyAction MigrationPortalStart = new NotifyAction("migration_start", "migration start");
        public static readonly INotifyAction MigrationPortalSuccess = new NotifyAction("migration_success", "migration success");
        public static readonly INotifyAction MigrationPortalError = new NotifyAction("migration_error", "migration error");
        public static readonly INotifyAction MigrationPortalServerFailure = new NotifyAction("migration_server_failure", "migration_server_failure");
        public static readonly INotifyAction PortalRename = new NotifyAction("portal_rename", "portal_rename");

        public static readonly INotifyAction MailboxCreated = new NotifyAction("mailbox_created");
        public static readonly INotifyAction MailboxWithoutSettingsCreated = new NotifyAction("mailbox_without_settings_created");
        public static readonly INotifyAction MailboxPasswordChanged = new NotifyAction("mailbox_password_changed");

        public static readonly INotifyAction SaasAdminActivationV10 = new NotifyAction("saas_admin_activation_v10");
        public static readonly INotifyAction EnterpriseAdminActivationV10 = new NotifyAction("enterprise_admin_activation_v10");
        public static readonly INotifyAction EnterpriseWhitelabelAdminActivationV10 = new NotifyAction("enterprise_whitelabel_admin_activation_v10");
        public static readonly INotifyAction OpensourceAdminActivation = new NotifyAction("opensource_admin_activation");

        public static readonly INotifyAction SaasAdminWelcomeV10 = new NotifyAction("saas_admin_welcome_v10");
        public static readonly INotifyAction EnterpriseAdminWelcomeV10 = new NotifyAction("enterprise_admin_welcome_v10");
        public static readonly INotifyAction EnterpriseWhitelabelAdminWelcomeV10 = new NotifyAction("enterprise_whitelabel_admin_welcome_v10");

        public static readonly INotifyAction SaasUserActivationV10 = new NotifyAction("saas_user_activation_v10");
        public static readonly INotifyAction EnterpriseUserActivationV10 = new NotifyAction("enterprise_user_activation_v10");
        public static readonly INotifyAction EnterpriseWhitelabelUserActivationV10 = new NotifyAction("enterprise_whitelabel_user_activation_v10");

        public static readonly INotifyAction SaasUserWelcomeV10 = new NotifyAction("saas_user_welcome_v10");
        public static readonly INotifyAction EnterpriseUserWelcomeV10 = new NotifyAction("enterprise_user_welcome_v10");
        public static readonly INotifyAction EnterpriseWhitelabelUserWelcomeV10 = new NotifyAction("enterprise_whitelabel_user_welcome_v10");
        public static readonly INotifyAction EnterpriseWhitelabelUserWelcomeCustomMode = new NotifyAction("enterprise_whitelabel_user_welcome_custom_mode");

        public static readonly INotifyAction SaasGuestActivationV10 = new NotifyAction("saas_guest_activation_v10");
        public static readonly INotifyAction EnterpriseGuestActivationV10 = new NotifyAction("enterprise_guest_activation_v10");
        public static readonly INotifyAction EnterpriseWhitelabelGuestActivationV10 = new NotifyAction("enterprise_whitelabel_guest_activation_v10");

        public static readonly INotifyAction SaasGuestWelcomeV10 = new NotifyAction("saas_guest_welcome_v10");
        public static readonly INotifyAction EnterpriseGuestWelcomeV10 = new NotifyAction("enterprise_guest_welcome_v10");
        public static readonly INotifyAction EnterpriseWhitelabelGuestWelcomeV10 = new NotifyAction("enterprise_whitelabel_guest_welcome_v10");

        public static readonly INotifyAction EnterpriseAdminCustomizePortalV10 = new NotifyAction("enterprise_admin_customize_portal_v10");
        public static readonly INotifyAction EnterpriseWhitelabelAdminCustomizePortalV10 = new NotifyAction("enterprise_whitelabel_admin_customize_portal_v10");
        public static readonly INotifyAction EnterpriseAdminInviteTeammatesV10 = new NotifyAction("enterprise_admin_invite_teammates_v10");
        public static readonly INotifyAction EnterpriseAdminWithoutActivityV10 = new NotifyAction("enterprise_admin_without_activity_v10");
        public static readonly INotifyAction EnterpriseAdminUserDocsTipsV10 = new NotifyAction("enterprise_admin_user_docs_tips_v10");
        public static readonly INotifyAction EnterpriseAdminUserAppsTipsV10 = new NotifyAction("enterprise_admin_user_apps_tips_v10");

        public static readonly INotifyAction EnterpriseAdminTrialWarningBefore7V10 = new NotifyAction("enterprise_admin_trial_warning_before7_v10");
        public static readonly INotifyAction EnterpriseAdminTrialWarningV10 = new NotifyAction("enterprise_admin_trial_warning_v10");

        public static readonly INotifyAction EnterpriseAdminPaymentWarningBefore7V10 = new NotifyAction("enterprise_admin_payment_warning_before7_v10");
        public static readonly INotifyAction EnterpriseWhitelabelAdminPaymentWarningBefore7V10 = new NotifyAction("enterprise_whitelabel_admin_payment_warning_before7_v10");
        public static readonly INotifyAction EnterpriseAdminPaymentWarningV10 = new NotifyAction("enterprise_admin_payment_warning_v10");
        public static readonly INotifyAction EnterpriseWhitelabelAdminPaymentWarningV10 = new NotifyAction("enterprise_whitelabel_admin_payment_warning_v10");

        public static readonly INotifyAction SaasAdminInviteTeammatesV10 = new NotifyAction("saas_admin_invite_teammates_v10");
        public static readonly INotifyAction SaasAdminWithoutActivityV10 = new NotifyAction("saas_admin_without_activity_v10");
        public static readonly INotifyAction SaasAdminUserDocsTipsV10 = new NotifyAction("saas_admin_user_docs_tips_v10");
        public static readonly INotifyAction SaasAdminUserComfortTipsV10 = new NotifyAction("saas_admin_user_comfort_tips_v10");
        public static readonly INotifyAction SaasAdminUserAppsTipsV10 = new NotifyAction("saas_admin_user_apps_tips_v10");

        public static readonly INotifyAction SaasAdminTrialWarningBefore5V10 = new NotifyAction("saas_admin_trial_warning_before5_v10");
        public static readonly INotifyAction SaasAdminTrialWarningV10 = new NotifyAction("saas_admin_trial_warning_v10");
        public static readonly INotifyAction SaasAdminTrialWarningAfter5V10 = new NotifyAction("saas_admin_trial_warning_after5_v10");
        public static readonly INotifyAction SaasAdminTrialWarningAfter30V10 = new NotifyAction("saas_admin_trial_warning_after30_v10");
        public static readonly INotifyAction SaasAdminTrialWarningAfterHalfYearV10 = new NotifyAction("saas_admin_trial_warning_after_half_year_v10");

        public static readonly INotifyAction SaasAdminPaymentWarningBefore5V10 = new NotifyAction("saas_admin_payment_warning_before5_v10");
        public static readonly INotifyAction SaasAdminPaymentWarningAfter1V10 = new NotifyAction("saas_admin_payment_warning_after1_v10");

        public static readonly INotifyAction SaasAdminPaymentAfterMonthlySubscriptionsV10 = new NotifyAction("saas_admin_payment_after_monthly_subscriptions_v10");

        public static readonly INotifyAction OpensourceAdminSecurityTips = new NotifyAction("opensource_admin_security_tips");
        public static readonly INotifyAction OpensourceAdminDocsTips = new NotifyAction("opensource_admin_docs_tips");

        public static readonly INotifyAction PersonalActivate = new NotifyAction("personal_activate");
        public static readonly INotifyAction PersonalAfterRegistration1 = new NotifyAction("personal_after_registration1");
        public static readonly INotifyAction PersonalAfterRegistration7 = new NotifyAction("personal_after_registration7");
        public static readonly INotifyAction PersonalAfterRegistration14 = new NotifyAction("personal_after_registration14");
        public static readonly INotifyAction PersonalAfterRegistration21 = new NotifyAction("personal_after_registration21");
        public static readonly INotifyAction PersonalAfterRegistration28 = new NotifyAction("personal_after_registration28");
        public static readonly INotifyAction PersonalConfirmation = new NotifyAction("personal_confirmation");
        public static readonly INotifyAction PersonalPasswordChange = new NotifyAction("personal_change_password");
        public static readonly INotifyAction PersonalEmailChange = new NotifyAction("personal_change_email");
        public static readonly INotifyAction PersonalProfileDelete = new NotifyAction("personal_profile_delete");

        public static readonly INotifyAction PersonalCustomModeAfterRegistration1 = new NotifyAction("personal_custom_mode_after_registration1");
        public static readonly INotifyAction PersonalCustomModeAfterRegistration7 = new NotifyAction("personal_custom_mode_after_registration7");
        public static readonly INotifyAction PersonalCustomModeConfirmation = new NotifyAction("personal_custom_mode_confirmation");
        public static readonly INotifyAction PersonalCustomModePasswordChange = new NotifyAction("personal_custom_mode_change_password");
        public static readonly INotifyAction PersonalCustomModeEmailChange = new NotifyAction("personal_custom_mode_change_email");
        public static readonly INotifyAction PersonalCustomModeProfileDelete = new NotifyAction("personal_custom_mode_profile_delete");
    }
}
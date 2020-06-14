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


window.blankPages = (function($) {
    var page;

    var init = function() {
        page = $('#blankPage');
    };

    function showEmptyAccounts() {
        var buttons = [{
                text: MailScriptResource.EmptyScrAccountsButton,
                cssClass: "addFirstElement",
                handler: function () {
                    ASC.Controls.AnchorController.move('#accounts');
                    accountsModal.addBox();
                    return false;
                },
                href: null
            }];

        if (ASC.Mail.Constants.COMMON_DOMAIN_AVAILABLE)
            buttons.push({
                text: MailScriptResource.CreateMailboxBtn,
                cssClass: "addFirstElement",
                handler: function () {
                    ASC.Controls.AnchorController.move('#accounts');
                    accountsModal.addMailbox();
                    return false;
                },
                href: null
            });

        showPage(
            "accountsEmptyScreen",
            MailScriptResource.EmptyScrAccountsHeader,
            MailScriptResource.EmptyScrAccountsDescription,
            'accounts',
            buttons
        );
    }

    function showEmptyTags() {
        var buttons = [{
            text: MailScriptResource.EmptyScrTagsButton,
            cssClass: "addFirstElement",
            handler: function() {
                tagsModal.showCreate();
                return false;
            },
            href: null
        }];

        showPage(
            "tagsEmptyScreen",
            MailScriptResource.EmptyScrTagsHeader,
            MailScriptResource.EmptyScrTagsDescription,
            'tags',
            buttons
        );
    }

    function showEmptyUserFolders() {
        var buttons = [{
            text: MailScriptResource.EmptyScrUserFoldersButton,
            cssClass: "addFirstElement",
            handler: function () {
                userFoldersPage.createFolder();
                return false;
            },
            href: null
        }];

        showPage(
            "userFoldersEmptyScreen",
            MailScriptResource.EmptyScrUserFoldersHeader,
            MailScriptResource.EmptyScrUserFoldersDescription,
            'userfolder',
            buttons
        );
    }

    function showEmptyFilters() {
        var buttons = [{
            text: MailScriptResource.EmptyScrFiltersButton,
            cssClass: "addFirstElement",
            handler: function () {
                filtersPage.createFilter();
                return false;
            },
            href: null
        }];

        showPage(
            "filtersEmptyScreen",
            MailScriptResource.EmptyScrFiltersHeader,
            MailScriptResource.EmptyScrFiltersDescription,
            'userFilter',
            buttons
        );
    }

    function showNoLettersFilter() {
        var buttons = [{
            text: MailScriptResource.ResetFilterButton,
            cssClass: "clearFilterButton",
            handler: function() {
                folderFilter.reset();
                return false;
            },
            href: null
        }];

        showPage(
            "filterNoLettersEmptyScreen",
            MailScriptResource.NoLettersFilterHeader,
            MailScriptResource.NoLettersFilterDescription,
            'filter',
            buttons
        );
    }

    function showEmptyFolder() {
        var header = null;
        var description = null;
        var imgClass = null;

        var buttons = [{
            text: null,
            cssClass: "addFirstElement",
            handler: function () {
                messagePage.compose();
                return false;
            },
            href: null
        }];

        if (TMMail.pageIs('inbox')) {
            header = MailScriptResource.EmptyInboxHeader;
            description = MailScriptResource.EmptyInboxDescription;
            imgClass = 'inbox';
            buttons[0].text = MailScriptResource.EmptyInboxButton;
        } else if (TMMail.pageIs('sent')) {
            header = MailScriptResource.EmptySentHeader;
            description = MailScriptResource.EmptySentDescription;
            imgClass = 'sent';
            buttons[0].text = MailScriptResource.EmptySentButton;
        } else if (TMMail.pageIs('drafts')) {
            header = MailScriptResource.EmptyDraftsHeader;
            description = MailScriptResource.EmptyDraftsDescription;
            imgClass = 'drafts';
            buttons[0].text = MailScriptResource.EmptyDraftsButton;
        } else if (TMMail.pageIs('templates')) {
            header = MailScriptResource.EmptyTemplatesHeader;
            description = MailScriptResource.EmptyTemplatesDescription;
            imgClass = 'drafts';
            buttons[0].text = MailScriptResource.EmptyTemplatesButton;
        } else if (TMMail.pageIs('trash')) {
            header = MailScriptResource.EmptyTrashHeader;
            description = MailScriptResource.EmptyTrashDescription;
            imgClass = 'trash';
            buttons = [];
        } else if (TMMail.pageIs('spam')) {
            header = MailScriptResource.EmptySpamHeader;
            description = MailScriptResource.EmptySpamDescription;
            imgClass = 'spam';
            buttons = [];
        } else if (TMMail.pageIs('userfolder')) { 
            header = MailScriptResource.EmptyUserFolderHeader; 
            description = MailScriptResource.EmptyUserFolderDescription;
            imgClass = 'inbox'; // TODO: Change to userfolder
            buttons = [];
        }

        showPage("folderEmptyScreen", header, description, imgClass, buttons);
    }

    function showEmptyCrmContacts() {
        var buttons = [{
            text: MailScriptResource.EmptyScrCrmButton,
            cssClass: "addFirstElement",
            handler: null,
            href: "/products/crm/"
        }];

        showPage(
            "crmContactsEmptyScreen",
            MailScriptResource.EmptyScrCrmHeader,
            MailScriptResource.EmptyScrCrmDescription,
            'contacts',
            buttons
        );
    }

    function showEmptyMailContacts() {
        var buttons = [{
            text: MailScriptResource.CreateContactButton,
            cssClass: "addFirstElement",
            handler: function () {
                editContactModal.show(null, true);
                return false;
            },
            href: null
        }];

        showPage(
            "mailContactsEmptyScreen",
            MailScriptResource.EmptyScrCrmHeader,
            MailScriptResource.EmptyScrMailContactsDescription,
            'contacts',
            buttons
        );
    }

    function showNoCrmContacts() {
        var buttons = [{
            text: MailScriptResource.ResetFilterButton,
            cssClass: "clearFilterButton",
            handler: function () {
                contactsPage.resetFilter();
                return false;
            },
            href: null
        }];

        showPage(
            "filterNoCrmContactsEmptyScreen",
            MailScriptResource.ResetCrmContactsFilterHeader,
            MailScriptResource.ResetCrmContactsFilterDescription,
            'filter',
            buttons
        );
    }

    function showNoTlContacts() {
        var buttons = [{
            text: MailScriptResource.ResetFilterButton,
            cssClass: "clearFilterButton",
            handler: function () {
                contactsPage.resetFilter();
                return false;
            },
            href: null
        }];

        showPage(
            "filterNoTlContactsEmptyScreen",
            MailScriptResource.ResetTlContactsFilterHeader,
            MailScriptResource.ResetTlContactsFilterDescription,
            'filter',
            buttons
        );
    }

    function showNoMailContacts() {
        var buttons = [{
            text: MailScriptResource.ResetFilterButton,
            cssClass: "clearFilterButton",
            handler: function () {
                contactsPage.resetFilter();
                return false;
            },
            href: null
        }];

        showPage(
            "filterNoMailContactsEmptyScreen",
            MailScriptResource.ResetMailContactsFilterHeader,
            MailScriptResource.ResetMailContactsFilterDescription,
            'filter',
            buttons
        );
    }

    function showNoMailDomains() {
        var buttons = [{
            text: window.MailAdministrationResource.NoDomainSetUpButton,
            cssClass: "addFirstElement",
            handler: function () {
                createDomainModal.show(administrationManager.getServerInfo());
                return false;
            },
            href: null
        }];

        if (ASC.Resources.Master.Standalone) {
            buttons.push(
            {
                text: window.MailAdministrationResource.MigrateFromMSExchangeButton,
                cssClass: "linkMseFaq",
                handler: null,
                href: ASC.Resources.Master.HelpLink + "/server/docker/enterprise/migrate-from-exchange.aspx",
                skipNewLine: true,
                openNewTab: true
            });
        }

        showPage(
            "domainsEmptyScreen",
            window.MailAdministrationResource.NoDomainSetUpHeader,
            window.MailAdministrationResource.NoDomainSetUpDescription,
            'domains',
            buttons
        );
    }

    //buttons = [{ text: "", cssClass: "", handler: function(){}, href: "" }]
    function showPage(id, header, description, imgClass, buttons) {

        var buttonHtml = undefined;

        if (buttons) {
            var tmpl = $.template("emptyScrButtonTmpl");
            buttonHtml = tmpl($, {data : { buttons: buttons }});
        }

        var screen = $.tmpl("emptyScrTmpl",
            {
                ID: id,
                ImgCss: imgClass,
                Header: header,
                Describe: TMMail.htmlEncode(description),
                ButtonHTML: buttonHtml
            });

        page.empty().html(screen);

        var btnArray = page.find("#{0} .emptyScrBttnPnl a".format(id));
        $.each(btnArray, function (index, value) {
            if (buttons[index] && buttons[index].handler) {
                $(value).click(buttons[index].handler);
            }
        });

        page.show();
        TMMail.scrollTop();
    }

    function hide() {
        page.hide();
    }

    return {
        init: init,
        showEmptyAccounts: showEmptyAccounts,
        showNoLettersFilter: showNoLettersFilter,
        showEmptyFolder: showEmptyFolder,
        showEmptyCrmContacts: showEmptyCrmContacts,
        showEmptyMailContacts: showEmptyMailContacts,
        showNoCrmContacts: showNoCrmContacts,
        showNoTlContacts: showNoTlContacts,
        showNoMailContacts: showNoMailContacts,
        showEmptyTags: showEmptyTags,
        showNoMailDomains: showNoMailDomains,
        showEmptyUserFolders: showEmptyUserFolders,
        showEmptyFilters: showEmptyFilters,
        hide: hide
    };
})(jQuery);
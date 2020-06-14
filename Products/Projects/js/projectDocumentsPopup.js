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


window.ProjectDocumentsPopup = (function () {
    var isInit = false;
    var rootFolderId;
    var isAddedFile;
    var appendToListAttachFiles;

    var init = function (projectFolderId, isAddedFileMethod, appendToListAttachFilesMethod) {
        if (rootFolderId && rootFolderId !== projectFolderId) {
            isInit = false;
        }

        if (!isInit) {
            isInit = true;
            rootFolderId = projectFolderId;

            window.addEventListener("message",
                function (message) {
                    try {
                        var data = jq.parseJSON(message.data);
                    } catch (e) {
                        console.error(e);
                        return;
                    }

                    if (!data.files) {
                        PopupKeyUpActionProvider.EnableEsc = true;
                        jq.unblockUI();
                        return;
                    }

                    attachSelectedFiles(data.files);
                },
                false);

            jq("#portalDocUploader").off("click").on("click", showPortalDocUploader);

            isAddedFile = isAddedFileMethod;
            appendToListAttachFiles = appendToListAttachFilesMethod;
        }
    };

    var showPortalDocUploader = function () {
        if (jq("#fileChoiceFrame").data("folder") != rootFolderId) {
            jq("#fileChoiceFrame").remove();

            var frameUrl = jq("#attachFrame").data("frame");
            frameUrl += "&folderid=" + encodeURIComponent(rootFolderId);
            jq("<iframe/>",
                {
                    "data-folder": rootFolderId,
                    "frameborder": 0,
                    "height": "535px",
                    "id": "fileChoiceFrame",
                    "scrolling": "no",
                    "src": frameUrl,
                    "width": "100%",
                    "onload": "javascript:ProjectDocumentsPopup.frameLoad(" + rootFolderId + ");return false;"
                })
                .appendTo("#attachFrame");
        }

        var margintop = jq(window).scrollTop() - 135;
        margintop = margintop + 'px';

        PopupKeyUpActionProvider.EnableEsc = false;
        jq.blockUI({
            message: jq("#popupDocumentUploader"),
            css: {
                left: '50%',
                top: '25%',
                opacity: '1',
                border: 'none',
                padding: '0px',
                width: "1002px",

                cursor: 'default',
                textAlign: 'left',
                position: 'absolute',
                'margin-left': "-512px",
                'margin-top': margintop,
                'background-color': 'White'
            },

            overlayCSS: {
                backgroundColor: '#AAA',
                cursor: 'default',
                opacity: '0.3'
            },
            focusInput: false,
            baseZ: 666,

            fadeIn: 0,
            fadeOut: 0,

            onBlock: function () {
            }
        });

        jq("#attachFrame").css("visibility", "hidden");
        jq("#popupDocumentUploader .loader-page").show();
    };

    var attachSelectedFiles = function (selectedFiles) {
        var listfiles = new Array();
        for (var i = 0; i < selectedFiles.length; i++) {
            var file = selectedFiles[i];
            var fileName = file.title;
            var exttype = ASC.Files.Utility.getCssClassByFileTitle(file.title, true);
            var fileId = file.id;
            var version = file.version;
            var versionGroup = file.version_group;
            var access = file.access;

            var type;
            if (ASC.Files.Utility.CanImageView(fileName)) {
                type = "image";
            } else {
                if (ASC.Files.Utility.CanWebEdit(fileName)) {
                    type = "editedFile";
                } else {
                    if (ASC.Files.Utility.CanWebView(fileName)) {
                        type = "viewedFile";
                    } else {
                        type = "noViewedFile";
                    }
                }
            }

            if (!isAddedFile(fileName, fileId)) {

                var viewUrl = ASC.Files.Utility.GetFileDownloadUrl(fileId);
                var docEditUrl = ASC.Files.Utility.GetFileWebEditorUrl(fileId);
                var editUrl = ASC.Files.Utility.GetFileWebEditorUrl(fileId);

                var fileTmpl = {
                    title: fileName,
                    access: access,
                    type: type,
                    exttype: exttype,
                    id: fileId,
                    version: version,
                    versionGroup: versionGroup,
                    viewUrl: viewUrl,
                    editUrl: editUrl,
                    docEditUrl: docEditUrl,
                    fromProjectDocs: true,
                    trashAction: "deattach"
                };
                listfiles.push(fileTmpl);

                fileTmpl.attachFromPrjDocFlag = true;
                jq(document).trigger("addFile", fileTmpl);
            }
        }

        if (listfiles.length != 0) {
            appendToListAttachFiles(listfiles);
        }

        PopupKeyUpActionProvider.EnableEsc = true;
        jq.unblockUI();
    };

    function frameLoad (folderId) {
        jq("#fileChoiceFrame")[0].contentWindow.ASC.Files.FileChoice.eventAfter = function () {
            jq("#fileChoiceFrame")[0].contentWindow.ASC.Files.FileSelector.fileSelectorTree.displayAsRoot(folderId);

            jq("#popupDocumentUploader .loader-page").hide();
            jq("#attachFrame").css("visibility", "visible");
        };
    }

    return {
        init: init,

        frameLoad: frameLoad
    };
})(jQuery);
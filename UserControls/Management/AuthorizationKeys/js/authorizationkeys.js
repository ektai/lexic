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


if (typeof ASC === "undefined")
    ASC = {};

ASC.AuthorizationKeysManager = (function () {
    function init() {
        jq("#authKeysContainer .on_off_button:not(.disable)").click(function () {
            var switcherBtn = jq(this);
            var itemName = switcherBtn.attr("id").replace("switcherBtn", "");
            if (switcherBtn.hasClass("off")) {
                var popupDialog = jq("#popupDialog" + itemName);
                window.StudioBlockUIManager.blockUI(popupDialog, 600, 600, 0);
            } else {
                save(itemName, false);
            }
        });
        
        jq(".popupContainerClass .cancelButton").click(function () {
            PopupKeyUpActionProvider.CloseDialog();
        });
        
        jq(".popupContainerClass .saveButton").click(function () {
            var saveButton = jq(this);
            var itemName = saveButton.attr("id").replace("saveBtn", "");
            save(itemName, true);
        });

        jq(".popupContainerClass input.textEdit").keyup(function (key) {
            var inputObj = jq(this);
            var popupObj = inputObj.parents(".popupContainerClass");
            var saveBtn = popupObj.find(".saveButton");
            var itemName = saveBtn.attr("id").replace("saveBtn", "");
            var inputs = jq("#popupDialog" + itemName + " .auth-service-key");

            //checkParams(saveBtn, inputs); todo: need to create not required fields
            
            if ((key.keyCode || key.which) == 13) {
                var inputList = popupObj.find(".textEdit");
                
                jq.each(inputList, function (index, obj) {
                    if (inputObj.is(obj)) {
                        if (index == inputList.length - 1) {
                            saveBtn.click();
                        } else {
                            jq(inputList[index + 1]).focus();
                        }
                        return false;
                    }
                    return true;
                });
            }
        });
    };

    function save(itemName, enable) {
        var props = [];

        var keys = jq("#popupDialog" + itemName + " .auth-service-key");
        for (var i = 0; i < keys.length; i++) {
            //if (keys[i].value == "") return; //todo: need to create not required fields
            props.push({ Name: keys[i].id, Value: enable ? keys[i].value.trim() : null });
        }

        jq("#popupDialog" + itemName).block();

        window.AuthorizationKeys.SaveAuthKeys(itemName, props,
            function(result) {
                jq("#popupDialog" + itemName).unblock();
                PopupKeyUpActionProvider.CloseDialog();

                if (result.error != null) {
                    toastr.error(result.error.Message);
                } else {
                    if (result.value) {
                        toastr.success(ASC.Resources.Master.Resource.SuccessfullySaveSettingsMessage);
                        jq("#switcherBtn" + itemName).toggleClass("on off");
                    }
                }
            });
    };

function checkParams(saveBtn, keys) {
    
    var disabled = false;
    for (var i = 0; i < keys.length; i++) {
        if (keys[i].value == '') {
           disabled = true;
           break;
        }
    }

    if (!disabled) {
        saveBtn.removeClass('disabled');
    } else {
        saveBtn.addClass('disabled');
    } 
}

    return {
        init: init
    };
})();

jq(function() {
    ASC.AuthorizationKeysManager.init();
});
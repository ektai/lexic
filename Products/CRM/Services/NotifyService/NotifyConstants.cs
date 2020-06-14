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


#region Import

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ASC.Core;
using ASC.Core.Users;
using ASC.Notify;
using ASC.Notify.Model;
using ASC.Notify.Patterns;
using ASC.Notify.Recipients;
using ASC.Core.Tenants;

#endregion

namespace ASC.Web.CRM.Services.NotifyService
{
    public static class NotifyConstants
    {

        public static readonly INotifyAction Event_SetAccess = new NotifyAction("SetAccess", "set access for users");

        public static readonly INotifyAction Event_ResponsibleForTask = new NotifyAction("ResponsibleForTask", "responsible for task");

        public static readonly INotifyAction Event_TaskReminder = new NotifyAction("TaskReminder", "auto reminder about task");

        public static readonly INotifyAction Event_ResponsibleForOpportunity = new NotifyAction("ResponsibleForOpportunity", "responsible for opportunity");
       
        public static readonly INotifyAction Event_AddRelationshipEvent = new NotifyAction("AddRelationshipEvent", "add relationship event");

        public static readonly INotifyAction Event_ExportCompleted = new NotifyAction("ExportCompleted", "export is completed");

        public static readonly INotifyAction Event_ExportCompletedCustomMode = new NotifyAction("ExportCompletedCustomMode", "export is completed");

        public static readonly INotifyAction Event_ImportCompleted = new NotifyAction("ImportCompleted", "import is completed");

        public static readonly INotifyAction Event_ImportCompletedCustomMode = new NotifyAction("ImportCompletedCustomMode", "import is completed");

        public static readonly INotifyAction Event_CreateNewContact = new NotifyAction("CreateNewContact", "create new contact");

        public static readonly string Tag_AdditionalData = "AdditionalData";

        public static readonly string Tag_EntityID = "EntityID";

        public static readonly string Tag_EntityTitle = "EntityTitle";

        public static readonly string Tag_EntityRelativeURL = "EntityRelativeURL";

        public static readonly string Tag_EntityListRelativeURL = "EntityListRelativeURL";

        public static readonly string Tag_EntityListTitle = "EntityListTitle";

    }
}
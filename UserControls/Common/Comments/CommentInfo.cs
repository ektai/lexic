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


using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ASC.Web.Studio.UserControls.Common.Comments
{
    [DataContract]
    public class Attachment
    {
        [DataMember(Name = "FileName")]
        public string FileName { get; set; }

        [DataMember(Name = "FilePath")]
        public string FilePath { get; set; }
    }

    [DataContract]
    public class CommentInfo
    {
        [DataMember(Name = "commentID")]
        public string CommentID { get; set; }

        [DataMember(Name = "userID")]
        public Guid UserID { get; set; }

        [DataMember(Name = "userPost")]
        public string UserPost { get; set; }

        [DataMember(Name = "userFullName")]
        public string UserFullName { get; set; }

        [DataMember(Name = "userProfileLink")]
        public string UserProfileLink { get; set; }

        [DataMember(Name = "userAvatarPath")]
        public string UserAvatarPath { get; set; }

        [DataMember(Name = "commentBody")]
        public string CommentBody { get; set; }

        [DataMember(Name = "inactive")]
        public bool Inactive { get; set; }

        [DataMember(Name = "isRead")]
        public bool IsRead { get; set; }

        [DataMember(Name = "isEditPermissions")]
        public bool IsEditPermissions { get; set; }

        [DataMember(Name = "isResponsePermissions")]
        public bool IsResponsePermissions { get; set; }

        public DateTime TimeStamp { get; set; }

        [DataMember(Name = "timeStampStr")]
        public string TimeStampStr { get; set; }

        [DataMember(Name = "commentList")]
        public IList<CommentInfo> CommentList { get; set; }

        [DataMember(Name = "attachments")]
        public IList<Attachment> Attachments { get; set; }
    }
}
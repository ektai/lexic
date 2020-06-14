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

namespace ASC.Blogs.Core.Domain
{
    public class TagStat {
        public string Name;
        public int Count;
    }
	public class Tag
    {
        private Guid _ID;
        //private Guid _UserID;
        private string _Content;
        //private DateTime _Datetime;
        private Post _Post;

        public Guid PostId;

        public Tag() { }

        public Tag(Post blog)
        {
            _Post = blog;
            //_Blog.AddTag(this);
        }

        public virtual Guid ID
        {
            get { return _ID; }
            set { _ID = value; }
        }        
        public virtual Post Post
        {
            get { return _Post; }
            protected set { _Post = value; }
        }
        //public virtual Guid UserID
        //{
        //    get { return _UserID; }
        //    set { _UserID = value; }
        //}
        public virtual string Content
        {
            get { return _Content; }
            set { _Content = value; }
        }
        //public virtual DateTime Datetime
        //{
        //    get { return _Datetime; }
        //    set { _Datetime = value; }
        //}

        /// <summary>
        /// Hash code should ONLY contain the "business value signature" of the object and not the ID
        /// </summary>
        public override int GetHashCode()
        {
            return (GetType().FullName + "|" +
                    _ID.ToString()).GetHashCode();
        }

    }
}

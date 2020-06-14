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
using System.Linq;
using System.Reflection;
using ASC.Web.People.Resources;

namespace ASC.Web.People.Core.Import
{
    public class ContactInfo
    {
        private string firstName;
        private string lastName;
        private string email;

        [Resource("FirstName")]
        public string FirstName
        {
            get { return (firstName ?? "").Trim(); }
            set { firstName = value; }
        }

        [Resource("LastName")]
        public string LastName
        {
            get { return (lastName ?? "").Trim(); }
            set { lastName = value; }
        }

        [Resource("Email")]
        public string Email
        {
            get { return (email ?? "").Trim(); }
            set { email = value; }
        }

        public static List<KeyValuePair<string, string>> GetColumns()
        {
            return typeof(ContactInfo).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(r =>
                {
                    var attr = r.GetCustomAttribute<ResourceAttribute>();
                    return new KeyValuePair<string, string>(attr.Title,
                        PeopleResource.ResourceManager.GetString(attr.Title));
                }).ToList();
        }

        public override bool Equals(object obj)
        {
            try
            {
                if (obj is ContactInfo)
                {
                    var o = obj as ContactInfo;
                    return Email.Equals(o.Email);
                }
            }
            catch
            {
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Email.GetHashCode();
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ResourceAttribute : Attribute
    {
        public string Title { get; private set; }

        public ResourceAttribute(string title)
        {
            Title = title;
        }
    }
}
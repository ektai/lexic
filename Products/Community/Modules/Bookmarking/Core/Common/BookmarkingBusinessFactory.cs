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


using System.Web;

namespace ASC.Bookmarking.Common
{

	public static class BookmarkingBusinessFactory
	{
		public static T GetObjectFromSession<T>() where T : class, new()
		{
		    T obj;
			var key = typeof(T).ToString();
            if (HttpContext.Current.Session != null)
            {
                obj = (T) HttpContext.Current.Session[key];
                if (obj == null)
                {
                    obj = new T();
                    HttpContext.Current.Session[key] = obj;
                }
            }
            else
            {
                obj = (T)HttpContext.Current.Items[key];
                if (obj == null)
                {
                    obj = new T();
                    HttpContext.Current.Items[key] = obj;
                }
            }
		    return obj;
		}

        public static void UpdateObjectInSession<T>(T obj) where T : class, new()
        {
            var key = typeof(T).ToString();
            if (HttpContext.Current.Session != null)
            {
                HttpContext.Current.Session[key] = obj;
            }
            else
            {
                HttpContext.Current.Items[key] = obj;
            }
        }

	    public static void UpdateDisplayMode(BookmarkDisplayMode mode)
	    {
	        var key = typeof (BookmarkDisplayMode).Name;

	        if (HttpContext.Current != null && HttpContext.Current.Session != null)
	            HttpContext.Current.Session.Add(key, mode);
	    }

	    public static BookmarkDisplayMode GetDisplayMode()
	    {
	        var key = typeof (BookmarkDisplayMode).Name;

	        if (HttpContext.Current != null && HttpContext.Current.Session != null)
	            return (BookmarkDisplayMode) HttpContext.Current.Session[key];

	        return BookmarkDisplayMode.AllBookmarks;
	    }
	}
}

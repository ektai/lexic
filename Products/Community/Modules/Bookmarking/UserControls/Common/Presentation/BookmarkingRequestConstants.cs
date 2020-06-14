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


namespace ASC.Web.UserControls.Bookmarking.Common.Presentation
{
	public static class BookmarkingRequestConstants
	{
		//Page address to be used in a get request
		public const string BookmarkingPageName = "Default.aspx";
		public const string Question_Sybmol = "?";
		public const string BookmarkInfoPageName = "BookmarkInfo.aspx";
		public const string FavouriteBookmarksPageName = "FavouriteBookmarks.aspx";
		public const string CreateBookmarkPageName = "CreateBookmark.aspx";
		public const string UrlGetRequest = "url";
		public const string SelectedTab = "selectedtab";
		public const string SelectedTabBookmarkCommnets = "bookmarkcommnetstab";
		public const string SelectedTabBookmarkAddedBy = "bookmarkaddedbytab";
		public const string BookmarksCreatedByUserPageName = "UserBookmarks.aspx";
		public const string UidParam = "uid";

		//Sorting constants
		public const string SortByParam = "sortby";
		public const string SortByDateParam = "date";
		public const string SortByRaitingParam = "raiting";
		public const string MostRecentParam = "mostrecent";
		public const string TopOfTheDayParam = "topoftheday";
		public const string WeekParam = "week";
		public const string MonthParam = "month";
		public const string YearParam = "year";
		public const string PopularityParam = "popularity";
		public const string NameParam = "name";

		public const string EqualsSymbol = "=";
		public const string AMPERSAND_SYMBOL = "&";

		//Search
		public const string Search = "search";

		//Tags
		public const string Tag = "tag";

		//URL
		public const string URL_Prefix = "http://";
		public const string URL_HTTPS_Prefix = "https://";
		public const string Default_URL = "http://";

		//Tags image
		public const string TagsImageName = "tags.png";

		//Pagination
		public const string Pagination = "p";

		//No thumbnail available image
		public const string NoImageAvailable = "noimageavailable.jpg";

		public const string BookmarkingBasePath = "~/Products/Community/Modules/Bookmarking";

		public const string BookmarkingStorageManagerID = "bookmarking";
		
	}
}

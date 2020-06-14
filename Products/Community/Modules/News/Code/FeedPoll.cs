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


#region Usings

using System;
using System.Collections.Generic;
using ASC.Core.Tenants;
using ASC.Web.Community.Product;

#endregion

namespace ASC.Web.Community.News.Code
{
	[Serializable]
	public class FeedPoll : Feed
	{
		public FeedPollType PollType { get; set; }

		public DateTime StartDate { get; set; }

		public DateTime EndDate { get; set; }

		public List<FeedPollVariant> Variants { get; private set;}

		internal List<FeedPollAnswer> Answers;


		public FeedPoll()
		{
			FeedType = FeedType.Poll;
			PollType = FeedPollType.SimpleAnswer;
            StartDate = TenantUtil.DateTimeNow();
			EndDate = StartDate.AddYears(100);
			Variants = new List<FeedPollVariant>();
			Answers = new List<FeedPollAnswer>();
		}

		public int GetVariantVoteCount(long variantId)
		{
			return Answers.FindAll(a => a.VariantId == variantId).Count;
		}

		public bool IsUserVote(string userId)
		{
		    if (CommunitySecurity.IsOutsider()) return true;

            return Answers.Exists(a => a.UserId == userId);
		}
	}
}

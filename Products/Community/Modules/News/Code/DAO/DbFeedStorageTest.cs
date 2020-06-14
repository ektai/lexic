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
using ASC.Common.Data;
using ASC.Core;
using NUnit.Framework;
using ASC.Web.Community.News.Resources;

namespace ASC.Web.Community.News.Code.DAO
{
	[TestFixture]
	public class DbFeedStorageTest
	{
		private IFeedStorage feedStorage;

		private enum DbType { SQLite, MySQL }

		public DbFeedStorageTest()
		{
			var dbType = DbType.SQLite;

			if (dbType == DbType.MySQL)
			{
				DbRegistry.RegisterDatabase("news", "MySql.Data.MySqlClient", "Server=teamlab;Database=Test;UserID=dev;Pwd=dev");
				//new DbManager("news").ExecuteNonQuery(NewsResource.DbSchema_MySQL);
			}
			if (dbType == DbType.SQLite)
			{
				DbRegistry.RegisterDatabase("news", "System.Data.SQLite", "Data Source=feed.sqlite;Version=3");
				//new DbManager("news").ExecuteNonQuery(NewsResource.DbSchema_SQLite);
			}

			feedStorage = new DbFeedStorage(10);
		}

		[Test]
		public void FeedTest()
		{
			var feed1 = new Feed();
			feed1.FeedType = FeedType.News;
			feed1.Caption = "aaa";
			feed1.Text = "bbb";
			var feed1Id = feedStorage.SaveFeed(feed1, TODO, TODO).Id;
			feedStorage.SaveFeed(feed1, TODO, TODO);

			var feed2 = new Feed();
			feed2.FeedType = FeedType.Order;
			feed2.Caption = "ccca";
			feed2.Text = "ddd";
			var feed2Id = feedStorage.SaveFeed(feed2, TODO, TODO).Id;

			var feeds = feedStorage.GetFeeds(FeedType.News, Guid.Empty, 0, 0);
			CollectionAssert.AreEquivalent(new[] { feed1 }, feeds);
			feeds = feedStorage.GetFeeds(FeedType.Order, Guid.Empty, 0, 0);
			CollectionAssert.AreEquivalent(new[] { feed2 }, feeds);
			feeds = feedStorage.GetFeeds(FeedType.Advert, Guid.Empty, 0, 0);
			CollectionAssert.IsEmpty(feeds);

			feeds = feedStorage.GetFeeds(FeedType.All, Guid.NewGuid(), 0, 0);
			CollectionAssert.IsEmpty(feeds);

			feeds = feedStorage.SearchFeeds("c", FeedType.All, Guid.Empty, 0, 0);
			CollectionAssert.AreEquivalent(new[] { feed2 }, feeds);
			feeds = feedStorage.SearchFeeds("a");
			CollectionAssert.AreEquivalent(new[] { feed1, feed2 }, feeds);

			var feedTypes = feedStorage.GetUsedFeedTypes();
			CollectionAssert.AreEquivalent(new[] { FeedType.News, FeedType.Order }, feedTypes);

			feed2 = feedStorage.GetFeed(feed2Id);
			Assert.IsAssignableFrom(typeof(FeedNews), feed2);
			Assert.AreEqual(FeedType.Order, feed2.FeedType);
			Assert.AreEqual("ccca", feed2.Caption);
			Assert.AreEqual("ddd", feed2.Text);

			var c1 = new FeedComment(feed1Id) { Comment = "c1", Inactive = true };
			var c2 = new FeedComment(feed1Id) { Comment = "c2" };
			var c3 = new FeedComment(feed2Id) { Comment = "c3" };
			var c1Id = feedStorage.SaveFeedComment(c1).Id;
			var c2Id = feedStorage.SaveFeedComment(c2).Id;
			feedStorage.SaveFeedComment(c3);
			feedStorage.SaveFeedComment(c3);

			var comments = feedStorage.GetFeedComments(feed2Id);
			CollectionAssert.AreEquivalent(new[] { c3 }, comments);

			comments = feedStorage.GetFeedComments(feed1Id);
			CollectionAssert.AreEquivalent(new[] { c1, c2 }, comments);

			feedStorage.RemoveFeedComment(c2Id);
			comments = feedStorage.GetFeedComments(feed1Id);
			CollectionAssert.AreEquivalent(new[] { c1 }, comments);

			c1 = feedStorage.GetFeedComment(c1Id);
			Assert.AreEqual("c1", c1.Comment);
			Assert.IsTrue(c1.Inactive);

			feedStorage.ReadFeed(feed2Id, SecurityContext.CurrentAccount.ID.ToString());

			feedStorage.RemoveFeed(feed2Id);
			feedStorage.RemoveFeed(feed1Id);
			feed1 = feedStorage.GetFeed(feed1Id);
			Assert.IsNull(feed1);
			comments = feedStorage.GetFeedComments(feed1Id);
			CollectionAssert.IsEmpty(comments);
		}

		[Test]
		public void PollTest()
		{
			var userId = SecurityContext.CurrentAccount.ID.ToString();

			var poll = new FeedPoll();
			poll.Caption = "Poll1";
			poll.Variants.Add(new FeedPollVariant() { Name = "v1" });
			poll.Variants.Add(new FeedPollVariant() { Name = "v2" });
			poll.Variants.Add(new FeedPollVariant() { Name = "v3" });
			feedStorage.SaveFeed(poll, TODO, TODO);
			poll.Variants.RemoveAt(2);
			feedStorage.SaveFeed(poll, TODO, TODO);
			poll.Variants.Add(new FeedPollVariant() { Name = "v3" });
			feedStorage.SaveFeed(poll, TODO, TODO);

			poll = new FeedPoll();
			poll.Caption = "Poll2";
			poll.PollType = FeedPollType.MultipleAnswer;
			poll.Variants.Add(new FeedPollVariant() { Name = "v1" });
			feedStorage.SaveFeed(poll, TODO, TODO);

			var polls = feedStorage.GetFeeds(FeedType.Poll, Guid.Empty, 0, 0);
			Assert.AreEqual(2, polls.Count);

			var poll1 = (FeedPoll)feedStorage.GetFeed(polls[1].Id);
			Assert.AreEqual(3, poll1.Variants.Count);
			Assert.AreEqual("v1", poll1.Variants[0].Name);
			feedStorage.PollVote(userId, new[] { poll1.Variants[0].ID, poll1.Variants[1].ID });

			var poll2 = (FeedPoll)feedStorage.GetFeed(polls[0].Id);
			Assert.AreEqual(FeedPollType.MultipleAnswer, poll2.PollType);
			Assert.AreEqual(1, poll2.Variants.Count);

			poll1 = (FeedPoll)feedStorage.GetFeed(poll1.Id);
			Assert.AreEqual(1, poll1.GetVariantVoteCount(poll1.Variants[0].ID));
			Assert.AreEqual(1, poll1.GetVariantVoteCount(poll1.Variants[1].ID));
			Assert.AreEqual(0, poll1.GetVariantVoteCount(poll1.Variants[2].ID));

			Assert.IsTrue(poll1.IsUserVote(userId));
			Assert.IsFalse(poll2.IsUserVote(userId));

			feedStorage.RemoveFeed(poll1.Id);
			feedStorage.RemoveFeed(poll2.Id);
		}
	}
}
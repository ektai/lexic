﻿/*
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
using System.Web;
using System.IO;

using NUnit.Framework;
using Newtonsoft.Json;

using ASC.Core;
using ASC.Web.Core;
using ASC.Web.People.Core.Import;
using ASC.Web.Studio.Utility;


namespace ASC.Web.People.Tests.Common
{
    [TestFixture]
    public class PeopleImportParserTests
    {
        const string referenceFirstName = "TestFirstName";
        const string referenceLastName = "TestLastName";
        const string referenceEmail = "testlocalpart@testdomain.com";

        ContactInfo headrersDefault = new ContactInfo()
        {
            FirstName = "FirstName",
            LastName = "LastName",
            Email = "Email"
        };

        ContactInfo headrersUser = new ContactInfo()
        {
            FirstName = "MyFirstName",
            LastName = "MyLastName",
            Email = "MyEmail"
        };

        ContactInfo user = new ContactInfo()
        {
            FirstName = referenceFirstName,
            LastName = referenceLastName,
            Email = referenceEmail
        };

        ContactInfo userQ = new ContactInfo()
        {
            FirstName = "TestFirstName SQ"
        };

        FileParameters inputParameters = new FileParameters();

        [TearDown]
        public void CleanUp()
        {
            inputParameters.Encode = 0;
            inputParameters.Separator = 0;
            inputParameters.Delimiter = 0;
            inputParameters.Position = 0;
            inputParameters.UserHeader = "";
        }

        [Test]
        public void ComaDefaultRawTest()
        {
            Stream inputStream = File.OpenRead(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,@"..\..\Common\Files\Coma.csv"));

            IUserImporter importer = new TextFileUserImporter(inputStream, inputParameters);

            var rawUsers = importer.GetRawUsers();

            Assert.AreEqual(referenceFirstName, rawUsers[0].Item2[0]);
            Assert.AreEqual(referenceLastName, rawUsers[0].Item2[1]);
            Assert.AreEqual(referenceEmail, rawUsers[0].Item2[2]);
        }

        [Test]
        public void SemicolonDefaultRawTest()
        {
            Stream inputStream = File.OpenRead(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,@"..\..\Common\Files\Semicolon.csv"));
            inputParameters.Separator = 1;

            IUserImporter importer = new TextFileUserImporter(inputStream, inputParameters);

            var rawUsers = importer.GetRawUsers();

            Assert.AreEqual(referenceFirstName, rawUsers[0].Item2[0]);
            Assert.AreEqual(referenceLastName, rawUsers[0].Item2[1]);
            Assert.AreEqual(referenceEmail, rawUsers[0].Item2[2]);
        }

        [Test]
        public void ColonDefaultRawTest()
        {
            Stream inputStream = File.OpenRead(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,@"..\..\Common\Files\Colon.csv"));
            inputParameters.Separator = 2;

            IUserImporter importer = new TextFileUserImporter(inputStream, inputParameters);

            var rawUsers = importer.GetRawUsers();

            Assert.AreEqual(referenceFirstName, rawUsers[0].Item2[0]);
            Assert.AreEqual(referenceLastName, rawUsers[0].Item2[1]);
            Assert.AreEqual(referenceEmail, rawUsers[0].Item2[2]);
        }

        [Test]
        public void TabDefaultRawTest()
        {
            Stream inputStream = File.OpenRead(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Common\Files\Tab.csv"));
            inputParameters.Separator = 3;

            IUserImporter importer = new TextFileUserImporter(inputStream, inputParameters);

            var rawUsers = importer.GetRawUsers();

            Assert.AreEqual(referenceFirstName, rawUsers[0].Item2[0]);
            Assert.AreEqual(referenceLastName, rawUsers[0].Item2[1]);
            Assert.AreEqual(referenceEmail, rawUsers[0].Item2[2]);
        }

        [Test]
        public void SpaceDefaultRawTest()
        {
            Stream inputStream = File.OpenRead(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Common\Files\Space.csv"));
            inputParameters.Separator = 4;

            IUserImporter importer = new TextFileUserImporter(inputStream, inputParameters);

            var rawUsers = importer.GetRawUsers();

            Assert.AreEqual(referenceFirstName, rawUsers[0].Item2[0]);
            Assert.AreEqual(referenceLastName, rawUsers[0].Item2[1]);
            Assert.AreEqual(referenceEmail, rawUsers[0].Item2[2]);
        }

        [Test]
        public void ComaDefaultDiscoveredTest()
        {
            Stream inputStream = File.OpenRead(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Common\Files\Coma.csv"));
            inputParameters.UserHeader = "FirstName,LastName,Email";

            IUserImporter importer = new TextFileUserImporter(inputStream, inputParameters);

            var discoveredUsers = importer.GetDiscoveredUsers();

            Assert.AreEqual(headrersDefault.FirstName, discoveredUsers.ElementAt(0).FirstName);
            Assert.AreEqual(headrersDefault.LastName, discoveredUsers.ElementAt(0).LastName);
            Assert.AreEqual(headrersDefault.Email, discoveredUsers.ElementAt(0).Email);
            Assert.AreEqual(user.FirstName, discoveredUsers.ElementAt(1).FirstName);
            Assert.AreEqual(user.LastName, discoveredUsers.ElementAt(1).LastName);
            Assert.AreEqual(user.Email, discoveredUsers.ElementAt(1).Email);
        }

        [Test]
        public void SemicolonDefaultDiscoveredTest()
        {
            Stream inputStream = File.OpenRead(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Common\Files\Semicolon.csv"));
            inputParameters.Separator = 1;
            inputParameters.UserHeader = "FirstName,LastName,Email";

            IUserImporter importer = new TextFileUserImporter(inputStream, inputParameters);

            var discoveredUsers = importer.GetDiscoveredUsers();

            Assert.AreEqual(headrersDefault.FirstName, discoveredUsers.ElementAt(0).FirstName);
            Assert.AreEqual(headrersDefault.LastName, discoveredUsers.ElementAt(0).LastName);
            Assert.AreEqual(headrersDefault.Email, discoveredUsers.ElementAt(0).Email);
            Assert.AreEqual(user.FirstName, discoveredUsers.ElementAt(1).FirstName);
            Assert.AreEqual(user.LastName, discoveredUsers.ElementAt(1).LastName);
            Assert.AreEqual(user.Email, discoveredUsers.ElementAt(1).Email);
        }

        [Test]
        public void ColonDefaultDiscoveredTest()
        {
            Stream inputStream = File.OpenRead(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Common\Files\Colon.csv"));
            inputParameters.Separator = 2;
            inputParameters.UserHeader = "FirstName,LastName,Email";

            IUserImporter importer = new TextFileUserImporter(inputStream, inputParameters);

            var discoveredUsers = importer.GetDiscoveredUsers();

            Assert.AreEqual(headrersDefault.FirstName, discoveredUsers.ElementAt(0).FirstName);
            Assert.AreEqual(headrersDefault.LastName, discoveredUsers.ElementAt(0).LastName);
            Assert.AreEqual(headrersDefault.Email, discoveredUsers.ElementAt(0).Email);
            Assert.AreEqual(user.FirstName, discoveredUsers.ElementAt(1).FirstName);
            Assert.AreEqual(user.LastName, discoveredUsers.ElementAt(1).LastName);
            Assert.AreEqual(user.Email, discoveredUsers.ElementAt(1).Email);
        }

        [Test]
        public void TabDefaultDiscoveredTest()
        {
            Stream inputStream = File.OpenRead(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Common\Files\Tab.csv"));
            inputParameters.Separator = 3;
            inputParameters.UserHeader = "FirstName,LastName,Email";

            IUserImporter importer = new TextFileUserImporter(inputStream, inputParameters);

            var discoveredUsers = importer.GetDiscoveredUsers();

            Assert.AreEqual(headrersDefault.FirstName, discoveredUsers.ElementAt(0).FirstName);
            Assert.AreEqual(headrersDefault.LastName, discoveredUsers.ElementAt(0).LastName);
            Assert.AreEqual(headrersDefault.Email, discoveredUsers.ElementAt(0).Email);
            Assert.AreEqual(user.FirstName, discoveredUsers.ElementAt(1).FirstName);
            Assert.AreEqual(user.LastName, discoveredUsers.ElementAt(1).LastName);
            Assert.AreEqual(user.Email, discoveredUsers.ElementAt(1).Email);
        }

        [Test]
        public void SpaceDefaultDiscoveredTest()
        {
            Stream inputStream = File.OpenRead(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Common\Files\Space.csv"));
            inputParameters.Separator = 4;
            inputParameters.UserHeader = "FirstName,LastName,Email";

            IUserImporter importer = new TextFileUserImporter(inputStream, inputParameters);

            var discoveredUsers = importer.GetDiscoveredUsers();

            Assert.AreEqual(headrersDefault.FirstName, discoveredUsers.ElementAt(0).FirstName);
            Assert.AreEqual(headrersDefault.LastName, discoveredUsers.ElementAt(0).LastName);
            Assert.AreEqual(headrersDefault.Email, discoveredUsers.ElementAt(0).Email);
            Assert.AreEqual(user.FirstName, discoveredUsers.ElementAt(1).FirstName);
            Assert.AreEqual(user.LastName, discoveredUsers.ElementAt(1).LastName);
            Assert.AreEqual(user.Email, discoveredUsers.ElementAt(1).Email);
        }

        [Test]
        public void ComaWithHeaderDiscoveredTest()
        {
            Stream inputStream = File.OpenRead(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Common\Files\Coma_WH.csv"));
            inputParameters.Position = 1;
            inputParameters.UserHeader = "FirstName,LastName,Email";

            IUserImporter importer = new TextFileUserImporter(inputStream, inputParameters);

            var discoveredUsers = importer.GetDiscoveredUsers();

            Assert.AreEqual(headrersUser.FirstName, discoveredUsers.ElementAt(0).FirstName);
            Assert.AreEqual(headrersUser.LastName, discoveredUsers.ElementAt(0).LastName);
            Assert.AreEqual(headrersUser.Email, discoveredUsers.ElementAt(0).Email);
            Assert.AreEqual(user.FirstName, discoveredUsers.ElementAt(1).FirstName);
            Assert.AreEqual(user.LastName, discoveredUsers.ElementAt(1).LastName);
            Assert.AreEqual(user.Email, discoveredUsers.ElementAt(1).Email);
        }

        [Test]
        public void SemicolonWithHeaderDiscoveredTest()
        {
            Stream inputStream = File.OpenRead(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Common\Files\Semicolon_WH.csv"));
            inputParameters.Separator = 1;
            inputParameters.Position = 1;
            inputParameters.UserHeader = "FirstName,LastName,Email";

            IUserImporter importer = new TextFileUserImporter(inputStream, inputParameters);

            var discoveredUsers = importer.GetDiscoveredUsers();

            Assert.AreEqual(headrersUser.FirstName, discoveredUsers.ElementAt(0).FirstName);
            Assert.AreEqual(headrersUser.LastName, discoveredUsers.ElementAt(0).LastName);
            Assert.AreEqual(headrersUser.Email, discoveredUsers.ElementAt(0).Email);
            Assert.AreEqual(user.FirstName, discoveredUsers.ElementAt(1).FirstName);
            Assert.AreEqual(user.LastName, discoveredUsers.ElementAt(1).LastName);
            Assert.AreEqual(user.Email, discoveredUsers.ElementAt(1).Email);
        }

        [Test]
        public void ColonWithHeaderDiscoveredTest()
        {
            Stream inputStream = File.OpenRead(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Common\Files\Colon_WH.csv"));
            inputParameters.Separator = 2;
            inputParameters.Position = 1;
            inputParameters.UserHeader = "FirstName,LastName,Email";

            IUserImporter importer = new TextFileUserImporter(inputStream, inputParameters);

            var discoveredUsers = importer.GetDiscoveredUsers();

            Assert.AreEqual(headrersUser.FirstName, discoveredUsers.ElementAt(0).FirstName);
            Assert.AreEqual(headrersUser.LastName, discoveredUsers.ElementAt(0).LastName);
            Assert.AreEqual(headrersUser.Email, discoveredUsers.ElementAt(0).Email);
            Assert.AreEqual(user.FirstName, discoveredUsers.ElementAt(1).FirstName);
            Assert.AreEqual(user.LastName, discoveredUsers.ElementAt(1).LastName);
            Assert.AreEqual(user.Email, discoveredUsers.ElementAt(1).Email);
        }

        [Test]
        public void TabWithHeaderDiscoveredTest()
        {
            Stream inputStream = File.OpenRead(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Common\Files\Tab_WH.csv"));
            inputParameters.Separator = 3;
            inputParameters.Position = 1;
            inputParameters.UserHeader = "FirstName,LastName,Email";

            IUserImporter importer = new TextFileUserImporter(inputStream, inputParameters);

            var discoveredUsers = importer.GetDiscoveredUsers();

            Assert.AreEqual(headrersUser.FirstName, discoveredUsers.ElementAt(0).FirstName);
            Assert.AreEqual(headrersUser.LastName, discoveredUsers.ElementAt(0).LastName);
            Assert.AreEqual(headrersUser.Email, discoveredUsers.ElementAt(0).Email);
            Assert.AreEqual(user.FirstName, discoveredUsers.ElementAt(1).FirstName);
            Assert.AreEqual(user.LastName, discoveredUsers.ElementAt(1).LastName);
            Assert.AreEqual(user.Email, discoveredUsers.ElementAt(1).Email);
        }

        [Test]
        public void SpaceWithHeaderDiscoveredTest()
        {
            Stream inputStream = File.OpenRead(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Common\Files\Space_WH.csv"));
            inputParameters.Separator = 4;
            inputParameters.Position = 1;
            inputParameters.UserHeader = "FirstName,LastName,Email";

            IUserImporter importer = new TextFileUserImporter(inputStream, inputParameters);

            var discoveredUsers = importer.GetDiscoveredUsers();

            Assert.AreEqual(headrersUser.FirstName, discoveredUsers.ElementAt(0).FirstName);
            Assert.AreEqual(headrersUser.LastName, discoveredUsers.ElementAt(0).LastName);
            Assert.AreEqual(headrersUser.Email, discoveredUsers.ElementAt(0).Email);
            Assert.AreEqual(user.FirstName, discoveredUsers.ElementAt(1).FirstName);
            Assert.AreEqual(user.LastName, discoveredUsers.ElementAt(1).LastName);
            Assert.AreEqual(user.Email, discoveredUsers.ElementAt(1).Email);
        }

        [Test]
        public void ComaSinglequoteDiscoveredTest()
        {
            Stream inputStream = File.OpenRead(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Common\Files\Coma_SQ.csv"));
            inputParameters.Delimiter = 1;
            inputParameters.UserHeader = "FirstName,LastName,Email";

            IUserImporter importer = new TextFileUserImporter(inputStream, inputParameters);

            var discoveredUsers = importer.GetDiscoveredUsers();

            Assert.AreEqual(headrersDefault.FirstName, discoveredUsers.ElementAt(0).FirstName);
            Assert.AreEqual(headrersDefault.LastName, discoveredUsers.ElementAt(0).LastName);
            Assert.AreEqual(headrersDefault.Email, discoveredUsers.ElementAt(0).Email);
            Assert.AreEqual(userQ.FirstName, discoveredUsers.ElementAt(1).FirstName);
            Assert.AreEqual(user.LastName, discoveredUsers.ElementAt(1).LastName);
            Assert.AreEqual(user.Email, discoveredUsers.ElementAt(1).Email);
        }

        [Test]
        public void SemicolonSinglequoteDiscoveredTest()
        {
            Stream inputStream = File.OpenRead(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Common\Files\Semicolon_SQ.csv"));
            inputParameters.Separator = 1;
            inputParameters.Delimiter = 1;
            inputParameters.UserHeader = "FirstName,LastName,Email";

            IUserImporter importer = new TextFileUserImporter(inputStream, inputParameters);

            var discoveredUsers = importer.GetDiscoveredUsers();

            Assert.AreEqual(headrersDefault.FirstName, discoveredUsers.ElementAt(0).FirstName);
            Assert.AreEqual(headrersDefault.LastName, discoveredUsers.ElementAt(0).LastName);
            Assert.AreEqual(headrersDefault.Email, discoveredUsers.ElementAt(0).Email);
            Assert.AreEqual(userQ.FirstName, discoveredUsers.ElementAt(1).FirstName);
            Assert.AreEqual(user.LastName, discoveredUsers.ElementAt(1).LastName);
            Assert.AreEqual(user.Email, discoveredUsers.ElementAt(1).Email);
        }

        [Test]
        public void ColonSinglequoteDiscoveredTest()
        {
            Stream inputStream = File.OpenRead(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Common\Files\Colon_SQ.csv"));
            inputParameters.Separator = 2;
            inputParameters.Delimiter = 1;
            inputParameters.UserHeader = "FirstName,LastName,Email";

            IUserImporter importer = new TextFileUserImporter(inputStream, inputParameters);

            var discoveredUsers = importer.GetDiscoveredUsers();

            Assert.AreEqual(headrersDefault.FirstName, discoveredUsers.ElementAt(0).FirstName);
            Assert.AreEqual(headrersDefault.LastName, discoveredUsers.ElementAt(0).LastName);
            Assert.AreEqual(headrersDefault.Email, discoveredUsers.ElementAt(0).Email);
            Assert.AreEqual(userQ.FirstName, discoveredUsers.ElementAt(1).FirstName);
            Assert.AreEqual(user.LastName, discoveredUsers.ElementAt(1).LastName);
            Assert.AreEqual(user.Email, discoveredUsers.ElementAt(1).Email);
        }

        [Test]
        public void TabSinglequoteDiscoveredTest()
        {
            Stream inputStream = File.OpenRead(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Common\Files\Tab_SQ.csv"));
            inputParameters.Separator = 3;
            inputParameters.Delimiter = 1;
            inputParameters.UserHeader = "FirstName,LastName,Email";

            IUserImporter importer = new TextFileUserImporter(inputStream, inputParameters);

            var discoveredUsers = importer.GetDiscoveredUsers();

            Assert.AreEqual(headrersDefault.FirstName, discoveredUsers.ElementAt(0).FirstName);
            Assert.AreEqual(headrersDefault.LastName, discoveredUsers.ElementAt(0).LastName);
            Assert.AreEqual(headrersDefault.Email, discoveredUsers.ElementAt(0).Email);
            Assert.AreEqual(userQ.FirstName, discoveredUsers.ElementAt(1).FirstName);
            Assert.AreEqual(user.LastName, discoveredUsers.ElementAt(1).LastName);
            Assert.AreEqual(user.Email, discoveredUsers.ElementAt(1).Email);
        }

        [Test]
        public void SpaceSinglequoteDiscoveredTest()
        {
            Stream inputStream = File.OpenRead(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Common\Files\Space_SQ.csv"));
            inputParameters.Separator = 4;
            inputParameters.Delimiter = 1;
            inputParameters.UserHeader = "FirstName,LastName,Email";

            IUserImporter importer = new TextFileUserImporter(inputStream, inputParameters);

            var discoveredUsers = importer.GetDiscoveredUsers();

            Assert.AreEqual(headrersDefault.FirstName, discoveredUsers.ElementAt(0).FirstName);
            Assert.AreEqual(headrersDefault.LastName, discoveredUsers.ElementAt(0).LastName);
            Assert.AreEqual(headrersDefault.Email, discoveredUsers.ElementAt(0).Email);
            Assert.AreEqual(userQ.FirstName, discoveredUsers.ElementAt(1).FirstName);
            Assert.AreEqual(user.LastName, discoveredUsers.ElementAt(1).LastName);
            Assert.AreEqual(user.Email, discoveredUsers.ElementAt(1).Email);
        }

        [Test]
        public void ComaWithHeaderSinglequoteDiscoveredTest()
        {
            Stream inputStream = File.OpenRead(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Common\Files\Coma_WH_SQ.csv"));
            inputParameters.Delimiter = 1;
            inputParameters.Position = 1;
            inputParameters.UserHeader = "FirstName,LastName,Email";

            IUserImporter importer = new TextFileUserImporter(inputStream, inputParameters);

            var discoveredUsers = importer.GetDiscoveredUsers();

            Assert.AreEqual(headrersUser.FirstName, discoveredUsers.ElementAt(0).FirstName);
            Assert.AreEqual(headrersUser.LastName, discoveredUsers.ElementAt(0).LastName);
            Assert.AreEqual(headrersUser.Email, discoveredUsers.ElementAt(0).Email);
            Assert.AreEqual(userQ.FirstName, discoveredUsers.ElementAt(1).FirstName);
            Assert.AreEqual(user.LastName, discoveredUsers.ElementAt(1).LastName);
            Assert.AreEqual(user.Email, discoveredUsers.ElementAt(1).Email);
        }

        [Test]
        public void SemicolonWithHeaderSinglequoteDiscoveredTest()
        {
            Stream inputStream = File.OpenRead(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Common\Files\Semicolon_WH_SQ.csv"));
            inputParameters.Separator = 1;
            inputParameters.Delimiter = 1;
            inputParameters.Position = 1;
            inputParameters.UserHeader = "FirstName,LastName,Email";

            IUserImporter importer = new TextFileUserImporter(inputStream, inputParameters);

            var discoveredUsers = importer.GetDiscoveredUsers();

            Assert.AreEqual(headrersUser.FirstName, discoveredUsers.ElementAt(0).FirstName);
            Assert.AreEqual(headrersUser.LastName, discoveredUsers.ElementAt(0).LastName);
            Assert.AreEqual(headrersUser.Email, discoveredUsers.ElementAt(0).Email);
            Assert.AreEqual(userQ.FirstName, discoveredUsers.ElementAt(1).FirstName);
            Assert.AreEqual(user.LastName, discoveredUsers.ElementAt(1).LastName);
            Assert.AreEqual(user.Email, discoveredUsers.ElementAt(1).Email);
        }

        [Test]
        public void ColonWithHeaderSinglequoteDiscoveredTest()
        {
            Stream inputStream = File.OpenRead(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Common\Files\Colon_WH_SQ.csv"));
            inputParameters.Separator = 2;
            inputParameters.Delimiter = 1;
            inputParameters.Position = 1;
            inputParameters.UserHeader = "FirstName,LastName,Email";

            IUserImporter importer = new TextFileUserImporter(inputStream, inputParameters);

            var discoveredUsers = importer.GetDiscoveredUsers();

            Assert.AreEqual(headrersUser.FirstName, discoveredUsers.ElementAt(0).FirstName);
            Assert.AreEqual(headrersUser.LastName, discoveredUsers.ElementAt(0).LastName);
            Assert.AreEqual(headrersUser.Email, discoveredUsers.ElementAt(0).Email);
            Assert.AreEqual(userQ.FirstName, discoveredUsers.ElementAt(1).FirstName);
            Assert.AreEqual(user.LastName, discoveredUsers.ElementAt(1).LastName);
            Assert.AreEqual(user.Email, discoveredUsers.ElementAt(1).Email);
        }

        [Test]
        public void TabWithHeaderSinglequoteDiscoveredTest()
        {
            Stream inputStream = File.OpenRead(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Common\Files\Tab_WH_SQ.csv"));
            inputParameters.Separator = 3;
            inputParameters.Delimiter = 1;
            inputParameters.Position = 1;
            inputParameters.UserHeader = "FirstName,LastName,Email";

            IUserImporter importer = new TextFileUserImporter(inputStream, inputParameters);

            var discoveredUsers = importer.GetDiscoveredUsers();

            Assert.AreEqual(headrersUser.FirstName, discoveredUsers.ElementAt(0).FirstName);
            Assert.AreEqual(headrersUser.LastName, discoveredUsers.ElementAt(0).LastName);
            Assert.AreEqual(headrersUser.Email, discoveredUsers.ElementAt(0).Email);
            Assert.AreEqual(userQ.FirstName, discoveredUsers.ElementAt(1).FirstName);
            Assert.AreEqual(user.LastName, discoveredUsers.ElementAt(1).LastName);
            Assert.AreEqual(user.Email, discoveredUsers.ElementAt(1).Email);
        }

        [Test]
        public void SpaceWithHeaderSinglequoteDiscoveredTest()
        {
            Stream inputStream = File.OpenRead(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Common\Files\Space_WH_SQ.csv"));
            inputParameters.Separator = 4;
            inputParameters.Delimiter = 1;
            inputParameters.Position = 1;
            inputParameters.UserHeader = "FirstName,LastName,Email";

            IUserImporter importer = new TextFileUserImporter(inputStream, inputParameters);

            var discoveredUsers = importer.GetDiscoveredUsers();

            Assert.AreEqual(headrersUser.FirstName, discoveredUsers.ElementAt(0).FirstName);
            Assert.AreEqual(headrersUser.LastName, discoveredUsers.ElementAt(0).LastName);
            Assert.AreEqual(headrersUser.Email, discoveredUsers.ElementAt(0).Email);
            Assert.AreEqual(userQ.FirstName, discoveredUsers.ElementAt(1).FirstName);
            Assert.AreEqual(user.LastName, discoveredUsers.ElementAt(1).LastName);
            Assert.AreEqual(user.Email, discoveredUsers.ElementAt(1).Email);
        }

        [Test]
        public void WithoutFirstAndLastDefaultDiscoveredTest()
        {
            Stream inputStream = File.OpenRead(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Common\Files\ComaWithoutFirstAndLast.csv"));
            inputParameters.UserHeader = "FirstName,LastName,Email";

            IUserImporter importer = new TextFileUserImporter(inputStream, inputParameters);

            var discoveredUsers = importer.GetDiscoveredUsers();

            Assert.AreEqual("", (string)discoveredUsers.ElementAt(1).FirstName);
            Assert.AreEqual("", (string)discoveredUsers.ElementAt(1).LastName);
        }

        [Test]
        public void CreateFirstAndLastDefaultDiscoveredTest()
        {
            Stream inputStream = File.OpenRead(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Common\Files\ComaWithoutFirstAndLast.csv"));
            inputParameters.UserHeader = "FirstName,LastName,Email";

            IUserImporter importer = new TextFileUserImporter(inputStream, inputParameters);

            var discoveredUsers = importer.GetDiscoveredUsers();

            Assert.AreEqual("testlocal", (string)discoveredUsers.ElementAt(2).FirstName);
            Assert.AreEqual("part", (string)discoveredUsers.ElementAt(2).LastName);
        }
    }
}

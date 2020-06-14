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
using System.IO;
using System.Web;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization.Json;
using System.Text;
using ASC.Common.Security;
using ASC.CRM.Core.Dao;
using ASC.Web.CRM.Core;
using ASC.Web.Studio;
using Autofac;

namespace ASC.Web.CRM
{
    public abstract class BasePage : MainPage
    {
        protected ILifetimeScope Scope { get; set; }
        protected internal DaoFactory DaoFactory { get; set; }

        protected BasePage()
        {
            Scope = DIHelper.Resolve();
            DaoFactory = Scope.Resolve<DaoFactory>();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            PageLoad();
        }

        public void JsonPublisher<T>(T data, String jsonClassName) where T : class
        {
            String json;

            using (var stream = new MemoryStream())
            {
                var serializer = new DataContractJsonSerializer(typeof(T));
                serializer.WriteObject(stream, data);
                json = Encoding.UTF8.GetString(stream.ToArray());
            }

            Page.RegisterInlineScript(String.Format(" var {1} = {0};", json, jsonClassName), onReady: false);
        }
      
        protected abstract void PageLoad();

        protected override void OnUnload(EventArgs e)
        {
            if (Scope != null)
            {
                Scope.Dispose();
            }
            base.OnUnload(e);
        }
    }
}

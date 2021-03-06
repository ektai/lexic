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


using ASC.Common.Data;
using ASC.Common.DependencyInjection;
using ASC.CRM.Core;
using ASC.CRM.Core.Dao;
using ASC.Web.Studio.Utility;
using Autofac;

namespace ASC.Web.CRM.Core
{
    public static class DIHelper
    {
        internal static IContainer Builder;
        private static bool isRegistered;
        private static readonly object Locker = new object();

        public static void Register()
        {
            if (isRegistered) return;

            lock (Locker)
            {
                if (isRegistered) return;

                var container = AutofacConfigLoader.Load("crm");

                container.Register(c => DbManager.FromHttpContext("default"))
                    .AsSelf()
                    .As<IDbManager>()
                    .InstancePerRequest();

                Builder = container.Build();
                isRegistered = true;
            }
        }

        public static ILifetimeScope Resolve()
        {
            return Resolve(TenantProvider.CurrentTenantID);
        }


        public static ILifetimeScope Resolve(int tenantID)
        {
            Register();

            var scope = Builder.BeginLifetimeScope(Autofac.Core.Lifetime.MatchingScopeLifetimeTags.RequestLifetimeScopeTag);
            scope.Resolve<IDbManager>();
            scope.Resolve<DaoFactory>(new TypedParameter(typeof(int), tenantID));

            return scope;
        }
    }
}
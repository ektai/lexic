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
using ASC.Web.UserControls.Wiki.Data;
using ASC.Web.UserControls.Wiki.Handlers;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Utility;
using System.Web;

namespace ASC.Web.UserControls.Wiki
{
    public class VersionEventArgs : EventArgs
    {
        public Guid UserID { get; set; }
        public DateTime Date { get; set; }
        public int Version { get; set; }
    }


    public class BaseUserControl : System.Web.UI.UserControl
    {
        public static readonly string[] reservedPrefixes = new string[] { Constants.WikiCategoryKeyCaption, Constants.WikiInternalKeyCaption };

        private string _mainWikiClassName = "wiki";
        public delegate void PageEmptyHamdler(object sender, EventArgs e);
        public event PageEmptyHamdler PageEmpty;

        public delegate void PublishVersionInfoHandler(object sender, VersionEventArgs e);
        public event PublishVersionInfoHandler PublishVersionInfo;

        public delegate void WikiPageLoadedHandler(bool isNew, IWikiObjectOwner owner);
        public event WikiPageLoadedHandler WikiPageLoaded;

        protected WikiEngine Wiki
        {
            get { return new WikiEngine(); }
        }
        

        public void RiseWikiPageLoaded(IWikiObjectOwner owner)
        {
            RiseWikiPageLoaded(false, owner);
        }
        
        public void RiseWikiPageLoaded(bool isNew, IWikiObjectOwner owner)
        {
            if(WikiPageLoaded != null)
            {
                WikiPageLoaded(isNew, owner);
            }
        }

        public string MainWikiClassName
        {
            get { return _mainWikiClassName; }
            set{ _mainWikiClassName = value; }
        }

        
        protected void RisePageEmptyEvent()
        {
            if (PageEmpty != null)
            {
                PageEmpty(this, new EventArgs());
            }
        }

        protected string PathFromFCKEditor
        {
            get { return WikiSection.Section.FckeditorInfo.PathFrom.ToLower(); }
        }

        protected string BaseFCKRelPath
        {
            get { return WikiSection.Section.FckeditorInfo.BaseRelPath; }
        }

        protected void RisePublishVersionInfo(IVersioned container)
        {
            if (!this.Visible)
                return;

            if (PublishVersionInfo != null)
            {
                PublishVersionInfo(this, new VersionEventArgs()
                {
                    UserID = container.UserID,
                    Date = container.Date,
                    Version = container.Version
                });
            }
        }


        public int TenantId
        {
            get
            {
                if (ViewState["TenantId"] == null)
                    return 0;
                try
                {
                    return Convert.ToInt32(ViewState["TenantId"]);
                }
                catch (System.Exception) { }

                return 0;
            }
            set
            {
                ViewState["TenantId"] = value;
            }
        }


        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Page.RegisterBodyScripts("~/Products/Community/Modules/Wiki/scripts/editpage.js")
                .RegisterStyle("~/Products/Community/Modules/Wiki/content/main.css");
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        protected override void OnUnload(EventArgs e)
        {
            base.OnUnload(e);
        }


        private string _imageHandlerUrlFormat = WikiSection.Section.ImageHangler.UrlFormat;
        public string ImageHandlerUrlFormat
        {
            get { return _imageHandlerUrlFormat; }
            set { _imageHandlerUrlFormat = value; }
        }
    }
}
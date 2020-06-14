﻿<%@ Page Language="C#" MasterPageFile="~/Masters/basetemplate.master" AutoEventWireup="true" EnableViewState="false" CodeBehind="Auth.aspx.cs" Inherits="ASC.Web.Studio.Auth" %>
<%@ Import Namespace="ASC.Core.Billing" %>
<%@ Import Namespace="ASC.Web.Studio.Core" %>
<%@ MasterType TypeName="ASC.Web.Studio.Masters.BaseTemplate" %>
<%@ Import Namespace="ASC.Core" %>

<asp:Content ContentPlaceHolderID="PageContent" runat="server">
    <% if (CoreContext.Configuration.Standalone)
       { %>
    <input type="hidden" id="customerId" value="<%= HttpUtility.HtmlEncode(LicenseReader.CustomerId ?? "") %>" />
    <% } %>

    <% if (CoreContext.Configuration.Personal)
       { %>
    <asp:PlaceHolder runat="server" ID="AutorizeDocuments"></asp:PlaceHolder>
    <% }
       else
       { %>
    <div class="auth-form-page">
        <div id="GreetingBlock" class="authForm <%= withHelpBlock ? "" : "help-block-none" %>">
            <%--header and logo--%>
            <div class="header">
                <img class="logo" src="<%= LogoPath %>" alt="<%= TenantName.HtmlEncode() %>" />
                <h1 class="header-base big blue-text"><%= TenantName.HtmlEncode() %></h1>
            </div>

            <asp:PlaceHolder runat="server" ID="AuthorizeHolder"></asp:PlaceHolder>

            <div class="help-block-signin">
                <asp:PlaceHolder runat="server" ID="CommunitationsHolder"></asp:PlaceHolder>
            </div>
        </div>
    </div>
    <% } %>
    <% if (!string.IsNullOrEmpty(SetupInfo.UserVoiceURL))
       { %>
    <script type="text/javascript" src="<%= SetupInfo.UserVoiceURL %>"></script>
    <% } %>
</asp:Content>

<%--<asp:Content ContentPlaceHolderID="FooterContent" runat="server">
    <% if (!CoreContext.Configuration.Personal)
       { %>
    <div class="footerAuth">
        <%= Resource.PoweredBy %>
        <a href="http://www.lexic.xyz/" title="www.lexic.xyz" class="link underline" target="_blank">www.lexic.xyz</a>
    </div>
    <% } %>
</asp:Content>--%>

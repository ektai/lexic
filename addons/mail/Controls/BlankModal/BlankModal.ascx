﻿<%@ Assembly Name="ASC.Web.Mail" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BlankModal.ascx.cs" Inherits="ASC.Web.Mail.Controls.BlankModal" %>
<%@ Import Namespace="ASC.Core" %>
<%@ Import Namespace="ASC.Web.Mail.Resources" %>

<div class="backdrop hidden" blank-page=""></div>

<div blank-page="" class="dashboard-center-box mail <%= IsAdmin() ? "for-admin" : "for-user" %> hidden">
    <div class="header">
        <span class="close" onclick=" blankModal.close(); ">&times;</span><%= MailResource.BlankModalHeader %>
    </div>
    <div class="content clearFix">
        <% if (IsAdmin() && ASC.Web.Mail.Configuration.Settings.IsAdministrationPageAvailable())
           { %>
            <div class="module-block">
                <div class="img server"></div>
                <div class="title"><%= MailResource.BlankModalMailServerTitle %></div>
                <ul>
                    <li><%= MailResource.BlankModalMailServerTip1 %></li>
                    <% if (CoreContext.Configuration.Standalone)
                       { %>
                           <li><%= MailResource.BlankModalMailServerTip2Stanalone %></li> 
                    <% }
                       else
                       {%>
                           <li><%= MailResource.BlankModalMailServerTip2 %></li>
                      <% } %>
                    <li><%= MailResource.BlankModalMailServerTip3 %></li>
                </ul>
            </div>
        <% } %>
        <div class="module-block">
            <div class="img contacts"></div>
            <div class="title"><%= MailResource.BlankModalAccountsTitle %></div>
            <ul>
                <li><%= MailResource.BlankModalAccountsTip1 %></li>
                <li><%= MailResource.BlankModalAccountsTip2 %></li>
                <li><%= MailResource.BlankModalAccountsTip3 %></li>
            </ul>
        </div>
        <div class="module-block">
            <div class="img tags"></div>
            <div class="title"><%= MailResource.BlankModalTagsTitle %></div>
            <ul>
                <li><%= MailResource.BlankModalTagsTip1 %></li>
                <li><%= MailResource.BlankModalTagsTip2 %></li>
                <li><%= MailResource.BlankModalTagsTip3 %></li>
            </ul>
        </div>
        <% if (IsCrmAvailable())
           { %>
            <div class="module-block">
                <div class="img crm"></div>
                <div class="title"><%= MailResource.BlankModalCRMTitle %></div>
                <ul>
                    <li><%= MailResource.BlankModalCRMTip1 %></li>
                    <li><%= MailResource.BlankModalCRMTip2 %></li>
                    <li><%= MailResource.BlankModalCRMTip3 %></li>
                </ul>
            </div>
        <% } %>
    </div>
    <div class="dashboard-buttons">
        <a class="button huge create-button" href="javascript:void(0)" onclick=" blankModal.addAccount(); "><%= MailResource.BlankModalCreateBtn %></a>
        <% if (IsAdmin())
           { %>
            <% if (ASC.Web.Mail.Configuration.Settings.IsAdministrationPageAvailable())
               { %>
            <span class="or-split"><%= MailResource.Or %></span>
            <a class="link create-button-link" href="javascript:void(0)" onclick=" blankModal.setUpDomain(); "><%= MailResource.BlankModalSetUpDomainBtn %></a>
            <% } %>
        <% }
           else
           { %>
            <% if (ASC.Web.Mail.Configuration.Settings.IsMailCommonDomainAvailable())
               { %>
            <span class="or-split"><%= MailResource.Or %></span>
            <a class="link create-button-link" href="javascript:void(0)" onclick=" blankModal.addMailbox(); "><%= MailResource.BlankModalCreateMailboxBtn %></a>
            <% } %>
        <% } %>
    </div>
</div>
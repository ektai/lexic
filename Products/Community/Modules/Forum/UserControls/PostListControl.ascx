﻿<%@ Assembly Name="ASC.Web.Community" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PostListControl.ascx.cs" Inherits="ASC.Web.UserControls.Forum.PostListControl" %>
<%@ Import Namespace="ASC.Web.UserControls.Forum.Resources" %>

<asp:PlaceHolder runat="server" ID="pollHolder"></asp:PlaceHolder>
<div class="clearFix">
    <asp:PlaceHolder ID="postListHolder" runat="server"></asp:PlaceHolder>
</div>
<%if (PostPagesCount>20){ %>
<div class="navigationLinkBox forums">
    <table id="tableForNavigation" cellpadding="4" cellspacing="0">
        <tbody>
            <tr>
                <td>
                    <div style="clear: right; display: inline-block;">
                        <asp:PlaceHolder ID="bottomPageNavigatorHolder" runat="server"></asp:PlaceHolder>
                    </div>
                </td>
                <td style="text-align: right;">
                    <span class="gray-text"><%=ForumUCResource.TotalPages%>: </span>
                    <span class="gray-text" style="margin-right: 20px;"><%=PostPagesCount%></span>
                    <span class="gray-text"><%=ForumUCResource.ShowOnPage%>: </span>
                    <select class="top-align">
                        <option value="20">20</option>
                        <option value="50">50</option>
                        <option value="75">75</option>
                        <option value="100">100</option>
                    </select>
                </td>
            </tr>
        </tbody>
    </table>
</div>
<%} %>


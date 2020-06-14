<%@ Assembly Name="ASC.Web.Mail" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MailBox.ascx.cs" Inherits="ASC.Web.Mail.Controls.MailBox" %>
<%@ Import Namespace="ASC.Web.Mail.Resources" %>

<%@ Register TagPrefix="sc" Namespace="ASC.Web.Studio.Controls.Common" Assembly="ASC.Web.Studio" %>

<div id="mailBoxContainer" class="mailBoxContainer">

    <asp:PlaceHolder runat="server" ID="ControlPlaceHolder"></asp:PlaceHolder>

    <asp:PlaceHolder ID="TagsPageHolder" runat="server"></asp:PlaceHolder>
    <div id="popupDocumentUploader">
        <asp:PlaceHolder ID="_phDocUploader" runat="server"></asp:PlaceHolder>
    </div>

    <asp:PlaceHolder runat="server" ID="fileholder"></asp:PlaceHolder>

    <div id="itemContainer">

        <div class="filterPanel hidden">
            <div id="FolderFilter"></div>
        </div>

        <div class="contentMenuWrapper messagesList" style="display: none">
            <ul class="clearFix contentMenu contentMenuDisplayAll" id="MessagesListGroupButtons">
                <li class="menuAction menuActionSelectAll">
                    <div class="menuActionSelect">
                        <input id="SelectAllMessagesCB" type="checkbox" title="<%= MailResource.SelectAll %>" />
                    </div>
                    <div id="SelectAllMessagesDropdown" class="down_arrow" title="<%= MailResource.Select %>" />
                </li>
                <li class="menuAction menuActionDelete">
                    <span title="<%= MailResource.DeleteBtnLabel %>"><%= MailResource.DeleteBtnLabel %></span>
                </li>
                <li class="menuAction menuActionNotSpam">
                    <span title="<%= MailScriptResource.NotSpamLabel %>"><%= MailScriptResource.NotSpamLabel %></span>
                </li>
                <li class="menuAction menuActionRestore">
                    <span title="<%= MailScriptResource.RestoreBtnLabel %>"><%= MailScriptResource.RestoreBtnLabel %></span>
                </li>
                <li class="menuAction menuActionSpam">
                    <span title="<%= MailScriptResource.SpamLabel %>"><%= MailScriptResource.SpamLabel %></span>
                </li>
                <li class="menuAction menuActionMoveTo">
                    <span title="<%= MailResource.MoveTo %>"><%= MailResource.MoveTo %></span>
                    <div class="down_arrow"></div>
                </li>
                <li class="menuAction menuActionAddTag">
                    <span title="<%= MailResource.AddTag %>"><%= MailResource.AddTag %></span>
                    <div class="down_arrow"></div>
                </li>
                <li class="menuAction menuActionRead" read="<%= MailScriptResource.ReadLabel %>" unread="<%= MailScriptResource.UnreadLabel %>">
                    <span title="<%= MailScriptResource.ReadLabel %>"><%= MailScriptResource.ReadLabel %></span>
                </li>
                <li class="menuAction menuActionImportant" important="<%= MailScriptResource.MarkImportantLabel %>" notimportant="<%= MailScriptResource.MarkNotImportantLabel %>">
                    <span title="<%= MailScriptResource.MarkImportantLabel %>"><%= MailScriptResource.MarkImportantLabel %></span>
                </li>
                <li class="menuAction menuActionMore" style="display: none">
                    <span title="<%= MailResource.MoreMenuButton %>">...</span>
                </li>
                <li class="menu-action-simple-pagenav"></li>
                <li class="menu-action-checked-count" id="OverallDeselectAll">
                    <div class="baseLinkAction">
                        <span title="<%= MailResource.OverallDeselectAll %>"><%= MailResource.OverallDeselectAll %></span>
                    </div>
                </li>
                <li class="menu-action-checked-count" id="OverallSelectionNumber">
                    <div>
                        <b><span id="OverallSelectionNumberText"></span></b>&nbsp;<span><%= MailResource.OverallSelected %></span>
                    </div>
                </li>
                <li class="menu-action-on-top">
                    <a class="on-top-link" onclick=" javascript:window.scrollTo(0, 0); ">
                        <%= MailResource.OnTopLabel %>
                    </a>
                </li>
            </ul>
            <div class="header-menu-spacer">&nbsp;</div>
        </div>
    </div>

    <div id="helpPanel" class="display-none"></div>

    <div id="bottomNavigationBar" style="display: none">
        <table id="tableForMessagesNavigation" class="mail-navigation-table" cellpadding="4" cellspacing="0" border="0">
            <tbody>
                <tr>
                    <td>
                        <div id="divForMessagesPager" class="mail-navigation-pager">
                            <asp:PlaceHolder ID="_phPagerContent" runat="server"></asp:PlaceHolder>
                        </div>
                    </td>
                    <td style="text-align: right;">
                        <span class="mail-gray-text" id="TotalItems"></span>
                        <span class="mail-gray-text mail-navigation-total" id="totalItemsOnAllPages"></span>

                        <span class="mail-gray-text"><%= MailResource.ShowOnPage %>:&nbsp;</span>
                        <select class="top-align">
                            <option value="25">25</option>
                            <option value="50">50</option>
                            <option value="75">75</option>
                            <option value="100">100</option>
                        </select>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>

    <div id="messagesActionMenu" class="studio-action-panel">
        <ul class="dropdown-content">
            <li class="openMail">
                <a class="dropdown-item"><%= MailResource.OpenBtnLabel %></a>
            </li>
            <li class="openNewTabMail">
                <a class="dropdown-item"><%= MailResource.OpenInNewTabBtnLabel %></a>
            </li>
            <li class="openSeparator">
                <div class="dropdown-item-seporator"></div>
            </li>
            <li class="replyMail">
                <a class="dropdown-item"><%= MailResource.ReplyBtnLabel %></a>
            </li>
            <li class="replyAllMail">
                <a class="dropdown-item"><%= MailResource.ReplyAllBtnLabel %></a>
            </li>
            <li class="forwardMail">
                <a class="dropdown-item"><%= MailResource.ForwardLabel %></a>
            </li>
            <li class="createEmail">
                <a class="dropdown-item"><%= MailResource.CreateEmailToSenderLabel %></a>
            </li>
            <li class="composeSeparator">
                <div class="dropdown-item-seporator"></div>
            </li>
            <li class="setReadMail">
                <a class="dropdown-item"><%= MailResource.MarkAsRead %></a>
            </li>
            <li class="markImportant">
                <a class="dropdown-item"><%= MailResource.MarkAsImportant %></a>
            </li>
            <li class="markSeparator">
                <div class="dropdown-item-seporator"></div>
            </li>
            <% if (IsMailPrintAvailable())
               { %>

            <li class="printMail">
                <a class="dropdown-item"><%= MailScriptResource.PrintBtnLabel %></a>
            </li>
             <li class="printSeparator">
                <div class="dropdown-item-seporator"></div>
            </li>
            <% } %>
            <li class="moveToFolder">
                <a class="dropdown-item"><%= MailScriptResource.SpamLabel %></a>
            </li>
            <li class="deleteMail">
                <a class="dropdown-item"><%= MailResource.DeleteBtnLabel %></a>
            </li>
        </ul>
    </div>
</div>


<div id="removeQuestionWnd" style="display: none">
    <sc:Container ID="QuestionPopup" runat="server">
        <Header>
        </Header>
        <Body>
            <div class="mail-confirmationAction">
                <p class="questionText"></p>
            </div>
            <div class="buttons">
                <button class="button middle blue remove" type="button"><%= MailResource.DeleteBtnLabel %></button>
                <button class="button middle gray cancel" type="button"><%= MailScriptResource.CancelBtnLabel %></button>
            </div>
        </Body>
    </sc:Container>
</div>


<div id="attachmentActionMenu" class="studio-action-panel">
    <ul class="dropdown-content">
        <li><a class="downloadAttachment dropdown-item"><%= MailResource.DownloadAttachment %></a></li>
        <li><a class="viewAttachment dropdown-item"><%= MailResource.ViewAttachment %></a></li>
        <li><a class="editAttachment dropdown-item"><%= MailResource.EditAttachment %></a></li>
        <li><a class="saveAttachmentToDocs dropdown-item"><%= MailResource.SaveAttachToDocs %></a></li>
        <li><a class="saveAttachmentToCalendar dropdown-item"><%= MailResource.SaveAttachToCalendar %></a></li>
    </ul>
</div>

<div id="attachmentEditActionMenu" class="studio-action-panel">
    <ul class="dropdown-content">
        <li><a class="downloadAttachment dropdown-item"><%= MailResource.DownloadAttachment %></a></li>
        <li><a class="viewAttachment dropdown-item"><%= MailResource.ViewAttachment %></a></li>
        <li><a class="deleteAttachment dropdown-item"><%= MailResource.DeleteAttachment %></a></li>
    </ul>
</div>

<div id="messageActionMenu" class="studio-action-panel">
    <ul class="dropdown-content">
        <li><a class="replyMail dropdown-item"><%= MailResource.ReplyBtnLabel %></a></li>
        <li><a class="replyAllMail dropdown-item"><%= MailResource.ReplyAllBtnLabel %></a></li>
        <li><a class="forwardMail dropdown-item"><%= MailResource.ForwardLabel %></a></li>
        <li><a class="singleViewMail dropdown-item"><%= MailResource.SingleViewLabel %></a></li>
        <li><a class="deleteMail dropdown-item"><%= MailResource.DeleteBtnLabel %></a></li>
        <% if (IsMailPrintAvailable())
           { %>
        <li><a class="printMail dropdown-item"><%= MailScriptResource.PrintBtnLabel %></a></li>
        <% } %>
        <li><a class="alwaysHideImages dropdown-item"><%= MailScriptResource.HideImagesLabel %></a></li>
        <li><a class="exportMessageToCrm dropdown-item"><%= MailResource.ExportMessageToCRM %></a></li>
        <li><a class="createPersonalContact dropdown-item"><%=MailScriptResource.CreatePersonalContact %></a></li>
        <li><a class="createCrmPerson dropdown-item"><%= MailScriptResource.CreateNewCRMPerson %></a></li>
        <li><a class="createCrmCompany dropdown-item"><%= MailScriptResource.CreateNewCRMCompany %></a></li>
    </ul>
</div>

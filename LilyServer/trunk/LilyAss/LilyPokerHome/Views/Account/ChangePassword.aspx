<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<LilyPokerHome.Models.ChangePasswordModel>" %>

<asp:Content ID="changePasswordTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Change Password - My ASP.NET MVC Application
</asp:Content>

<asp:Content ID="changePasswordContent" ContentPlaceHolderID="MainContent" runat="server">
    <hgroup class="title">
        <h1>Change Password.</h1>        
        <h2>Use this form to change your password.</h2>
    </hgroup>

    <p class="message-info">
        Passwords must be at least <%: Membership.MinRequiredPasswordLength %> characters long.
    </p>

    <% using (Html.BeginForm()) { %>
        <%: Html.ValidationSummary() %>

        <fieldset>
            <legend>Change Password Form</legend>
            <ol>
                <li>
                    <%: Html.LabelFor(m => m.OldPassword) %>
                    <%: Html.PasswordFor(m => m.OldPassword) %>        
                </li>
                <li>
                    <%: Html.LabelFor(m => m.NewPassword) %>
                    <%: Html.PasswordFor(m => m.NewPassword) %>                    
                </li>
                <li>
                    <%: Html.LabelFor(m => m.ConfirmPassword) %>
                    <%: Html.PasswordFor(m => m.ConfirmPassword) %>                    
                </li>
            </ol>
            <input type="submit" value="Change password" />
        </fieldset>
    <% } %>
    <br/>
    <%: Html.ActionLink("用户权限管理", "AddUserToRoles", "ActionManager")%>
    <%: Html.ActionLink("Action管理", "Index", "ActionManager")%>
</asp:Content>

<asp:Content ID="scriptsContent" ContentPlaceHolderID="ScriptsSection" runat="server">
    <%: Scripts.Render("~/bundles/jqueryval") %>
</asp:Content>

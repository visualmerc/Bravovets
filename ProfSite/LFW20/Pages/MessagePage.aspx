<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MessagePage.aspx.cs" Inherits="MessagePage" %>
<% var language = ProfSite.Infrastructure.LanguageRoute.GetLanguage(HttpContext.Current.Request.Headers["Host"]); %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
<%--    <link href="/LFW20/Styles/LFW.css" type="text/css" rel="stylesheet" />--%>
</head>
<body>
    <form id="form1" runat="server">
    <div>
     <!-- This part of the page contains LFW20 controls. Do not remove! -->
    <asp:Panel ID="LFW20Container" runat="server">
        <fieldset>
            <legend>Message page</legend>
            <asp:Literal runat="server" ID="litMessage"></asp:Literal>
        </fieldset>
    </asp:Panel>
    <!-- End of LFW20 controls. -->
    </div>
    </form>
     <%if (language == "fr")
        { %>
            <noscript>
                <a href="//policy.merck.sitemorse.com/e0adce0b-fr.html" style="z-index: 32001; background: #fff none no-repeat scroll 0 0; text-decoration: none; display: block; position: fixed; bottom: 0; right: 20px; font: normal normal normal 10pt Verdana,Geneva,sans-serif; color: #000000; border: 1px solid #b202c2; border-bottom: 0; border-radius: 5px 5px 0 0; padding: 4px 20px; cursor: pointer; opacity: 0.8">cookies</a>
            </noscript>
            <script type="text/javascript" src="//policy.merck.sitemorse.com/e0adce0b_panel-fr.js"></script>
       <% }%>
</body>
</html>

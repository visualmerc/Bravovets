<%@ Page Language="C#" AutoEventWireup="true" EnableViewState="false" ValidateRequest="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Blocked Page</title>
<%--    <link href="/LFW20/Styles/LFW.css" type="text/css" rel="stylesheet" />--%>
    <meta http-equiv="Content-Type" content="text/html;charset=UTF-8" />
</head>
<body>
    <form id="LFW20form" class="universalForm" runat="server">
    <!-- This part of the page contains LFW20 controls. Do not remove! -->
    <asp:Panel ID="LFW20Container" runat="server">
        <fieldset>
            <legend>Blocked</legend>
            <asp:Literal runat="server" ID="litBlocked"></asp:Literal>
        </fieldset>
    </asp:Panel>
    <!-- End of LFW20 controls. -->
    </form>
</body>
</html>

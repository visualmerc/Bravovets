<%@ Page Language="C#" AutoEventWireup="true" EnableViewState="false" ValidateRequest="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Request explicit permission</title>
    <%--<link href="/LFW20/Styles/LFW.css" type="text/css" rel="stylesheet" />--%>
    <meta http-equiv="Content-Type" content="text/html;charset=UTF-8" />
</head>
<body>
    <form id="LFW20form" runat="server" class="universalForm">
    <!-- This part of the page contains LFW20 controls. Do not remove! -->
    <asp:Panel ID="LFW20Container" runat="server">
        <fieldset>
            <legend>Request explicit permission</legend>
            <asp:Literal runat="server" ID="litUpgradeExplicitPermissionForm"></asp:Literal>
        </fieldset>
        <dl class="formCols" id="fsubmit">
            <dt><span class="displayNone"></span></dt>
            <dd>
                <asp:Button runat="server" ID="butRequest" Text="Request" />
            </dd>
        </dl>
    </asp:Panel>
    <!-- End of LFW20 controls. -->
    </form>
</body>
</html>

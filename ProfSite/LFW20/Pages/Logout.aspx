<%@ Page Language="C#" AutoEventWireup="true" EnableViewState="false" ValidateRequest="false" %>

<%@ Import Namespace="ProfSite.Resources" %>

<% var language = ProfSite.Infrastructure.LanguageRoute.GetLanguage(HttpContext.Current.Request.Headers["Host"]); %>

<!doctype html>
<html lang="en">

<head runat="server">
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <title>Bravovets</title>
    <meta name="description" content="">
    <meta name="viewport" content="width=device-width">
    <link rel="icon" href="~/images/favicon.png" />

    <link type="text/css" rel="stylesheet" href="https://fast.fonts.net/cssapi/36b782c8-14a6-42ac-b9e8-820a1ab045fd.css" />
    <link rel="stylesheet" href="/Content/main.css">
    <link rel="stylesheet" href="/Content/main2.css">
    <script src="/Scripts/modernizr.js"></script>
</head>
<body class="globalpage global-bg">
    <script type="text/javascript">
        window.location = "/lfw20/pages/login.aspx";
    </script>

    <!--[if lt IE 10]>
    <p class="browsehappy">
        You are using an <strong>outdated</strong>
        browser. Please
        <a href="http://browsehappy.com/">upgrade your browser</a>
        to improve your experience.
    </p>
    <![endif]-->
    <div class="circles-header"></div>
    <div class="dog-bg"></div>

    <div class="page-wrapper">
        <header class="navbar bv-nav" role="banner">
            <div class="container">
                <div class="row nav-wrapper">
                    <div class="col-sm-4">
                        <div class="navbar-header">
                            <a href="/bravecto" class="navbar-brand bravovets"></a>
                        </div>
                    </div>
                </div>
            </div>
        </header>
        <div class="page-content-wrapper">
            <div class="container">
                <div class="row">
                    <div class="col-sm-6">
                        <div class="white-trans-bg">
                            <hr class="plus diagonal">
                            <form id="LFW20form" runat="server" class="universalForm">
                                <!-- This part of the page contains LFW20 controls. Do not remove! -->
                                <asp:Panel ID="LFW20Container" runat="server">
                                    <fieldset>
                                        <asp:Literal runat="server" ID="litLogOut"></asp:Literal>
                                    </fieldset>
                                </asp:Panel>
                                <!-- End of LFW20 controls. -->
                            </form>
                            <div class="clearfix"></div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <footer>
        <div class="container">
            <div class="row">
                <div class="col-xs-12 footer-nav-wrapper">
                    <img class="footer-logo hidden-xs" src="/images/logo-MerckAH.png" alt="<%=Resource.Shared_Layout_FooterMerckAnimalHealthLogo %>" />
                    <hr class="plus diagonal hidden-xs">
                    <img class="footer-logo centered visible-xs" src="/images/logo-MerckAH.png" alt="<%=Resource.Shared_Layout_FooterMerckAnimalHealthLogo %>" />
                </div>
            </div>
        </div>
    </footer>
</body>
</html>

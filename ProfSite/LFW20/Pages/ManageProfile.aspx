<%@ Page Title="Manage Profile" Language="C#" AutoEventWireup="true" ValidateRequest="false" %>

<%@ Import Namespace="ProfSite.Resources" %>

<%  var language = ProfSite.Infrastructure.LanguageRoute.GetLanguage(HttpContext.Current.Request.Headers["Host"]); %>
<!doctype html>
<html lang="en">

<head runat="server">
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <title><%=Resource.Lfw20_Pages_Profile_Title %></title>
    <meta name="description" content="">
    <meta name="viewport" content="width=device-width">
    <link rel="icon" href="~/images/favicon.png" />

    <link type="text/css" rel="stylesheet" href="https://fast.fonts.net/cssapi/36b782c8-14a6-42ac-b9e8-820a1ab045fd.css" />

    <link rel="stylesheet" href="~/Content/main.css">
    <link rel="stylesheet" href="/Content/main2.css">
    <!-- endbuild -->
    <script src="/Scripts/modernizr.js"></script>
</head>

<body class="bravectopage">
    <!-- test for an outdated browser -->
    <!--[if lt IE 10]>
<p class="browsehappy">
    You are using an <strong>outdated</strong>
    browser. Please
    <a href="http://browsehappy.com/">upgrade your browser</a>
    to improve your experience.
</p>
<![endif]-->
    <div class="circles-header"></div>
    <div class="page-wrapper">
        <header class="navbar bv-nav" role="banner">
            <div class="container">
                <div class="row nav-wrapper">
                    <div class="col-sm-4">
                        <div class="navbar-header">
                            <button class="navbar-toggle" type="button" data-toggle="collapse" data-target=".bs-navbar-collapse">
                                <span class="sr-only"><%=Resource.Shared_Layout_Bravecto_HeaderToggleNavigation %></span>
                                <span class="icon-bar"></span>
                                <span class="icon-bar"></span>
                                <span class="icon-bar"></span>
                            </button>
                            <a href="/bravecto" class="navbar-brand bravovets">
                                <img src="/images/logo-bravecto@2x.png" alt="Bravecto Logo" class="logo-bravecto"></a>
                        </div>
                    </div>
                    <div class="col-sm-8">
                        <nav class="collapse navbar-collapse bs-navbar-collapse" role="navigation">
                            <ul class="nav navbar-nav navbar-right navbar-bravecto">
                                <li>
                                    <a href="/bravecto/innovation"><%=Resource.BravectoInnovation %></a>
                                </li>
                                <li>
                                    <a href="/bravecto/compliance"><%=Resource.BravectoCompliance %></a>
                                </li>
                                <li>
                                    <a href="/bravecto/newbusiness"><%=Resource.BravectoNewBusiness %></a>
                                </li>
                                <li class="bravovets-link">
                                    <a href="/dashboard"><%=Resource.Shared_SiteMap_BravoVets %></a>
                                </li>
                                <ul class="nav navbar-nav product-info">
                                    <li>
                                        <a target="_blank" href="/docs/Bravecto_SPC-PIL_en.pdf"><%=Resource.Shared_Layout_Bravecto_HeaderDownloadProductInfo %>
                                        </a>
                                    </li>
                                    <!-- TODO: Uncomment this when the search functionality is added to the site -->
                                    <!--  <li class="search"><a href="#footer-search">Search<span class="icon-search"></span></a></li> -->
                                </ul>
                            </ul>
                        </nav>
                    </div>
                </div>
            </div>
        </header>

        <div class="page-content-wrapper customize-lfw registration no-margin">
            <div class="container">
                <div class="row">
                    <div class="col-sm-6 col-sm-offset-3">
                        <div class="white-trans-bg">
                            <div class="lfwbrav_content">
                                <form id="LFW20form" runat="server" class="universalForm">
                                    <!-- This part of the page contains LFW20 controls. Do not remove! -->
                                    <asp:Panel ID="LFW20Container" runat="server">
                                        <fieldset>
                                            <legend>User Profile</legend>
                                            <asp:Literal runat="server" ID="litRegistrationForm"></asp:Literal>
                                        </fieldset>
                                        <dl id="fsubmit" class="formCols">
                                            <dt><span class="displayNone"></span></dt>
                                            <dd>
                                                <asp:Button runat="server" ID="butSave" Text="Save" />
                                            </dd>
                                        </dl>
                                    </asp:Panel>
                                    <!-- End of LFW20 controls. -->
                                </form>
                            </div>
                            <a href="/LFW20/Pages/ChangePassword.aspx"><%=Resource.BravoVets_Footer_ChangePassword %></a>
                            <a href="/LFW20/Pages/ChangeEmail.aspx"><%=Resource.BravoVets_Footer_ChangeEmail %></a>
                            <input type="hidden" name="__auto_csrf_token" value="****secret-token****" />
                            <div class="clearfix"></div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <footer>
        <div class="container">
            <!-- TODO: Uncomment this when search is added to the site -->
            <!-- <div id="footer-search" class="row search-wrapper visible-xs">
			<div class="col-xs-12"><input type="text" placeholder="Search"></div>
		</div> -->
            <div class="row">
                <div class="col-xs-12 footer-nav-wrapper">
                    <ul class="nav nav-pills">
                        <li>
                            <a <%=Resource.BravoVets_Footer_Target %> href="<%=Resource.BravoVets_Footer_Disclaimer %>"><%=Resource.Shared_Layout_FooterDisclaimer %></a>
                        </li>
                        <li>
                            <a <%=Resource.BravoVets_Footer_Target %> href="<%=Resource.BravoVets_Footer_PrivacyPolicy %>"><%=Resource.Shared_Layout_FooterPrivacyPolicy %></a>
                        </li>
                    </ul>
                </div>
            </div>
        </div>
    </footer>
    <script src="/Scripts/jquery-1.10.2.min.js"></script>
    <script src="/Scripts/bootstrap.min.js"></script>

    <%if (language == "fr")
        { %>
            <noscript>
                <a href="//policy.merck.sitemorse.com/e0adce0b-fr.html" style="z-index: 32001; background: #fff none no-repeat scroll 0 0; text-decoration: none; display: block; position: fixed; bottom: 0; right: 20px; font: normal normal normal 10pt Verdana,Geneva,sans-serif; color: #000000; border: 1px solid #b202c2; border-bottom: 0; border-radius: 5px 5px 0 0; padding: 4px 20px; cursor: pointer; opacity: 0.8">cookies</a>
            </noscript>
            <script type="text/javascript" src="//policy.merck.sitemorse.com/e0adce0b_panel-fr.js"></script>
       <% }%>
</body>

</html>

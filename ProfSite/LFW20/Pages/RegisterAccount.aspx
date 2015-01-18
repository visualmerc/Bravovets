<%@ Page Title="Register Account" Language="C#" AutoEventWireup="true" ValidateRequest="false" %>

<%@ Import Namespace="ProfSite.Resources" %>

<% var language = ProfSite.Infrastructure.LanguageRoute.GetLanguage(HttpContext.Current.Request.Headers["Host"]); %>

<!doctype html>
<html lang="en">

<head runat="server">
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <title><%=Resource.Bravecto %></title>
    <meta name="description" content="">
    <meta name="viewport" content="width=device-width">
    <link rel="icon" href="~/images/favicon.png" />

    <link type="text/css" rel="stylesheet" href="https://fast.fonts.net/cssapi/36b782c8-14a6-42ac-b9e8-820a1ab045fd.css" />

    <link rel="stylesheet" href="/Content/main.css">
    <link rel="stylesheet" href="/Content/main2.css">
    <!-- endbuild -->
    <script src="/Scripts/modernizr.js"></script>
</head>

<body class="globalpage global-bg">
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

        <div class="page-content-wrapper customize-lfw">
            <div class="container">
                <div class="row">
                    <div class="col-sm-6 col-sm-offset-3">
                        <div class="white-trans-bg">
                            <div class="lfwbrav_content">
                                <form id="LFW20form" class="universalForm" runat="server">
                                    <!-- This part of the page contains LFW20 controls. Do not remove! -->
                                    <asp:Panel ID="LFW20Container" runat="server">
                                        <fieldset>
                                            <legend>Register account</legend>
                                            <asp:Literal runat="server" ID="litRegistrationForm"></asp:Literal>
                                        </fieldset>
                                        <dl class="formCols" id="fsubmit">
                                            <dt><span class="displayNone"></span></dt>
                                            <dd>
                                                <asp:Button runat="server" ID="butSave" Text="Save" />
                                            </dd>
                                        </dl>
                                    </asp:Panel>
                                    <!-- End of LFW20 controls. -->
                                </form>
                            </div>
                            <a href="/LFW20/Pages/Login.aspx"><%=Resource.BravoVets_Footer_Login %></a>
                            <a href="/LFW20/Pages/RequestPassword.aspx"><%=Resource.BravoVets_Footer_ForgotPassword %></a>
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
                            <a target="_blank" href="<%=Resource.BravoVets_Footer_Disclaimer %>"><%=Resource.Shared_Layout_FooterDisclaimer %></a>
                        </li>
                        <li>
                            <a target="_blank" href="<%=Resource.BravoVets_Footer_PrivacyPolicy %>"><%=Resource.Shared_Layout_FooterPrivacyPolicy %></a>
                        </li>
				</ul>
			</div>
		</div>
	</div>
</footer>
    <%if (language == "fr")
        { %>
            <noscript>
                <a href="//policy.merck.sitemorse.com/e0adce0b-fr.html" style="z-index: 32001; background: #fff none no-repeat scroll 0 0; text-decoration: none; display: block; position: fixed; bottom: 0; right: 20px; font: normal normal normal 10pt Verdana,Geneva,sans-serif; color: #000000; border: 1px solid #b202c2; border-bottom: 0; border-radius: 5px 5px 0 0; padding: 4px 20px; cursor: pointer; opacity: 0.8">cookies</a>
            </noscript>
            <script type="text/javascript" src="//policy.merck.sitemorse.com/e0adce0b_panel-fr.js"></script>
       <% }%>
</body>

</html>

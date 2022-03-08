<%-- Copyright (c) 2016-2017 Swiss Agency for Development and Cooperation (SDC)

The program users must agree to the following terms:

Copyright notices
This program is free software: you can redistribute it and/or modify it under the terms of the GNU AGPL v3 License as published by the 
Free Software Foundation, version 3 of the License.
This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of 
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU AGPL v3 License for more details www.gnu.org.

Disclaimer of Warranty
There is no warranty for the program, to the extent permitted by applicable law; except when otherwise stated in writing the copyright 
holders and/or other parties provide the program "as is" without warranty of any kind, either expressed or implied, including, but not 
limited to, the implied warranties of merchantability and fitness for a particular purpose. The entire risk as to the quality and 
performance of the program is with you. Should the program prove defective, you assume the cost of all necessary servicing, repair or correction.

Limitation of Liability 
In no event unless required by applicable law or agreed to in writing will any copyright holder, or any other party who modifies and/or 
conveys the program as permitted above, be liable to you for damages, including any general, special, incidental or consequential damages 
arising out of the use or inability to use the program (including but not limited to loss of data or data being rendered inaccurate or losses 
sustained by you or third parties or a failure of the program to operate with any other programs), even if such holder or other party has been 
advised of the possibility of such damages.

In case of dispute arising out or in relation to the use of the program, it is subject to the public law of Switzerland. The place of jurisdiction is Berne.--%>
<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ForgotPassword.aspx.vb" Inherits="IMIS.ForgotPassword" Async="true" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title><%= imisgen.getMessage("T_FORGOTPASSWORD", True) %></title>
    <link href="StyleSheets/Imis.css" rel="stylesheet" />
    <script src="Javascripts/jquery-1.8.2.min.js"></script>

 <meta charset="utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <!-- Tell the browser to be responsive to screen width -->
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <meta name="description" content="" />
    <meta name="author" content="" />
    <!-- Favicon icon -->
    <link rel="icon" type="image/png" sizes="16x16" href="../assets/images/favicon.png" />
  
    <!-- Bootstrap Core CSS -->
    <link href="../assets/plugins/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <!-- Custom CSS -->
    <link href="CSS/style.css" rel="stylesheet" />
    <!-- You can change the theme colors from here -->
    <link href="CSS/colors/blue.css" id="theme" rel="stylesheet" />
    <!-- HTML5 Shim and Respond.js IE8 support of HTML5 elements and media queries -->
    <!-- WARNING: Respond.js doesn't work if you view the page via file:// -->
    <!--[if lt IE 9]>
    <script src="https://oss.maxcdn.com/libs/html5shiv/3.7.0/html5shiv.js"></script>
    <script src="https://oss.maxcdn.com/libs/respond.js/1.4.2/respond.min.js"></script>
<![endif]-->


</head>
<body>

     <form id="form1" runat="server">
     <%--<asp:HiddenField ID="hfOfflineHFIDFlag" runat="server" Value="0" />--%>
   <!-- ============================================================== -->
    <!-- Preloader - style you can find in spinners.css -->
    <!-- ============================================================== -->
    <div class="preloader">
        <svg class="circular" viewBox="25 25 50 50">
            <circle class="path" cx="50" cy="50" r="20" fill="none" stroke-width="2" stroke-miterlimit="10" /> </svg>
    </div>
    <!-- ============================================================== -->
    <!-- Main wrapper - style you can find in pages.scss -->
    <!-- ============================================================== -->
    <section id="wrapper">
        <div class="login-register" style="background-color:#002D55;">        
           
            
            <div class="form-group m-b-0">
                        <div class="col-sm-12 text-center">
                       <img src="../assets/images/logo-text.png" style="padding-bottom: 20px;"  alt="IMIS"  />
                            </div>
                    </div>
			
            <div class="login-box card">
            <div class="card-body">
            					
                <div class="form-horizontal form-material" id="loginform" >
                    
                     <div class="form-group m-b-0">
                        <div class="col-sm-12 text-center">
                       <h5 class="box-title m-b-20"> Forgot Password </h5>
                    <h5 class="box-title m-b-20"> <asp:Literal ID="heading" runat="server" Text='<%$RESOURCES:Resource, L_FOROGOTPASSWORDHEADING %>'></asp:Literal></h5>
                            </div>
                    </div>
                    
                    
                    
                    
                        <div class="col-xs-12 col-sm-12 col-md-12 m-t-10 text-center">
                            <div class="social">
                                <asp:Literal ID="msg" runat="server" ></asp:Literal>
								<asp:ValidationSummary ID="vs" runat="server" DisplayMode="List" ValidationGroup="submit" />
								</div>
                        </div>
                    
					
					<div class="form-group ">
                        <div class="col-xs-12">
              <asp:Label ID="L_LoginName" runat="server" Text='<%$ Resources:Resource,L_USERNAME %>'></asp:Label>
                <asp:TextBox ID="txtLoginName" CssClass="form-control" runat="server" ></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ValidationGroup="submit" 
                        ControlToValidate="txtLoginName" ErrorMessage="*" SetFocusOnError="True">*</asp:RequiredFieldValidator>
                    </div>
                    <div class="form-group">
                        <div class="col-xs-12">
               <asp:Label ID="L_Password" runat="server" Text='<%$ Resources:Resource,L_NEWPASSWORD %>'></asp:Label></div>
               <asp:TextBox ID="txtPassword" CssClass="form-control" runat="server" TextMode="Password" ></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfPassword" runat="server"  ControlToValidate="txtPassword" ErrorMessage='<%$ Resources:Resource, V_PASSWORDREQUIRED %>'  ValidationGroup="submit" Text="*"></asp:RequiredFieldValidator>
                

                            </div>
                    </div>
                    
                  <div class="form-group text-center m-t-20">
                        <div class="col-xs-12">
                      <asp:Button CssClass="btn btn-info btn-lg btn-block" ID="btnSubmit" runat="server" Text="Submit" ValidationGroup="submit" ClientIDMode="Static" />
                        </div>
                    </div>
                   
                    <div class="form-group m-b-0">
                        <div class="col-sm-12 text-center">
                       <a href="Default.aspx"><asp:Literal ID="Login" runat="server" Text='<%$RESOURCES:Resource, L_BACKTOLOGIN %>'></asp:Literal></a>
                            </div>
                    </div>
                </div>
            
            </div>
          </div>
        </div>
        
    </section>
    <!-- ============================================================== -->
    <!-- End Wrapper -->
    <!-- ============================================================== -->
    <!-- ============================================================== -->
    <!-- All Jquery -->
    <!-- ============================================================== -->
    <script type="text/javascript" src="../assets/plugins/jquery/jquery.min.js"></script>
    <!-- Bootstrap tether Core JavaScript -->
    <script type="text/javascript" src="../assets/plugins/bootstrap/js/popper.min.js"></script>
    <script type="text/javascript" src="../assets/plugins/bootstrap/js/bootstrap.min.js"></script>
    <!-- slimscrollbar scrollbar JavaScript -->
    <script type="text/javascript" src="CSS/jquery.slimscroll.js"></script>
    <!--Wave Effects -->
    <script type="text/javascript" src="CSS/waves.js"></script>
    <!--Menu sidebar -->
    <script type="text/javascript" src="CSS/sidebarmenu.js"></script>
    <!--stickey kit -->
    <script type="text/javascript" src="../assets/plugins/sticky-kit-master/dist/sticky-kit.min.js"></script>
    <script type="text/javascript" src="../assets/plugins/sparkline/jquery.sparkline.min.js"></script>
    <!--Custom JavaScript -->
    <script type="text/javascript" src="CSS/custom.min.js"></script>
    <!-- ============================================================== -->
    <!-- Style switcher -->
    <!-- ============================================================== -->
    <script type="text/javascript" src="../assets/plugins/styleswitcher/jQuery.style.switcher.js"></script>

    </form>







</body>
</html>

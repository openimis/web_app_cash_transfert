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
<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Default.aspx.vb" Inherits="IMIS.Login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
   
    <meta charset="utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <!-- Tell the browser to be responsive to screen width -->
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <meta name="description" content="" />
    <meta name="author" content="" />
    <!-- Favicon icon -->
    <link rel="icon" type="image/png" sizes="16x16" href="../assets/images/favicon.png" />
    <title>Login</title>
    <!-- Bootstrap Core CSS -->
    <link href="../assets/plugins/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <!-- Custom CSS -->
    <link href="css/style.css" rel="stylesheet" />
    <!-- You can change the theme colors from here -->
    <link href="CSS/colors/blue.css" id="theme" rel="stylesheet" />
    <!-- HTML5 Shim and Respond.js IE8 support of HTML5 elements and media queries -->
    <!-- WARNING: Respond.js doesn't work if you view the page via file:// -->
    <!--[if lt IE 9]>
    <script src="https://oss.maxcdn.com/libs/html5shiv/3.7.0/html5shiv.js"></script>
    <script src="https://oss.maxcdn.com/libs/respond.js/1.4.2/respond.min.js"></script>
<![endif]-->


    <link href="StyleSheets/Imis.css" rel="stylesheet" type="text/css" />
    <script src="Javascripts/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="Javascripts/Exact.js" type="text/javascript"></script>
    <script type="text/javascript" language="javascript">
        function PreventBack(){window.history.forward();}
        setTimeout("PreventBack();",0);
        window.onunload = function() { null };
        var offlineHFMsg

        if ('<%= imisgen.offlineHF %>' == 'True') {
            offlineHFMsg = '<%= imisgen.getMessage("M_OFFLINEHFID", True)%>';
        } else {
            offlineHFMsg = '<%= imisgen.getMessage("M_OFFLINECHFID", True ) %>';
        }

        function SendOfflineHFID(popupBtnSource, evArgs) {
            if (popupBtnSource == "ok") {
                theForm.__EVENTARGUMENT.value = "SaveOfflineHFID";
                var objPopupFieldValues = evArgs[0]; //alert(objPopupFieldValues.txtOfflineHF);

                theForm.submit();
            } else if (popupBtnSource == "cancel")
                $("form").fadeIn();

        }

        $(document).ready(function() {
            var offlineHFFlag = parseInt($("#<%=hfOfflineHFIDFlag.ClientID %>").val());
            var args = new Array();
            var popupHTML = '<span style="color:#555;">' + offlineHFMsg + '</span>';

            if (offlineHFFlag == 1) {

                args.push({ txtOfflineHF: "" });

                //                popupHTML += '<br/><table align="left" style="width:100%;position:relative;margin-top:10px;" >';
                //                popupHTML += '<tr><td>';
                //                popupHTML += '<span style="color:#70A5DA;"><%= imisgen.getMessage("L_OFFLINEHFID", True)%></span>';
                //                popupHTML += '</td><td>';
                //                popupHTML += '<input type="text" name="txtOfflineHF" class="numbersOnly" style="border:1px solid #70A5DA;padding:3px;" />';
                //                popupHTML += '</td></tr></table>';


                popup.acceptBTN_Text = '<%= imisgen.getMessage("L_OK", True)%>'; //"SAVE"; 
                popup.rejectBTN_Text = '<%= imisgen.getMessage("L_CANCEL", True) %>'; //"CANCEL";

                $("form").fadeOut();
                popup.promptTxtboxName = "txtOfflineHF";
                popup.prompt(popupHTML, SendOfflineHFID, args, true)
                $("#promptTxtbox").addClass("numbersOnly");
                bindAlphaNumber();
            }


          
        });

      

        (function closeChildWindowOnLogout() {
            if (window.opener != null || window.opener != undefined) {
                var pageUrl = window.location; 
                if ((/Default.aspx/i).test(pageUrl)) {
                    window.close();
                    if (!(/Default.aspx/i).test(window.opener.location))
                        window.opener.location = pageUrl;
                }
            }
        })();

        
    </script>
    <style type="text/css">
        #SelectPic
        {
        	padding: 10px;
            width: 100%;
        	margin:auto;
         	text-align:center;
            vertical-align:bottom;
        	position:fixed;
	        top:0;
	        left:0;
	        height:100%;
	        z-index:1001;
	        background-color:Gray;
	        opacity:0.9;
	        filter:alpha(opacity=90);
	        display:none;
	        vertical-align:top;
	    }

       
   </style>
</head>
<body>
    <form id="form1" runat="server">
     <asp:HiddenField ID="hfOfflineHFIDFlag" runat="server" Value="0" />
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
                       <h5 class="box-title m-b-20"> Nafa Management Information System (NMIS) </h5>
                    <h5 class="box-title m-b-20"> Sign In</h5>
                            </div>
                    </div>
                    
                    
                    
                    
                        <div class="col-xs-12 col-sm-12 col-md-12 m-t-10 text-center">
                            <div class="social">
                                
								<asp:Label ID="lblMessage" runat="server" ForeColor="Red"></asp:Label>
								</div>
                        </div>
                    
					
					<div class="form-group ">
                        <div class="col-xs-12">
                <%= imisgen.getMessage("L_USERNAME",False)%>
               <asp:TextBox ID="txtUserName" autocomplete="off" CssClass="form-control" runat="server" required=""></asp:TextBox>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtUserName" ErrorMessage="*" SetFocusOnError="True" ForeColor="Red" Display="Dynamic">*</asp:RequiredFieldValidator>
            </div>
                    </div>
                    <div class="form-group">
                        <div class="col-xs-12">
                        <%= imisgen.getMessage("L_PASSWORD" )%>
               <asp:TextBox ID="txtPassword" CssClass="form-control" runat="server" TextMode="Password"  required="" ></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
                        ControlToValidate="txtPassword" ErrorMessage="*" SetFocusOnError="True" ForeColor="Red" Display="Dynamic">*</asp:RequiredFieldValidator>
                </div>
                    </div>
                    
                  <div class="form-group text-center m-t-20">
                        <div class="col-xs-12">
                            <asp:Button CssClass="btn btn-info btn-lg btn-block" ID="btnLogin" runat="server" Text="Login" />
                        </div>
                    </div>
                   
                    <div class="form-group m-b-0">
                        <div class="col-sm-12 text-center">
                       <a href="ForgotPassword.aspx" id="to-recover" class="text-info m-l-5"><i class="fa fa-lock m-r-5"></i> <asp:Label ID="lblForgotPassword" runat="server" Text='<%$RESOURCES:Resource, L_FORGOTPASSWORD %>'></asp:Label></a> 
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

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
<%@ Page Language="vb" AutoEventWireup="false"  MasterPageFile="~/IMIS.Master" CodeBehind="Reports.aspx.vb" Inherits="IMIS.Reports" 
title='<%$ Resources:Resource,L_REPORTS%>' %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>



<asp:Content ID="HeadContent" ContentPlaceHolderID="head" Runat="Server">

<script type="text/javascript" language="javascript">

/*** REPORT TYPE CRITERIA SWITCHING,CACHEING AND RESELECTING.......****/
    var $lstBox = null;
    var $table = null;
    var $criterias = null;
    var tid = queryString("tid");
    var StartDate;
    var EndDate;

    $(document).ready(function() { //execute first before pageLoadExtend...
        $lstBox = $("#<%=lstboxReportSelector.ClientID %>");
        $lstBox.change(function() {
            filterCriteria("change", "slow");
            getVisibleRegion();
        });

        $("#<%=btnPreview.ClientID %>").click(function () {
            var lstBoxID = $lstBox.val();
         
            $("#<%=lblMsg.ClientID %>").html("");
            if (lstBoxID == 12 && $("#<%=ddlPreviousReportDate.ClientID %>").val() > 0) {
                popup.acceptBTN_Text = '<%=imisgen.getMessage("L_YES", True)%>';
                popup.rejectBTN_Text = '<%=imisgen.getMessage("L_NO", True)%>';
                popup.confirm('<%=imisgen.getMessage("M_REPORTINGDATECHANGED", True ) %>', function (btn) {
                    if (btn == "ok") {
                        reDoPostBack("<%=btnPreview.UniqueID %>");
                    }
                });
                cacheCriteria();
                return false;
            }
            var flag = true;
            $criterias.find("select, input[type=text]").each(function () {
                
                if ($(this).is("input[type=text]")) {
                    flag = isValidJSDate($(this).val()); 
                    if (!flag) {
                        $("#<%=lblMsg.ClientID %>").html('<%= imisgen.getMessage("M_INVALIDDATE", True)%>');
                        $(this).focus();
                        return false;
                    }
                    if (flag == true) {
                        StartDateFill();
                        EndDateFill();
                        if (new Date(StartDate) > new Date(EndDate)) {
                            $("#<%=lblMsg.ClientID %>").html('<%= imisgen.getMessage("M_STARTDATESHOULDBELESSTHANENDDATE", True)%>');
                            flag = false;
                            $("#<%=txtSTARTData.ClientID%>").focus();
                            return false;
                        }
                    }
                }
                else if ($(this).is("select")) {
                    if ($(this).is("#<%=ddlMonth.ClientID %>")) {
                        if (!($lstBox.val() == 1 || $lstBox.val() == 2 || $lstBox.val() == 6)) {
                            if ($(this).val() == 0) {
                                flag = false;
                                $(this).focus();
                                $("#<%=lblMsg.ClientID %>").html('<%= imisgen.getMessage("M_SELECTMONTH", True)%>');
                                return false;
                            }
                        }

                    } else if ($(this).is("#<%=ddlYear.ClientID %>")) {
                        if (isNaN($(this).val())) {
                            flag = false;
                            $("#<%=lblMsg.ClientID %>").html('<%= imisgen.getMessage("M_PLEASESELECTAYEAR", True)%>');
                            $(this).focus();
                            return false;
                        }
                    }
                    else if ($(this).is("#<%=ddlRegion.ClientID%>")) {
                        var lstBoxID = $lstBox.val();
                       
                        if (lstBoxID == 2 || lstBoxID == 17 || lstBoxID == 18 ) {
                            
                            if ($("#<%=ddlRegion.ClientID %>").val() == 0) {
                                flag = false;
                                $("#<%=lblMsg.ClientID %>").html('<%= imisgen.getMessage("M_PLEASESELECTAREGION", True)%>')
                                $(this).focus();
                                return false;
                            }
                        }
                    }

                    else if ($(this).is("#<%=ddlRegionWoNational.ClientID%>")) {
                        var lstBoxID = $lstBox.val();
                        if (lstBoxID == 3 || lstBoxID == 10 || lstBoxID == 12 || lstBoxID == 13) {
                           
                            if ($("#<%=ddlRegionWoNational.ClientID %>").val() == 0) {
                                flag = false;
                                $("#<%=lblMsg.ClientID %>").html('<%= imisgen.getMessage("M_PLEASESELECTAREGION", True)%>')
                                $(this).focus();
                                return false;
                            }
                        }
                    }
 
                    else if ($(this).is("#<%=ddlDistrictWoNational.ClientID%>")) {
                        var lstBoxID = $lstBox.val();
                        if (lstBoxID == 2 || lstBoxID == 12) {
                            if ($("#<%=ddlDistrictWoNational.ClientID%>").val() == 0) {
                                flag = false;
                                $("#<%=lblMsg.ClientID %>").html('<%= imisgen.getMessage("M_PLEASESELECTADISTRICT", True)%>')
                                $(this).focus();
                                return false;
                            }
                        }
                    }

                    else if ($(this).is("#<%=ddlProduct.ClientID %>")) {
                        var lstBoxID = $lstBox.val();
                        if ($("#<%=ddlProduct.ClientID %>").val() == 0) {
                            flag = false;
                            $("#<%=lblMsg.ClientID %>").html('<%= imisgen.getMessage("M_PLEASESELECTAPRODUCT", True)%>')
                            $(this).focus();
                            return false;
                        }
                    }

                    else if ($(this).is("#<%=ddlProductStrict.ClientID%>")) {
                        var lstBoxID = $lstBox.val();
                        
                        if (lstBoxID == 6 || lstBoxID == 3 || lstBoxID == 18) {
                            if ($("#<%=ddlProductStrict.ClientID %>").val() == 0 || $("#<%=ddlProductStrict.ClientID %>").val() == null)                                                        {
                                flag = false;
                                $("#<%=lblMsg.ClientID %>").html('<%= imisgen.getMessage("M_PLEASESELECTAPRODUCT", True)%>')
                                $(this).focus();
                                return false;
                               }

                        }
                      }
                    else if ($(this).is("#<%=ddlHF.ClientID %>")) {
                        var lstBoxID = $lstBox.val();
                        if (lstBoxID == 13) {
                            if ($(this).val() == 0) {
                                flag = false;
                                $("#<%=lblMsg.ClientID %>").html('<%= imisgen.getMessage("M_PLEASESELECTHEALTHFACILITY", True)%>')
                                $(this).focus();
                                return false;
                            }
                        }
                    } else if ($(this).is("#<%=ddlRegion.ClientID%>")) {
                        var lstBoxID = $lstBox.val();
                        if (lstBoxID == 14) {
                            if ($(this).val() == 0) {
                                flag = false;
                                $("#<%=lblMsg.ClientID %>").html('<%= imisgen.getMessage("M_PLEASESELECTAREGION", True)%>')
                                $(this).focus();
                                return false;
                            }
                        }
                    } 
                }
            });

           

            cacheCriteria(); 
            return flag;
        }); //end of button preview click...
        if ($("#<%=hfCompleted.ClientID %>").val() == 0) tid = "";

        getVisibleRegion();

    });



    function getVisibleRegion() {
        var region = '';
        if ($('#<%= ddlRegion.ClientID%>').parent().is(":visible") == true)
            region = '<%= ddlRegion.ClientID %>';
        else if ($('#<%= ddlRegionWoNational.ClientID %>').parent().is(":visible") == true)
                region = '<%= ddlRegionWoNational.ClientID%>';

        $('#<%=hfVisibleRegion.ClientID %>').val(region);

       
    }

    function reDoPostBack(eventtargetid) {
        theForm.__EVENTTARGET.value = eventtargetid;
        theForm.submit();
    }

    function StartDateFill() {
        StartDate = $('#<%=txtSTARTData.ClientID %>').val();
        var day = StartDate.substr(0, 2);
        var Month = StartDate.substr(3, 2);
        var Year = StartDate.substr(6, 4);
        StartDate = Month + "/" + day + "/" + Year;
    }
    function EndDateFill() {
        EndDate = $('#<%=txtENDData.ClientID %>').val();
        var day = EndDate.substr(0, 2);
        var Month = EndDate.substr(3, 2);
        var Year = EndDate.substr(6, 4);
        EndDate = Month + "/" + day + "/" + Year;
    }
    function pageLoadExtend() {
        $table = $("#CriteriaTable");
        if ($lstBox == null)
            $lstBox = $("#<%=lstboxReportSelector.ClientID %>");
        filterCriteria("load", 0);
        getVisibleRegion();
    }
    function cacheCriteria(criterias) {
        var $criteriaControls = $table.find("select,input[type=text],input[type=hidden]").add("#<%=ddlPreviousReportDate.ClientID %>");
        var criterias = '{';
        $criteriaControls.each(function () {
            if ($(this).is("select")) {
                criterias += '"' + $(this).attr("id") + '" : "' + $(this).val() + '"';
            } else if ($(this).is("input[type=text]") || $(this).is("input[type=hidden]")) {
                criterias += '"' + $(this).attr("id") + '" : "' + $(this).val() + '"';
            } 
            if (!$(this).is($criteriaControls.last()))
                criterias += ",";

        });
        criterias += '}';
        $("#<%=hfCriteriaCache.ClientID %>").val(criterias);

        
        
    }
  
    function filterCriteria(eventType,animationSpeed) {
       
        if (typeof (animationSpeed) == "undefined")
            animationSpeed == "slow";

        if (eventType == "change")
            tid = $lstBox.val();
        else if (eventType == "load") {
            if (tid == "") {
                if ($lstBox.val() > 0)
                    tid = $lstBox.val();
                else {
                    tid = $lstBox.find("option").eq(0).val();                    
                }
            }                       
        }
        $table.find("li").add("div.mf").fadeOut(0, function () {
            $(this).hide();
        });  
       
        if (tid == 1) {
            $criterias = $(".sr"); 
         } else if (tid == 2) { 
            $criterias = $(".brl");
         }else if (tid == 3) {
             $criterias = $(".ppr");
         } else if (tid == 4) {
            $criterias = $(".pprUC");
         } else if (tid == 5) {
            $criterias = $(".crp");
         } else if (tid == 6) {
            $criterias = $(".dcrp"); 
        } else if (tid == 7) {
            $criterias = $(".carp");
        }
        else if (tid == 8) {
            $criterias = $(".ua");


        } else if (tid == 9) {
            $criterias = $(".ua");
           
        }

        else if (tid == 10) {
             $criterias = $(".iwp");
         } else if (tid == 11) {
             $criterias = $(".pco");
         } else if (tid == 12) {
             $criterias = $(".mf").add("div.mf");
         } else if (tid == 13) {
             $criterias = $(".co");
         } else if (tid == 14) {
             $criterias = $(".perc");
         } else if (tid == 15) {
             $criterias = $(".fio");
         } else if (tid == 16) {
             $criterias = $(".pi");
         } else if (tid == 17) {
             $criterias = $(".rnw");
         }
         else if (tid == 18) {
             $criterias = $(".ca");
         }
         else if (tid == 19) {
             $criterias = $(".rp");
        }
         else if (tid == 20) {
             $criterias = $(".brl");
         }
         if ($criterias != null) {
             $criterias.fadeIn(animationSpeed, function () {
                 $(this).show();
             });

         }
              
        tid = "";
  }
   
 ///.....end of Criterias Manipulation....***/
</script>


<style type="text/css">

table{width:100%;}
table td.DataEntry{text-align:left;}
table td.FormLabel{text-align:right;}
table tr td.DataEntry{width:auto;}
table tr td.FormLabel{width:auto;}
#process-table td.Empty{width:540px;}
.Month{width:90px;}
.Year{width:60px;}
.dateInput{width:66px;}


    #CriteriaTable
    {
        list-style:none;
        margin:0;
        padding:0;
    }
        #CriteriaTable li
        {
            display:inline-block;
            margin:4px;
            
        }
            #CriteriaTable li .FormLabel
            {
                display:block;
                text-align:left;
                height:auto;
                width:auto;
            }


</style>

</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="Body" Runat="Server">
    
  <div class="divBody" >
       <asp:HiddenField ID="hfCriteriaCache" Value = "#" runat="server" />
     <asp:HiddenField ID="hfCompleted" Value = "9" runat="server" />
      
        <table class="catlabel">
            <tr>
                <td >
                   <asp:label  
                           ID="L_PROCESS"  
                           runat="server" 
                           Text='<%$ Resources:Resource,L_REPORTS %>'>
                   </asp:label>   
                    
                </td>
            </tr>
        </table>
        <asp:UpdatePanel ID="upDistrict" runat="server"  > 
                                <%--<Triggers>
                                <asp:PostBackTrigger ControlID="ddlDistrict" />
                                </Triggers>--%>
      <ContentTemplate>
        <asp:Panel ID="pnlTop" runat="server"  CssClass="panel" GroupingText="" >
            <table>
                <tr>
                    <td>
                        <ul id="CriteriaTable">
                            <li style="display:none">
                                <asp:HiddenField ID="hfVisibleRegion" Value="" runat="server" />
                            </li>
                             <li class="ppr pprUC brl crp dcrp carp">
                                  <asp:Label ID="Label3" runat="server" Text="<%$ Resources:Resource,L_PAYROLLCYCLE %>"></asp:Label>
                                 <asp:DropDownList ID="payrollcycleDropDownList" runat="server" AutoPostBack="true" OnSelectedIndexChanged="payrollcycleDropDownList_SelectedIndexChanged">
                        </asp:DropDownList>
                            </li>
                            <li class="pc ua pco mf co perc fio pi rnw rp">
                                <asp:Label ID="lblSTART" runat="server" Text='<%$ Resources:Resource,L_DATEFROM %>' cssClass="FormLabel"></asp:Label>

                                <asp:TextBox ID="txtSTARTData" runat="server" size="10" Width="80px" class="dateInput">
                                </asp:TextBox>

                                <asp:Button ID="btnSTARTData" runat="server" Height="20px" padding-bottom="3px" Width="20px" />
                                <ajax:CalendarExtender ID="txtSTARTData_CalendarExtender" runat="server"
                                    Format="dd/MM/yyyy" PopupButtonID="btnSTARTData" TargetControlID="txtSTARTData">
                                </ajax:CalendarExtender>
                                <ajax:MaskedEditExtender ID="txtSTARTData_MaskedEditExtender" runat="server"
                                    CultureDateFormat="dd/MM/YYYY"
                                    TargetControlID="txtSTARTData" Mask="99/99/9999" MaskType="Date"
                                    UserDateFormat="DayMonthYear">
                                </ajax:MaskedEditExtender>
                            </li>
                           
                            <li class="poi poiC ">
                                <asp:Label ID="lblMonthPOI" runat="server"  Text='<%$ Resources:Resource, L_MONTH %>' CssClass="FormLabel"></asp:Label>
                                 <asp:DropDownList ID="ddlMonthPOI" Width="90px" runat="server">
                                </asp:DropDownList>
                            </li>

                            <li class="poi poiC ">
                                <asp:Label ID="lblQuarter" runat="server" Text='<%$ Resources:Resource,T_QUARTER%>' cssClass="FormLabel"></asp:Label>
                                <asp:DropDownList ID="ddlQuarter" runat="server" Width="80px" AutoPostBack="true"></asp:DropDownList>
                            </li>

                            <li class="pc ua pco mf co perc fio pi rnw rp">
                                <asp:Label ID="lblEND" runat="server" Text='<%$ Resources:Resource,L_DATETO %>' cssClass="FormLabel" ></asp:Label>
                                <asp:TextBox ID="txtENDData" Width="80px" runat="server" Size="10" class=" dateInput">
                                </asp:TextBox>

                                <asp:Button ID="btnENDData" runat="server" Height="20px" padding-bottom="3px" Width="20px" />
                                <ajax:CalendarExtender ID="txtENDData_CalendarExtender" runat="server"
                                    Format="dd/MM/yyyy" PopupButtonID="btnENDData" TargetControlID="txtENDData">
                                </ajax:CalendarExtender>
                                <ajax:MaskedEditExtender ID="txtENDData_MaskedEditExtender" runat="server"
                                    CultureDateFormat="dd/MM/YYYY"
                                    TargetControlID="txtENDData" Mask="99/99/9999" MaskType="Date"
                                    UserDateFormat="DayMonthYear">
                                </ajax:MaskedEditExtender>
                            </li>

                             <li class="ppr pprUC brl crp dcrp carp">
                                <asp:Label ID="lblMonth" runat="server"  Text="Start Month" CssClass="FormLabel"></asp:Label>
                                <asp:DropDownList Enabled="false"  ID="ddlMonth" Width="90px" runat="server">
                                </asp:DropDownList>
                               
                            </li>
                            <li class="ppr pprUC brl crp dcrp carp" >
                                <asp:Label ID="lblYear" runat="server" Text="Start Year" CssClass="FormLabel"></asp:Label>
                                <asp:DropDownList ID="ddlYear" Enabled="false" Width="68px" runat="server">
                                </asp:DropDownList>
                            </li>
                            <li class="ppr pprUC brl crp dcrp carp">
                                <asp:Label ID="lblddlMonthPOIEnd" runat="server"  Text="End Month" CssClass="FormLabel"></asp:Label>
                                 <asp:DropDownList ID="ddlMonthPOIEnd" Enabled="false" Width="90px" runat="server">
                                </asp:DropDownList>
                            </li>
                             <li class="ppr pprUC brl crp dcrp carp" >
                                <asp:Label ID="lblddlYear" runat="server" Text="End Year" CssClass="FormLabel"></asp:Label>
                                <asp:DropDownList ID="ddlYearEnd" Enabled="false" Width="68px" runat="server">
                                </asp:DropDownList>
                            </li>
                            <li class="ppr pprUC brl crp dcrp carp">
                                 <asp:Label ID="lblRegion" runat="server"  Text='<%$ Resources:Resource,L_REGION%>' CssClass="FormLabel"></asp:Label>
                                <asp:DropDownList ID="ddlRegion" Enabled="false" runat="server"
                                    AutoPostBack="True">
                                </asp:DropDownList>
                            </li>

                             <li class="ppr pprUC brl br crp dcrp carp" >
                                <asp:Label ID="Label2" runat="server" Text='<%$ Resources:Resource,L_PRODUCT%>' CssClass="FormLabel"></asp:Label>
                                <asp:DropDownList ID="ddlAllProducts" Enabled="false" runat="server"></asp:DropDownList>
                            </li>
                            

                             <li class="poi pc pco mf rnw co fio pi poiC iwp  ">
                                 <asp:Label ID="lblRegionWoNational" runat="server"  Text='<%$ Resources:Resource,L_REGION%>' CssClass="FormLabel"></asp:Label>
                                <asp:DropDownList ID="ddlRegionWoNational" runat="server"
                                    AutoPostBack="True">
                                </asp:DropDownList>
                            </li>

                            <li class="pd doi  perc ca">
                                <asp:Label ID="lblDistrict" runat="server"  Text='<%$ Resources:Resource,L_DISTRICT%>' CssClass="FormLabel"></asp:Label>
                                <asp:DropDownList ID="ddlDistrict" runat="server"
                                    AutoPostBack="True">
                                </asp:DropDownList>
                                
                            </li>

                             <li class="poi pc pco mf rnw co fio pi poiC iwp ">
                                <asp:Label ID="Label1" runat="server"  Text='<%$ Resources:Resource,L_DISTRICT%>' CssClass="FormLabel"></asp:Label>
                                <asp:DropDownList ID="ddlDistrictWoNational" runat="server"
                                    AutoPostBack="True">
                                </asp:DropDownList>
                                
                            </li>

                            <%--<li class="perc" >
                                <asp:Label ID="Label1" runat="server" Text='<%$ Resources:Resource,L_DISTRICT%>' CssClass="FormLabel"></asp:Label>
                                <asp:DropDownList ID="ddlDistrict1" runat="server"></asp:DropDownList>
                            </li>--%>
                            <li class="ua">
                                <asp:Label ID="lblUserName" runat="server"  Text='<%$ Resources:Resource, T_USERNAME %>' cssClass="FormLabel"></asp:Label>
                                <asp:DropDownList ID="ddlUserName" runat="server"></asp:DropDownList>
                            </li>

                            <li class="pc   poi pco mf rnw sr  ">
                                <asp:Label ID="lblProducts" runat="server"  Text='<%$ Resources:Resource,L_PRODUCT%>' CssClass="FormLabel"></asp:Label>
                                <asp:DropDownList ID="ddlProduct" runat="server"></asp:DropDownList>
                                
                            </li>
                           
                            <li class="pd doi ca">
                                 <asp:Label ID="lblProductStrict" runat="server"  Text='<%$ Resources:Resource,L_PRODUCT%>' CssClass="FormLabel"></asp:Label>
                                <asp:DropDownList ID="ddlProductStrict" runat="server" AutoPostBack="true">
                                </asp:DropDownList>
                            </li>
                            <li class="fio" > 
                                <asp:Label ID="lblWard" runat="server" Text="<%$ Resources:Resource,L_WARD%>" CssClass="FormLabel"></asp:Label>
                                <asp:DropDownList ID="ddlWards" AutoPostBack="true" runat="server"></asp:DropDownList>
                            </li>
                            <li  class="ua" >
                                <asp:Label ID="lblAction" runat="server" Text="<%$ Resources:Resource,L_ACTION%>" CssClass="FormLabel"></asp:Label>
                                <asp:DropDownList ID="ddlAction" runat="server"></asp:DropDownList>
                            </li>

                            <li class="doi co poiC">
                                <asp:Label ID="lblHFCode" runat="server"  Text='<%$ Resources:Resource,L_HFCODE%>' CssClass="FormLabel"></asp:Label>
                                <asp:DropDownList ID="ddlHF" runat="server"></asp:DropDownList>


                            </li>
                            <li class="pc ps" >
                                <asp:Label ID="lblPaymentType" runat="server" Text="<%$ Resources:Resource,L_PAYMENTTYPE%>" CssClass="FormLabel"></asp:Label>
                                <asp:DropDownList ID="ddlPaymentType" runat="server" ></asp:DropDownList>
                            </li>
                            <li class="fio">
                                <asp:Label ID="lblVillage" runat="server"  Text="<%$ Resources:Resource,L_VILLAGE%>"  CssClass="FormLabel"></asp:Label>
                                <asp:DropDownList ID="ddlVillages" runat="server" AutoPostBack="true"></asp:DropDownList>
                            </li>
                            <li class="ua">
                                <asp:Label ID="lblEntity" runat="server"  Text="Entity"  CssClass="FormLabel"></asp:Label>
                                <asp:DropDownList ID="ddlEntity" runat="server"></asp:DropDownList>
                            </li>


                            <li class="mf">
                                <asp:Label ID="L_Payer" runat="server" Text='<%$ Resources:Resource,L_PAYER %>' CssClass="FormLabel"></asp:Label>
                                <asp:DropDownList ID="ddlPayer" runat="server"></asp:DropDownList>
                            </li>

                            <li class="iwp pi rnw" >
                                <asp:Label ID="lblEnrolmentOfficer" runat="server" Text='<%$ Resources:Resource,L_ENROLMENTOFFICERS%>' CssClass="FormLabel"></asp:Label>
                                <asp:DropDownList ID="ddlEnrolmentOfficer"  Width="150px" runat="server"></asp:DropDownList>
                            </li>


                            <li class="co" >
                                <asp:Label ID="lblClaimStatus" runat="server" Text='<%$ Resources:Resource,L_CLAIMSTATUS %>' CssClass="FormLabel"></asp:Label>
                                <asp:DropDownList ID="ddlClaimStatus" runat="server" ></asp:DropDownList>

                            </li>
                            <li  class="fio">
                                <asp:Label ID="lblPolicyStatus" runat="server" Text='<%$ Resources:Resource,L_STATUS %>' CssClass="FormLabel"></asp:Label>
                                <asp:DropDownList ID="ddlPolicyStatus" runat="server" ></asp:DropDownList>
                            </li>
                            <li class="rnw">
                                <asp:Label ID="lblSorting" runat="server"  Text='<%$ Resources:Resource,L_SORTING%>' CssClass="FormLabel"></asp:Label>
                                <asp:DropDownList ID="ddlSorting" runat="server"></asp:DropDownList>
                            </li>

                            <li class=" " style="display:none" >
                                <asp:Label ID="lblCatchmentArea" runat="server"></asp:Label>
                            </li> 
                                                  </ul>
                    </td>
                </tr>
            </table>
            
        </asp:Panel>
     </ContentTemplate>
   </asp:UpdatePanel>
        <br />
        <br />
        <table>
            <tr>
                <td>
                      <asp:UpdatePanel ID="upPrevReport" runat="server">
                           <ContentTemplate>
                               <div class="mf" style="position:relative;display:none;">
                                  <div style="position:absolute;right:109px;top:0px;">
                                     <asp:Label ID="lblPreviousReport" class="FormLabel mf" runat="server" Text='<%$ Resources:Resource,L_PREVIOUS %>'></asp:Label>
                                     <asp:DropDownList ID="ddlPreviousReportDate" class="mf" runat="server"></asp:DropDownList>
                                  </div>
                                </div>
                           </ContentTemplate>
                      </asp:UpdatePanel>
                      <asp:ListBox ID="lstboxReportSelector" runat="server" Height="244px" Width="364px" CssClass="panel"> 
                      </asp:ListBox>
                </td>
                <td></td>
            </tr>
        </table>
           
        </div>
       <asp:Panel ID="pnlButtons" runat="server"   CssClass="panelbuttons" >
        <table width="100%" cellpadding="10 10 10 10" align="center">
             <tr align="center">
                  <td align="left">
                          <asp:Button 
                            
                            ID="btnPreview" 
                            runat="server" 
                            Text='<%$ Resources:Resource,B_PREVIEW %>' ValidationGroup="check" />
                              
                    </td>     
                    <td align="right">
                          <asp:Button 
                            
                            ID="B_CANCEL" 
                            runat="server" 
                            Text='<%$ Resources:Resource,B_CANCEL%>' />
                              
                    </td>                 
                </tr>
            </table>             
        </asp:Panel> 
   
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Footer" Runat="Server">
    <asp:Label text="" runat="server" ID="lblMsg">
    </asp:Label>
</asp:Content>

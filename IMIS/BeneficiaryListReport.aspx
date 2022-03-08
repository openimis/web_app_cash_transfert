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
<%@ Page Language="vb" MasterPageFile="~/IMIS.Master" AutoEventWireup="false" CodeBehind="BeneficiaryListReport.aspx.vb" Inherits="IMIS.BeneficiaryListReport" Title='BeneficiaryListReport' %>

 <%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Body" runat="Server">

    

    <script type="text/javascript">
        $(document).ready(function () {
            $('#<%= btnSearch.ClientID %>').click(function (e) {
                var passed = true;
                $DateControls = $('.dateCheck');
                
                $DateControls.each(function () {
                   
                    if ($(this).val().length > 0 && !isValidJSDate($(this).val())) {
                        $('#<%=lblMsg.ClientID%>').html('<%= imisgen.getMessage("M_INVALIDDATE", True) %>');
                         $(this).focus();
                         passed = false;
                         return false;
                     }
                 });
                 if (passed == false) {
                     return false;
                 }
            });
        });
    </script>
    <div class="divBody">
    <asp:TextBox
                                                ID="txtHidden"
                                                runat="server"
                                                Width="120px"
                                                CssClass="dateCheck" Visible="false" >
                                            </asp:TextBox>
        <table class="catlabel">
            <tr>
                <td>
                    <asp:Label ID="Label8" runat="server" Text='<%$ Resources:Resource,L_SELECTCRITERIA %>'></asp:Label>
                </td>

            </tr>

        </table>
        <div>
         


        </div>
        <asp:UpdatePanel ID="upDistrict" runat="server">
            <Triggers>
                <asp:PostBackTrigger ControlID="btnSearch" />
            </Triggers>
            <ContentTemplate>
                <!-- Hidden Field -->
<%--<asp:HiddenField ID="hidForModel" runat="server" />
                <asp:Panel ID="pnlTop" runat="server" CssClass="panelTop" Style="height: auto;" GroupingText='<%$ Resources:Resource,L_PAYMENT %>'>
                     <asp:ModalPopupExtender ID="ModalPopupExtender1" BehaviorID="mpe" runat="server"
    PopupControlID="pnlPopup" TargetControlID="hidForModel" CancelControlID = "btnHide">
</asp:ModalPopupExtender>
            <asp:Panel ID="pnlPopup" runat="server" Width="100px" Height="100px" CssClass="modalPopup" >
    <div class="panel-header">
        Payment Already Generated ! Clcik Yes to Regenerate or Cancel. 
    </div>
    <div class="body">
        <asp:Button ID="btnYes" OnClick="btnYes_Click" Text="Yes" runat="server" CssClass="btn btn-primary" />
         <asp:Button ID="btnHide" runat="server" Text="Hide Modal Popup" />
    </div>
</asp:Panel>--%>
                    <table>
                        <tr>
                            <td>
                                <table>
                                    <tr>
                                       
                                         
                                                 </td
                                       
                                    </tr>
                                    <tr>
                                         <td class="FormLabel">
                                            <asp:Label ID="lblRegion" runat="server" Text="<%$ Resources:Resource,L_REGION %>"></asp:Label>
                                        </td>

                                        <td class="DataEntry">
                                            <asp:DropDownList ID="ddlRegion" runat="server" AutoPostBack="false">
                                                <%--<asp:ListItem Value="00">National</asp:ListItem>--%>
                                            </asp:DropDownList>

                                        </td>     
                                     

                                          

                                    </tr>
                                    <tr>
                                        
                                               <td class="FormLabel">
                                            <asp:Label 
                                                ID="Label9"
                                                runat="server" 
                                                Text='Program'>
                                            </asp:Label>
                                        </td>
                                        <td class="DataEntry">
                                                            <asp:DropDownList ID="ddlProduct" runat="server" AutoPostBack="false">
                                                            <asp:ListItem Value="0">Select Product</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </td>  

                                        
                                        <td class="auto-style1">
                                            <asp:Label ID="Label2" runat="server" Text='Start Date'></asp:Label>
                                        </td>

                                        <td class="DataEntry">
                                            <asp:TextBox
                                                ID="txtDateFrom"
                                                runat="server"
                                                Width="120px"
                                                CssClass="dateCheck">
                                            </asp:TextBox>
                                            <asp:Button
                                                ID="btnDateFrom" 
                                                runat="server"
                                                Class="dateButton"
                                                padding-bottom="3px" />

                                            <asp:CalendarExtender
                                                ID="CalendarExtender2"
                                                runat="server"
                                                TargetControlID="txtDateFrom"
                                                Format="dd/MM/yyyy"
                                                PopupButtonID="btnDateFrom" ClearTime="True">
                                            </asp:CalendarExtender>

                                        </td>
                                        
                                        <td class="auto-style1">
                                            <asp:Label ID="Label1" runat="server" Text='End Date'></asp:Label>
                                        </td>

                                        <td class="DataEntry">
                                            <asp:TextBox
                                                ID="txtDateTo"
                                                runat="server"
                                                Width="120px"
                                                CssClass="dateCheck">
                                            </asp:TextBox>
                                            <asp:Button
                                                ID="btnDateTo" 
                                                runat="server"
                                                Class="dateButton"
                                                padding-bottom="3px" />

                                            <asp:CalendarExtender
                                                ID="CalendarExtender1"
                                                runat="server"
                                                TargetControlID="txtDateTo"
                                                Format="dd/MM/yyyy"
                                                PopupButtonID="btnDateTo" ClearTime="True">
                                            </asp:CalendarExtender>

                                        </td>
                                        <%--tRANSACTION --%>
                                        
                                 <td>       
                                <asp:Button class="button" ID="btnSearch" runat="server"
                                    Text='Generate'></asp:Button>
                                <br />

                            </td>
                        </tr>
                    </table>

                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
        <table class="catlabel">
            <tr>
                <td>
                    <asp:Label
                        ID="L_FOUNDPAYMENTS"
                        runat="server"
                        Text='<%$ Resources:Resource,L_FOUNDPAYMENTS %>'>
                    </asp:Label>
                </td>

            </tr>

        </table>
       
        <asp:Button ID="exceltxt" Visible="false" runat="server" OnClick="exceltxt_Click" Text="Export To Excel" />
        <asp:Panel ID="pnlBody" runat="server" CssClass="panelBody">
       <asp:GridView  ID="gvPayments" runat="server"   
      AutoGenerateColumns="False"
                GridLines="None"
                AllowPaging="true" PagerSettings-FirstPageText="First Page" PagerSettings-LastPageText="Last Page" PagerSettings-Mode="NumericFirstLast" PageSize="9"
                CssClass="mGrid"
                PagerStyle-CssClass="pgr" DataKeyNames="PaymentDetailsID"
                        
                        EmptyDataText='<%$ Resources:Resource,L_NORECORDS %>'>
                       
                        <Columns>
                                  <asp:BoundField DataField="InsuranceNumber"  HeaderText='Social Registry #' HeaderStyle-Width ="70px">  </asp:BoundField> 
                       
                    <asp:BoundField DataField="OtherNames"    HeaderText='First Name'  HeaderStyle-Width ="80px" ></asp:BoundField>     
                          <asp:BoundField DataField="LastName"    HeaderText='Last Name'  HeaderStyle-Width ="80px" ></asp:BoundField>
                            <asp:BoundField DataField="phone"  HeaderText='Phone' HeaderStyle-Width ="70px">  </asp:BoundField>
                         <asp:BoundField DataField="Gender"  HeaderText='Gender' HeaderStyle-Width ="70px">  </asp:BoundField>
                           <asp:BoundField DataField="ProductCode"  HeaderText='Program' HeaderStyle-Width ="70px">  </asp:BoundField>
                            <asp:BoundField DataField="Amount"  HeaderText='Amount' HeaderStyle-Width ="70px">  </asp:BoundField>
                             <asp:BoundField DataField="regionname"  HeaderText='Region' HeaderStyle-Width ="70px">  </asp:BoundField>
                            <asp:BoundField DataField="districtname"  HeaderText='District' HeaderStyle-Width ="70px">  </asp:BoundField>
                            <asp:BoundField DataField="wardname"  HeaderText='Ward' HeaderStyle-Width ="70px">  </asp:BoundField>
                           <asp:BoundField DataField="villagename"  HeaderText='Settlement' HeaderStyle-Width ="70px">  </asp:BoundField>
                             <asp:BoundField DataField="ValidityFrom"  HeaderText='Start Date' HeaderStyle-Width ="70px"> </asp:BoundField>    
                            <asp:BoundField DataField="ValidityTo"  HeaderText='End Date' HeaderStyle-Width ="70px">  </asp:BoundField> 
                       
                            
                            
        
                 </Columns>
            <PagerSettings FirstPageText="First Page" LastPageText="Last Page" Mode="NumericFirstLast" />

                <PagerStyle CssClass="pgr" />
                <AlternatingRowStyle CssClass="alt" />
                <SelectedRowStyle CssClass="srs" />
                <RowStyle CssClass="normal" />
                   </asp:GridView>
           
                    
        </asp:Panel>
    </div>
    <asp:Panel ID="pnlButtons" runat="server" CssClass="panelbuttons">
        <table width="100%" cellpadding="10 10 10 10" align="right">
            <tr align="right">
                <td align="right" colspan="11" width="600">
                    <asp:Button
                        ID="B_SUBMIT" Visible="false"
                        runat="server" Style="font-weight: bold; color: Red"
                        Text='<%$ Resources:Resource,B_SUBMIT%>' />
                </td>
                 <td align="center">

                    <asp:Button
                        Visible="false"
                        ID="B_VIEW"
                        runat="server"
                        Text='<%$ Resources:Resource,B_VIEW%>' />
                </td>
                <td align="right" colspan="1">
                    <asp:Button Visible="false"
                        ID="B_CANCEL"
                        runat="server"
                        Text='<%$ Resources:Resource,B_CANCEL%>' />

                </td>
            </tr>
        </table>
    </asp:Panel>






</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Footer" Runat="Server" Visible="true" >
    
    <asp:Label text="" runat="server" ID="lblMsg"> </asp:Label>
</asp:Content>
<asp:Content ID="Content4" runat="server" contentplaceholderid="head">
    <style type="text/css">
        .auto-style1 {
            height: 27px;
            width: 150px;
            text-align: right;
            color: Blue;
            font-weight: normal;
            font-family: Verdana, Arial, Helvetica, sans-serif;
            font-size: 11px;
            padding-right: 5px;
        }
    </style>
</asp:Content>

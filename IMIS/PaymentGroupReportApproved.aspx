<%@ Page Title="Payments" Language="vb" AutoEventWireup="false" MasterPageFile="~/IMIS.Master" CodeBehind="PaymentGroupReportApproved.aspx.vb" Inherits="IMIS.PaymentGroupReportApproved " %>

<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="ajax" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"> 
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Body" runat="server">
    
    <asp:UpdatePanel ID="upClaim" runat="server" RenderMode="Inline" > 
<Triggers>
<asp:PostBackTrigger ControlID="B_SUBMIT" />
</Triggers>
<ContentTemplate>
    <div class="divBody">
        <asp:HiddenField ID="hfICDID" runat="server" />
        <asp:HiddenField ID="hfICDCode" runat="server" />
        <asp:HiddenField ID="hfClaimAdminAdjustibility" runat="server" Value="" />
        <table class="catlabel" runat="server" visible="false"  >
            <tr>
                <td>
                    <asp:Label
                        ID="L_SELECTCRITERIA"
                        runat="server"
                        Text='<%$ Resources:Resource,L_SELECTCRITERIA %>'>
                    </asp:Label>

                </td>
            </tr>
        </table>



        <asp:Panel ID="pnlTop" Visible="false"   runat="server" CssClass="panelTop" Height="165px" GroupingText='<%$ Resources:Resource,L_PaymentGroupReport %>' oncontextmenu="return false;">


            <table>
                <tr>
                    <td class="FormLabel">
                        <asp:Label ID="L_REGION" runat="server" Text="<%$ Resources:Resource,L_REGION %>"></asp:Label>
                    </td>
                    <td class="DataEntry">
                        <asp:DropDownList ID="ddlRegion" runat="server" AutoPostBack="true">
                        </asp:DropDownList>
                    </td>
                    
                    <td class="FormLabel">
                        <asp:Label ID="L_PaymentDate" runat="server" Text='<%$ Resources:Resource,L_PAYDATEFROM %>'></asp:Label>
                    </td>
                    <td class="DataEntry">
                        <asp:TextBox ID="txtDateOfPaymentFrom" runat="server" Text="" Width="120px" CssClass="dateCheck"></asp:TextBox>
                        <asp:Button ID="btnDateOfPaymentFrom" runat="server" padding-bottom="3px"
                            Class="dateButton" />
                        <ajax:CalendarExtender ID="CalendarExtender1" runat="server"
                            Format="dd/MM/yyyy" PopupButtonID="btnDateOfPaymentFrom" TargetControlID="txtDateOfPaymentFrom"></ajax:CalendarExtender>
                    </td>
                </tr>
                <tr>
                
                   
                    <td class="FormLabel">
                        <asp:Label ID="L_PAYDATETO" runat="server" Text='<%$ Resources:Resource,L_PAYDATETO %>'></asp:Label>
                    </td>
                    <td class="DataEntry">
                        <asp:TextBox ID="txtDateOfPaymentTo" runat="server" Text="" Width="120px" CssClass="dateCheck"></asp:TextBox>
                        <asp:Button ID="btnDateOfPaymentTo" runat="server" padding-bottom="3px"
                            Class="dateButton" />
                        <ajax:CalendarExtender ID="CalendarExtender2" runat="server"
                            Format="dd/MM/yyyy" PopupButtonID="btnDateOfPaymentTo" TargetControlID="txtDateOfPaymentTo"></ajax:CalendarExtender>
                    </td>
                </tr>
                <tr>
                    <td class="FormLabel">
                        <asp:Label ID="lblProduct" runat="server" class="FormLabel" Text="<%$ Resources:Resource,L_POLICIES%>"></asp:Label>
                    </td>
                    <td class="DataEntry">
                        <asp:DropDownList ID="ddlproduct" runat="server">
                        </asp:DropDownList>
                    </td>
                    


                    <td class="FormLabel">
                        <asp:Button class="button" ID="btnSearch" runat="server"
                            Text='<%$ Resources:Resource,B_SEARCH %>'></asp:Button>
                    </td>
                </tr>
            </table>
        </asp:Panel>

        <table>
            <tr>
                <td>
                    <table class="catlabel">
                        <tr>
                            <td>
                                <asp:Label
                                    ID="L_FOUNDPAYMENTS"
                                    runat="server"
                                    Text='<%$ Resources:Resource,L_APPROVEDPAYROLL %>'></asp:Label>
                            </td>

                        </tr>

                    </table>
                </td>
             <%--   <td align="right">
                    <asp:Label ID="lblSelectToSubmit" runat="server" Text='<%$ Resources:Resource,L_MARKASPAID %>' Style="margin-left: 579px" CssClass="FormLabel"></asp:Label>
                    <asp:CheckBox ID="chkStatusPaid" runat="server"/>
                </td>--%>

            </tr>
        </table>
         
        <asp:Panel ID="pnlBody" runat="server" CssClass="panelBody">
            <asp:GridView ID="gvPayments" runat="server"
                AutoGenerateColumns="False"
                GridLines="None"
                AllowPaging="true" PagerSettings-FirstPageText="First Page" PagerSettings-LastPageText="Last Page" PagerSettings-Mode="NumericFirstLast" PageSize="9"
                CssClass="mGrid"
                PagerStyle-CssClass="pgr"
                
                EmptyDataText='<%$ Resources:Resource,L_NORECORDS %>'> 
                <Columns>
                    <asp:CommandField SelectText="Select" ShowSelectButton="true"
                        ItemStyle-CssClass="HideButton" HeaderStyle-CssClass="HideButton">
                        <HeaderStyle CssClass="HideButton" />
                        <ItemStyle CssClass="HideButton" />
                    </asp:CommandField>
                     <asp:HyperLinkField DataNavigateUrlFields = "ProductCode,startdate,enddate,regionid,payrollcycleid" DataTextField="ProductCode" DataNavigateUrlFormatString = "PaymentsApproval.aspx?p={0}&s={1}&e={2}&r={3}&pr={4}" HeaderText='<%$ Resources:Resource,L_POLICIES %>' HeaderStyle-Width ="100px" >
                        <HeaderStyle Width="100px" /> 
                    </asp:HyperLinkField>    
                     <asp:HyperLinkField DataNavigateUrlFields = "payrollcycleid" DataTextField="payrollCycleName" DataNavigateUrlFormatString = "OverviewPayrollCycle.aspx?PayrollCycleID={0}" HeaderText='<%$ Resources:Resource,P_PAYROLLCYCLENAME %>' HeaderStyle-Width ="100px" >
                        <HeaderStyle Width="100px" /> 
                    </asp:HyperLinkField>    
                     <asp:BoundField DataField="LocationName"  HeaderText='Region' HeaderStyle-Width ="70px">  </asp:BoundField> 
                             <asp:BoundField DataField="startdate"  HeaderText='Start Date' HeaderStyle-Width ="70px">  </asp:BoundField> 
                            <asp:BoundField DataField="enddate"  HeaderText='End Date' HeaderStyle-Width ="70px">  </asp:BoundField> 
                </Columns> 
                <PagerSettings FirstPageText="First Page" LastPageText="Last Page" Mode="NumericFirstLast" />

                <PagerStyle CssClass="pgr" />
                <AlternatingRowStyle CssClass="alt" />
                <SelectedRowStyle CssClass="srs" />
                <RowStyle CssClass="normal" />
            </asp:GridView>
        </asp:Panel>
    </div>

    <asp:Panel ID="pnlButtons" runat="server" CssClass="panelbuttons" Visible="false" >
        <table width="100%" cellpadding="10 10 10 10" align="right">
            <tr align="right">
                <td align="right" colspan="11" width="600">
                    <asp:Button
                        ID="B_SUBMIT"
                        runat="server" Style="font-weight: bold; color: Red"
                        Text='<%$ Resources:Resource,B_SUBMIT%>' />
                </td>
                <td align="right" colspan="1">
                    <asp:Button
                        ID="B_CANCEL"
                        runat="server"
                        Text='<%$ Resources:Resource,B_CANCEL%>' />

                </td>
            </tr>
        </table>
    </asp:Panel>
</ContentTemplate></asp:UpdatePanel>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="footer" runat="server">
</asp:Content>
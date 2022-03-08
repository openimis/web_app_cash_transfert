<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/IMIS.Master" CodeBehind="ImportPaymentDetails.aspx.vb" Inherits="IMIS.ImportPaymentDetails" %>
<asp:Content ID="contenthead" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="contentBody" ContentPlaceHolderID="Body" runat="server">
    <asp:Panel ID="pnlUploads" runat="server" CssClass="panel">
        <asp:Panel ID="PlUploadPayments" runat="server">
            <table class="catlabel">
                <tr>
                    <td>
                        <asp:Label ID="Label12" runat="server"
                            Text='<%$ Resources:Resource,L_UPLOADPAYMENTS %>'></asp:Label>

                    </td>


                </tr>
            </table>
            <asp:Panel ID="Panel7" runat="server" CssClass="panel"  Height="120px" GroupingText="">
                <table>
                    <tr>
                        <td>
                            <table>
                                <tr>
                                    <td class="FormLabel ExtractTd">
                                        <asp:FileUpload ID="FileUploadPayments" runat="server" />
                                    </td>
                                    <td class="DataEntry"></td><td class="DataEntry"></td>
                                     <td class="FormLabel ExtractTd"><asp:Label ID="Label18" Width="100%" runat="server" Text='<%$ Resources:Resource,P_PAYROLLCYCLENAME %>' ></asp:Label></td>
                                    <td class="DataEntry"> <asp:DropDownList ID="ddlPayrollCycle" runat="server"  AutoPostBack="True"></asp:DropDownList></td>
                                    <td class="DataEntry"></td>
                                    <td class="DataEntry"></td>
                                    <td class="FormLabel ExtractTd"><asp:Label ID="Label11" runat="server" Text='<%$ Resources:Resource,L_POLICIES1 %>'></asp:Label></td>
                                    <td class="DataEntry"> <asp:DropDownList ID="ddlProgram" runat="server"  ></asp:DropDownList></td>
                                    <td class="DataEntry"></td>
                                    <td class="DataEntry">                                        
                                        &nbsp;<asp:CheckBox ID="chkisExceptional" runat="server" Text="<%$ Resources:Resource, L_ISEXCEPTIONAL %>" CssClass="checkbox" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="FormLabel ExtractTd">
                                        <asp:Label ID="lblMonth" runat="server"  Text='<%$ Resources:Resource, L_STARTMONTH %>'></asp:Label>
                                    </td>
                                    <td class="DataEntry">
                                         <asp:DropDownList ID="ddlMonth"  runat="server"></asp:DropDownList>
                                     </td>
                                    <td class="FormLabel ExtractTd">
                                        <asp:Label ID="lblYear" runat="server" Text='<%$ Resources:Resource, L_STARTYEAR %>'></asp:Label>
                                    </td>
                                    <td class="DataEntry">
                                        <asp:DropDownList ID="ddlYear"  runat="server"></asp:DropDownList>
                                    </td>
                                    <td class="DataEntry"></td>
                                    <td class="FormLabel ExtractTd">
                                        <asp:Label ID="Label15" runat="server"  Text='<%$ Resources:Resource, L_ENDMONTH %>'></asp:Label>
                                    </td>
                                    <td class="DataEntry">
                                        <asp:DropDownList ID="ddlMonth1"  runat="server"></asp:DropDownList>
                                    </td>
                         
                                    <td class="FormLabel ExtractTd">
                                        <asp:Label ID="Label17" runat="server" Text='<%$ Resources:Resource, L_ENDYEAR %>'></asp:Label>
                                    </td>
                                    <td class="DataEntry">
                                        <asp:DropDownList ID="ddlYear1" runat="server"></asp:DropDownList>
                                    </td>
                                    <td class="DataEntry"></td>
                                    <td align="right" class="auto-style2">
                                        <asp:Button ID="btnUploadPayments" runat="server"
                                            Text='<%$ Resources:Resource,B_UPLOAD%>' ValidationGroup="UploadPayments"    />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="ExtractTd">
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server"
                                            ControlToValidate="FileUploadPayments" ErrorMessage="M_PLEASESELECTTHEFILE"
                                            SetFocusOnError="True" Text='<%$ Resources:Resource, M_PLEASESELECTTHEFILE %>' ValidationGroup="UploadPayments" Display="Dynamic" ForeColor="Red"></asp:RequiredFieldValidator>
                                    </td>
                                    <td class="ExtractTd">
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server"
                                            ControlToValidate="ddlPayrollCycle" ErrorMessage="Please Select A Payroll Cycle"
                                            SetFocusOnError="True" Text='Please Select A Payroll Cycle' ValidationGroup="UploadPayments" Display="Dynamic" ForeColor="Red"></asp:RequiredFieldValidator>
                                    </td>
                                    <td class="DataEntry">&nbsp;</td>
                                    <td class="DataEntry">&nbsp;</td>
                                    <td class="DataEntry">&nbsp;</td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>

        </asp:Panel>
    </asp:Panel>

    <asp:Panel ID="Panel1" runat="server" CssClass="panelbuttons">
        <table width="100%" cellpadding="10 10 10 10">
            <tr>

                <td align="left">
                        
                </td>
                <td align="center">
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
<asp:Content ID="contentFooter" ContentPlaceHolderID="footer" runat="server">
</asp:Content>

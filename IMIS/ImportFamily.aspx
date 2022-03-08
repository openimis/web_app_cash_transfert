<%@ Page Title="Import" Language="vb" AutoEventWireup="false" MasterPageFile="~/IMIS.Master" CodeBehind="ImportFamily.aspx.vb" Inherits="IMIS.ImportFamily" %>
<asp:Content ID="head" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="body" ContentPlaceHolderID="Body" runat="server">
    <asp:UpdatePanel ID="upClaim" runat="server" RenderMode="Inline" > 
<Triggers>
<asp:PostBackTrigger ControlID="B_SUBMIT" />
<asp:PostBackTrigger ControlID="B_UPLOAD" />
</Triggers>
<ContentTemplate>
    <div class="divBody">
        <table class="catlabel">
            <tr>
                <td>
                    <asp:Label
                        ID="L_IMPORTFAMILY"
                        runat="server"
                        Text='<%$ Resources:Resource,L_IMPORTFAMILY %>'>
                    </asp:Label>

                </td>
            </tr>
        </table>

        <asp:Panel ID="pnlTop" runat="server" CssClass="panelTop" Height="95px" GroupingText='SELECT FILE TO IMPORT' oncontextmenu="return false;">
            <table>
                <tr><td>
                    <asp:Label ID="lblMessage" runat="server" Visible="False"></asp:Label></td></tr>
                <tr>
                    <td><asp:FileUpload ID="B_IMPORTHOUSEHOLDFILE" runat="server" Width="880px" /></td>
                    <td><asp:Button ID="B_UPLOAD" runat="server" Text="Upload" /></td>
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
                                    ID="L_FAMILYTOIMPORT"
                                    runat="server"
                                    Text='<%$ Resources:Resource,L_FAMILYTOIMPORT %>'></asp:Label>
                            </td>

                        </tr>

                    </table>
                </td>
            </tr>
        </table>
        <asp:Panel ID="pnlBody" runat="server" CssClass="panelBody">
            <asp:GridView ID="gvFamilyToImport" runat="server"
                AutoGenerateColumns="False"
                GridLines="None"
                AllowPaging="true" PagerSettings-FirstPageText="First Page" PagerSettings-LastPageText="Last Page" PagerSettings-Mode="NumericFirstLast" PageSize="9"
                CssClass="mGrid"
                PagerStyle-CssClass="pgr"
                DataKeyNames="InsuranceNumber"
                EmptyDataText='<%$ Resources:Resource,L_NORECORDS %>'>

                <Columns>
                    <asp:BoundField DataField="InsuranceNumber" DataFormatString="{0:d}" HeaderText='Insurance Number' 
                        SortExpression="InsuranceNumber" HeaderStyle-Width="80px">
                    <HeaderStyle Width="80px" />
                    </asp:BoundField>
                    <asp:BoundField DataField="PermanentVillageCode" DataFormatString="{0:d}" HeaderText="Permanent Village Code"
                        SortExpression="PermanentVillageCode" HeaderStyle-Width="80px">
                    <HeaderStyle Width="80px" />
                    </asp:BoundField>
                    <asp:BoundField DataField="OtherNames" DataFormatString="{0:d}" HeaderText='Other Names'
                        SortExpression="OtherNames" HeaderStyle-Width="80px">
                    <HeaderStyle Width="80px" />
                    </asp:BoundField>
                    <asp:BoundField DataField="LastName" DataFormatString="{0:d}" HeaderText="Last Name"
                        SortExpression="LastName" HeaderStyle-Width="80px">
                    <HeaderStyle Width="80px" />
                    </asp:BoundField>
                    <asp:BoundField DataField="BirthDate" DataFormatString="{0:d}" HeaderText='Birth Date'
                        SortExpression="BirthDate" HeaderStyle-Width="80px">
                    <HeaderStyle Width="80px" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Gender" DataFormatString="{0:d}" HeaderText='Gender' 
                        SortExpression="Gender" HeaderStyle-Width="80px">
                    <HeaderStyle Width="80px" />
                    </asp:BoundField>
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
<asp:Content ID="footer" ContentPlaceHolderID="footer" runat="server">
</asp:Content>

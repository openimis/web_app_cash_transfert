<%@ Page Title="Payments" Language="vb" AutoEventWireup="false" MasterPageFile="~/IMIS.Master" CodeBehind="PaymentsApproval.aspx.vb" Inherits="IMIS.PaymentsApproval" %>

<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="ajax" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">

        function OnClickSelectAll(sender) {
            $('#<%=gvPayments.ClientID  %> tr').each(function (i) {
                $row = $(this);
                $cell1 = $row.find("td").eq(1);
                $checkbx = $cell1.find("input[type=checkbox]").eq(0);
                console.log($checkbx);
                $checkbx.checked = sender.checked
                $checkbx.attr("checked", sender.checked);
                
            });
        }
    </script> 
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
        <table class="catlabel" runat="server" visible="false">
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



        <asp:Panel ID="pnlTop" runat="server" Visible="false"  CssClass="panelTop" GroupingText="Payment Approval" oncontextmenu="return false;">

             
            <table>
                <tr>
                    <td class="FormLabel">
                        <asp:Label ID="L_REGION" runat="server" Text="<%$ Resources:Resource,L_REGION %>"></asp:Label>
                    </td>
                    <td class="DataEntry">
                        <asp:DropDownList ID="ddlRegion" runat="server" AutoPostBack="true">
                        </asp:DropDownList>
                    </td>
                 <td class="DataEntry">
                                <asp:Label ID="lblMonth" runat="server"  Text='<%$ Resources:Resource, L_STARTMONTH %>' CssClass="FormLabel"></asp:Label>
                                
                                             </td>
                                        <td class="DataEntry">
                                        <asp:DropDownList ID="ddlMonth" Width="90px" runat="server">
                                </asp:DropDownList>
                              
                                             </td>
                                        <td class="DataEntry">
                                <asp:Label ID="lblYear" runat="server" Text='<%$ Resources:Resource, L_STARTYEAR %>' CssClass="FormLabel"></asp:Label>
                                </td>
                                        <td class="DataEntry">
                                            <asp:DropDownList ID="ddlYear" Width="68px" runat="server">
                                </asp:DropDownList>
                       
                                        </td>
                    
                </tr>
                <tr>
                  
                    
                </tr>
                <tr>
                    <td class="FormLabel">
                        <asp:Label ID="lblProduct" runat="server" class="FormLabel" Text="Program"></asp:Label>
                    </td>
                    <td class="DataEntry">
                        <asp:DropDownList ID="ddlproduct" runat="server">
                        </asp:DropDownList>
                    </td>
                    
                    <td class="DataEntry">
                            
                                <asp:Label ID="Label1" runat="server"  Text='<%$ Resources:Resource, L_ENDMONTH %>' CssClass="FormLabel"></asp:Label>
                                </td>
                                        <td class="DataEntry">
                                            <asp:DropDownList ID="ddlMonth1" Width="90px" runat="server">
                                </asp:DropDownList>
                               </td>
                           <td class="DataEntry">
                                <asp:Label ID="Label2" runat="server" Text='<%$ Resources:Resource, L_ENDYEAR %>' CssClass="FormLabel"></asp:Label>
                               </td>
                                        <td class="DataEntry">
                               <asp:DropDownList ID="ddlYear1" Width="68px" runat="server">
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
                                    Text='<%$ Resources:Resource,L_FOUNDPAYMENTS %>'></asp:Label>
                            </td>
                            <td>
                                &nbsp &nbsp
                                </td> 
                            <td>
                   
                        </tr>

                    </table>
                </td>
                
                <td align="right">
                    <asp:Label ID="lblSelectToSubmit" runat="server" Text='<%$ Resources:Resource,L_MARKASAPPROVE %>' Style="margin-left: 579px" CssClass="FormLabel"></asp:Label>
                    <asp:CheckBox ID="chkStatusPaid" runat="server"/>
                </td>

            </tr>
        </table>
        <br />
        <div class="from-group row">
                                  <div class="col-md-6">
                                       <asp:Label   Font-Bold="true" Font-Size="X-Large" ForeColor="Green" runat="server" ID="txtApprovedCount" Text="" ></asp:Label>
                 </td> 
                                      </div> 
            </div>
        <table>
            <tr>
                <td align="right">
                    <asp:Label ID="Label3" runat="server" Text='<%$ Resources:Resource,L_DISPLAYONLYEXCEPTIONAL %>' Style="margin-left: 579px" CssClass="FormLabel"></asp:Label>
                    <asp:CheckBox ID="chkDisplayExceptional" runat="server" AutoPostBack="True" />
                </td>
            </tr>
        </table>
         
        <br />
        <asp:Panel ID="pnlBody" runat="server" CssClass="panelBody">
            <asp:GridView ID="gvPayments" runat="server"
                AutoGenerateColumns="False"
                GridLines="None"
                AllowPaging="true" PagerSettings-FirstPageText="First Page" PagerSettings-LastPageText="Last Page" PagerSettings-Mode="NumericFirstLast" PageSize="9"
                CssClass="mGrid"
                PagerStyle-CssClass="pgr"
                DataKeyNames="PaymentDetailsID"
                EmptyDataText='<%$ Resources:Resource,L_NORECORDS %>'>

                     <Columns>
                    <asp:CommandField SelectText="Select" ShowSelectButton="true"
                        ItemStyle-CssClass="HideButton" HeaderStyle-CssClass="HideButton">
                        <HeaderStyle CssClass="HideButton" />
                        <ItemStyle CssClass="HideButton" />
                    </asp:CommandField>
                    <asp:TemplateField HeaderStyle-Width="20px">
                        <HeaderTemplate>
                            <asp:CheckBox ID="chkPaymentSelectAll" runat="server" onclick="OnClickSelectAll(this)" />
                        </HeaderTemplate>
                        <ItemTemplate>
                        <asp:CheckBox ID="chkPaymentSetector" runat="server"  />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="ProductName" DataFormatString="{0:d}" HeaderText='<%$ Resources:Resource,L_POLICIES%>' 
                        SortExpression="ProductName" HeaderStyle-Width="80px">
                    <HeaderStyle Width="80px" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Insuree" DataFormatString="{0:d}" HeaderText="PaymentDetailsID"
                        SortExpression="PaymentDetailsID" HeaderStyle-Width="80px">
                    <HeaderStyle Width="80px" />
                    </asp:BoundField>
                             <asp:BoundField DataField="approvalStatusName" DataFormatString="{0:d}" HeaderText='<%$ Resources:Resource,L_APPROVALSTATUS %>'
                        SortExpression="approvalStatusName" HeaderStyle-Width="80px">
                    <HeaderStyle Width="80px" />
                    </asp:BoundField>
                    <asp:BoundField DataField="PaymentDate" DataFormatString="{0:d}" HeaderText='<%$ Resources:Resource,L_PAYDATE%>'
                        SortExpression="PaymentDate" HeaderStyle-Width="70px">
                    <HeaderStyle Width="70px" />
                    </asp:BoundField>
                   
                    <asp:BoundField DataField="Amount" HeaderText='<%$ Resources:Resource,L_AMOUNT%>'
                        SortExpression="ReceivedAmount" HeaderStyle-Width="70px">
                    <HeaderStyle Width="70px" />
                    </asp:BoundField>


                    <asp:BoundField DataField="Validity" DataFormatString="{0:d}" HeaderText='<%$ Resources:Resource,L_VALIDFROM %>' 
                        SortExpression="Validity" HeaderStyle-Width="70px">
                    <HeaderStyle Width="70px" />
                    </asp:BoundField>

                    <asp:BoundField DataField="RegionName" DataFormatString="{0:d}" HeaderText='Region' 
                        SortExpression="RegionName" HeaderStyle-Width="80px">
                    <HeaderStyle Width="80px" />
                    </asp:BoundField>
                         <asp:BoundField DataField="DistrictName" DataFormatString="{0:d}" HeaderText='District' 
                        SortExpression="DistrictName" HeaderStyle-Width="80px">
                    <HeaderStyle Width="80px" />
                    </asp:BoundField>
                         <asp:BoundField DataField="VillageName" DataFormatString="{0:d}" HeaderText='Settlement' 
                        SortExpression="VillageName" HeaderStyle-Width="80px">
                    <HeaderStyle Width="80px" />
                    </asp:BoundField>
                           <asp:BoundField DataField="TransactionNo" DataFormatString="{0:d}" HeaderText='Transaction No' 
                        SortExpression="TransactionNo" HeaderStyle-Width="80px">
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
                <td align="right" colspan="9" width="600">
                    <asp:Button  
                        ID="B_EXPORTDRAFT"
                        runat="server"
                        Text='<%$ Resources:Resource,B_EXPORTDRAFT%>' />
                </td>
                <td align="right" colspan="1">
                    <asp:Button
                        ID="B_EXPORTFINAL"
                        runat="server"
                        Text='<%$ Resources:Resource,B_EXPORTFINAL%>' />
                </td>
                <td align="right" colspan="1">
                    <asp:Button
                        ID="B_SUBMIT"
                        runat="server" Style="font-weight: bold; color: Red"
                        Text='<%$ Resources:Resource,B_APPROVE%>' />
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

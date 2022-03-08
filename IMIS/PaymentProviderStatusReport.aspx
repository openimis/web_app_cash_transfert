<%@ Page Title="Payments" Language="vb" AutoEventWireup="false" MasterPageFile="~/IMIS.Master" CodeBehind="PaymentProviderStatusReport.aspx.vb" Inherits="IMIS.PaymentProviderStatusReport" %>

<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="ajax" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
       
  <%--  <script type="text/javascript">

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
    </script>--%>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Body" runat="server">
    <%--<script type="text/javascript"  src="CSS/bootstrap.min.js"></script>
    <link   href="CSS/bootstrap.min.css" rel="stylesheet" />
    <script type="text/javascript" src="CSS/jquery-1.10.2.min.js"></script>
    <script type="text/javascript" src="CSS/jquery.dataTables.min.js"></script>
    <link href="Javascripts/jquery.dataTables.min.css" rel="stylesheet" />
    <script type="text/javascript" src="CSS/dataTables.buttons.min.js"></script>
    <script  type="text/javascript" src="CSS/buttons.print.min.js"></script>
    <script  type="text/javascript" src="CSS/buttons.colVis.min.js"></script>
    <script type="text/javascript" src="CSS/buttons.html5.min.js"></script>
        --%>
   <%-- <script type="text/javascript">
        $(function () {
            $('[id$=gvPayments]').prepend($("<thead></thead>").append($('[id$=gvPayments]').find("tr:first"))).DataTable({
                "responsive": true,
                "sPaginationType": "full_numbers",
            });
        });
    </script>--%>
    <asp:UpdatePanel ID="upClaim" runat="server" RenderMode="Inline" > 
<Triggers>
<asp:PostBackTrigger ControlID="B_SUBMIT" />
<%--<asp:PostBackTrigger ControlID="B_ADD" />--%>
</Triggers>
<ContentTemplate>
    <div class="divBody">
        <asp:HiddenField ID="hfICDID" runat="server" />
        <asp:HiddenField ID="hfICDCode" runat="server" />
        <asp:HiddenField ID="hfClaimAdminAdjustibility" runat="server" Value="" />
        <table class="catlabel">
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



        <asp:Panel ID="pnlTop" runat="server" CssClass="panelTop" Height="165px" GroupingText='<%$ Resources:Resource,G_HEALTHFACILITY %>' oncontextmenu="return false;">

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
                    
                       <td class="DataEntry">
                                <asp:Label ID="lblPaymentStatus" runat="server" Text=" Payment Status" CssClass="FormLabel"></asp:Label>
                               </td>
                    <td class="DataEntry">
                             <asp:DropDownList ID="paymentStatus" runat="server">
                            <asp:ListItem Value="1">Paid</asp:ListItem>
                            <asp:ListItem Value="0">UnPaid</asp:ListItem>
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
                  <%--  <asp:TemplateField HeaderStyle-Width="20px">
                        <HeaderTemplate>
                            <asp:CheckBox ID="chkPaymentSelectAll" runat="server" onclick="OnClickSelectAll(this)" />
                        </HeaderTemplate>
                        <ItemTemplate>
                        <asp:CheckBox ID="chkPaymentSetector" runat="server" AutoPostBack="True"/>
                        </ItemTemplate>
                    </asp:TemplateField>--%>
                    <asp:BoundField DataField="InsuranceNumber" DataFormatString="{0:d}" HeaderText='Social Registry #' 
                        SortExpression="InsuranceNumber" HeaderStyle-Width="80px">
                    <HeaderStyle Width="80px" />
                    </asp:BoundField>
                           <asp:BoundField DataField="OtherNames"    HeaderText='First Name'  HeaderStyle-Width ="80px" ></asp:BoundField>     
                          <asp:BoundField DataField="LastName"    HeaderText='Last Name'  HeaderStyle-Width ="80px" ></asp:BoundField>
                            <asp:BoundField DataField="phone"  HeaderText='Contact Number' HeaderStyle-Width ="70px">  </asp:BoundField>
                         <asp:BoundField DataField="Gender"  HeaderText='Gender' HeaderStyle-Width ="70px">  </asp:BoundField>
                           <asp:BoundField DataField="ProductCode"  HeaderText='Program' HeaderStyle-Width ="70px">  </asp:BoundField>
                            <asp:BoundField DataField="Amount"  HeaderText='Amount' HeaderStyle-Width ="70px">  </asp:BoundField>
                             <asp:BoundField DataField="regionname"  HeaderText='Region' HeaderStyle-Width ="70px">  </asp:BoundField>
                            <asp:BoundField DataField="districtname"  HeaderText='District' HeaderStyle-Width ="70px">  </asp:BoundField>
                            <asp:BoundField DataField="wardname"  HeaderText='Ward' HeaderStyle-Width ="70px">  </asp:BoundField>
                           <asp:BoundField DataField="villagename"  HeaderText='Settlement' HeaderStyle-Width ="70px">  </asp:BoundField>
                             <asp:BoundField DataField="StartDate"  HeaderText='Start Date' HeaderStyle-Width ="70px"> </asp:BoundField>    
                            <asp:BoundField DataField="EndDate"  HeaderText='End Date' HeaderStyle-Width ="70px">  </asp:BoundField> 
                       
                    <asp:BoundField DataField="PaymentStatusName" DataFormatString="{0:d}" HeaderText='<%$ Resources:Resource,L_PAYMENTSTATUS %>'
                        SortExpression="PaymentStatusName" HeaderStyle-Width="80px">
                    <HeaderStyle Width="80px" />
                    </asp:BoundField>
                    
                    <asp:BoundField DataField="PaymentDate" DataFormatString="{0:d}" HeaderText='<%$ Resources:Resource,L_PAYDATE%>'
                        SortExpression="PaymentDate" HeaderStyle-Width="70px">
                    <HeaderStyle Width="70px" />
                    </asp:BoundField>
                    <%--<asp:BoundField DataField="ReceiptNo" HeaderText='<%$ Resources:Resource,L_RECEIPT%>'
                        SortExpression="ReceiptNo" HeaderStyle-Width="70px">
                    <HeaderStyle Width="70px" />
                    </asp:BoundField>
                    <asp:BoundField DataField="TransactionNo" HeaderText='<%$ Resources:Resource,L_TRANSACTIONNUMBER%>'
                        SortExpression="TransactionNo" HeaderStyle-Width="70px">
                    <HeaderStyle Width="70px" />
                    </asp:BoundField>
                    <asp:BoundField DataField="ReceivedAmount" HeaderText='<%$ Resources:Resource,L_AMOUNT%>'
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
                    </asp:BoundField>--%>
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
                        Text='<%$ Resources:Resource,B_SUBMIT%>' Visible="false"  />
                </td>
                <td align="right" colspan="1">
                    <asp:Button
                        ID="B_CANCEL"
                        runat="server"
                        Text='<%$ Resources:Resource,B_CANCEL%>' Visible="false"  />

                </td>
            </tr>
        </table>
    </asp:Panel>
</ContentTemplate></asp:UpdatePanel>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="footer" runat="server">
</asp:Content>
<%@ Page Language="vb" MasterPageFile="~/IMIS.Master" AutoEventWireup="false" CodeBehind="AddPayrollCycle.aspx.vb" Inherits="IMIS.AddPayrollCycle" Title='AddPayrollCycle' %>

 <%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Body" runat="Server">
     
      <asp:UpdatePanel ID="upClaim" runat="server" RenderMode="Inline" > 
<Triggers>
<asp:PostBackTrigger ControlID="btnAdd" />
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
                        Text='<%$ Resources:Resource,H_ADDNEWPAYROLLCYCLE %>'>
                    </asp:Label>

                </td>
            </tr>
        </table>



        <asp:Panel ID="pnlTop" runat="server" CssClass="panelTop" Height="165px"  oncontextmenu="return false;">

                 <table>
                     <tr>
                         <tb>
                             <br />
                         </tb>
                     </tr>
                <tr> 
                            <td class="FormLabel">
                                <asp:Label ID="L_OTHERNAMES0" runat="server" Text="<%$ Resources:Resource,P_PAYROLLCYCLENAME %>">
                                </asp:Label>
                            </td>
                            <td class="DataEntry">
                                <asp:TextBox ID="txtPaycycleNames" runat="server"   MaxLength="100" Width="150px"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldOtherNames1" runat="server" ControlToValidate="txtPaycycleNames" SetFocusOnError="True" ValidationGroup="check" ForeColor="Red" Display="Dynamic"
                                        Text='*'>
                                </asp:RequiredFieldValidator> 
                            </td> 
                    <td class="FormLabel">
                        <asp:Label ID="L_REGION" runat="server" Text="<%$ Resources:Resource,L_REGION %>"></asp:Label>
                    </td>
                    <td class="DataEntry">
                        <asp:DropDownList ID="ddlRegion" runat="server" AutoPostBack="true">  </asp:DropDownList>
                    </td>
                     <td class="auto-style2">
                              <asp:RequiredFieldValidator ID="RequiredFieldRegion" runat="server"
                                  ControlToValidate="ddlRegion" InitialValue="0"
                                  SetFocusOnError="True"
                                  ValidationGroup="check" ForeColor="Red" Display="Dynamic"
                                        Text='*'></asp:RequiredFieldValidator>
                          </td>
                    <td class="DataEntry">
                                <asp:Label ID="lblMonth" runat="server"  Text='<%$ Resources:Resource, L_STARTMONTH %>' CssClass="FormLabel"></asp:Label>
                    </td>
                    <td class="DataEntry">
                         <asp:DropDownList ID="ddlMonth" Width="90px" runat="server">  </asp:DropDownList>
                     </td>
                          <td class="auto-style2">
                              <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server"
                                  ControlToValidate="ddlMonth" InitialValue="0"
                                  SetFocusOnError="True"
                                  ValidationGroup="check" ForeColor="Red" Display="Dynamic"
                                        Text='*'></asp:RequiredFieldValidator>
                          </td>           
                    <td class="DataEntry">
                                <asp:Label ID="lblYear" runat="server" Text='<%$ Resources:Resource, L_STARTYEAR %>' CssClass="FormLabel"></asp:Label>
                                </td>
                    <td class="DataEntry">
                                <asp:DropDownList ID="ddlYear" Width="68px" runat="server">   </asp:DropDownList>
                    </td>
                        
                </tr>
                <tr>
                  
                    
                </tr>
                <tr>
                    <td class="FormLabel">
                        <asp:Label ID="lblProduct" runat="server" class="FormLabel" Text="Program"></asp:Label>
                    </td>
                    <td class="DataEntry">
                        <asp:DropDownList ID="ddlproduct" runat="server"> </asp:DropDownList>
                    </td>
                     <td class="auto-style2">
                              <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="ddlproduct" InitialValue="0"
                               SetFocusOnError="True"   ValidationGroup="check" ForeColor="Red" Display="Dynamic" Text='*'></asp:RequiredFieldValidator>
                        </td>
                    <td class="DataEntry">
                            
                                <asp:Label ID="Label1" runat="server"  Text='<%$ Resources:Resource, L_ENDMONTH %>' CssClass="FormLabel"></asp:Label>
                                </td>
                    <td class="DataEntry">
                               <asp:DropDownList ID="ddlMonth1" Width="90px" runat="server"> </asp:DropDownList>
                    </td>
                         <td class="auto-style2">
                              <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="ddlMonth1" InitialValue="0"
                               SetFocusOnError="True"   ValidationGroup="check" ForeColor="Red" Display="Dynamic" Text='*'></asp:RequiredFieldValidator>
                        </td>
                    <td class="DataEntry">
                                <asp:Label ID="Label2" runat="server" Text='<%$ Resources:Resource, L_ENDYEAR %>' CssClass="FormLabel"></asp:Label>
                               </td>
                    <td class="DataEntry">
                               <asp:DropDownList ID="ddlYear1" Width="68px" runat="server">   </asp:DropDownList>
                     </td> 
                    
                    <td class="FormLabel">   
                        <asp:Button runat="server"  ValidationGroup="check" ID="btnAdd" OnClick="btnAddPayrollCycle_Click" Text="ADD" /> 
                    </td>
                </tr>
                    
            </table>
                <br />
            <table>
                 <tr>
                         <td>
                             <asp:Panel runat="server" ID="plnRegenerate" Visible="false" >
                                 <asp:Label ID="lblParollAlert" Font-Bold="true" ForeColor="Red" Font-Size="Larger"  runat="server" Text=""></asp:Label> 
                             </asp:Panel>
                            
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
                                     Text='<%$ Resources:Resource,H_PAYROLLCYCLE %>'>></asp:Label>
                            </td>

                        </tr>

                    </table>
                </td> 

            </tr>
        </table>
        <asp:Panel ID="pnlBody" runat="server" CssClass="panelBody">
            <asp:GridView ID="gvPayments" runat="server"
                AutoGenerateColumns="False"
                GridLines="None"
                AllowPaging="true" PagerSettings-FirstPageText="First Page" PagerSettings-LastPageText="Last Page" PagerSettings-Mode="NumericFirstLast" PageSize="10"
                CssClass="mGrid"  DataKeyNames="PayrollCycleID"
                PagerStyle-CssClass="pgr"
                
                EmptyDataText='<%$ Resources:Resource,L_NORECORDS %>'>

                <Columns>
                    <asp:CommandField SelectText="Select" ShowSelectButton="true"
                        ItemStyle-CssClass="HideButton" HeaderStyle-CssClass="HideButton">
                        <HeaderStyle CssClass="HideButton" />
                        <ItemStyle CssClass="HideButton" />
                    </asp:CommandField>
                      <asp:HyperLinkField DataNavigateUrlFields = "PayrollCycleID" DataTextField="name" DataNavigateUrlFormatString = "OverviewPayrollCycle.aspx?PayrollCycleID={0}" HeaderText='<%$ Resources:Resource,P_PAYROLLCYCLENAME %>' HeaderStyle-Width ="100px" >
                        <HeaderStyle Width="100px" /> 
                    </asp:HyperLinkField>  
                         <asp:BoundField DataField="productCode"    HeaderText='Program'  HeaderStyle-Width ="80px" ></asp:BoundField>     
                         <asp:BoundField DataField="RegionName"    HeaderText='Region'  HeaderStyle-Width ="80px" ></asp:BoundField>
                          <asp:BoundField DataField="StartDate"    HeaderText='Start Date'  HeaderStyle-Width ="80px" ></asp:BoundField>
                            <asp:BoundField DataField="EndDate"  HeaderText='End Date' HeaderStyle-Width ="70px">  </asp:BoundField>
                         <asp:BoundField DataField="Status"  HeaderText='Status' HeaderStyle-Width ="70px">  </asp:BoundField> 
                       
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
         <asp:Label text="" runat="server" ID="lblMsg"> </asp:Label>
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
</ContentTemplate>
          
      </asp:UpdatePanel>

    <script type="text/javascript">

        function ShowConfirm(msg) {
            if (confirm(msg)) {
               
                document.getElementById("HideLinkBt").click();

            }
        }
    </script>


</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Footer" Runat="Server" Visible="true" >
    
   
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

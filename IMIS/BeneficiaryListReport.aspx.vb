''Copyright (c) 2016-2017 Swiss Agency for Development and Cooperation (SDC)
''
''The program users must agree to the following terms:
''
''Copyright notices
''This program is free software: you can redistribute it and/or modify it under the terms of the GNU AGPL v3 License as published by the 
''Free Software Foundation, version 3 of the License.
''This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of 
''MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU AGPL v3 License for more details www.gnu.org.
''
''Disclaimer of Warranty
''There is no warranty for the program, to the extent permitted by applicable law; except when otherwise stated in writing the copyright 
''holders and/or other parties provide the program "as is" without warranty of any kind, either expressed or implied, including, but not 
''limited to, the implied warranties of merchantability and fitness for a particular purpose. The entire risk as to the quality and 
''performance of the program is with you. Should the program prove defective, you assume the cost of all necessary servicing, repair or correction.
''
''Limitation of Liability 
''In no event unless required by applicable law or agreed to in writing will any copyright holder, or any other party who modifies and/or 
''conveys the program as permitted above, be liable to you for damages, including any general, special, incidental or consequential damages 
''arising out of the use or inability to use the program (including but not limited to loss of data or data being rendered inaccurate or losses 
''sustained by you or third parties or a failure of the program to operate with any other programs), even if such holder or other party has been 
''advised of the possibility of such damages.
''
''In case of dispute arising out or in relation to the use of the program, it is subject to the public law of Switzerland. The place of jurisdiction is Berne.
'
' 
'




Imports System.Drawing
Imports System.IO
Imports System.Windows.Forms
Imports OfficeOpenXml.Style

Partial Public Class BeneficiaryListReport
    Inherits System.Web.UI.Page


    Public payment_id As String
    Private ePayment As New IMIS_EN.tblPayment
    Private eProduct As New IMIS_EN.tblPolicy
    Private Payment As New IMIS_BI.PaymentBI

    Private GenPayment As New IMIS_BI.GenPayBI
    Protected imisgen As New IMIS_Gen
    'Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
    '    AddRowSelectToGridView(gvPayments)
    '    MyBase.Render(writer)
    'End Sub
    'Private Sub AddRowSelectToGridView(ByVal gv As GridView)
    '    For Each row As GridViewRow In gv.Rows
    '        row.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(gv, "Select$" + row.RowIndex.ToString(), True))
    '        If Not row.Cells(12).Text = "&nbsp;" Then
    '            row.Style.Value = "color:#000080;font-style:italic;text-decoration:line-through"
    '        End If

    'Next
    'End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        RunPageSecurity()
        Try

            If Not IsPostBack = True Then

                FillRegion()

                FillProducts()


                'Session("ParentUrl") = "GeneratePayment.aspx"
            End If

        Catch ex As Exception
            lblMsg.Text = imisgen.getMessage("M_ERRORMESSAGE")
            imisgen.Alert(imisgen.getMessage("M_ERRORMESSAGE"), pnlBody, alertPopupTitle:="Nafa MIS")
            'EventLog.WriteEntry("IMIS", Page.Title & " : " & imisgen.getLoginName(Session("User")) & " : " & ex.Message, EventLogEntryType.Error, 999)
        End Try
    End Sub


    Private Sub FillProducts()
        Dim dtProducts As DataTable = GenPayment.GetProducts()
        ddlProduct.DataSource = dtProducts
        ddlProduct.DataValueField = "ProdId"
        ddlProduct.DataTextField = "ProductCode"
        ddlProduct.DataBind()
    End Sub
    Private Sub FillRegion()

        Dim dtRegions As DataTable = GenPayment.GetRegions(imisgen.getUserId(Session("User")), True)

        ddlRegion.DataSource = dtRegions
        ddlRegion.DataValueField = "RegionId"
        ddlRegion.DataTextField = "RegionName"
        ddlRegion.DataBind()
    End Sub

    Private Sub RunPageSecurity()
        Dim RoleID As Integer = imisgen.getRoleId(Session("User"))
        Dim UserID As Integer = imisgen.getUserId(Session("User"))
        If Payment.RunPageSecurity(IMIS_EN.Enums.Pages.FindPayment, Page) Then
            B_VIEW.Enabled = Payment.checkRights(IMIS_EN.Enums.Rights.PaymentSearch, UserID)
        Else
            Dim RefUrl = Request.Headers("Referer")
            Server.Transfer("Redirect.aspx?perm=0&page=" & IMIS_EN.Enums.Pages.FindPayment.ToString & "&retUrl=" & RefUrl)
        End If
    End Sub
    Private Sub loadSecurity()
        Dim RoleID As Integer = imisgen.getRoleId(Session("User"))

    End Sub
    Private Sub B_VIEW_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles B_VIEW.Click

        If gvPayments.SelectedDataKey Is Nothing Then
            Response.Redirect("Premium.aspx?f=0&p=0")
        End If

        Response.Redirect("Premium.aspx?f=" & gvPayments.SelectedDataKey.Values("FamilyID") & "&p=" & gvPayments.SelectedDataKey.Values("PremiumID") & "&po=" & gvPayments.SelectedDataKey.Values("PolicyID"))
    End Sub

    Protected Sub gvProducts_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles gvPayments.PageIndexChanging
        gvPayments.PageIndex = e.NewPageIndex

    End Sub

    Private Sub LoadGrid() Handles btnSearch.Click, gvPayments.PageIndexChanged

        loadSecurity()
        Dim eFamily As New IMIS_EN.tblFamilies
        Dim eInsuree As New IMIS_EN.tblInsuree

        If Trim(txtDateFrom.Text).Length > 0 Then
            If IsDate(txtDateFrom.Text) Then
                ePayment.DateFrom = Date.Parse(txtDateFrom.Text)
            Else
                imisgen.Alert(imisgen.getMessage("M_INVALIDDATE"), pnlButtons, alertPopupTitle:="Nafa MIS")
                Return
            End If
        End If
        If Trim(txtDateTo.Text).Length > 0 Then
            If IsDate(txtDateTo.Text) Then
                ePayment.DateTo = Date.Parse(txtDateTo.Text)
            Else
                imisgen.Alert(imisgen.getMessage("M_INVALIDDATE"), pnlButtons, alertPopupTitle:="Nafa MIS")
                Return
            End If
        End If

        If Val(ddlProduct.SelectedValue) > 0 Then
            eProduct.ProdID = Val(ddlProduct.SelectedValue)

            ePayment.ProductCode = ddlProduct.SelectedItem.Text
        End If

        'If Val(ddlProduct.SelectedIndex.ToString()) > 0 Then
        'End If
        ePayment.RegionId = Val(ddlRegion.SelectedValue)
        ePayment.ProductId = Val(ddlProduct.SelectedValue)




        gvPayments.DataSource = Nothing


        Dim dtPaymentRegional As DataTable = GenPayment.getBeneficiaryReport(ePayment)

        'L_FOUNDPAYMENTS.Text = If(dtPaymentRegional.Rows.Count = 0, imisgen.getMessage("L_NO"), Format(dtPaymentRegional.Rows.Count, "#,###")) & " " & imisgen.getMessage("L_FOUNDPAYMENTS")
        gvPayments.DataSource = dtPaymentRegional
        gvPayments.SelectedIndex = -1
                gvPayments.DataBind()


    End Sub
    Private Sub DisableButtonsOnEmptyRows(ByRef gv As GridView)
        If gv.Rows.Count = 0 Then
            'B_VIEW.Enabled = False
        End If
    End Sub
    Protected Sub ExportToExcel(sender As Object, e As EventArgs)

    End Sub
    Protected Sub exceltxt_Click(sender As Object, e As EventArgs)
        Try
            Response.ClearContent()
            Response.AddHeader("content-disposition", "attachment; filename=gvtoexcel.xls")
            Response.ContentType = "application/excel"
            Dim sw As New System.IO.StringWriter()
            Dim htw As New HtmlTextWriter(sw)
            gvPayments.RenderControl(htw)
            Response.Write(sw.ToString())
            HttpContext.Current.Response.Close()


        Catch ex As Exception
            Dim msg As String = ex.Message
        End Try
    End Sub
    Public Overrides Sub VerifyRenderingInServerForm(ByVal control As System.Web.UI.Control)
        ' Do NOT call MyBase.VerifyRenderingInServerForm
    End Sub
    'Protected Sub gvPayments_SelectedIndexChanged(sender As Object, e As EventArgs) Handles gvPayments.SelectedIndexChanged
    '    If gvPayments.SelectedDataKey Is Nothing Then
    '        Response.Redirect("PaymentOverview.aspx?p=0")
    '    End If
    '    payment_id = gvPayments.SelectedDataKey.Values("PaymentUUID").ToString()
    '    Response.Redirect("PaymentOverview.aspx?p=" & payment_id)
    'End Sub

    Private Sub ddlRegion_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlRegion.SelectedIndexChanged
        If Val(ddlRegion.SelectedValue) > 0 Then
            'FillDistrict()
        Else
            ddlProduct.Items.Clear()
        End If

    End Sub


End Class
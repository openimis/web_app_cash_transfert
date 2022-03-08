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
Imports System.Threading
Imports System.Windows.Forms
Imports OfficeOpenXml.Style

Partial Public Class AddPayrollCycle
    Inherits System.Web.UI.Page


    Public payment_id As String
    Private ePayment As New IMIS_EN.tblPayment
    Private eProduct As New IMIS_EN.tblPolicy
    Private Payment As New IMIS_BI.PaymentBI

    Private PaymentProviderStatusBI As New IMIS_BI.PaymentProviderPaidStatusBI
    Private reports As New IMIS_BI.ReportsBI
    Private GenPayment As New IMIS_BI.GenPayBI
    Protected imisgen As New IMIS_Gen
    Private FindPaymentProviderPaymentsB As New IMIS_BI.FindPaymentProviderPaymentsBI
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Try
            If Not IsPostBack = True Then
                LoadGrid()
                'get user id from session
                Dim UserID As Integer
                UserID = imisgen.getUserId(Session("User"))

                'call the fillregion method to populate ddlRegion control

                FillRegion()
                FillProduct()
                FillYear()
                FillMonth()

            End If
        Catch ex As Exception
            imisgen.Alert(imisgen.getMessage("M_ERRORMESSAGE"), pnlButtons, alertPopupTitle:="Nafa MIS")
            'EventLog.WriteEntry("IMIS", Page.Title & " : " & imisgen.getLoginName(Session("User")) & " : " & ex.Message, EventLogEntryType.Error, 999)
            Return
        End Try
    End Sub
    Private Sub FillYear()
        ddlYear.DataSource = reports.GetYears(2010, Year(Now()) + 5)
        ddlYear.DataValueField = "Year"
        ddlYear.DataTextField = "Year"
        ddlYear.DataBind()
        ddlYear.SelectedValue = Year(Now())
        ddlYear1.DataSource = reports.GetYears(2010, Year(Now()) + 5)
        ddlYear1.DataValueField = "Year"
        ddlYear1.DataTextField = "Year"
        ddlYear1.DataBind()
        ddlYear1.SelectedValue = Year(Now())

        'ddlYear.Attributes.Add("style", "display:hidden;")
        'lblYear.Attributes.Add("style", "display:hidden;")
    End Sub
    Private Sub FillMonth()

        ddlMonth.DataSource = reports.GetMonths(1, 12)
        ddlMonth.DataValueField = "MonthNum"
        ddlMonth.DataTextField = "MonthName"
        ddlMonth.DataBind()
        ddlMonth1.DataSource = reports.GetMonths(1, 12)
        ddlMonth1.DataValueField = "MonthNum"
        ddlMonth1.DataTextField = "MonthName"
        ddlMonth1.DataBind()
        'ddlMonth.Attributes.Add("style", "display:hidden;")
        'lblMonth.Attributes.Add("style", "display:hidden;")
    End Sub
    Private Sub ddlRegion_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlRegion.SelectedIndexChanged
        Session("RegionSelected") = ddlRegion.SelectedValue.ToString
        FillDistricts()

    End Sub

    Private Sub ddlDistrict_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlRegion.SelectedIndexChanged
        'Session("DistrictIDFindClaims") = ddlDistrict.SelectedValue.ToString()
        'FillHF(imisgen.getUserId(Session("User")))
    End Sub

    Private Sub FillRegion()
        Dim GenPayment As New IMIS_BI.GenPayBI
        Dim dtRegions As DataTable = GenPayment.GetRegions(imisgen.getUserId(Session("User")), True)

        ddlRegion.DataSource = dtRegions
        ddlRegion.DataValueField = "RegionId"
        ddlRegion.DataTextField = "RegionName"
        ddlRegion.DataBind()

        If dtRegions.Rows.Count = 1 Then
            FillDistricts()

        End If
    End Sub

    Private Sub FillDistricts()
        'ddlDistrict.DataSource = PaymentProviderStatusBI.GetDistricts(imisgen.getUserId(Session("User")), True, Val(ddlRegion.SelectedValue))
        'ddlDistrict.DataValueField = "DistrictId"
        'ddlDistrict.DataTextField = "DistrictName"
        'ddlDistrict.DataBind()
    End Sub

    Private Sub FillProduct()
        ddlproduct.DataSource = PaymentProviderStatusBI.GetProducts(imisgen.getUserId(Session("User")))
        ddlproduct.DataValueField = "ProdId"
        ddlproduct.DataTextField = "ProductCode"
        ddlproduct.DataBind()
    End Sub


    Private Sub LoadGrid() Handles gvPayments.PageIndexChanged

        Dim dtPayrollCycle As DataTable = GenPayment.GetPayrollCycleGridView()
        gvPayments.DataSource = dtPayrollCycle
        gvPayments.SelectedIndex = -1
        gvPayments.DataBind()

    End Sub

    Protected Sub gvProducts_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles gvPayments.PageIndexChanging
        gvPayments.PageIndex = e.NewPageIndex

    End Sub

    Protected Sub B_CANCEL_Click(sender As Object, e As EventArgs) Handles B_CANCEL.Click
        Response.Redirect("Home.aspx")
    End Sub

    Protected Sub btnAddPayrollCycle_Click(sender As Object, e As EventArgs)

        ePayment.RegionId = Val(ddlRegion.SelectedValue)

        If Val(ddlproduct.SelectedValue) > 0 Then
            eProduct.ProdID = Val(ddlproduct.SelectedValue)
            ePayment.ProdId = Val(ddlproduct.SelectedValue)
            ePayment.ProductCode = ddlproduct.SelectedItem.Text
        End If

        ePayment.ProductCode = ddlproduct.SelectedItem.Text
        ePayment.PayCycleName = txtPaycycleNames.Text

        ePayment.StartMonth = ddlMonth.SelectedItem.Text
        ePayment.StartYear = ddlYear.SelectedValue
        ePayment.EndMonth = ddlMonth1.SelectedItem.Text
        ePayment.EndYear = ddlYear1.SelectedValue

        ' check if payrol cycle already exist
        Dim ifRExists As Boolean = GenPayment.ifPayrollCycleExist(ePayment)
        If ifRExists = True Then
            plnRegenerate.Visible = True
            lblParollAlert.Text = " Payroll Cycle already Exist. contact Administrator for further information !"
        Else
            GenPayment.InsertPayrollCycle(ePayment)
            LoadGrid()
        End If

    End Sub


End Class
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

Partial Public Class OverviewPayrollCycle
    Inherits System.Web.UI.Page

    Public payrollCycleID As Integer
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
            Dim IDPassed As String = HttpContext.Current.Request.QueryString("PayrollCycleID")
            payrollCycleID = Val(IDPassed)
            If Not IsPostBack = True Then

                'get user id from session
                Dim UserID As Integer
                UserID = imisgen.getUserId(Session("User"))

                'call the fillregion method to populate ddlRegion control

                FillRegion()
                FillProduct()
                FillYear()
                FillMonth()

                If HttpContext.Current.Request.QueryString("PayrollCycleID") IsNot Nothing Then
                    ePayment.PayrollCycleID = payrollCycleID
                    GenPayment.GetPayrollCycleDetails(ePayment)
                    txtPaycycleNames.Text = ePayment.PayCycleName
                    ddlRegion.SelectedValue = ePayment.RegionId
                    ddlMonth.SelectedValue = ePayment.StartMonth
                    ddlYear.SelectedValue = ePayment.EndYear
                    ddlMonth1.SelectedValue = ePayment.EndMonth
                    ddlYear1.SelectedValue = ePayment.EndYear
                    ddlproduct.SelectedValue = ePayment.ProdId
                    StatusDropDownList.SelectedValue = ePayment.PaymentStatusName
                End If
                LoadGrid()

                'check if payroll generated already
                If gvPayments.Rows.Count() >= 1 Then
                    ddlRegion.Enabled = False
                    ddlMonth.Enabled = False
                    ddlYear.Enabled = False
                    ddlproduct.Enabled = False
                    ddlMonth1.Enabled = False
                    ddlYear1.Enabled = False
                End If

            End If
        Catch ex As Exception
            imisgen.Alert(imisgen.getMessage("M_ERRORMESSAGE"), pnlButtons, alertPopupTitle:="Nafa MIS")
            'EventLog.WriteEntry("IMIS", Page.Title & " : " & imisgen.getLoginName(Session("User")) & " : " & ex.Message, EventLogEntryType.Error, 999)
            Return
        End Try
        CheckPayrollCycleStatus()
    End Sub

    Private Sub CheckPayrollCycleStatus()
        If StatusDropDownList.SelectedValue = "Close" Then
            btnGeneratePay.Visible = False
            btnRegenerate.Visible = False
            plnRegenerate.Visible = False
        Else

            If gvPayments.Rows.Count() >= 1 Then
                ddlRegion.Enabled = False
                ddlMonth.Enabled = False
                ddlYear.Enabled = False
                ddlproduct.Enabled = False
                ddlMonth1.Enabled = False
                ddlYear1.Enabled = False
            End If

        End If
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

    End Sub
    Private Sub FillMonth()

        ddlMonth.DataSource = reports.GetMonths(1, 12)
        ddlMonth.DataValueField = "MonthName"
        ddlMonth.DataTextField = "MonthName"
        ddlMonth.DataBind()
        ddlMonth1.DataSource = reports.GetMonths(1, 12)
        ddlMonth1.DataValueField = "MonthName"
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


    Protected Sub B_CANCEL_Click(sender As Object, e As EventArgs) Handles B_CANCEL.Click
        Response.Redirect("Home.aspx")
    End Sub

    Protected Sub btnUpdatePayrollcycle_Click(sender As Object, e As EventArgs)

        ePayment.RegionId = Val(ddlRegion.SelectedValue)
        ePayment.PayrollCycleID = payrollCycleID
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
        ePayment.PaymentStatusName = StatusDropDownList.SelectedValue
        GenPayment.UpdatePayrollCycle(ePayment)
        LoadGrid()
        CheckPayrollCycleStatus()
    End Sub

    Protected Sub btnGeneratePay_Click(sender As Object, e As EventArgs)

        Dim eFamily As New IMIS_EN.tblFamilies
        Dim eInsuree As New IMIS_EN.tblInsuree
        ePayment.RegionId = Val(ddlRegion.SelectedValue)


        If Val(ddlproduct.SelectedValue) > 0 Then
            eProduct.ProdID = Val(ddlproduct.SelectedValue)

            ePayment.ProductCode = ddlproduct.SelectedItem.Text
        End If

        ePayment.ProductCode = ddlproduct.SelectedItem.Text

        ePayment.ProductId = Val(ddlproduct.SelectedValue)
        ePayment.StartMonth = ddlMonth.SelectedItem.Text
        ePayment.StartYear = ddlYear.SelectedValue
        ePayment.EndMonth = ddlMonth1.SelectedItem.Text
        ePayment.EndYear = ddlYear1.SelectedValue
        ePayment.PayrollCycleID = payrollCycleID


        If (ddlRegion.SelectedItem.Text.Equals("National")) Then
            Dim ifExists As Boolean = GenPayment.IfExistNational(ePayment)
            If ifExists = True Then
                Dim isApproved As Boolean = GenPayment.IfNationalPayrollApproved(ePayment)
                If isApproved = True Then
                    plnRegenerate.Visible = True
                    lblParollAlert.Text = "This payroll has been marked as approved and cannot be regenerated. Contact administrator for further information !"
                    btnRegenerate.Visible = False
                Else
                    lblParollAlert.Text = " Note! Payroll for this selected Region and selected criteria has already been generated. click Regenerate button below if you wish to continue and Regenerate !."
                    plnRegenerate.Visible = True
                    btnRegenerate.Visible = True
                End If
                gvPayments.DataSource = Nothing
            Else
                plnRegenerate.Visible = False
                btnRegenerate.Visible = False
                'gvPayments.DataSource = Nothing

                Dim dtPayment As DataTable = GenPayment.getPayNational(ePayment, eProduct)
                Dim dtPaymentNational As DataTable = GenPayment.getGridPayNational(ePayment)

                gvPayments.DataSource = dtPaymentNational
                gvPayments.SelectedIndex = -1
                gvPayments.DataBind()
            End If

        Else
            Dim ifRExists As Boolean = GenPayment.IfExistRegional(ePayment)
            If ifRExists = True Then
                Dim isRApproved As Boolean = GenPayment.IfRegionalPayrollApproved(ePayment)
                If isRApproved = True Then
                    plnRegenerate.Visible = True
                    lblParollAlert.Text = "This payroll has been marked as approved and cannot be regenerated. Contact administrator for further information !"
                    btnRegenerate.Visible = False
                Else
                    lblParollAlert.Text = " Note! Payroll for this selected Region and selected criteria has already been generated. click Regenerate button below if you wish to continue and Regenerate !."
                    plnRegenerate.Visible = True
                    btnRegenerate.Visible = True
                End If
            Else
                plnRegenerate.Visible = False
                btnRegenerate.Visible = False
                Dim dtPaymentRegion As DataTable = GenPayment.getPayRegional(ePayment, eProduct)
                Dim dtPaymentRegional As DataTable = GenPayment.getGridPayRegional(ePayment)
                gvPayments.DataSource = dtPaymentRegional
                gvPayments.SelectedIndex = -1
                gvPayments.DataBind()
            End If

        End If
        If gvPayments.Rows.Count() >= 1 Then
            ddlRegion.Enabled = False
            ddlMonth.Enabled = False
            ddlYear.Enabled = False
            ddlproduct.Enabled = False
            ddlMonth1.Enabled = False
            ddlYear1.Enabled = False
        End If
        CheckPayrollCycleStatus()
    End Sub

    Protected Sub btnRegeerate_Click(sender As Object, e As EventArgs)
        Dim eFamily As New IMIS_EN.tblFamilies
        Dim eInsuree As New IMIS_EN.tblInsuree
        ePayment.RegionId = Val(ddlRegion.SelectedValue)
        ePayment.PayrollCycleID = payrollCycleID

        If Val(ddlproduct.SelectedValue) > 0 Then
            eProduct.ProdID = Val(ddlproduct.SelectedValue)

            ePayment.ProductCode = ddlproduct.SelectedItem.Text
        End If

        ePayment.ProductCode = ddlproduct.SelectedItem.Text

        ePayment.ProductId = Val(ddlproduct.SelectedValue)
        ePayment.StartMonth = ddlMonth.SelectedItem.Text
        ePayment.StartYear = ddlYear.SelectedValue
        ePayment.EndMonth = ddlMonth1.SelectedItem.Text
        ePayment.EndYear = ddlYear1.SelectedValue


        If (ddlRegion.SelectedItem.Text.Equals("National")) Then
            Dim isApproved As Boolean = GenPayment.IfNationalPayrollApproved(ePayment)
            If isApproved = True Then
                plnRegenerate.Visible = True
                lblParollAlert.Text = "This payroll has been marked as approved and cannot be regenerated. Contact administrator for further information !"
                btnRegenerate.Visible = False
            Else
                Dim dtPayment As DataTable = GenPayment.getPayNational(ePayment, eProduct)
                Dim dtPaymentNational As DataTable = GenPayment.getGridPayNational(ePayment)
                gvPayments.DataSource = dtPaymentNational
                gvPayments.SelectedIndex = -1
                gvPayments.DataBind()
            End If
        Else
            Dim ifRExists As Boolean = GenPayment.IfExistRegional(ePayment)
            If ifRExists = True Then
                Dim isRApproved As Boolean = GenPayment.IfRegionalPayrollApproved(ePayment)
                If isRApproved = True Then
                    plnRegenerate.Visible = True
                    lblParollAlert.Text = "This payroll has been marked as approved and cannot be regenerated. Contact administrator for further information !"
                    btnRegenerate.Visible = False
                Else
                    Dim dtPaymentRegion As DataTable = GenPayment.getPayRegional(ePayment, eProduct)
                    Dim dtPaymentRegional As DataTable = GenPayment.getGridPayRegional(ePayment)
                    gvPayments.DataSource = dtPaymentRegional
                    gvPayments.SelectedIndex = -1
                    gvPayments.DataBind()
                End If

            End If

        End If

        CheckPayrollCycleStatus()
    End Sub
    Private Sub LoadGrid()

        Dim eFamily As New IMIS_EN.tblFamilies
        Dim eInsuree As New IMIS_EN.tblInsuree
        ePayment.RegionId = Val(ddlRegion.SelectedValue)


        If Val(ddlproduct.SelectedValue) > 0 Then
            eProduct.ProdID = Val(ddlproduct.SelectedValue)

            ePayment.ProductCode = ddlproduct.SelectedItem.Text
        End If

        ePayment.PayrollCycleID = payrollCycleID
        ePayment.ProductCode = ddlproduct.SelectedItem.Text

        ePayment.ProductId = Val(ddlproduct.SelectedValue)
        ePayment.StartMonth = ddlMonth.SelectedItem.Text
        ePayment.StartYear = ddlYear.SelectedValue
        ePayment.EndMonth = ddlMonth1.SelectedItem.Text
        ePayment.EndYear = ddlYear1.SelectedValue

        Dim ifExists As Boolean = GenPayment.IfExistNational(ePayment)



        If (ddlRegion.SelectedItem.Text.Equals("National")) Then
            Dim dtPaymentNational As DataTable = GenPayment.getGridPayNational(ePayment)
            gvPayments.DataSource = dtPaymentNational
            gvPayments.SelectedIndex = -1
            gvPayments.DataBind()

        Else
            Dim dtPaymentNational As DataTable = GenPayment.getGridPayRegional(ePayment)

            gvPayments.DataSource = dtPaymentNational
            gvPayments.SelectedIndex = -1
            gvPayments.DataBind()

        End If
        If (gvPayments.Rows.Count() >= 1) Then
            btnGeneratePay.Visible = False
            plnRegenerate.Visible = True
            lblParollAlert.Visible = False
            btnRegenerate.Visible = True
        End If
    End Sub


    Protected Sub LoadGrid1() Handles gvPayments.PageIndexChanged
        Dim eFamily As New IMIS_EN.tblFamilies
        Dim eInsuree As New IMIS_EN.tblInsuree
        ePayment.RegionId = Val(ddlRegion.SelectedValue)


        If Val(ddlproduct.SelectedValue) > 0 Then
            eProduct.ProdID = Val(ddlproduct.SelectedValue)

            ePayment.ProductCode = ddlproduct.SelectedItem.Text
        End If

        ePayment.ProductCode = ddlproduct.SelectedItem.Text

        ePayment.ProductId = Val(ddlproduct.SelectedValue)
        ePayment.StartMonth = ddlMonth.SelectedItem.Text
        ePayment.StartYear = ddlYear.SelectedValue
        ePayment.EndMonth = ddlMonth1.SelectedItem.Text
        ePayment.EndYear = ddlYear1.SelectedValue

        Dim ifExists As Boolean = GenPayment.IfExistNational(ePayment)



        If (ddlRegion.SelectedItem.Text.Equals("National")) Then
            Dim dtPaymentNational As DataTable = GenPayment.getGridPayNational(ePayment)
            gvPayments.DataSource = dtPaymentNational
            gvPayments.SelectedIndex = -1
            gvPayments.DataBind()

        Else
            Dim dtPaymentNational As DataTable = GenPayment.getGridPayRegional(ePayment)

            gvPayments.DataSource = dtPaymentNational
            gvPayments.SelectedIndex = -1
            gvPayments.DataBind()

        End If
        If (gvPayments.Rows.Count() >= 1) Then
            btnGeneratePay.Visible = False
            plnRegenerate.Visible = True
            lblParollAlert.Visible = False
            btnRegenerate.Visible = True
        End If
    End Sub

    Protected Sub gvPayments_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles gvPayments.PageIndexChanging
        gvPayments.PageIndex = e.NewPageIndex
    End Sub

End Class
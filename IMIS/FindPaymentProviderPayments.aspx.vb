Imports System.Diagnostics
<Assembly: DebuggerDisplay("{ToString}", Target:=GetType(Date))>
Public Class FindPaymentProviderPayments
    Inherits System.Web.UI.Page


    Private FindPaymentProviderPaymentsB As New IMIS_BI.FindPaymentProviderPaymentsBI
    Protected imisgen As New IMIS_Gen
    Public payment_id As String
    Private reports As New IMIS_BI.ReportsBI
    Private ePayment As New IMIS_EN.tblPayment
    Private Payment As New IMIS_BI.FindPaymentProviderPaymentsBI

    'page load event to prepopulate some controls
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Try
            If IsPostBack = True Then Return

            'get user id from session
            Dim UserID As Integer
            UserID = imisgen.getUserId(Session("User"))

            'call the fillregion method to populate ddlRegion control
            FillRegion()
            If Request.QueryString("c") = "c" Then
                ddlRegion.SelectedValue = CType(Session("RegionSelected"), Integer)
                FillDistricts()
                ddlDistrict.SelectedValue = 0
            End If

            'fill HFCode
            FillHF(UserID)


            FillProduct()
            FillYear()
            FillMonth()

        Catch ex As Exception
            imisgen.Alert(imisgen.getMessage("M_ERRORMESSAGE"), pnlButtons, alertPopupTitle:="Nafa MIS")
            'EventLog.WriteEntry("IMIS", Page.Title & " : " & imisgen.getLoginName(Session("User")) & " : " & ex.Message, EventLogEntryType.Error, 999)
            Return
        End Try
    End Sub

    Private Sub ddlRegion_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlRegion.SelectedIndexChanged
        Session("RegionSelected") = ddlRegion.SelectedValue.ToString
        FillDistricts()

    End Sub

    Private Sub ddlDistrict_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlDistrict.SelectedIndexChanged, ddlRegion.SelectedIndexChanged
        Session("DistrictIDFindClaims") = ddlDistrict.SelectedValue.ToString()
        FillHF(imisgen.getUserId(Session("User")))
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
        ddlDistrict.DataSource = FindPaymentProviderPaymentsB.GetDistricts(imisgen.getUserId(Session("User")), True, Val(ddlRegion.SelectedValue))
        ddlDistrict.DataValueField = "DistrictId"
        ddlDistrict.DataTextField = "DistrictName"
        ddlDistrict.DataBind()
    End Sub

    Private Sub FillProduct()
        ddlproduct.DataSource = FindPaymentProviderPaymentsB.GetProducts(imisgen.getUserId(Session("User")))
        ddlproduct.DataValueField = "ProdId"
        ddlproduct.DataTextField = "ProductCode"
        ddlproduct.DataBind()
    End Sub

    Private Sub FillHF(ByVal UserID As Integer)
        Dim LocationId As Integer = 0
        If Val(ddlDistrict.SelectedValue) > 0 Then
            LocationId = Val(ddlDistrict.SelectedValue)
        ElseIf Val(ddlRegion.SelectedValue) > 0 Then
            LocationId = Val(ddlRegion.SelectedValue)
        End If


    End Sub

    Private Sub LoadGrid() Handles btnSearch.Click, gvPayments.PageIndexChanged

        ePayment.AuditUserID = imisgen.getUserId(Session("User"))

        ePayment.RegionId = Val(ddlRegion.SelectedValue)

        If Val(ddlDistrict.SelectedValue) > 0 Then ePayment.DistrictId = ddlDistrict.SelectedValue

        ePayment.ProdId = Val(ddlproduct.SelectedValue)
        ePayment.ProductCode = ddlproduct.SelectedItem.Text

        ePayment.StartMonth = ddlMonth.SelectedItem.Text
        ePayment.StartYear = ddlYear.SelectedValue
        ePayment.EndMonth = ddlMonth1.SelectedItem.Text
        ePayment.EndYear = ddlYear1.SelectedValue


        Dim dtPayment As DataTable = Payment.getPayment(ePayment)

        Dim StatusColumn As New Data.DataColumn("PaymentStatusName", GetType(System.String))
        dtPayment.Columns.Add(StatusColumn)
        For Each row As DataRow In dtPayment.Rows
            row(StatusColumn) = Payment.GetPayementStatus(If(IsDBNull(row("PaymentStatus")), 0, row("PaymentStatus")))
        Next

        gvPayments.DataSource = dtPayment
        gvPayments.SelectedIndex = -1
        gvPayments.DataBind()

    End Sub

    Protected Sub B_SUBMIT_Click(sender As Object, e As EventArgs) Handles B_SUBMIT.Click
        If chkStatusPaid.Checked Then

            If chkBoxMarkAllAsPaid.Checked Then
                ePayment.RegionId = Val(ddlRegion.SelectedValue)
                ePayment.ProductCode = ddlproduct.SelectedItem.Text
                ePayment.StartMonth = ddlMonth.SelectedItem.Text
                ePayment.StartYear = ddlYear.SelectedValue
                ePayment.EndMonth = ddlMonth1.SelectedItem.Text
                ePayment.EndYear = ddlYear1.SelectedValue
                ePayment.PaymentStatus = 1
                ePayment.AuditUserID = imisgen.getUserId(Session("User"))
                Dim Message As String = Payment.UpdateAllPaymentStatus(ePayment)
            Else
                For Each row As GridViewRow In gvPayments.Rows
                    Dim PaymentId As Integer = Convert.ToInt32(gvPayments.DataKeys(row.RowIndex).Value)

                    ePayment.PaymentID = PaymentId
                    ePayment.PaymentStatus = 1
                    ePayment.AuditUserID = imisgen.getUserId(Session("User"))
                    ' Access the CheckBox
                    Dim cb As CheckBox = row.FindControl("chkPaymentSetector")
                    If cb IsNot Nothing AndAlso cb.Checked Then
                        ' First, get the PaymentId for the selected row 
                        Dim Message As String = Payment.UpdatePaymentStatus(ePayment)

                    End If
                Next
            End If
        Else
            If chkBoxMarkAllAsPaid.Checked Then
                ePayment.RegionId = Val(ddlRegion.SelectedValue)
                ePayment.ProductCode = ddlproduct.SelectedItem.Text
                ePayment.StartMonth = ddlMonth.SelectedItem.Text
                ePayment.StartYear = ddlYear.SelectedValue
                ePayment.EndMonth = ddlMonth1.SelectedItem.Text
                ePayment.EndYear = ddlYear1.SelectedValue
                ePayment.PaymentStatus = 1
                ePayment.AuditUserID = imisgen.getUserId(Session("User"))
                Dim Message As String = Payment.UpdateAllPaymentStatus(ePayment)
            End If
        End If
            LoadGrid()
    End Sub
    Protected Sub gvProducts_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles gvPayments.PageIndexChanging
        gvPayments.PageIndex = e.NewPageIndex
    End Sub

    Protected Sub B_CANCEL_Click(sender As Object, e As EventArgs) Handles B_CANCEL.Click
        Response.Redirect("Home.aspx")
    End Sub


End Class
'Public Class FindPaymentProviderPayments
'    Inherits System.Web.UI.Page

'    Private FindPaymentProviderPaymentsB As New IMIS_BI.FindPaymentProviderPaymentsBI
'    Protected imisgen As New IMIS_Gen
'    Public payment_id As String
'    Private ePayment As New IMIS_EN.tblPayment
'    Private Payment As New IMIS_BI.FindPaymentProviderPaymentsBI

'    'page load event to prepopulate some controls
'    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

'        Try
'            If IsPostBack = True Then Return

'            'get user id from session
'            Dim UserID As Integer
'            UserID = imisgen.getUserId(Session("User"))

'            'call the fillregion method to populate ddlRegion control
'            FillRegion()
'            If Request.QueryString("c") = "c" Then
'                ddlRegion.SelectedValue = CType(Session("RegionSelected"), Integer)
'                FillDistricts()
'                ddlDistrict.SelectedValue = 0
'            End If

'            'fill HFCode
'            FillHF(UserID)
'            If ddlHFCode.Items.Count = 1 Then
'                txtHFName.Enabled = False
'            End If

'            FillProduct()

'        Catch ex As Exception
'            imisgen.Alert(imisgen.getMessage("M_ERRORMESSAGE"), pnlButtons, alertPopupTitle:="Nafa MIS")
'            'EventLog.WriteEntry("IMIS", Page.Title & " : " & imisgen.getLoginName(Session("User")) & " : " & ex.Message, EventLogEntryType.Error, 999)
'            Return
'        End Try
'    End Sub

'    Private Sub ddlRegion_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlRegion.SelectedIndexChanged
'        Session("RegionSelected") = ddlRegion.SelectedValue.ToString
'        FillDistricts()

'    End Sub

'    Private Sub ddlDistrict_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlDistrict.SelectedIndexChanged, ddlRegion.SelectedIndexChanged
'        Session("DistrictIDFindClaims") = ddlDistrict.SelectedValue.ToString()
'        FillHF(imisgen.getUserId(Session("User")))
'    End Sub

'    Private Sub FillRegion()
'        Dim dtRegions As DataTable = FindPaymentProviderPaymentsB.GetRegions(imisgen.getUserId(Session("User")), True)
'        ddlRegion.DataSource = dtRegions
'        ddlRegion.DataValueField = "RegionId"
'        ddlRegion.DataTextField = "RegionName"
'        ddlRegion.DataBind()

'        If dtRegions.Rows.Count = 1 Then
'            FillDistricts()

'        End If
'    End Sub

'    Private Sub FillDistricts()
'        ddlDistrict.DataSource = FindPaymentProviderPaymentsB.GetDistricts(imisgen.getUserId(Session("User")), True, Val(ddlRegion.SelectedValue))
'        ddlDistrict.DataValueField = "DistrictId"
'        ddlDistrict.DataTextField = "DistrictName"
'        ddlDistrict.DataBind()
'    End Sub

'    Private Sub FillProduct()
'        ddlproduct.DataSource = FindPaymentProviderPaymentsB.GetProducts(imisgen.getUserId(Session("User")))
'        ddlproduct.DataValueField = "ProdId"
'        ddlproduct.DataTextField = "ProductCode"
'        ddlproduct.DataBind()
'    End Sub

'    Private Sub FillHF(ByVal UserID As Integer)
'        Dim LocationId As Integer = 0
'        If Val(ddlDistrict.SelectedValue) > 0 Then
'            LocationId = Val(ddlDistrict.SelectedValue)
'        ElseIf Val(ddlRegion.SelectedValue) > 0 Then
'            LocationId = Val(ddlRegion.SelectedValue)
'        End If
'        ddlHFCode.DataSource = FindPaymentProviderPaymentsB.GetHFCodes(UserID, LocationId)
'        ddlHFCode.DataValueField = "HfID"
'        ddlHFCode.DataTextField = "HFCODE"
'        ddlHFCode.DataBind()
'        If Request.QueryString("c") = "c" Then
'            If IsPostBack = False Then
'                ddlHFCode.SelectedValue = CType(Session("HFID"), Integer)
'                ''''Clear HFID session
'                Session.Remove("HFID")
'            End If
'        End If

'    End Sub

'    Private Sub LoadGrid() Handles btnSearch.Click, gvPayments.PageIndexChanged

'        ePayment.AuditUserID = imisgen.getUserId(Session("User"))

'        ePayment.RegionId = Val(ddlRegion.SelectedValue)

'        If Val(ddlDistrict.SelectedValue) > 0 Then ePayment.DistrictId = ddlDistrict.SelectedValue

'        ePayment.ProdId = Val(ddlproduct.SelectedValue)

'        ePayment.HfId = Val(ddlHFCode.SelectedValue)

'        ePayment.ReceiptNo = If(txtReceiptNo.Text = "", Nothing, txtReceiptNo.Text)

'        ePayment.TransactionNumber = If(txtTransactionNumber.Text = "", Nothing, txtTransactionNumber.Text)

'        ePayment.HfName = If(txtHFName.Text = "", Nothing, txtHFName.Text)

'        If Trim(txtDateOfPaymentFrom.Text).Length > 0 Then
'            If IsDate(txtDateOfPaymentFrom.Text) Then
'                ePayment.DateFrom = Date.Parse(txtDateOfPaymentFrom.Text)
'            Else
'                imisgen.Alert(imisgen.getMessage("M_INVALIDDATE"), pnlButtons, alertPopupTitle:="Nafa MIS")
'                Return
'            End If
'        End If

'        If Trim(txtDateOfPaymentTo.Text).Length > 0 Then
'            If IsDate(txtDateOfPaymentTo.Text) Then
'                ePayment.DateTo = Date.Parse(txtDateOfPaymentTo.Text)
'            Else
'                imisgen.Alert(imisgen.getMessage("M_INVALIDDATE"), pnlButtons, alertPopupTitle:="Nafa MIS")
'                Return
'            End If
'        End If

'        Dim dtPayment As DataTable = Payment.getPayment(ePayment)

'        Dim StatusColumn As New Data.DataColumn("PaymentStatusName", GetType(System.String))
'        dtPayment.Columns.Add(StatusColumn)
'        For Each row As DataRow In dtPayment.Rows
'            row(StatusColumn) = Payment.GetPayementStatus(If(IsDBNull(row("PaymentStatus")), 0, row("PaymentStatus")))
'        Next

'        gvPayments.DataSource = dtPayment
'        gvPayments.SelectedIndex = -1
'        gvPayments.DataBind()

'    End Sub

'    Protected Sub B_SUBMIT_Click(sender As Object, e As EventArgs) Handles B_SUBMIT.Click
'        If chkStatusPaid.Checked Then
'            ' Iterate through the Products.Rows property
'            For Each row As GridViewRow In gvPayments.Rows
'                ' Access the CheckBox
'                Dim cb As CheckBox = row.FindControl("chkPaymentSetector")
'                If cb IsNot Nothing AndAlso cb.Checked Then
'                    ' First, get the PaymentId for the selected row
'                    Dim PaymentId As Integer = Convert.ToInt32(gvPayments.DataKeys(row.RowIndex).Value)
'                    ePayment.PaymentID = PaymentId
'                    ePayment.PaymentStatus = 1
'                    ePayment.AuditUserID = imisgen.getUserId(Session("User"))
'                    Dim Message As String = Payment.UpdatePaymentStatus(ePayment)
'                    LoadGrid()
'                End If
'            Next
'        End If

'    End Sub
'    Protected Sub gvProducts_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles gvPayments.PageIndexChanging
'        gvPayments.PageIndex = e.NewPageIndex
'    End Sub

'    Protected Sub B_CANCEL_Click(sender As Object, e As EventArgs) Handles B_CANCEL.Click
'        Response.Redirect("Home.aspx")
'    End Sub
'End Class
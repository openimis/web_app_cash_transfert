Public Class PaymentProviderStatusReport
    Inherits System.Web.UI.Page

    Private PaymentProviderStatusBI As New IMIS_BI.PaymentProviderPaidStatusBI
    Protected imisgen As New IMIS_Gen
    Public payment_id As String
    Private reports As New IMIS_BI.ReportsBI
    Private ePayment As New IMIS_EN.tblPayment
    Private Payment As New IMIS_BI.PaymentProviderPaidStatusBI

    'page load event to prepopulate some controls
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Try
            If IsPostBack = True Then Return

            'get user id from session
            Dim UserID As Integer
            UserID = imisgen.getUserId(Session("User"))

            'call the fillregion method to populate ddlRegion control

            FillRegion()
            FillProduct()
            FillYear()
            FillMonth()
        Catch ex As Exception
            imisgen.Alert(imisgen.getMessage("M_ERRORMESSAGE"), pnlButtons, alertPopupTitle:="Nafa MIS")
            'EventLog.WriteEntry("IMIS", Page.Title & " : " & imisgen.getLoginName(Session("User")) & " : " & ex.Message, EventLogEntryType.Error, 999)
            Return
        End Try
    End Sub
    Private Sub FillYear()
        ddlYear.DataSource = Reports.GetYears(2010, Year(Now()) + 5)
        ddlYear.DataValueField = "Year"
        ddlYear.DataTextField = "Year"
        ddlYear.DataBind()
        ddlYear.SelectedValue = Year(Now())
        ddlYear1.DataSource = Reports.GetYears(2010, Year(Now()) + 5)
        ddlYear1.DataValueField = "Year"
        ddlYear1.DataTextField = "Year"
        ddlYear1.DataBind()
        ddlYear1.SelectedValue = Year(Now())

        'ddlYear.Attributes.Add("style", "display:hidden;")
        'lblYear.Attributes.Add("style", "display:hidden;")
    End Sub
    Private Sub FillMonth()

        ddlMonth.DataSource = Reports.GetMonths(1, 12)
        ddlMonth.DataValueField = "MonthNum"
        ddlMonth.DataTextField = "MonthName"
        ddlMonth.DataBind()
        ddlMonth1.DataSource = Reports.GetMonths(1, 12)
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

    Private Sub FillHF(ByVal UserID As Integer)
        'Dim LocationId As Integer = 0
        'If Val(ddlDistrict.SelectedValue) > 0 Then
        '    LocationId = Val(ddlDistrict.SelectedValue)
        'ElseIf Val(ddlRegion.SelectedValue) > 0 Then
        '    LocationId = Val(ddlRegion.SelectedValue)
        'End If
        'ddlHFCode.DataSource = PaymentProviderStatusBI.GetHFCodes(UserID, LocationId)
        'ddlHFCode.DataValueField = "HfID"
        'ddlHFCode.DataTextField = "HFCODE"
        'ddlHFCode.DataBind()
        'If Request.QueryString("c") = "c" Then
        '    If IsPostBack = False Then
        '        ddlHFCode.SelectedValue = CType(Session("HFID"), Integer)
        '        ''''Clear HFID session
        '        Session.Remove("HFID")
        '    End If
        'End If

    End Sub

    Private Sub LoadGrid() Handles btnSearch.Click, gvPayments.PageIndexChanged

        ePayment.AuditUserID = imisgen.getUserId(Session("User"))

        ePayment.RegionId = Val(ddlRegion.SelectedValue)
        ePayment.PaymentStatus = Val(paymentStatus.SelectedValue)



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

    'Protected Sub B_SUBMIT_Click(sender As Object, e As EventArgs) Handles B_SUBMIT.Click
    '    If chkStatusPaid.Checked Then
    '        ' Iterate through the Products.Rows property
    '        For Each row As GridViewRow In gvPayments.Rows
    '            ' Access the CheckBox
    '            Dim cb As CheckBox = row.FindControl("chkPaymentSetector")
    '            If cb IsNot Nothing AndAlso cb.Checked Then
    '                ' First, get the PaymentId for the selected row
    '                Dim PaymentId As Integer = Convert.ToInt32(gvPayments.DataKeys(row.RowIndex).Value)
    '                ePayment.PaymentID = PaymentId
    '                ePayment.PaymentStatus = 1
    '                ePayment.AuditUserID = imisgen.getUserId(Session("User"))
    '                Dim Message As String = Payment.UpdatePaymentStatus(ePayment)
    '                LoadGrid()
    '            End If
    '        Next
    '    End If

    'End Sub
    Protected Sub gvProducts_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles gvPayments.PageIndexChanging
        gvPayments.PageIndex = e.NewPageIndex
    End Sub

    Protected Sub B_CANCEL_Click(sender As Object, e As EventArgs) Handles B_CANCEL.Click
        Response.Redirect("Home.aspx")
    End Sub
End Class
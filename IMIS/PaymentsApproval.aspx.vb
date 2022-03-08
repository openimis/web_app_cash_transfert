Imports System.Diagnostics
Imports ExcelImportExport

<Assembly: DebuggerDisplay("{ToString}", Target:=GetType(Date))>

Public Class PaymentsApproval
    Inherits System.Web.UI.Page

    Private ICD As New IMIS_BI.RegistersBI

    Private FindPaymentProviderPaymentsB As New IMIS_BI.FindPaymentProviderPaymentsBI
    Protected imisgen As New IMIS_Gen
    Private reports As New IMIS_BI.ReportsBI
    Public payment_id As String
    Private ePayment As New IMIS_EN.tblPayment
    Private ePayments As New IMIS_EN.tblPayment
    Private Payment As New IMIS_BI.FindPaymentProviderPaymentsBI

    'page load event to prepopulate some controls
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Try
            If IsPostBack = True Then Return

            If Session("User") Is Nothing Then Response.Redirect("Default.aspx")
            ePayment.AuditUserID = imisgen.getUserId(Session("User"))

            Dim g As String = Request.QueryString("r")

            Dim ff As String = Request.QueryString("s")
            If HttpContext.Current.Request.QueryString("p") IsNot Nothing Then
                ePayment.ProductCode = HttpContext.Current.Request.QueryString("p")
            End If
            If HttpContext.Current.Request.QueryString("s") IsNot Nothing Then
                ePayment.StartMonth = HttpContext.Current.Request.QueryString("s")
            End If
            If HttpContext.Current.Request.QueryString("e") IsNot Nothing Then
                ePayment.EndMonth = HttpContext.Current.Request.QueryString("e")
            End If
            If HttpContext.Current.Request.QueryString("r") IsNot Nothing Then
                ePayment.RegionId = Val(HttpContext.Current.Request.QueryString("r"))
            End If



            Dim dtPayment As DataTable = Payment.GetPaymentForApproval(ePayment)
            Dim StatusColumn As New Data.DataColumn("approvalStatusName", GetType(System.String))
            dtPayment.Columns.Add(StatusColumn)
            For Each row As DataRow In dtPayment.Rows
                row(StatusColumn) = Payment.GetApprovalStatus(If(IsDBNull(row("approvalStatus")), 0, row("approvalStatus")))
            Next
            gvPayments.DataSource = dtPayment
            gvPayments.SelectedIndex = -1
            gvPayments.DataBind()

            'Get Payment Approval Counts
            checkpaymentApprovalStatus(ePayment)
        Catch ex As Exception
            imisgen.Alert(imisgen.getMessage("M_ERRORMESSAGE"), pnlButtons, alertPopupTitle:="Nafa MIS")
            Return
        End Try

    End Sub

    Private Sub checkpaymentApprovalStatus(ePayment As IMIS_EN.tblPayment)
        Payment.GetPaymentApprovalCounts(ePayment)
        Dim approvedCount As String = ePayment.ApprovedCount.ToString()
        Dim totalCount As String = ePayment.TotalCount.ToString()
        txtApprovedCount.Text = approvedCount + " Of " + totalCount + " Approved"
        If (ePayment.ApprovedCount = ePayment.TotalCount) Then
            B_SUBMIT.Visible = False
            chkStatusPaid.Visible = False
            lblSelectToSubmit.Visible = False
            B_EXPORTDRAFT.Visible = False
        Else
            B_SUBMIT.Visible = True
            B_EXPORTDRAFT.Visible = True
        End If
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


    End Sub



    Private Sub FillRegion()
        Dim GenPayment As New IMIS_BI.GenPayBI
        Dim dtRegions As DataTable = GenPayment.GetRegions(imisgen.getUserId(Session("User")), True)

        ddlRegion.DataSource = dtRegions
        ddlRegion.DataValueField = "RegionId"
        ddlRegion.DataTextField = "RegionName"
        ddlRegion.DataBind()

    End Sub



    Private Sub FillProduct()
        ddlproduct.DataSource = FindPaymentProviderPaymentsB.GetProducts(imisgen.getUserId(Session("User")))
        ddlproduct.DataValueField = "ProdId"
        ddlproduct.DataTextField = "ProductCode"
        ddlproduct.DataBind()
    End Sub


    Private Sub LoadGrid() Handles btnSearch.Click, gvPayments.PageIndexChanged
        ePayment.AuditUserID = imisgen.getUserId(Session("User"))

        If HttpContext.Current.Request.QueryString("p") IsNot Nothing Then
            ePayment.ProductCode = HttpContext.Current.Request.QueryString("p")
        End If
        If HttpContext.Current.Request.QueryString("s") IsNot Nothing Then
            ePayment.StartMonth = HttpContext.Current.Request.QueryString("s")
        End If
        If HttpContext.Current.Request.QueryString("e") IsNot Nothing Then
            ePayment.EndMonth = HttpContext.Current.Request.QueryString("e")
        End If
        If HttpContext.Current.Request.QueryString("r") IsNot Nothing Then
            ePayment.RegionId = Val(HttpContext.Current.Request.QueryString("r"))
        End If

        Dim dtPayment As DataTable = Payment.GetPaymentForApproval(ePayment)
        Dim StatusColumn As New Data.DataColumn("approvalStatusName", GetType(System.String))
        dtPayment.Columns.Add(StatusColumn)
        For Each row As DataRow In dtPayment.Rows
            row(StatusColumn) = Payment.GetApprovalStatus(If(IsDBNull(row("approvalStatus")), 0, row("approvalStatus")))
        Next
        gvPayments.DataSource = dtPayment
        gvPayments.SelectedIndex = -1
        gvPayments.DataBind()
        checkpaymentApprovalStatus(ePayment)
        'ePayment.AuditUserID = imisgen.getUserId(Session("User"))
        'ePayment.RegionId = Val(ddlRegion.SelectedValue)

        'ePayment.ProdId = Val(ddlproduct.SelectedValue)
        'ePayment.ProductCode = ddlproduct.SelectedItem.Text
        'ePayment.StartMonth = ddlMonth.SelectedItem.Text
        'ePayment.StartYear = ddlYear.SelectedValue
        'ePayment.EndMonth = ddlMonth1.SelectedItem.Text
        'ePayment.EndYear = ddlYear1.SelectedValue



        'Dim dtPayment As DataTable = Payment.GetPaymentForApproval(ePayment)

        ''Dim StatusColumn As New Data.DataColumn("PaymentStatusName", GetType(System.String))
        ''dtPayment.Columns.Add(StatusColumn)
        ''For Each row As DataRow In dtPayment.Rows
        ''    row(StatusColumn) = Payment.GetPayementStatus(If(IsDBNull(row("PaymentStatus")), 0, row("PaymentStatus")))
        ''Next

        'Dim StatusColumn As New Data.DataColumn("approvalStatusName", GetType(System.String))
        'dtPayment.Columns.Add(StatusColumn)
        'For Each row As DataRow In dtPayment.Rows
        '    row(StatusColumn) = Payment.GetApprovalStatus(If(IsDBNull(row("approvalStatus")), 0, row("approvalStatus")))
        'Next

        'gvPayments.DataSource = dtPayment
        'gvPayments.SelectedIndex = -1
        'gvPayments.DataBind()

    End Sub

    Private Sub CchkDisplayExceptional_CheckedChanged(sender As Object, e As EventArgs) Handles chkDisplayExceptional.CheckedChanged
        Try
            ePayment.AuditUserID = imisgen.getUserId(Session("User"))

            If HttpContext.Current.Request.QueryString("p") IsNot Nothing Then
                ePayment.ProductCode = HttpContext.Current.Request.QueryString("p")
            End If
            If HttpContext.Current.Request.QueryString("s") IsNot Nothing Then
                ePayment.StartMonth = HttpContext.Current.Request.QueryString("s")
            End If
            If HttpContext.Current.Request.QueryString("e") IsNot Nothing Then
                ePayment.EndMonth = HttpContext.Current.Request.QueryString("e")
            End If
            If HttpContext.Current.Request.QueryString("r") IsNot Nothing Then
                ePayment.RegionId = Val(HttpContext.Current.Request.QueryString("r"))
            End If

            Dim isExceptional As Boolean = False
            If chkDisplayExceptional.Checked Then
                isExceptional = True
            End If

            Dim dtPayment As DataTable = Payment.GetPaymentForApproval(ePayment, isExceptional)
            Dim StatusColumn As New Data.DataColumn("approvalStatusName", GetType(System.String))
            dtPayment.Columns.Add(StatusColumn)
            For Each row As DataRow In dtPayment.Rows
                row(StatusColumn) = Payment.GetApprovalStatus(If(IsDBNull(row("approvalStatus")), 0, row("approvalStatus")))
            Next
            gvPayments.DataSource = dtPayment
            gvPayments.SelectedIndex = -1
            gvPayments.DataBind()

            'Get Payment Approval Counts
            checkpaymentApprovalStatus(ePayment)
        Catch ex As Exception
            imisgen.Alert(imisgen.getMessage("M_ERRORMESSAGE"), pnlButtons, alertPopupTitle:="Nafa MIS")
            Return
        End Try
    End Sub
    Protected Sub B_SUBMIT_Click(sender As Object, e As EventArgs) Handles B_SUBMIT.Click
        If chkStatusPaid.Checked Then
            gvPayments.AllowPaging = False
            LoadGrid()
            For Each row As GridViewRow In gvPayments.Rows
                ' First, get the PaymentId for the selected row
                Dim PaymentId As Integer = Convert.ToInt32(gvPayments.DataKeys(row.RowIndex).Value)
                Dim b As Int64 = gvPayments.Rows.Count
                'generate unique payment id
                Dim dateUniqieCnv As String
                dateUniqieCnv = DateTime.Now.ToString("ddmmyyhhmmss")

                Dim transactionnumber = HttpContext.Current.Request.QueryString("p") + "" + dateUniqieCnv.ToString() + "" + PaymentId.ToString()
                ePayment.PaymentID = PaymentId
                ePayment.ApprovalStatus = 1
                ePayment.TransactionNumber = transactionnumber
                ePayment.AuditUserID = imisgen.getUserId(Session("User"))
                Dim Message As String = Payment.UpdatePaymentApprovalStatus(ePayment)
            Next
            gvPayments.AllowPaging = True
            LoadGrid()
        Else
            ' Iterate through the Products.Rows property
            For Each row As GridViewRow In gvPayments.Rows
                ' Access the CheckBox

                Dim cb As CheckBox = row.FindControl("chkPaymentSetector")
                If cb IsNot Nothing AndAlso cb.Checked Then
                    ' First, get the PaymentId for the selected row
                    Dim PaymentId As Integer = Convert.ToInt32(gvPayments.DataKeys(row.RowIndex).Value)

                    Dim dateUniqieCnv As String
                    dateUniqieCnv = DateTime.Now.ToString("ddmmyyhhmmss")

                    Dim dateUniqieCnv1 As String
                    dateUniqieCnv1 = DateTime.Now.ToString("MMddmmssff")
                    Dim transactionnumber = HttpContext.Current.Request.QueryString("p") + "" + dateUniqieCnv.ToString() + "" + PaymentId.ToString()

                    ePayment.PaymentID = PaymentId
                    ePayment.ApprovalStatus = 1
                    ePayment.TransactionNumber = transactionnumber
                    ePayment.AuditUserID = imisgen.getUserId(Session("User"))
                    Dim Message As String = Payment.UpdatePaymentApprovalStatus(ePayment)
                End If
            Next
        End If

        LoadGrid()

    End Sub
    Protected Sub gvProducts_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles gvPayments.PageIndexChanging
        gvPayments.PageIndex = e.NewPageIndex
    End Sub

    Protected Sub B_CANCEL_Click(sender As Object, e As EventArgs) Handles B_CANCEL.Click
        Response.Redirect("Home.aspx")
    End Sub

    Private Sub DownloadFile(path As String, contentType As String)
        Dim strCommand As String = ""
        Dim file As System.IO.FileInfo = New System.IO.FileInfo(path)
        If file.Exists Then
            strCommand = "attachment;filename=" & System.IO.Path.GetFileName(path)
            Response.AppendHeader("Content-Disposition", strCommand)
            Response.ContentType = contentType
            Response.TransmitFile(path)
            Response.End()
            Response.Flush()
        End If
    End Sub

    Protected Sub B_EXPORTFINAL_Click(sender As Object, e As EventArgs) Handles B_EXPORTFINAL.Click
        Dim Message As String = ""

        If gvPayments.Rows.Count > 0 Then
            If HttpContext.Current.Request.QueryString("p") IsNot Nothing Then
                ePayment.ProductCode = HttpContext.Current.Request.QueryString("p")
            End If
            If HttpContext.Current.Request.QueryString("s") IsNot Nothing Then
                ePayment.StartMonth = HttpContext.Current.Request.QueryString("s")
            End If
            If HttpContext.Current.Request.QueryString("e") IsNot Nothing Then
                ePayment.EndMonth = HttpContext.Current.Request.QueryString("e")
            End If
            If HttpContext.Current.Request.QueryString("r") IsNot Nothing Then
                ePayment.RegionId = Val(HttpContext.Current.Request.QueryString("r"))
            End If
            If HttpContext.Current.Request.QueryString("pr") IsNot Nothing Then
                ePayment.PayrollCycleID = Val(HttpContext.Current.Request.QueryString("pr"))
            End If
            Response.Redirect("DownloadPayments.aspx?RegionId=" & ePayment.RegionId.ToString() + "&ProdId=" & ePayment.RegionId.ToString() +
           "&ProductCode=" & ePayment.ProductCode + "&StartMonth=" & ePayment.StartMonth +
           "&StartYear=" & ePayment.StartMonth + "&EndMonth=" & ePayment.EndMonth +
           "&EndYear=" & ePayment.EndMonth + "&PayrollCycleId=" & ePayment.PayrollCycleID.ToString() + "&Export=Final")
        Else
            Message = "Nothing To Export"
            imisgen.Alert(Message, pnlBody, alertPopupTitle:="Nafa MIS")
        End If

    End Sub

    Protected Sub B_EXPORTDRAFT_Click(sender As Object, e As EventArgs) Handles B_EXPORTDRAFT.Click

        Dim Message As String = ""

        If gvPayments.Rows.Count > 0 Then
            If HttpContext.Current.Request.QueryString("p") IsNot Nothing Then
                ePayment.ProductCode = HttpContext.Current.Request.QueryString("p")
            End If
            If HttpContext.Current.Request.QueryString("s") IsNot Nothing Then
                ePayment.StartMonth = HttpContext.Current.Request.QueryString("s")
            End If
            If HttpContext.Current.Request.QueryString("e") IsNot Nothing Then
                ePayment.EndMonth = HttpContext.Current.Request.QueryString("e")
            End If
            If HttpContext.Current.Request.QueryString("r") IsNot Nothing Then
                ePayment.RegionId = Val(HttpContext.Current.Request.QueryString("r"))
            End If

            If HttpContext.Current.Request.QueryString("pr") IsNot Nothing Then
                ePayment.PayrollCycleID = Val(HttpContext.Current.Request.QueryString("pr"))
            End If

            Response.Redirect("DownloadPayments.aspx?RegionId=" & ePayment.RegionId.ToString() + "&ProdId=" & ePayment.RegionId.ToString() +
          "&ProductCode=" & ePayment.ProductCode + "&StartMonth=" & ePayment.StartMonth +
          "&StartYear=" & ePayment.StartMonth + "&EndMonth=" & ePayment.EndMonth +
          "&EndYear=" & ePayment.EndMonth + "&PayrollCycleId=" & ePayment.PayrollCycleID.ToString() + "&Export=Draft")
        Else
            Message = "Nothing To Export"
            imisgen.Alert(Message, pnlBody, alertPopupTitle:="Nafa MIS")
        End If

    End Sub

End Class

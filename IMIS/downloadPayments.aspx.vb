Imports ExcelImportExport
Imports DocumentFormat.OpenXml

Public Class DownloadPayments
    Inherits System.Web.UI.Page

    Private ePayment As New IMIS_EN.tblPayment
    Private Payment As New IMIS_BI.FindPaymentProviderPaymentsBI
    Private InsureeBI As New IMIS_BI.InsureeBI
    Protected imisgen As New IMIS_Gen

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        ePayment.AuditUserID = imisgen.getUserId(Session("User"))

        ePayment.RegionId = Server.UrlDecode(Request.QueryString("RegionId"))
        Dim PayrollCycleIdtxt As String = Server.UrlDecode(Request.QueryString("PayrollCycleId"))
        Dim PayrollCycleId As Integer
        If Integer.TryParse(PayrollCycleIdtxt, PayrollCycleId) Then
            ePayment.PayrollCycleID = PayrollCycleId
        End If

        ePayment.ProductCode = Server.UrlDecode(Request.QueryString("ProductCode"))
        ePayment.StartMonth = Server.UrlDecode(Request.QueryString("StartMonth"))
        ePayment.EndMonth = Server.UrlDecode(Request.QueryString("EndMonth"))

        Dim Export As String = Request.QueryString("Export")

        Dim referer = Request.UrlReferrer.ToString()

        Dim dtPayment As DataTable = Payment.GetPaymentToExport(ePayment, Export)

        Dim lastCell As Integer = dtPayment.Columns.Count

        dtPayment.Columns.Add("Voucher Number", Type.GetType("System.String"))

        dtPayment.Columns.Add("Payment Status", Type.GetType("System.String"))

        dtPayment.Columns.Add("Reasons for not Paid", Type.GetType("System.String"))

        dtPayment.Columns.Add("Received Date", Type.GetType("System.String"))

        'dtPayment.Columns.Add("Procurator", Type.GetType("System.String"))

        'dtPayment.Columns.Add("Procurator Phone Number", Type.GetType("System.String"))

        'dtPayment.Columns.Add("Identification Type", Type.GetType("System.String"))

        'dtPayment.Columns.Add("Identification Number", Type.GetType("System.String"))

        dtPayment.Columns.Add("PayPoint Code", Type.GetType("System.String"))

        Dim opt As ValidOptions = New ValidOptions()

        opt.ColumnName = "Reasons for not Paid"
        opt.validOptions = New List(Of String) From {"Absent", "Declined payment", "Beneficiary not identified by the community", "Alternate payee not listed", "Others"}

        Dim opt1 As ValidOptions = New ValidOptions()
        opt1.ColumnName = "Payment Status"
        opt1.validOptions = New List(Of String) From {"Not Paid", "Paid"}

        'Dim dtTypeOfIdentity = InsureeBI.GetTypeOfIdentity()
        'Dim identityTypeList = New List(Of String)

        'For i = 0 To (dtTypeOfIdentity.Rows.Count - 1)
        '    identityTypeList.Add(dtTypeOfIdentity.Rows(i).Item("IdentificationTypes"))
        'Next

        'Dim opt2 As ValidOptions = New ValidOptions()
        'opt2.ColumnName = "Identification Type"
        'opt2.validOptions = identityTypeList

        Dim validOptions As New List(Of ValidOptions)
        validOptions.Add(opt)
        validOptions.Add(opt1)
        'validOptions.Add(opt2)

        Dim filename As String

        If Export = "Draft" Then
            filename = "ApprovedPaymentsDraft"
            ExcelData.ExportToExcel(dtPayment, filename, False, 0, 0, validOptions)
        Else
            filename = "ApprovedPayments"
            ExcelData.ExportToExcel(dtPayment, filename, True, lastCell + 1, dtPayment.Columns.Count, validOptions)
        End If

        Response.Redirect(referer)

    End Sub

End Class
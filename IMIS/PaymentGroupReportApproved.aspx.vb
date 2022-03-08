Imports System.IO

Public Class PaymentGroupReportApproved
    Inherits System.Web.UI.Page

    Private PaymentProviderStatusBI As New IMIS_BI.PaymentProviderPaidStatusBI
    Protected imisgen As New IMIS_Gen
    Public payment_id As String
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
            If Request.QueryString("c") = "c" Then
                ddlRegion.SelectedValue = CType(Session("RegionSelected"), Integer)

            End If

            'fill HFCode


            FillProduct()

            Dim dtPayment As DataTable = Payment.getPaymentGroupReport(ePayment, 2)
            gvPayments.DataSource = dtPayment
            gvPayments.SelectedIndex = -1
            gvPayments.DataBind()

        Catch ex As Exception
            imisgen.Alert(imisgen.getMessage("M_ERRORMESSAGE"), pnlButtons, alertPopupTitle:="Nafa MIS")
            'EventLog.WriteEntry("IMIS", Page.Title & " : " & imisgen.getLoginName(Session("User")) & " : " & ex.Message, EventLogEntryType.Error, 999)
            Return
        End Try
    End Sub

    Private Sub ddlRegion_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlRegion.SelectedIndexChanged
        Session("RegionSelected") = ddlRegion.SelectedValue.ToString


    End Sub



    Private Sub FillRegion()
        Dim dtRegions As DataTable = PaymentProviderStatusBI.GetRegions(imisgen.getUserId(Session("User")), True)
        ddlRegion.DataSource = dtRegions
        ddlRegion.DataValueField = "RegionId"
        ddlRegion.DataTextField = "RegionName"
        ddlRegion.DataBind()

    End Sub



    Private Sub FillProduct()
        ddlproduct.DataSource = PaymentProviderStatusBI.GetProducts(imisgen.getUserId(Session("User")))
        ddlproduct.DataValueField = "ProdId"
        ddlproduct.DataTextField = "ProductCode"
        ddlproduct.DataBind()
    End Sub


    Private Sub LoadGrid() Handles gvPayments.PageIndexChanged

        Dim dtPayment As DataTable = Payment.getPaymentGroupReport(ePayment, 2)
        gvPayments.DataSource = dtPayment
        gvPayments.SelectedIndex = -1
        gvPayments.DataBind()

    End Sub


    Protected Sub gvProducts_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles gvPayments.PageIndexChanging
        gvPayments.PageIndex = e.NewPageIndex
    End Sub

    Protected Sub B_CANCEL_Click(sender As Object, e As EventArgs) Handles B_CANCEL.Click
        Response.Redirect("Home.aspx")
    End Sub


    Public Overrides Sub VerifyRenderingInServerForm(control As Control)
        ' Verifies that the control is rendered 

    End Sub
End Class
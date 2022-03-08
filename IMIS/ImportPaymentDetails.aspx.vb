Imports ExcelImportExport

Public Class ImportPaymentDetails
    Inherits System.Web.UI.Page
    Protected imisgen As New IMIS_Gen
    Private ePayment As New IMIS_EN.tblPayment
    Private ImportPayments As New IMIS_BI.ImportPaymentsBI
    Private ProviderPaymentsBI As New IMIS_BI.FindPaymentProviderPaymentsBI
    Private reports As New IMIS_BI.ReportsBI
    Private GenPayment As New IMIS_BI.GenPayBI

    Dim lMaxFileSize As Long = 10000000

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            FillCombo()
            FillPayrollCycleDetails()
        End If
    End Sub

    Protected Sub btnUploadPayments_Click(sender As Object, e As EventArgs) Handles btnUploadPayments.Click
        Dim Message As String = ""
        Dim HasError As Boolean = False
        Dim ReturnVal As Integer
        Dim ReturnData As DataTable

        'check that the file has been selected and it's a valid file
        If (Not FileUploadPayments.PostedFile Is Nothing) Then
            If (FileUploadPayments.PostedFile.ContentLength > 0) Then
                'determine file name
                Dim sFileName As String =
               System.IO.Path.GetFileName(FileUploadPayments.PostedFile.FileName)
                Try
                    If FileUploadPayments.PostedFile.ContentLength <= lMaxFileSize Then

                        Dim filebase As New HttpPostedFileWrapper(FileUploadPayments.PostedFile)

                        Dim ExpectedColumnList = New List(Of String)

                        If chkisExceptional.Checked Then
                            ExpectedColumnList = New List(Of String) From {"TempID", "Beneficiary Name", "Procurator Name", "Procurator Phone Number", "Identification Type", "Identification Number", "Received Date", "PayPoint Code"}
                        Else
                            ExpectedColumnList = New List(Of String) From {"Social Registry No", "Beneficiary Name", "Telephone Number", "Region", "District", "Ward", "Program", "Settlement", "Amount", "Transaction Number", "Voucher Number", "Payment Status", "Reasons for not Paid", "Received Date", "PayPoint Code"}
                        End If

                        Dim imp As New ImportExport
                        Dim data As DataTable
                        data = imp.ImportExcel(filebase)

                        For Each column In data.Columns

                            If Not ExpectedColumnList.Contains(column.ToString) Then
                                Message = "Column Name " + column.ToString + " does not exist"
                                HasError = True
                                Exit For
                            End If
                        Next

                        If Not HasError Then
                            For Each item In ExpectedColumnList

                                If Not data.Columns.Contains(item) Then
                                    Message = "Column Name " + item + " is missing"
                                    HasError = True
                                    Exit For
                                End If
                            Next

                        End If

                        If Not HasError Then

                            If chkisExceptional.Checked Then
                                data.Columns.Add("Payment Status", Type.GetType("System.String"))
                                data.Columns.Add("IdentificationTypeCode", Type.GetType("System.String"))
                            Else
                                data.Columns.Add("RegionId", Type.GetType("System.String"))

                                data.Columns.Add("VillageId", Type.GetType("System.String"))
                            End If

                            data.Columns.Add("PaymentStatusCode", Type.GetType("System.String"))
                            data.Columns.Add("PayPointId", Type.GetType("System.String"))

                            Dim i As Integer
                            For i = 0 To (data.Rows.Count - 1)

                                Dim PaymentStatusCode = 0

                                If chkisExceptional.Checked Then
                                    data.Rows(i).Item("Payment Status") = "Paid"
                                Else

                                    If Not String.IsNullOrEmpty(data.Rows(i).Item("Region").ToString()) Then
                                        Dim dtRegionId = ImportPayments.GETLocationID("RegionId", data.Rows(i).Item("Region"))
                                        Dim RegionId As String = dtRegionId.Rows(0).Item("RegionId")


                                        If Not String.IsNullOrEmpty(RegionId) Then
                                            data.Rows(i).Item("RegionId") = RegionId
                                        Else
                                            Message = "Region " + data.Rows(i).Item("Region") + " does not exist. Row " + (i + 2).ToString
                                            HasError = True
                                            Exit For
                                        End If
                                    Else
                                        Message = "Region Is Empty On Row " + (i + 2).ToString
                                        HasError = True
                                        Exit For
                                    End If

                                    If Not String.IsNullOrEmpty(data.Rows(i).Item("Settlement").ToString()) Then
                                        Dim dtVillageId = ImportPayments.GETLocationID("SettlementId", data.Rows(i).Item("Region"), data.Rows(i).Item("Settlement"), data.Rows(i).Item("Ward"), data.Rows(i).Item("District"))

                                        If dtVillageId.Rows.Count > 0 Then
                                            Dim VillageId As String = dtVillageId.Rows(0).Item("VillageId")


                                            If Not String.IsNullOrEmpty(VillageId) Then
                                                data.Rows(i).Item("VillageId") = VillageId
                                            Else
                                                Message = "Settlement " + data.Rows(i).Item("Settlement") + " in Region " + data.Rows(i).Item("Region") +
                                        ", district " + data.Rows(i).Item("District") + ", ward " + data.Rows(i).Item("Ward") + " does not exist. Row " + (i + 2).ToString
                                                HasError = True
                                                Exit For
                                            End If
                                        Else
                                            Message = "Settlement " + data.Rows(i).Item("Settlement") + " in Region " + data.Rows(i).Item("Region") +
                                        ", district" + data.Rows(i).Item("District") + ", ward" + data.Rows(i).Item("Ward") + " does not exist. Row " + (i + 2).ToString
                                            HasError = True
                                            Exit For
                                        End If
                                    Else
                                        Message = "Region Is Empty On Row " + (i + 2).ToString
                                        HasError = True
                                        Exit For
                                    End If

                                    If Not String.IsNullOrEmpty(data.Rows(i).Item("Program").ToString()) Then

                                        Dim dtProgramCode = ImportPayments.GETProgramCode(data.Rows(i).Item("Program"))

                                        If dtProgramCode.Rows.Count > 0 Then
                                            Dim ProgramCode As String = dtProgramCode.Rows(0).Item("ProgramCode")
                                            If Not String.IsNullOrEmpty(ProgramCode) Then
                                                data.Rows(i).Item("Program Code") = ProgramCode
                                            Else
                                                Message = "Program " + data.Rows(i).Item("Program") + " does not exist. Row " + (i + 2).ToString
                                                HasError = True
                                                Exit For
                                            End If
                                        Else
                                            Message = "Program " + data.Rows(i).Item("Program") + " does not exist. Row " + (i + 2).ToString
                                            HasError = True
                                            Exit For
                                        End If
                                    Else
                                        Message = "Program Is Empty On Row " + (i + 2).ToString
                                        HasError = True
                                        Exit For
                                    End If

                                End If


                                If Not String.IsNullOrEmpty(data.Rows(i).Item("Payment Status").ToString()) Then

                                    PaymentStatusCode = ImportPayments.GettPaymentStatusCode(data.Rows(i).Item("Payment Status"))

                                    If Not PaymentStatusCode = -1 Then
                                        data.Rows(i).Item("PaymentStatusCode") = PaymentStatusCode
                                    Else
                                        Message = "Invalid Payment Status" + data.Rows(i).Item("Payment Status") + "On Row" + (i + 2).ToString
                                        HasError = True
                                        Exit For
                                    End If
                                Else
                                    Message = "Payment Status Is Empty On Row " + (i + 2).ToString
                                    HasError = True
                                    Exit For
                                End If

                                If Not String.IsNullOrEmpty(data.Rows(i).Item("PayPoint Code").ToString()) Then
                                    Dim dtPayPointId = ImportPayments.GETPayPointId(data.Rows(i).Item("PayPoint Code"))

                                    If dtPayPointId.Rows.Count > 0 Then
                                        Dim PayPointId As String = dtPayPointId.Rows(0).Item("PayPointId")
                                        If Not String.IsNullOrEmpty(PayPointId) Then
                                            data.Rows(i).Item("PayPointId") = PayPointId
                                        Else
                                            Message = "PayPoint Code " + data.Rows(i).Item("PayPoint Code") + " does not exist. Row " + (i + 2).ToString
                                            HasError = True
                                            Exit For
                                        End If
                                    Else
                                        Message = "PayPoint Code " + data.Rows(i).Item("PayPoint Code") + " does not exist. Row " + (i + 2).ToString
                                        HasError = True
                                        Exit For
                                    End If
                                End If


                                If PaymentStatusCode = 1 Then

                                    If String.IsNullOrEmpty(data.Rows(i).Item("Received Date").ToString()) Then
                                        Message = "Received Date Is Empty On Row " + (i + 2).ToString
                                        HasError = True
                                        Exit For
                                    End If

                                    If chkisExceptional.Checked Then

                                        If Not String.IsNullOrEmpty(data.Rows(i).Item("Identification Type").ToString()) Then
                                            Dim dtIdentificationTypeCode = ImportPayments.GETIdentificationTypeCode(data.Rows(i).Item("Identification Type"))
                                            If dtIdentificationTypeCode.Rows.Count > 0 Then
                                                Dim IdentificationTypeCode As String = dtIdentificationTypeCode.Rows(0).Item("IdentificationCode")

                                                If Not String.IsNullOrEmpty(IdentificationTypeCode) Then
                                                    data.Rows(i).Item("IdentificationTypeCode") = IdentificationTypeCode
                                                Else
                                                    Message = "Identification Type " + data.Rows(i).Item("Identification Type") + " does not exist. Row " + (i + 2).ToString
                                                    HasError = True
                                                    Exit For
                                                End If
                                            Else
                                                Message = "Identification Type " + data.Rows(i).Item("Identification Type") + " does not exist. Row " + (i + 2).ToString
                                                HasError = True
                                                Exit For
                                            End If
                                        Else
                                            Message = "Identification Type Is Empty On Row " + (i + 2).ToString
                                            HasError = True
                                            Exit For
                                        End If

                                        If String.IsNullOrEmpty(data.Rows(i).Item("Procurator Phone Number").ToString()) Then
                                            Message = "Procurator Phone Numbe Is Empty On Row " + (i + 2).ToString
                                            HasError = True
                                            Exit For
                                        End If

                                        If String.IsNullOrEmpty(data.Rows(i).Item("TempID").ToString()) Then
                                            Message = "TempID Is Empty On Row " + (i + 2).ToString
                                            HasError = True
                                            Exit For
                                        End If

                                    End If

                                End If

                            Next

                            If Not HasError Then

                                If Val(ddlProgram.SelectedValue) > 0 Then
                                    ePayment.ProdId = Val(ddlProgram.SelectedValue)
                                End If

                                If Val(ddlPayrollCycle.SelectedValue) > 0 Then
                                    ePayment.PayrollCycleID = Val(ddlPayrollCycle.SelectedValue)
                                End If

                                ePayment.StartMonth = ddlMonth.SelectedItem.Text
                                ePayment.StartYear = ddlYear.SelectedValue
                                ePayment.EndMonth = ddlMonth1.SelectedItem.Text
                                ePayment.EndYear = ddlYear1.SelectedValue


                                ReturnData = ImportPayments.ImportPaymentsData(data, ePayment, chkisExceptional.Checked)
                                ReturnVal = ReturnData.Rows(0).Item("ReturnVal")
                                Dim Row = ReturnData.Rows(0).Item("RowNo")

                                Select Case ReturnVal
                                    Case 0
                                        Message = imisgen.getMessage("M_PAYMENTSUPLOADED")
                                    Case 1
                                        Message = "Missing Transaction Number. Row " + Row.ToString
                                    Case 2
                                        Message = "Cannot Update record that has already been payed. Row " + Row.ToString
                                    Case 3
                                        Message = "Missing Date. Row " + Row.ToString
                                    Case 4
                                        Message = "Missing Amount. Row " + Row.ToString
                                    Case 5
                                        Message = "Missing Payment Status. Row " + Row.ToString
                                    Case 6
                                        Message = "Missing Program Code. Row " + Row.ToString
                                    Case 7
                                        Message = "Wrong Program Code. Row " + Row.ToString
                                    Case 8
                                        Message = "Wrong Region. Row " + Row.ToString
                                    Case 9
                                        Message = "Missing Settlement. Row " + Row.ToString
                                    Case 10
                                        Message = "Wrong VillageId. Row " + Row.ToString
                                    Case 11
                                        Message = "Wrong identification type. Row " + Row.ToString
                                    Case 12
                                        Message = "Transaction No does not exist. Row " + Row.ToString
                                    Case 13
                                        Message = "Missing SR Number. Row " + Row.ToString
                                    Case 14
                                        Message = "Reasons For Not Paid when Payment Status is Not Paid. Row " + Row.ToString
                                    Case 15
                                        Message = "Missing Identification Number. Row " + Row.ToString
                                    Case 16
                                        Message = "Missing Or Invalid PayPoint Code. Row " + Row.ToString
                                    Case 17
                                        Message = "Missing Or Invalid Voucher Number. Row " + Row.ToString
                                    Case 18
                                        Message = "Voucher Number Miss Match. Row " + Row.ToString
                                    Case 19
                                        Message = "Payroll Cycle does not exist "
                                    Case 20
                                        Message = "TransactionNo does not exist in Payroll Cycle. Row " + Row.ToString
                                    Case 21
                                        Message = "Incorrect date format for column Received Date. Row " + Row.ToString
                                    Case 22
                                        Message = "Incorrect Payment Status. Row " + Row.ToString
                                    Case 23
                                        Message = "Incorrect date format for column Amount. Row " + Row.ToString
                                    Case 24
                                        Message = "Region is Incorrect. Row " + Row.ToString
                                    Case 25
                                        Message = "Settlement is Incorrect. Row " + Row.ToString
                                    Case 26
                                        Message = "Incorrect date format for Paypoint Id. Row " + Row.ToString
                                    Case 27
                                        Message = "Missing Receiver Phone Number. Row " + Row.ToString
                                    Case Else
                                        Message = "An Error Occured. Please Check Your File And Try Again!"
                                End Select
                            End If
                        End If

                    Else
                        Message = "File Size if Over the Limit of " + lMaxFileSize.ToString
                    End If
                Catch exc As Exception
                    Message = "An Error Occured. Please Try Again!"

                    'delete file
                End Try
            Else
                Message = "Nothing to upload. Please Try Again!"
            End If


        Else
            Message = "Nothing to upload. Please Try Again!"
        End If

        imisgen.Alert(Message, Panel1, alertPopupTitle:="Nafa MIS")
    End Sub

    Private Sub FillCombo()

        ddlProgram.DataSource = ProviderPaymentsBI.GetProducts(imisgen.getUserId(Session("User")), True)
        ddlProgram.DataValueField = "ProdId"
        ddlProgram.DataTextField = "ProductName"
        ddlProgram.DataBind()

        ddlMonth.DataSource = Reports.GetMonths(1, 12)
        ddlMonth.DataValueField = "MonthNum"
        ddlMonth.DataTextField = "MonthName"
        ddlMonth.DataBind()

        ddlMonth1.DataSource = Reports.GetMonths(1, 12)
        ddlMonth1.DataValueField = "MonthNum"
        ddlMonth1.DataTextField = "MonthName"
        ddlMonth1.DataBind()

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

        ddlPayrollCycle.DataSource = GenPayment.GetPayrollCycle()
        ddlPayrollCycle.DataValueField = "PayrollCycleID"
        ddlPayrollCycle.DataTextField = "name"
        ddlPayrollCycle.DataBind()


    End Sub

    Private Sub ddlPayrollCycle_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlPayrollCycle.SelectedIndexChanged

        FillPayrollCycleDetails()

    End Sub

    Private Sub FillPayrollCycleDetails()

        ePayment.PayrollCycleID = Val(ddlPayrollCycle.SelectedValue)
        GenPayment.GetPayrollCycleDetails(ePayment)
        ddlMonth.SelectedValue = getMonthID(ePayment.StartMonth)
        ddlYear.SelectedValue = ePayment.EndYear
        ddlMonth1.SelectedValue = getMonthID(ePayment.EndMonth)
        ddlYear1.SelectedValue = ePayment.EndYear
        ddlProgram.SelectedValue = ePayment.ProdId

    End Sub

    Function getMonthID(monthName As String) As String
        Dim rtnValue As String = ""
        Select Case monthName
            Case "January"
                rtnValue = "1"
            Case "February"
                rtnValue = "2"
            Case "March"
                rtnValue = "3"
            Case "April"
                rtnValue = "4"
            Case "May"
                rtnValue = "5"
            Case "June"
                rtnValue = "6"
            Case "July"
                rtnValue = "7"
            Case "August"
                rtnValue = "8"
            Case "September"
                rtnValue = "9"
            Case "October"
                rtnValue = "10"
            Case "November"
                rtnValue = "11"
            Case "December"
                rtnValue = "12"
        End Select
        Return rtnValue
    End Function

End Class
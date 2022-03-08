Public Class ImportPaymentsDL

    Public Function ImportPaymentsData(ByVal dtPayments As DataTable, ByVal ePayment As IMIS_EN.tblPayment, ByVal isExceptional As Boolean) As DataTable
        Dim data As New ExactSQL
        Dim sSQL As String = ""
        If isExceptional Then
            sSQL = "uspAPIUpdateExcepCasePaymentDetails"
        Else
            sSQL = "uspAPIUpdatePaymentDetails"
        End If
        Dim ReturnValue As String
        Dim CaseValue As Integer
        Dim con As System.Data.SqlClient.SqlConnection = New SqlClient.SqlConnection
        Dim transcation As System.Data.SqlClient.SqlTransaction

        Dim ReturnVal As Integer
        Dim RowNo As Integer

        Dim ReturnData As DataTable = New DataTable
        ReturnData.Columns.Add("ReturnVal", Type.GetType("System.String"))
        ReturnData.Columns.Add("HasErrors", Type.GetType("System.String"))
        ReturnData.Columns.Add("RowNo", Type.GetType("System.String"))

        Dim Row As DataRow = ReturnData.NewRow

        Dim StartDate As String = ePayment.StartMonth + " " + ePayment.StartYear
        Dim EndDate As String = ePayment.EndMonth + " " + ePayment.EndYear
        Dim ProdId As Integer = ePayment.ProdId
        Dim PayrollCycleID As Integer = ePayment.PayrollCycleID

        Try

            Dim i As Integer
            For i = 0 To (dtPayments.Rows.Count - 1)
                RowNo = i + 2

                data.setSQLCommand(sSQL, CommandType.StoredProcedure)

                data.params("@ProdId", SqlDbType.Int, ProdId)
                data.params("@StartDate", SqlDbType.NVarChar, 50, StartDate)
                data.params("@EndDate", SqlDbType.NVarChar, 50, EndDate)
                data.params("@PayrollCycleID", SqlDbType.Int, PayrollCycleID)

                If isExceptional Then
                    data.params("@TempID", SqlDbType.NVarChar, dtPayments.Rows(i).Item("TempID"))
                    data.params("@ReceiverPhoneNumber", SqlDbType.NVarChar, dtPayments.Rows(i).Item("Procurator Phone Number"))
                    data.params("@IdentificationType", SqlDbType.NVarChar, dtPayments.Rows(i).Item("IdentificationTypeCode"))
                    If dtPayments.Rows(i).Item("Identification Number") IsNot Nothing Then
                        data.params("@IdentificationNumber", SqlDbType.NVarChar, dtPayments.Rows(i).Item("Identification Number"))
                    End If
                    If dtPayments.Rows(i).Item("Procurator Name") IsNot Nothing Then
                        data.params("@ReceiverName", SqlDbType.NVarChar, dtPayments.Rows(i).Item("Procurator Name"))
                    End If
                Else
                    data.params("@TransactionNo", SqlDbType.NVarChar, dtPayments.Rows(i).Item("Transaction Number"))
                    data.params("@ProgramCode", SqlDbType.NVarChar, dtPayments.Rows(i).Item("Program Code"))
                    If dtPayments.Rows(i).Item("Voucher Number") IsNot Nothing Then
                        data.params("@VoucherNumber", SqlDbType.NVarChar, dtPayments.Rows(i).Item("Voucher Number"))
                    End If
                    data.params("@InsuranceNumber", SqlDbType.NVarChar, dtPayments.Rows(i).Item("Social Registry No"))
                    If dtPayments.Rows(i).Item("Reasons for not Paid") IsNot Nothing Then
                        data.params("@ReasonsForNotPaid", SqlDbType.NVarChar, dtPayments.Rows(i).Item("Reasons for not Paid"))
                    End If

                    Dim Amounttxt As String = dtPayments.Rows(i).Item("Amount")
                    Dim Amount As Decimal
                    If Decimal.TryParse(Amounttxt, Amount) Then
                        data.params("@Amount", SqlDbType.Decimal, Amount)
                    Else
                        ReturnVal = 23
                        If Not transcation Is Nothing Then
                            data.RollbackSQLTransaction(transcation, con)
                        End If
                        Exit For
                    End If

                    Dim RegionIdtxt As String = dtPayments.Rows(i).Item("RegionId")
                    Dim RegionId As Integer
                    If Integer.TryParse(RegionIdtxt, RegionId) Then
                        data.params("@RegionId", SqlDbType.Int, RegionId)
                    Else
                        ReturnVal = 24
                        If Not transcation Is Nothing Then
                            data.RollbackSQLTransaction(transcation, con)
                        End If
                        Exit For
                    End If

                    Dim VillageIdtxt As String = dtPayments.Rows(i).Item("VillageId")
                    Dim VillageId As Integer
                    If Integer.TryParse(VillageIdtxt, VillageId) Then
                        data.params("@VillageId", SqlDbType.Int, VillageId)
                    Else
                        ReturnVal = 25
                        If Not transcation Is Nothing Then
                            data.RollbackSQLTransaction(transcation, con)
                        End If
                        Exit For
                    End If

                End If

                Dim PaymentStatustxt As String = dtPayments.Rows(i).Item("PaymentStatusCode")
                Dim PaymentStatus As Integer
                If Integer.TryParse(PaymentStatustxt, PaymentStatus) Then
                    data.params("@PaymentStatus", SqlDbType.Int, PaymentStatus)
                Else
                    ReturnVal = 22
                    If Not transcation Is Nothing Then
                        data.RollbackSQLTransaction(transcation, con)
                    End If
                    Exit For
                End If

                Dim ReceivedDatetxt As String = dtPayments.Rows(i).Item("Received Date").ToString
                Dim ReceivedDate As Date

                If DateTime.TryParse(ReceivedDatetxt, ReceivedDate) Then
                    data.params("@ReceivedDate", SqlDbType.Date, ReceivedDate)
                ElseIf PaymentStatus = 0 Then

                Else
                    ReturnVal = 21
                    If Not transcation Is Nothing Then
                        data.RollbackSQLTransaction(transcation, con)
                    End If
                    Exit For
                End If



                If Not String.IsNullOrEmpty(dtPayments.Rows(i).Item("PayPointId").ToString()) Then
                    Dim PayPointIdtxt As String = dtPayments.Rows(i).Item("PayPointId")
                    Dim PayPointId As Integer
                    If Integer.TryParse(PayPointIdtxt, PayPointId) Then
                        data.params("@PayPointId", SqlDbType.Int, PayPointId)
                    Else
                        ReturnVal = 26
                        If Not transcation Is Nothing Then
                            data.RollbackSQLTransaction(transcation, con)
                        End If
                        Exit For
                    End If
                End If


                data.params("@retValue", SqlDbType.NVarChar, "", ParameterDirection.ReturnValue)

                If con.State = 0 Then
                    con = data.OpenSQLConnection()
                    transcation = data.GetSQLTransaction("InsertPayments", con)
                End If

                data.ExecuteSQLTransaction(transcation, con)

                ReturnValue = data.sqlParameters("@retValue")

                If Integer.TryParse(ReturnValue, CaseValue) Then
                    Select Case CaseValue
                        Case 0
                            ReturnVal = CaseValue
                            If i = (dtPayments.Rows.Count - 1) Then
                                data.CommitSQLTransaction(transcation, con)
                            End If
                        Case 1 To 27
                            ReturnVal = CaseValue
                            data.RollbackSQLTransaction(transcation, con)
                            Exit For
                        Case Else
                            ReturnVal = -1
                            data.RollbackSQLTransaction(transcation, con)
                            Exit For
                    End Select
                End If

            Next

            Row("ReturnVal") = ReturnVal
            Row("RowNo") = RowNo
            ReturnData.Rows.Add(Row)
            Return ReturnData

        Catch ex As Exception
            ReturnVal = -1
            data.RollbackSQLTransaction(transcation, con)

            Row("ReturnVal") = ReturnVal
            Row("RowNo") = RowNo
            ReturnData.Rows.Add(Row)
            Return ReturnData

        End Try

    End Function

    Public Function GETLocationID(ByVal LocationType As String, ByVal RegionName As String, Optional VillageName As String = "", Optional WardName As String = "", Optional DistrictName As String = "") As DataTable
        Dim data As New ExactSQL
        Dim sSQL As String = ""
        If LocationType = "RegionId" Then
            sSQL = "select top 1 RegionId from tblAllLocations where RegionName = @RegionName"
        ElseIf LocationType = "SettlementId" Then
            sSQL = "select top 1 VillageId from tblAllLocations where VillageName = @VillageName and WardName = @WardName and DistrictName = @DistrictName and RegionName = @RegionName"
        End If
        data.setSQLCommand(sSQL, CommandType.Text)
        data.params("@RegionName", SqlDbType.NVarChar, 50, RegionName)
        If LocationType = "SettlementId" Then
            data.params("@VillageName", SqlDbType.NVarChar, 50, VillageName)
            data.params("@WardName", SqlDbType.NVarChar, 50, WardName)
            data.params("@DistrictName", SqlDbType.NVarChar, 50, DistrictName)
        End If

        Return data.Filldata

    End Function

    Public Function GETIdentificationTypeCode(ByVal IdentificationType As String) As DataTable
        Dim data As New ExactSQL
        Dim sSQL As String = ""
        sSQL = "Select IdentificationCode from tblIdentificationTypes where IdentificationTypes = @IdentificationType"

        data.setSQLCommand(sSQL, CommandType.Text)
        data.params("@IdentificationType", SqlDbType.NVarChar, 50, IdentificationType)

        Return data.Filldata

    End Function

    Public Function GETPayPointId(ByVal PayPointCode As String) As DataTable
        Dim data As New ExactSQL
        Dim sSQL As String = ""
        sSQL = "Select ID AS PayPointId from tblPayPoints  where PayPointCode = @PayPointCode"

        data.setSQLCommand(sSQL, CommandType.Text)
        data.params("@PayPointCode", SqlDbType.NVarChar, 50, PayPointCode)

        Return data.Filldata

    End Function

    Public Function GETProgramCode(ByVal Program As String) As DataTable
        Dim data As New ExactSQL
        Dim sSQL As String = ""
        sSQL = "Select ProductCode AS ProgramCode from tblproduct  where productname = @Program"

        data.setSQLCommand(sSQL, CommandType.Text)
        data.params("@Program", SqlDbType.NVarChar, 50, Program)

        Return data.Filldata

    End Function

End Class

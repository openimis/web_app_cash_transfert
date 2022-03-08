Public Class ImportPaypointsDL
    Public Function ImportPayPointData(ByVal dtPayPoints As DataTable) As DataTable
        Dim data As New ExactSQL
        Dim sSQL As String = "uspAPIEnterPayPoints"
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

        Try

            Dim i As Integer
            For i = 0 To (dtPayPoints.Rows.Count - 1)
                RowNo = i + 1

                data.setSQLCommand(sSQL, CommandType.StoredProcedure)

                Dim HFIDtxt As String = dtPayPoints.Rows(i).Item("HFID")
                Dim HFID As Integer
                If Integer.TryParse(HFIDtxt, HFID) Then
                    data.params("@HFID", SqlDbType.Int, HFID)
                End If

                Dim PayPointSettlementIDtxt As String = dtPayPoints.Rows(i).Item("PayPoint SettlementID")
                Dim PayPointSettlementID As Integer
                If Integer.TryParse(PayPointSettlementIDtxt, PayPointSettlementID) Then
                    data.params("@PayPointSettlementID", SqlDbType.Int, PayPointSettlementID)
                End If

                data.params("@PayPointCode", SqlDbType.NVarChar, dtPayPoints.Rows(i).Item("PayPoint Code"))

                data.params("@PayPointName", SqlDbType.NVarChar, dtPayPoints.Rows(i).Item("PayPoint Name"))

                data.params("@Geolocation", SqlDbType.NVarChar, dtPayPoints.Rows(i).Item("Geolocation"))


                data.params("@retValue", SqlDbType.NVarChar, "", ParameterDirection.ReturnValue)

                If con.State = 0 Then
                    con = data.OpenSQLConnection()
                    transcation = data.GetSQLTransaction("InsertPayPoints", con)
                End If

                data.ExecuteSQLTransaction(transcation, con)

                ReturnValue = data.sqlParameters("@retValue")

                If Integer.TryParse(ReturnValue, CaseValue) Then
                    Select Case CaseValue
                        Case 0
                            ReturnVal = CaseValue
                            If i = (dtPayPoints.Rows.Count - 1) Then
                                data.CommitSQLTransaction(transcation, con)
                            End If
                        Case 1 To 4
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

    Public Function GETLocationID(ByVal Settlement As String) As DataTable
        Dim data As New ExactSQL
        Dim sSQL As String = ""
        sSQL = "select top 1 VillageId as SettlementId  from tblAllLocations where VillageName = @Settlement"
        data.setSQLCommand(sSQL, CommandType.Text)
        data.params("@Settlement", SqlDbType.NVarChar, 50, Settlement)

        Return data.Filldata

    End Function

    Public Function GETHFID(ByVal HFName As String) As DataTable
        Dim data As New ExactSQL
        Dim sSQL As String = ""
        sSQL = "select top 1 HfID from tblHF where HFName = @HFName"
        data.setSQLCommand(sSQL, CommandType.Text)
        data.params("@HFName", SqlDbType.NVarChar, 50, HFName)

        Return data.Filldata

    End Function

End Class

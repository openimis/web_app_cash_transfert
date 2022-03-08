Public Class ImportFalmilyDAL
    Public Function ImportFamilyData(ByVal dtFamily As DataTable, Optional isExceptional As Boolean = False) As DataTable
        Dim data As New ExactSQL
        Dim sSQL As String

        If isExceptional Then
            sSQL = "uspAPIEnterExceptionalCaseFamily"
        Else
            sSQL = "uspAPIEnterFamily"
        End If

        Dim con As System.Data.SqlClient.SqlConnection = New SqlClient.SqlConnection
        Dim transcation As System.Data.SqlClient.SqlTransaction
        Dim ReturnValue As String
        Dim CaseValue As Integer

        Dim ReturnVal As Integer
        Dim HasErrors As Boolean = False
        Dim RowNo As Integer

        Dim ReturnData As DataTable = New DataTable
        ReturnData.Columns.Add("ReturnVal", Type.GetType("System.String"))
        ReturnData.Columns.Add("HasErrors", Type.GetType("System.String"))
        ReturnData.Columns.Add("RowNo", Type.GetType("System.String"))

        Dim Row As DataRow = ReturnData.NewRow

        Try

            Dim i As Integer
            For i = 0 To (dtFamily.Rows.Count - 1)
                RowNo = i + 1

                data.setSQLCommand(sSQL, CommandType.StoredProcedure)

                'data.params("@AuditUserID", SqlDbType.Int, 2)
                data.params("@PermanentVillageCode", SqlDbType.NVarChar, dtFamily.Rows(i).Item("PermanentVillageCode"))
                data.params("@InsuranceNumber", SqlDbType.NVarChar, dtFamily.Rows(i).Item("SR Number"))
                data.params("@OtherNames", SqlDbType.NVarChar, dtFamily.Rows(i).Item("OtherNames"))
                data.params("@LastName", SqlDbType.NVarChar, dtFamily.Rows(i).Item("LastName"))
                data.params("@BirthDate", SqlDbType.Date, dtFamily.Rows(i).Item("BirthDate"))
                data.params("@Gender", SqlDbType.NVarChar, dtFamily.Rows(i).Item("GenderCode"))
                'data.params("@PovertyStatus", SqlDbType.Bit, Int(dtFamily.Rows(i).Item("PovertyStatus")))
                data.params("@ConfirmationNo", SqlDbType.NVarChar, dtFamily.Rows(i).Item("ConfirmationNo"))
                data.params("@ConfirmationType", SqlDbType.NVarChar, dtFamily.Rows(i).Item("ConfirmationType"))
                data.params("@PermanentAddress", SqlDbType.NVarChar, dtFamily.Rows(i).Item("PermanentAddress"))
                data.params("@MaritalStatus", SqlDbType.NVarChar, dtFamily.Rows(i).Item("MaritalStatus"))
                data.params("@CurrentVillageCode", SqlDbType.NVarChar, dtFamily.Rows(i).Item("CurrentVillageCode"))
                data.params("@CurrentAddress", SqlDbType.NVarChar, dtFamily.Rows(i).Item("CurrentAddress"))
                data.params("@Proffesion", SqlDbType.NVarChar, dtFamily.Rows(i).Item("Proffesion"))
                data.params("@Education", SqlDbType.NVarChar, dtFamily.Rows(i).Item("Education"))
                data.params("@PhoneNumber", SqlDbType.NVarChar, dtFamily.Rows(i).Item("PhoneNumber"))
                data.params("@Email", SqlDbType.NVarChar, dtFamily.Rows(i).Item("Email"))
                data.params("@IdentificationType", SqlDbType.NVarChar, dtFamily.Rows(i).Item("IdentificationType"))


                Dim IdentificationNumbertxt As String = dtFamily.Rows(i).Item("IdentificationNumber")
                Dim IdentificationNumber As Integer
                If Integer.TryParse(IdentificationNumbertxt, IdentificationNumber) Then
                    data.params("@IdentificationNumber", SqlDbType.NVarChar, dtFamily.Rows(i).Item("IdentificationNumber"))
                Else
                    ReturnVal = 16
                    If Not transcation Is Nothing Then
                        data.RollbackSQLTransaction(transcation, con)
                    End If
                    Exit For
                End If
                data.params("@FSPCode", SqlDbType.NVarChar, dtFamily.Rows(i).Item("FSPCode"))
                data.params("@GroupType", SqlDbType.NVarChar, dtFamily.Rows(i).Item("GroupTypeCode"))

                If isExceptional Then
                    data.params("@TempID", SqlDbType.NVarChar, dtFamily.Rows(i).Item("TempID"))
                    data.params("@ProgramCode", SqlDbType.NVarChar, dtFamily.Rows(i).Item("ProgramCode"))
                    data.params("@EnrollDate", SqlDbType.Date, dtFamily.Rows(i).Item("EnrollDate"))
                    data.params("@StartDate", SqlDbType.Date, dtFamily.Rows(i).Item("StartDate"))
                    data.params("@ExpiryDate", SqlDbType.Date, dtFamily.Rows(i).Item("ExpiryDate"))
                End If

                data.params("@retValue", SqlDbType.NVarChar, "", ParameterDirection.ReturnValue)

                If con.State = 0 Then
                    con = data.OpenSQLConnection()
                    transcation = data.GetSQLTransaction("InsertHousehold", con)
                End If

                data.ExecuteSQLTransaction(transcation, con)

                ReturnValue = data.sqlParameters("@retValue")

                If Integer.TryParse(ReturnValue, CaseValue) Then
                    Select Case CaseValue
                        Case 0
                            ReturnVal = CaseValue
                            If i = (dtFamily.Rows.Count - 1) Then
                                data.CommitSQLTransaction(transcation, con)
                            End If
                        Case 1 To 25
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

            Return ReturnData
            data.RollbackSQLTransaction(transcation, con)

            Row("ReturnVal") = ReturnVal
            Row("RowNo") = RowNo
            ReturnData.Rows.Add(Row)
            Return ReturnData

        End Try

    End Function

    Public Function GETLocationCode(ByVal LocationName As String) As DataTable
        Dim data As New ExactSQL
        Dim sSQL As String = ""
        sSQL = "Select LocationCode from tblLocations where LocationName = @LocationName AND ValidityTo IS NULL AND LocationType ='V'"

        data.setSQLCommand(sSQL, CommandType.Text)
        data.params("@LocationName", SqlDbType.NVarChar, 50, LocationName)

        Return data.Filldata

    End Function

    Public Function GETGenderCode(ByVal Gender As String) As DataTable
        Dim data As New ExactSQL
        Dim sSQL As String = ""
        sSQL = "Select Code As GenderCode from tblGender where Gender = @Gender"

        data.setSQLCommand(sSQL, CommandType.Text)
        data.params("@Gender", SqlDbType.NVarChar, 50, Gender)

        Return data.Filldata

    End Function

    Public Function GETFamilyTypeCode(ByVal FamilyType As String) As DataTable
        Dim data As New ExactSQL
        Dim sSQL As String = ""
        sSQL = "Select FamilyTypeCode from tblFamilyTypes where FamilyType = @FamilyType"

        data.setSQLCommand(sSQL, CommandType.Text)
        data.params("@FamilyType", SqlDbType.NVarChar, 50, FamilyType)

        Return data.Filldata

    End Function

End Class

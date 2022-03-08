Imports System.Drawing
Imports System.IO
Imports ExcelImportExport

Public Class ImportFamily

    Inherits System.Web.UI.Page

    Protected imisgen As New IMIS_Gen
    Private ImportFamily As New IMIS_BI.ImportFamilyBI

    Dim lMaxFileSize As Long = 40960
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Protected Sub btnUpload_Click(sender As Object, e As EventArgs) Handles B_UPLOAD.Click
        Dim Message As String = ""
        Dim HasError As Boolean = False
        Dim ReturnVal As Integer
        Dim ReturnData As DataTable

        'check that the file has been selected and it's a valid file
        If (Not B_IMPORTHOUSEHOLDFILE.PostedFile Is Nothing) Then
            If (B_IMPORTHOUSEHOLDFILE.PostedFile.ContentLength > 0) Then
                'determine file name
                Dim sFileName As String =
               System.IO.Path.GetFileName(B_IMPORTHOUSEHOLDFILE.PostedFile.FileName)
                Try
                    If B_IMPORTHOUSEHOLDFILE.PostedFile.ContentLength <= lMaxFileSize Then

                        Dim filebase As New HttpPostedFileWrapper(B_IMPORTHOUSEHOLDFILE.PostedFile)

                        Dim imp As New ImportExport
                        Dim data As DataTable
                        data = imp.ImportExcel(filebase)
                        data.Columns.Add("PermanentVillageCode", Type.GetType("System.String"))

                        Dim i As Integer
                        For i = 0 To (data.Rows.Count - 1)

                            Dim dtPermanentVillageCode = ImportFamily.GETLocationCode(data.Rows(i).Item("PermanentVillage"))
                            Dim PermanentVillageCode As String = dtPermanentVillageCode.Rows(0).Item("PermanentVillageCode")

                            If Not String.IsNullOrEmpty(PermanentVillageCode) Then
                                data.Rows(i).Item("PermanentVillageCode") = PermanentVillageCode
                            Else
                                Message = "Permanent Village " + data.Rows(i).Item("PermanentVillage") + " does not exist"
                                HasError = True
                                Exit For
                            End If
                        Next

                        If Not HasError Then
                            ReturnData = ImportFamily.ImportFamilyData(data)

                            ReturnVal = ReturnData.Rows(0).Item("ReturnVal")
                            Dim Row = ReturnData.Rows(0).Item("RowNo")

                            Select Case ReturnVal
                                Case 0
                                    Message = imisgen.getMessage("M_HOUSEHOLDUPLOADED")
                                Case 1
                                    Message = "Insurance Number Is Required on Row " + Row.ToString
                                Case 2
                                    Message = "Insurance Number already exist on Row " + Row.ToString
                                Case 3
                                    Message = "Error With PermanentVillageCode on Row " + Row.ToString
                                Case 4
                                    Message = "Error With CurrentVillageCode on Row " + Row.ToString
                                Case 5
                                    Message = "Error With Gender. Row " + Row.ToString
                                Case 6
                                    Message = "BirthDate Is Required. Row " + Row.ToString
                                Case 7
                                    Message = "LastName Is Required. Row " + Row.ToString
                                Case 8
                                    Message = "OtherNames Is Required. Row " + Row.ToString
                                Case 9
                                    Message = "ConfirmationTypeCode Does Not Exist. Row " + Row.ToString
                                Case 10
                                    Message = " GroupType Does Not Exist. Row " + Row.ToString
                                Case 11
                                    Message = "MaritalStatus Is Not Valid. Row " + Row.ToString
                                Case 12
                                    Message = "Education Does Not Exit. Row " + Row.ToString
                                Case 13
                                    Message = "Profession Does Not Exit. Row " + Row.ToString
                                Case 14
                                    Message = "FSPCode Does Not Exit. Row " + Row.ToString
                                Case 15
                                    Message = "IdentificationType Does Not Exit. Row " + Row.ToString
                                Case Else
                                    Message = "An Error Occured. Please Check Your File And Try Again!"
                            End Select

                            gvFamilyToImport.DataSource = data
                            gvFamilyToImport.SelectedIndex = -1
                            gvFamilyToImport.DataBind()

                            lblMessage.Visible = True
                            lblMessage.ForeColor = Color.Green
                            lblMessage.Text = "File: " + sFileName +
                            " Return Message: " + Message

                        Else
                            lblMessage.Visible = True
                            lblMessage.ForeColor = Color.Red
                            lblMessage.Text = Message
                        End If

                    Else
                        'reject file
                        lblMessage.Visible = True
                        lblMessage.ForeColor = Color.Red
                        lblMessage.Text = "File Size if Over the Limit of " +
                       lMaxFileSize
                    End If
                Catch exc As Exception    'in case of an error
                    lblMessage.Visible = True
                    lblMessage.ForeColor = Color.Red
                    lblMessage.Text = "An Error Occured. Please Try Again!"

                    'delete file
                End Try
            Else
                lblMessage.Visible = True
                lblMessage.ForeColor = Color.Red
                lblMessage.Text = "Nothing to upload. Please Try Again!"
            End If


        Else
            lblMessage.Visible = True
            lblMessage.ForeColor = Color.Red
            lblMessage.Text = "Nothing to upload. Please Try Again!"
        End If
    End Sub

    Private Sub DeleteFile(ByVal strFileName As String)

        If strFileName.Trim().Length > 0 Then
            Dim fi As New FileInfo(strFileName)
            If (fi.Exists) Then    'if file exists, delete it
                fi.Delete()
            End If
        End If

    End Sub

    Protected Sub B_SUBMIT_Click(sender As Object, e As EventArgs) Handles B_SUBMIT.Click

    End Sub
End Class
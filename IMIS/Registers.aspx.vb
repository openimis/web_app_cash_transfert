''Copyright (c) 2016-2017 Swiss Agency for Development and Cooperation (SDC)
''
''The program users must agree to the following terms:
''
''Copyright notices
''This program is free software: you can redistribute it and/or modify it under the terms of the GNU AGPL v3 License as published by the 
''Free Software Foundation, version 3 of the License.
''This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of 
''MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU AGPL v3 License for more details www.gnu.org.
''
''Disclaimer of Warranty
''There is no warranty for the program, to the extent permitted by applicable law; except when otherwise stated in writing the copyright 
''holders and/or other parties provide the program "as is" without warranty of any kind, either expressed or implied, including, but not 
''limited to, the implied warranties of merchantability and fitness for a particular purpose. The entire risk as to the quality and 
''performance of the program is with you. Should the program prove defective, you assume the cost of all necessary servicing, repair or correction.
''
''Limitation of Liability 
''In no event unless required by applicable law or agreed to in writing will any copyright holder, or any other party who modifies and/or 
''conveys the program as permitted above, be liable to you for damages, including any general, special, incidental or consequential damages 
''arising out of the use or inability to use the program (including but not limited to loss of data or data being rendered inaccurate or losses 
''sustained by you or third parties or a failure of the program to operate with any other programs), even if such holder or other party has been 
''advised of the possibility of such damages.
''
''In case of dispute arising out or in relation to the use of the program, it is subject to the public law of Switzerland. The place of jurisdiction is Berne.
'
' 
'
Imports ExcelImportExport


Partial Public Class UploadICD
    Inherits System.Web.UI.Page

    Private ICD As New IMIS_BI.RegistersBI
    Protected imisgen As New IMIS_Gen
    Private ImportFamily As New IMIS_BI.ImportFamilyBI
    Private ImportPayments As New IMIS_BI.ImportPaymentsBI
    Private ImportPayPoints As New IMIS_BI.ImportPayPointsBI
    Private ProviderPaymentsBI As New IMIS_BI.FindPaymentProviderPaymentsBI
    Private reports As New IMIS_BI.ReportsBI
    Private GenPayment As New IMIS_BI.GenPayBI
    Private ePayment As New IMIS_EN.tblPayment

    Dim lMaxFileSize As Long = 10000000
    'Dim lMaxFileSize As Long = 40960


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        RunPageSecurity()

        If Request.Form("__EVENTTARGET") = btnUpload.ClientID Then
            btnUpload_Click(sender, New System.EventArgs)
        End If

        If Request.Form("__EVENTTARGET") = btnUploadLocations.ClientID Then
            btnUploadLocations_Click(sender, New System.EventArgs)
        End If

        If Request.Form("__EVENTTARGET") = btnUploadHF.ClientID Then
            btnUploadHF_Click(sender, New System.EventArgs)
        End If

        If Not Page.IsPostBack Then
            FillCombo()
            FillPayrollCycleDetails()
        End If
        If Not (Page.IsPostBack) Then
            PnlUploadLog.Visible = False
        End If

    End Sub

    Private Sub RunPageSecurity()
        Dim RefUrl = Request.Headers("Referer")
        Dim UserID As Integer = imisgen.getUserId(Session("User"))
        If Not ICD.RunPageSecurity(IMIS_EN.Enums.Pages.UploadICD, Page) Then


            Server.Transfer("Redirect.aspx?perm=0&page=" & IMIS_EN.Enums.Pages.UploadICD.ToString & "&retUrl=" & RefUrl)
        Else
            pnlUploadDiagnoses.Enabled = ICD.checkRights(IMIS_EN.Enums.Rights.DiagnosesUpload, UserID)
            FileUploadDiagnosis.Enabled = pnlUploadDiagnoses.Enabled
            pnlDownLoadICD.Enabled = ICD.checkRights(IMIS_EN.Enums.Rights.DiagnosesDownload, UserID)
            PnlUploadHF.Enabled = ICD.checkRights(IMIS_EN.Enums.Rights.HealthFacilitiesUpload, UserID)
            FileUploadHF.Enabled = PnlUploadHF.Enabled
            pnlDownLoadHF.Enabled = ICD.checkRights(IMIS_EN.Enums.Rights.HealthFacilitiesDownload, UserID)
            pnlUploadLocations.Enabled = ICD.checkRights(IMIS_EN.Enums.Rights.LocationUpload, UserID)
            FileUploadLocations.Enabled = pnlUploadLocations.Enabled
            pnlDownLoadLocations.Enabled = ICD.checkRights(IMIS_EN.Enums.Rights.LocationDonwload, UserID)

        End If
    End Sub

    Protected Sub btnUpload_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnUpload.Click
        If FileUploadDiagnosis.HasFile Then
            Try
                Dim Output As New Dictionary(Of String, Integer)
                Dim FileName As String = Server.MapPath("WorkSpace") & "\" & FileUploadDiagnosis.PostedFile.FileName
                FileUploadDiagnosis.SaveAs(FileName)
                Dim StratergyId = ddlUploadStrategy.SelectedValue
                Dim dtResult As New DataTable
                Dim LogFile As String = String.Empty
                Dim registerName As String
                registerName = imisgen.getMessage("L_DIAGNOSIS")
                Output = ICD.UploadICD(FileName, StratergyId, imisgen.getUserId(Session("User")), dtResult, chkDryRun.Checked, registerName, LogFile)
                'If chkDryRun.Checked = False Then lblMsg.Text = imisgen.getMessage("M_ICDUPLOADED")


                Dim dtError As New DataTable
                Dim dtConflict As New DataTable
                Dim dtFatalError As New DataTable
                Dim dv As DataView = dtResult.DefaultView

                dv.RowFilter = "ResultType='E' OR ResultType='FH' OR  ResultType='FR' OR  ResultType='FD' OR  ResultType='FM' OR  ResultType='FV' OR  ResultType='FI'"
                dtError = dv.ToTable
                dv.RowFilter = "ResultType='C'"
                dtConflict = dv.ToTable
                dv.RowFilter = "ResultType='FE'"
                dtFatalError = dv.ToTable





                Dim ErrMsg As String = "<h4><u>" & dtError.Rows.Count.ToString() & " " & If(dtError.Rows.Count = 1, imisgen.getMessage("L_ERROR"), imisgen.getMessage("L_ERRORS")) & "</u></h4>" & "<br>"
                For i As Int16 = 0 To dtError.Rows.Count - 1
                    ErrMsg += dtError.Rows(i)(0).ToString() & "<br>"
                Next


                Dim ConflictMsg As String = "<h4><u>" & dtConflict.Rows.Count.ToString() & "  " & If(dtConflict.Rows.Count = 1, imisgen.getMessage("L_CONFILCT"), imisgen.getMessage("L_CONFILCTS")) & "</u></h4>" & "<br>"
                For i As Int16 = 0 To dtConflict.Rows.Count - 1
                    ConflictMsg += dtConflict.Rows(i)(0).ToString() & "<br>"
                Next

                Dim FatalErrorMsg As String = "<h4><u>" & dtFatalError.Rows.Count.ToString() & " " & imisgen.getMessage("L_FATALERROR") & "</u></h4>" & "<br>"
                For i As Int16 = 0 To dtFatalError.Rows.Count - 1
                    FatalErrorMsg += dtFatalError.Rows(i)(0).ToString() & "<br>"
                Next

                Dim result As Integer = Output("returnValue")
                Dim str As String = ""

                Select Case result
                    Case 0
                        str = imisgen.getMessage("M_DIAGNOSESUPLOADED")
                    Case -1
                        str = imisgen.getMessage("M_UNEXPECTEDERRORONUPLOADICD")
                End Select
                If chkDryRun.Checked = False And dtFatalError.Rows.Count = 0 Then imisgen.Alert(str, Panel1, alertPopupTitle:="Nafa MIS", Queue:=True)

                Dim Msg As String = "<h4><u>" & imisgen.getMessage("M_UPLOADEDDIAGNOSES") & "</u></h4>" & "<br>" &
                imisgen.getMessage("L_SENT") & ": " & Output("DiagnosisSent") & "<br>" &
                imisgen.getMessage("L_INSERTED") & ": " & Output("Inserts") & "<br>" &
                imisgen.getMessage("L_UPDATED") & ": " & Output("Updates") & "<br> " &
                imisgen.getMessage("L_DELETED") & ": " & Output("Deletes") & "</br>"

                imisgen.Alert(Msg, Panel1, alertPopupTitle:="Nafa MIS", Queue:=True)
                If dtFatalError.Rows.Count > 0 Then
                    imisgen.Alert(HttpUtility.JavaScriptStringEncode(FatalErrorMsg), Panel1, alertPopupTitle:="Nafa MIS", Queue:=True)
                End If
                If dtError.Rows.Count > 0 Then
                    imisgen.Alert(HttpUtility.JavaScriptStringEncode(ErrMsg), Panel1, alertPopupTitle:="Nafa MIS", Queue:=True)
                End If
                If dtConflict.Rows.Count > 0 Then
                    imisgen.Alert(HttpUtility.JavaScriptStringEncode(ConflictMsg), Panel1, alertPopupTitle:="Nafa MIS", Queue:=True)
                End If



                showLastUploadLog(LogFile, registerName, System.IO.Path.GetFileName(FileName), ddlUploadStrategy.SelectedItem.Text, Format(DateTime.Now, "dd/MM/yyyy HH:mm"))


            Catch ex As Exception
                imisgen.Alert(imisgen.getMessage("M_ERRORMESSAGE", True), Panel1, alertPopupTitle:="Nafa MIS")
                'EventLog.WriteEntry("IMIS", Page.Title & " : " & imisgen.getLoginName(Session("User")) & " : " & ex.Message, EventLogEntryType.Error, 999)
            End Try
        End If
    End Sub



    Protected Sub B_CANCEL_Click(ByVal sender As Object, ByVal e As EventArgs) Handles B_CANCEL.Click
        Response.Redirect("Home.aspx")
    End Sub

    Protected Sub btnUploadLocations_Click(sender As Object, ByVal e As EventArgs) Handles btnUploadLocations.Click
        If FileUploadLocations.HasFile Then
            Dim Output As New Dictionary(Of String, Integer)
            Try
                Dim FileName As String = Server.MapPath("WorkSpace") & "\" & FileUploadLocations.PostedFile.FileName
                FileUploadLocations.SaveAs(FileName)
                Dim dtResult As New DataTable
                Dim StrategyId As Integer = ddlUploadStrategyLocation.SelectedValue
                Dim LogFile As String = String.Empty
                Dim registerName As String
                registerName = imisgen.getMessage("L_Locations")
                Output = ICD.UploadLocationsXML(FileName, StrategyId, imisgen.getUserId(Session("User")), dtResult, chkDryRunLocations.Checked, registerName, LogFile)
                Dim dtError As New DataTable
                Dim dtConflict As New DataTable
                Dim dtFatalError As New DataTable
                Dim dv As DataView = dtResult.DefaultView

                dv.RowFilter = "ResultType='E' OR ResultType='FH' OR  ResultType='FR' OR  ResultType='FD' OR  ResultType='FM' OR  ResultType='FV' OR  ResultType='FI'"
                dtError = dv.ToTable
                dv.RowFilter = "ResultType='C'"
                dtConflict = dv.ToTable
                dv.RowFilter = "ResultType='FE'"
                dtFatalError = dv.ToTable

                Dim ErrMsg As String = "<h4><u>" & dtError.Rows.Count.ToString() & " " & If(dtError.Rows.Count = 1, imisgen.getMessage("L_ERROR"), imisgen.getMessage("L_ERRORS")) & "</u></h4>" & "<br>"
                For i As Int16 = 0 To dtError.Rows.Count - 1
                    ErrMsg += dtError.Rows(i)(0).ToString() & "<br>"
                Next


                Dim ConflictMsg As String = "<h4><u>" & dtConflict.Rows.Count.ToString() & "  " & If(dtConflict.Rows.Count = 1, imisgen.getMessage("L_CONFILCT"), imisgen.getMessage("L_CONFILCTS")) & "</u></h4>" & "<br>"
                For i As Int16 = 0 To dtConflict.Rows.Count - 1
                    ConflictMsg += dtConflict.Rows(i)(0).ToString() & "<br>"
                Next

                Dim FatalErrorMsg As String = "<h4><u>" & dtFatalError.Rows.Count.ToString() & " " & imisgen.getMessage("L_FATALERROR") & "</u></h4>" & "<br>"
                For i As Int16 = 0 To dtFatalError.Rows.Count - 1
                    FatalErrorMsg += dtFatalError.Rows(i)(0).ToString() & "<br>"
                Next

                Dim result As Integer = Output("returnValue")
                Dim str As String = ""
                Select Case result
                    Case 0
                        str = imisgen.getMessage("M_LOCATIONUPLOADED")
                    Case -1
                        str = imisgen.getMessage("M_UNEXPECTEDERRORONIMPORTLOCATIONS")
                End Select
                If chkDryRunLocations.Checked = False And dtFatalError.Rows.Count = 0 Then imisgen.Alert(str, Panel1, alertPopupTitle:="Nafa MIS", Queue:=True)

                Dim Msg As String = "<h4><u>" & imisgen.getMessage("M_UPLOADEDLOCATIONS") & "</u></h4>" & "<br>" &
                                 "<b>" & imisgen.getMessage("L_REGION") & "</b><br>" &
                                 imisgen.getMessage("L_SENT") & ": " & Output("RegionSent") & "<br>" &
                                 imisgen.getMessage("L_INSERTED") & ": " & Output("RegionInsert") & "<br>" &
                                 imisgen.getMessage("L_UPDATED") & ": " & Output("RegionUpdate") & "</br><br>" &
                                 "<b>" & imisgen.getMessage("L_DISTRICT") & "</b><br>" &
                                 imisgen.getMessage("L_SENT") & ": " & Output("DistrictSent") & "</br>" &
                                 imisgen.getMessage("L_INSERTED") & ": " & Output("DistrictInsert") & "</br>" &
                                 imisgen.getMessage("L_UPDATED") & ": " & Output("DistrictUpdate") & "</br>" &
                                  "<b>" & imisgen.getMessage("L_WARD") & "</b><br>" &
                                 imisgen.getMessage("L_SENT") & ": " & Output("WardSent") & "</br>" &
                                 imisgen.getMessage("L_INSERTED") & ": " & Output("WardInsert") & "</br>" &
                                 imisgen.getMessage("L_UPDATED") & ": " & Output("WardUpdate") & "</br>" &
                                  "<b>" & imisgen.getMessage("L_VILLAGE") & "</b><br>" &
                                 imisgen.getMessage("L_SENT") & ": " & Output("VillageSent") & "</br>" &
                                 imisgen.getMessage("L_INSERTED") & ": " & Output("VillageInsert") & "</br>" &
                                 imisgen.getMessage("L_UPDATED") & ": " & Output("VillageUpdate") & "</br>"


                imisgen.Alert(Msg, Panel1, alertPopupTitle:="Nafa MIS", Queue:=True)
                If dtFatalError.Rows.Count > 0 Then
                    imisgen.Alert(HttpUtility.JavaScriptStringEncode(FatalErrorMsg), Panel1, alertPopupTitle:="Nafa MIS", Queue:=True)
                End If
                If dtError.Rows.Count > 0 Then
                    imisgen.Alert(HttpUtility.JavaScriptStringEncode(ErrMsg), Panel1, alertPopupTitle:="Nafa MIS", Queue:=True)
                End If
                If dtConflict.Rows.Count > 0 Then
                    imisgen.Alert(HttpUtility.JavaScriptStringEncode(ConflictMsg), Panel1, alertPopupTitle:="Nafa MIS", Queue:=True)
                End If



                showLastUploadLog(LogFile, registerName, System.IO.Path.GetFileName(FileName), ddlUploadStrategyLocation.SelectedItem.Text, Format(DateTime.Now, "dd/MM/yyyy HH:mm"))


            Catch ex As Exception
                imisgen.Alert(imisgen.getMessage("M_ERRORMESSAGE", True), Panel1, alertPopupTitle:="Nafa MIS")
                'EventLog.WriteEntry("IMIS", Page.Title & " : " & imisgen.getLoginName(Session("User")) & " : " & ex.Message, EventLogEntryType.Error, 999)
            End Try
        End If
    End Sub

    Protected Sub btnDownLoadICD_Click(sender As Object, e As EventArgs)
        Dim path As String = ICD.DownLoadDiagnosis()
        DownloadFile(path, "application/xml")
    End Sub

    Private Sub FillCombo()
        ddlUploadStrategy.DataSource = ICD.GetICDUploadStrategies()
        ddlUploadStrategy.DataValueField = "StrategyId"
        ddlUploadStrategy.DataTextField = "StrategyName"
        ddlUploadStrategy.DataBind()

        ddlUploadStrategyLocation.DataSource = ICD.GetLocationUploadStrategies()
        ddlUploadStrategyLocation.DataValueField = "StrategyId"
        ddlUploadStrategyLocation.DataTextField = "StrategyName"
        ddlUploadStrategyLocation.DataBind()

        ddlUploadStrategyHF.DataSource = ICD.GetUploadStrategiesHF()
        ddlUploadStrategyHF.DataValueField = "StrategyId"
        ddlUploadStrategyHF.DataTextField = "StrategyName"
        ddlUploadStrategyHF.DataBind()

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

        ddlPayrollCycle.DataSource = GenPayment.GetPayrollCycle()
        ddlPayrollCycle.DataValueField = "PayrollCycleID"
        ddlPayrollCycle.DataTextField = "name"
        ddlPayrollCycle.DataBind()


    End Sub

    Protected Sub btnUploadHF_Click(sender As Object, e As EventArgs) Handles btnUploadHF.Click
        If FileUploadHF.HasFile Then
            Dim Output As New Dictionary(Of String, Integer)
            Try
                Dim FileName As String = Server.MapPath("WorkSpace") & "\" & FileUploadHF.PostedFile.FileName
                Dim StrategyId As Integer = ddlUploadStrategyHF.SelectedValue
                Dim LogFile As String = String.Empty
                Dim dtResult As New DataTable
                FileUploadHF.SaveAs(FileName)
                Dim registerName As String
                registerName = imisgen.getMessage("L_HF")
                Output = ICD.UploadHF(FileName, StrategyId, imisgen.getUserId(Session("User")), dtResult, chkDryRunHF.Checked, registerName, LogFile)
                Dim dtError As New DataTable
                Dim dtConflict As New DataTable
                Dim dtFatalError As New DataTable
                Dim dv As DataView = dtResult.DefaultView

                dv.RowFilter = "ResultType='E' OR ResultType='FH' OR  ResultType='FR' OR  ResultType='FD' OR  ResultType='FM' OR  ResultType='FV' OR  ResultType='FI'"
                dtError = dv.ToTable
                dv.RowFilter = "ResultType='C'"
                dtConflict = dv.ToTable
                dv.RowFilter = "ResultType='FE'"
                dtFatalError = dv.ToTable

                Dim ErrMsg As String = "<h4><u>" & dtError.Rows.Count.ToString() & " " & If(dtError.Rows.Count = 1, imisgen.getMessage("L_ERROR"), imisgen.getMessage("L_ERRORS")) & "</u></h4>" & "<br>"
                For i As Int16 = 0 To dtError.Rows.Count - 1
                    ErrMsg += dtError.Rows(i)(0).ToString() & "<br>"
                Next


                Dim ConflictMsg As String = "<h4><u>" & dtConflict.Rows.Count.ToString() & "  " & If(dtConflict.Rows.Count = 1, imisgen.getMessage("L_CONFILCT"), imisgen.getMessage("L_CONFILCTS")) & "</u></h4>" & "<br>"
                For i As Int16 = 0 To dtConflict.Rows.Count - 1
                    ConflictMsg += dtConflict.Rows(i)(0).ToString() & "<br>"
                Next

                Dim FatalErrorMsg As String = "<h4><u>" & dtFatalError.Rows.Count.ToString() & " " & imisgen.getMessage("L_FATALERROR") & "</u></h4>" & "<br>"
                For i As Int16 = 0 To dtFatalError.Rows.Count - 1
                    FatalErrorMsg += dtFatalError.Rows(i)(0).ToString() & "<br>"
                Next

                Dim result As Integer = Output("returnValue")
                Dim str As String = ""
                Select Case result
                    Case 0
                        str = imisgen.getMessage("M_HFUPLOADED")
                    Case -1
                        str = imisgen.getMessage("M_UNEXPECTEDERRORONUPLOADHF")
                End Select
                If chkDryRunHF.Checked = False And dtFatalError.Rows.Count = 0 Then imisgen.Alert(str, Panel1, alertPopupTitle:="Nafa MIS", Queue:=True)

                Dim Msg As String = "<h4><u>" & imisgen.getMessage("M_UPLOADEDHF") & "</u></h4>" & "<br>" &
                                 "<b>" & imisgen.getMessage("L_HF") & "</b><br>" &
                                 imisgen.getMessage("L_SENT") & ": " & Output("SentHF") & "<br>" &
                                 imisgen.getMessage("L_INSERTED") & ": " & Output("Inserts") & "<br>" &
                                 imisgen.getMessage("L_UPDATED") & ": " & Output("Updates") & "</br><br>" &
                                 "<b>" & imisgen.getMessage("L_HFCATCHMENT") & "</b><br>" &
                                 imisgen.getMessage("L_SENT") & ": " & Output("sentCatchment") & "</br>" &
                                 imisgen.getMessage("L_INSERTED") & ": " & Output("InsertCatchment") & "</br>" &
                                 imisgen.getMessage("L_UPDATED") & ": " & Output("UpdateCatchment") & "</br>"

                imisgen.Alert(Msg, Panel1, alertPopupTitle:="Nafa MIS", Queue:=True)
                If dtFatalError.Rows.Count > 0 Then
                    imisgen.Alert(HttpUtility.JavaScriptStringEncode(FatalErrorMsg), Panel1, alertPopupTitle:="Nafa MIS", Queue:=True)
                End If
                If dtError.Rows.Count > 0 Then
                    imisgen.Alert(HttpUtility.JavaScriptStringEncode(ErrMsg), Panel1, alertPopupTitle:="Nafa MIS", Queue:=True)
                End If
                If dtConflict.Rows.Count > 0 Then
                    imisgen.Alert(HttpUtility.JavaScriptStringEncode(ConflictMsg), Panel1, alertPopupTitle:="Nafa MIS", Queue:=True)
                End If

                showLastUploadLog(LogFile, registerName, System.IO.Path.GetFileName(FileName), ddlUploadStrategyHF.SelectedItem.Text, Format(DateTime.Now, "dd/MM/yyyy HH:mm"))

            Catch ex As Exception
                imisgen.Alert(imisgen.getMessage("M_ERRORMESSAGE", True), Panel1, alertPopupTitle:="Nafa MIS")
                'EventLog.WriteEntry("IMIS", Page.Title & " : " & imisgen.getLoginName(Session("User")) & " : " & ex.Message, EventLogEntryType.Error, 999)
            End Try
        End If
    End Sub

    Protected Sub btnUploadFamily_Click(sender As Object, e As EventArgs) Handles btnUploadFamily.Click
        Dim Message As String = ""
        Dim HasError As Boolean = False
        Dim ReturnVal As Integer
        Dim ReturnData As DataTable

        'check that the file has been selected and it's a valid file
        If (Not FileUploadFamily.PostedFile Is Nothing) Then
            If (FileUploadFamily.PostedFile.ContentLength > 0) Then
                'determine file name
                Dim sFileName As String =
               System.IO.Path.GetFileName(FileUploadFamily.PostedFile.FileName)
                Try
                    If FileUploadFamily.PostedFile.ContentLength <= lMaxFileSize Then

                        Dim filebase As New HttpPostedFileWrapper(FileUploadFamily.PostedFile)

                        Dim ExpectedColumnList As List(Of String)

                        If isExceptional.Checked Then
                            ExpectedColumnList = New List(Of String) From {"PermanentVillage", "SR Number", "OtherNames", "LastName", "BirthDate", "Gender", "PovertyStatus", "ConfirmationNo", "ConfirmationType", "PermanentAddress", "MaritalStatus", "CurrentVillage", "CurrentAddress", "Proffesion", "Education", "PhoneNumber", "Email", "IdentificationType", "IdentificationNumber", "FSPCode", "GroupType", "TempID", "Program", "EnrollDate", "StartDate", "ExpiryDate"}
                        Else
                            ExpectedColumnList = New List(Of String) From {"PermanentVillage", "SR Number", "OtherNames", "LastName", "BirthDate", "Gender", "PovertyStatus", "ConfirmationNo", "ConfirmationType", "PermanentAddress", "MaritalStatus", "CurrentVillage", "CurrentAddress", "Proffesion", "Education", "PhoneNumber", "Email", "IdentificationType", "IdentificationNumber", "FSPCode", "GroupType"}
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

                            data.Columns.Add("PermanentVillageCode", Type.GetType("System.String"))
                            data.Columns.Add("CurrentVillageCode", Type.GetType("System.String"))
                            data.Columns.Add("GenderCode", Type.GetType("System.String"))
                        data.Columns.Add("GroupTypeCode", Type.GetType("System.String"))
                        data.Columns.Add("ProgramCode", Type.GetType("System.String"))

                        If data.Rows.Count = 0 Then
                                Message = "Cannot import empty file"
                                HasError = True
                            End If

                            If Not HasError Then
                                Dim i As Integer
                                For i = 0 To (data.Rows.Count - 1)

                                    If Not String.IsNullOrEmpty(data.Rows(i).Item("PermanentVillage").ToString()) Then
                                        Dim dtPermanentVillageCode = ImportFamily.GETLocationCode(data.Rows(i).Item("PermanentVillage"))
                                        Dim PermanentVillageCode As String = dtPermanentVillageCode.Rows(0).Item("LocationCode")


                                        If Not String.IsNullOrEmpty(PermanentVillageCode) Then
                                            data.Rows(i).Item("PermanentVillageCode") = PermanentVillageCode
                                        Else
                                            Message = "Permanent Village " + data.Rows(i).Item("PermanentVillage") + " does not exist"
                                            HasError = True
                                            Exit For
                                        End If
                                    Else
                                        Message = "Permanent Village Is Empty On Row " + (i + 2).ToString
                                        HasError = True
                                        Exit For
                                    End If


                                    If Not String.IsNullOrEmpty(data.Rows(i).Item("CurrentVillage").ToString()) Then

                                        Dim dtCurrentVillageCode = ImportFamily.GETLocationCode(data.Rows(i).Item("CurrentVillage"))
                                        Dim CurrentVillageCode As String = dtCurrentVillageCode.Rows(0).Item("LocationCode")

                                        If Not String.IsNullOrEmpty(CurrentVillageCode) Then
                                            data.Rows(i).Item("CurrentVillageCode") = CurrentVillageCode
                                        Else
                                            Message = "CurrentVillage " + data.Rows(i).Item("CurrentVillage") + " does not exist"
                                            HasError = True
                                            Exit For
                                        End If
                                    Else
                                        Message = "CurrentVillage Is Empty On Row " + (i + 2).ToString
                                        HasError = True
                                        Exit For

                                    End If

                                    If Not String.IsNullOrEmpty(data.Rows(i).Item("Gender").ToString()) Then
                                        Dim dtGenderCode = ImportFamily.GETGenderCode(data.Rows(i).Item("Gender"))
                                        If dtGenderCode.Rows.Count > 0 Then
                                            Dim GenderCode As String = dtGenderCode.Rows(0).Item("GenderCode")

                                            If Not String.IsNullOrEmpty(GenderCode) Then
                                                data.Rows(i).Item("GenderCode") = GenderCode
                                            Else
                                                Message = "Gender " + data.Rows(i).Item("Gender") + " does not exist"
                                                HasError = True
                                                Exit For
                                            End If
                                        Else
                                            Message = "Gender " + data.Rows(i).Item("Gender") + " does not exist"
                                            HasError = True
                                            Exit For
                                        End If
                                    Else
                                        Message = "Gender Is Empty On Row " + (i + 2).ToString
                                        HasError = True
                                        Exit For

                                    End If

                                If Not String.IsNullOrEmpty(data.Rows(i).Item("GroupType").ToString()) Then
                                    Dim dtFamilyTypeCode = ImportFamily.GETFamilyTypeCode(data.Rows(i).Item("GroupType"))
                                    If dtFamilyTypeCode.Rows.Count > 0 Then
                                        Dim FamilyTypeCode As String = dtFamilyTypeCode.Rows(0).Item("FamilyTypeCode")

                                        If Not String.IsNullOrEmpty(FamilyTypeCode) Then
                                            data.Rows(i).Item("GroupTypeCode") = FamilyTypeCode
                                        Else
                                            Message = "GroupType " + data.Rows(i).Item("GroupType") + " does not exist"
                                            HasError = True
                                            Exit For
                                        End If
                                    Else
                                        Message = "GroupType " + data.Rows(i).Item("GroupType") + " does not exist"
                                        HasError = True
                                        Exit For
                                    End If
                                Else
                                    Message = "GroupType Is Empty On Row " + (i + 2).ToString
                                    HasError = True
                                    Exit For

                                End If

                                If Not String.IsNullOrEmpty(data.Rows(i).Item("Program").ToString()) Then

                                    Dim dtProgramCode = ImportPayments.GETProgramCode(data.Rows(i).Item("Program"))

                                    If dtProgramCode.Rows.Count > 0 Then
                                        Dim ProgramCode As String = dtProgramCode.Rows(0).Item("ProgramCode")
                                        If Not String.IsNullOrEmpty(ProgramCode) Then
                                            data.Rows(i).Item("ProgramCode") = ProgramCode
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

                            Next

                                If Not HasError Then
                                ReturnData = ImportFamily.ImportFamilyData(data, isExceptional.Checked)
                                ReturnVal = ReturnData.Rows(0).Item("ReturnVal")
                                    Dim Row = ReturnData.Rows(0).Item("RowNo")

                                    Select Case ReturnVal
                                        Case 0
                                            Message = imisgen.getMessage("M_HOUSEHOLDUPLOADED")
                                        Case 1
                                        Message = "SR Number Is Required on Row " + Row.ToString
                                    Case 2
                                        Message = "SR Number already exist on Row " + Row.ToString
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
                                        Case 16
                                        Message = "Incorrect Format for Identification Number. Row " + Row.ToString
                                    Case 17
                                        Message = "Wrong format or missing Temporal Id. Row " + Row.ToString
                                    Case 18
                                        Message = "Temporal Id Already Existseesess. Row " + Row.ToString
                                    Case 19
                                        Message = "Missing ProgramCode. Row " + Row.ToString
                                    Case 20
                                        Message = "Wrong Program Code. Row " + Row.ToString
                                    Case Else
                                            Message = "An Error Occured. Please Check Your File And Try Again!"
                                    End Select
                                End If

                            End If

                        Else
                            Message = "File Size if Over the Limit of " + lMaxFileSize
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

    Protected Sub btnUploadPayPoints_Click(sender As Object, e As EventArgs) Handles btnUploadPayPoints.Click
        Dim Message As String = ""
        Dim HasError As Boolean = False
        Dim ReturnVal As Integer
        Dim ReturnData As DataTable

        'check that the file has been selected and it's a valid file
        If (Not FileUploadPayPoints.PostedFile Is Nothing) Then
            If (FileUploadPayPoints.PostedFile.ContentLength > 0) Then

                'determine file name
                Dim sFileName As String = System.IO.Path.GetFileName(FileUploadPayPoints.PostedFile.FileName)

                Try
                    If FileUploadPayPoints.PostedFile.ContentLength <= lMaxFileSize Then

                        Dim filebase As New HttpPostedFileWrapper(FileUploadPayPoints.PostedFile)

                        Dim imp As New ImportExport
                        Dim data As DataTable
                        data = imp.ImportExcel(filebase)

                        data.Columns.Add("HFID", Type.GetType("System.String"))

                        data.Columns.Add("PayPoint SettlementID", Type.GetType("System.String"))

                        Dim i As Integer
                        For i = 0 To (data.Rows.Count - 1)

                            Dim dtHFID = ImportPayPoints.GETHFID(data.Rows(i).Item("Financial Provider"))

                            If dtHFID.Rows.Count > 0 Then
                                Dim HFID As String = dtHFID.Rows(0).Item("HFID")


                                If Not String.IsNullOrEmpty(HFID) Then
                                    data.Rows(i).Item("HFID") = HFID
                                Else
                                    Message = "Financial Provider " + data.Rows(i).Item("Financial Provider") + " does not exist"
                                    HasError = True
                                    Exit For
                                End If
                            Else
                                Message = "Financial Provider " + data.Rows(i).Item("Financial Provider") + " does not exist"
                                HasError = True
                                Exit For
                            End If

                            Dim dtPayPointSettlementId = ImportPayPoints.GETLocationID(data.Rows(i).Item("PayPoint Settlement"))

                            If dtPayPointSettlementId.Rows.Count > 0 Then
                                Dim SettlementId As String = dtPayPointSettlementId.Rows(0).Item("SettlementId")


                                If Not String.IsNullOrEmpty(SettlementId) Then
                                    data.Rows(i).Item("PayPoint SettlementID") = SettlementId
                                Else
                                    Message = "PayPoint Settlement " + data.Rows(i).Item("PayPoint Settlement") + " does not exist"
                                    HasError = True
                                    Exit For
                                End If
                            Else
                                Message = "PayPoint Settlement " + data.Rows(i).Item("PayPoint Settlement") + " does not exist"
                                HasError = True
                                Exit For
                            End If
                        Next

                        If Not HasError Then
                            ReturnData = ImportPayPoints.ImportPayPointsData(data)
                            ReturnVal = ReturnData.Rows(0).Item("ReturnVal")
                            Dim Row = ReturnData.Rows(0).Item("RowNo")

                            Select Case ReturnVal
                                Case 0
                                    Message = imisgen.getMessage("M_PAYPOINTSUPLOADED")
                                Case 1
                                    Message = "Incorrect or Missing HFID"
                                Case 2
                                    Message = "Missing PayPoint Code"
                                Case 3
                                    Message = "Missing PayPoint Name"
                                Case 4
                                    Message = "Incorrect or Missing PayPoint SettlementID"
                                Case Else
                                    Message = "An Error Occured. Please Check Your File And Try Again!"
                            End Select
                        End If

                    Else
                        Message = "File Size if Over the Limit of " + lMaxFileSize
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

    Protected Sub btnDownloadHF_Click(sender As Object, e As EventArgs) Handles btnDownloadHF.Click
        Dim path As String = ICD.downLoadHFXML()
        DownloadFile(path, "application/xml")
    End Sub

    Protected Sub btnDownLoadLocation_Click(sender As Object, e As EventArgs) Handles btnDownLoadLocation.Click
        Dim path As String = ICD.DownLoadLocationsXML()
        DownloadFile(path, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
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

    Private Sub btnDiagnosisLog_Click(sender As Object, e As EventArgs) Handles btnDownloadLog.Click
        Try
            If hfDownloadLog.Value.Length > 0 Then
                DownloadFile(hfDownloadLog.Value, "application/text")
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub showLastUploadLog(DownLoadPath As String, RegisterName As String, UploadedFileName As String, StrategyName As String, LogDate As String)
        If (DownLoadPath.Length > 0 And (chkDryRun.Checked = False Or chkDryRunHF.Checked = False Or chkDryRunLocations.Checked = False)) Then
            btnDownloadLog.Visible = True
            hfDownloadLog.Value = DownLoadPath
            LblFileName.Text = UploadedFileName
            LblRegister.Text = RegisterName
            LblStrategy.Text = StrategyName
            LblDate.Text = LogDate
            PnlUploadLog.Visible = True
        Else
            btnDownloadLog.Visible = False
            hfDownloadLog.Value = ""

        End If
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

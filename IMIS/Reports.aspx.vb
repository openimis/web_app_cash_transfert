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

Imports System.Web.Script.Serialization

Partial Public Class Reports
    Inherits System.Web.UI.Page
    Private reports As New IMIS_BI.ReportsBI

    Private ePayment As New IMIS_EN.tblPayment
    Private eRelIndex As New IMIS_EN.tblRelIndex
    Public imisgen As New IMIS_Gen
    Private RoleID As Integer
    Private UserID As Integer
    Private userBI As New IMIS_BI.UserBI
    Private ds As New DataSet
    Private dt As New DataTable
    Private isClaimOverView As Boolean = False
    Private LocationName As String = ""
    Private dtLevel As DataTable
    Private GenPayment As New IMIS_BI.GenPayBI
    Private Sub RunPageSecurity()
        Dim RefUrl = Request.Headers("Referer")
        Dim RoleID As Integer = imisgen.getRoleId(Session("User"))

        If Not userBI.RunPageSecurity(IMIS_EN.Enums.Pages.Reports, Page) Then
            Server.Transfer("Redirect.aspx?perm=0&page=" & IMIS_EN.Enums.Pages.Reports.ToString & "&retUrl=" & RefUrl)
        End If
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        UserID = imisgen.getUserId(Session("User"))
        RoleID = imisgen.getRoleId(Session("User"))
        If IsPostBack Then Return
        FillPayrollCycle()
        RunPageSecurity()
        'Try
        FillRegions()
        FillRegionsWoNational()
        'FillDistricts()
        'FillDistrictsWoNational()
        FillProducts()
        FillHF(ddlDistrictWoNational)
        FillReportTypes()
        FillMonth()
        FillQuarter()
        FillUserName()
        FillYear()
        FillPaymentType()
        FillEnrolmentOfficer(ddlDistrictWoNational)
        FillPayer(ddlRegionWoNational, ddlDistrictWoNational)
        FillPreviousReportsDate()
        FillClaimStatus()
        FillPolicyStatus()
        FillEntities()
        FillActions()
        FillALLProducts()
        FillSorting()

        ReselectCachedCriteria()
        Dim SelectedValue As String = ddlProduct.SelectedValue
        Dim selectedValueStrict As String = Val(ddlProductStrict.SelectedValue)
        FillProducts()
        ddlProduct.SelectedValue = SelectedValue
        ddlProductStrict.SelectedValue = selectedValueStrict

        SelectedValue = ddlHF.SelectedValue
        FillHF(ddlDistrictWoNational)
        ddlHF.SelectedValue = SelectedValue
        SelectedValue = ddlPayer.SelectedValue
        FillPayer(ddlRegionWoNational, ddlDistrictWoNational)
        ddlPayer.SelectedValue = SelectedValue
        SelectedValue = ddlPreviousReportDate.SelectedValue
        FillPreviousReportsDate()
        ddlPreviousReportDate.SelectedValue = SelectedValue
        QuarterSelector()
        HideCriteriaControls()
        'Catch ex As Exception
        'Session("Msg") = ex.Message 'imisgen.getMessage("M_ERRORMESSAGE")
        ''EventLog.WriteEntry("IMIS", Page.Title & " : " & imisgen.getLoginName(Session("User")) & " : " & ex.Message, EventLogEntryType.Error, 999)
        'End Try
    End Sub
    Private Sub ReselectCachedCriteria()
        If Not Session("CriteriaCache") Is Nothing Then
            'hfCriteriaCache.Value = CType(Session("CriteriaCache"), String)
            Dim ctrl As Control
            Dim ddl As DropDownList = Nothing
            Dim txtbox As TextBox = Nothing
            Dim hf As HiddenField = Nothing
            If Not Session("CriteriaCache").ToString.Contains("{") Then
                Return
            End If
            Dim js As New JavaScriptSerializer()
            Dim json As Dictionary(Of String, String) = js.Deserialize(Of Dictionary(Of String, String))(Session("CriteriaCache"))

            For Each id As String In json.Keys  'id is control's clientid.
                ctrl = pnlTop.FindControl(id.Split("_")(1)) 'take last segment to get server controls id.
                If (TypeOf ctrl Is DropDownList) Then
                    ddl = CType(ctrl, DropDownList)
                    'Try
                    ddl.SelectedValue = json(id)
                    'Catch ex As Exception

                    'End Try



                    'If id.Contains("District") Then
                    If id = ddlRegion.ClientID Then
                        FillDistricts()
                        FillProducts()
                        FillHF(ddlDistrict)
                        ' FillWards()
                    End If

                    If id = ddlDistrict.ClientID Then
                        FillProducts()
                        FillHF(ddlDistrict)
                    End If

                    If id = ddlRegionWoNational.ClientID Then
                        FillDistrictsWoNational()
                        FillProducts()
                        FillHF(ddlDistrictWoNational)
                        FillWards()
                    End If

                    If id = ddlDistrictWoNational.ClientID Then
                        FillProducts()
                        FillHF(ddlDistrictWoNational)
                        FillWards()
                    End If

                    If id = ddlWards.ClientID Then
                        FillVillages()
                    End If
                ElseIf (TypeOf ctrl Is TextBox) Then
                    txtbox = CType(ctrl, TextBox)
                    If txtbox.ClientID = id Then
                        txtbox.Text = json(id)
                    End If


                ElseIf (TypeOf ctrl Is HiddenField) Then
                    hf = CType(ctrl, HiddenField)
                    hf.Value = json(id)

                End If
            Next

         

        End If


        


        ' Session.Remove("CriteriaCache")
    End Sub
    Private Sub HideCriteriaControls()
        'ddlPaymentType.Attributes("style") = "display:none;"
        'lblPaymentType.Attributes("style") = "display:none;"
        'ddlYear.Attributes("style") = "display:none;"
        'lblYear.Attributes("style") = "display:none;"
        'ddlMonth.Attributes("style") = "display:none;"
        'lblMonth.Attributes("style") = "display:none;"
        'ddlHF.Attributes("style") = "display:none;"
        'lblHFCode.Attributes("style") = "display:none;"
        'ddlProduct.Attributes("style") = "display:none;"
        'lblProducts.Attributes("style") = "display:none;"
        'ddlDistrict.Attributes("style") = "display:none;"
        'lblDistrict.Attributes("style") = "display:none;"
        'btnSTARTData.Attributes("style") = "display:none;"
        'btnENDData.Attributes("style") = "display:none;"
        'txtENDData.Attributes("style") = "display:none;"
        'txtSTARTData.Attributes("style") = "display:none;"
        'lblSTART.Attributes("style") = "display:none;"
        'lblEND.Attributes("style") = "display:none;"
        'lblPolicyStatus.Attributes("style") = "display:none;"
        'ddlPolicyStatus.Attributes("style") = "display:none;"
        'ddlClaimStatus.Attributes("style") = "display:none;"
        'lblClaimStatus.Attributes("style") = "display:none;"
    End Sub
    Private Sub FillReportTypes()
        lstboxReportSelector.DataSource = reports.GetReportTypes(UserID)
        lstboxReportSelector.DataTextField = "Name"
        lstboxReportSelector.DataValueField = "Id"
        lstboxReportSelector.DataBind()
    End Sub
    Private Sub FillRegions()
        Dim dt As DataTable = reports.GetRegions(imisgen.getUserId(Session("User")), True, True)
        ddlRegion.DataSource = dt
        ddlRegion.DataValueField = "RegionId"
        ddlRegion.DataTextField = "RegionName"
        ddlRegion.DataBind()
        If dt.Rows.Count = 1 Then
            FillDistricts()
        End If
    End Sub
    Private Sub FillRegionsWoNational()
        Dim dt As DataTable = reports.GetRegions(imisgen.getUserId(Session("User")), True, False)
        ddlRegionWoNational.DataSource = dt
        ddlRegionWoNational.DataValueField = "RegionId"
        ddlRegionWoNational.DataTextField = "RegionName"
        ddlRegionWoNational.DataBind()
        If dt.Rows.Count = 1 Then
            FillDistrictsWoNational()
        End If
    End Sub
    Private Sub FillDistricts()
        Dim dtDistricts As DataTable = reports.GetDistricts(imisgen.getUserId(Session("User")), True, Val(ddlRegion.SelectedValue))
        ddlDistrict.DataSource = dtDistricts
        ddlDistrict.DataValueField = "DistrictId"
        ddlDistrict.DataTextField = "DistrictName"
        ddlDistrict.DataBind()
        FillProducts()
        'If dtDistricts.Rows.Count > 0 Then
        FillHF(ddlDistrict)
        FillPayer(ddlRegion, ddlDistrict)
        FillPreviousReportsDate()
        ' FillWards()
        ' End If

        'dtDistricts = New DataTable
        'dtDistricts = reports.GetDistricts(imisgen.getUserId(Session("User")), True, False)
        'ddlDistrict1.DataSource = dtDistricts
        'ddlDistrict1.DataValueField = "DistrictId"
        'ddlDistrict1.DataTextField = "DistrictName"
        'ddlDistrict1.DataBind()

    End Sub
    Private Sub FillDistrictsWoNational()
        Dim dtDistricts As DataTable = reports.GetDistricts(imisgen.getUserId(Session("User")), True, Val(ddlRegionWoNational.SelectedValue))
        ddlDistrictWoNational.DataSource = dtDistricts
        ddlDistrictWoNational.DataValueField = "DistrictId"
        ddlDistrictWoNational.DataTextField = "DistrictName"
        ddlDistrictWoNational.DataBind()

        FillProducts()
        FillHF(ddlDistrictWoNational)
        FillPayer(ddlRegionWoNational, ddlDistrictWoNational)
        FillWards()
        FillEnrolmentOfficer(ddlDistrictWoNational)
    End Sub

    Private Sub FillWards()
        Dim dtWards As DataTable = reports.GetWards(If(Val(ddlDistrictWoNational.SelectedValue) = 0, -1, ddlDistrictWoNational.SelectedValue), True)
        Dim wards As Integer = dtWards.Rows.Count
        If wards > 0 Then
            ddlWards.DataSource = dtWards
            ddlWards.DataValueField = "WardId"
            ddlWards.DataTextField = "WardName"
            ddlWards.DataBind()
        Else
            ddlWards.Items.Clear()
        End If
        FillVillages()
    End Sub
    Private Sub FillVillages() 'Optional ByVal Wards As Integer = 1
        ddlVillages.Items.Clear()
        If ddlWards.SelectedIndex < 0 Then Exit Sub
        'If Wards > 0 Then
        'And Not ddlWards.SelectedValue = 0
        If Val(ddlWards.SelectedValue) > 0 Then
            ddlVillages.DataSource = reports.GetVillages(ddlWards.SelectedValue, True)
            ddlVillages.DataValueField = "VillageId"
            ddlVillages.DataTextField = "VillageName"
            ddlVillages.DataBind()
        End If
        'Else
        'ddlVillages.Items.Clear()
        'End If

    End Sub
    Private Sub FillProducts()
        'Try

        Dim RegionId As Integer
        Dim DistrictId As Integer
        Dim LocationId As Integer

        'Dim ProdLocationId As Integer = -2

        Dim SelectedValueID As Integer = Val(lstboxReportSelector.SelectedValue)
        If hfVisibleRegion.Value = ddlRegion.ClientID Then
            '
            If Val(ddlDistrict.SelectedValue) > 0 Then
                DistrictId = ddlDistrict.SelectedValue
            End If
            If Val(ddlRegion.SelectedValue) > 0 Or Val(ddlRegion.SelectedValue) = -1 Then
                RegionId = ddlRegion.SelectedValue
            End If

            If Val(ddlDistrict.SelectedValue) > 0 Then
                LocationId = Val(ddlDistrict.SelectedValue)
            Else
                LocationId = Val(ddlRegion.SelectedValue)
            End If
        ElseIf hfVisibleRegion.Value = ddlRegionWoNational.ClientID OrElse SelectedValueID = 0 Or SelectedValueID = 1 Then
            If Val(ddlDistrictWoNational.SelectedValue) > 0 Then
                DistrictId = ddlDistrictWoNational.SelectedValue
            End If
            If Val(ddlRegionWoNational.SelectedValue) > 0 Or Val(ddlRegionWoNational.SelectedValue) = -1 Then
                RegionId = ddlRegionWoNational.SelectedValue
            End If

            If Val(ddlDistrict.SelectedValue) > 0 Then
                LocationId = Val(ddlDistrictWoNational.SelectedValue)
            Else
                LocationId = Val(ddlRegionWoNational.SelectedValue)
            End If

        End If


        Dim dtproducts As DataTable = reports.GetProducts(imisgen.getUserId(Session("User")), True, RegionId, DistrictId)
        ddlProduct.DataSource = dtproducts
        ddlProduct.DataValueField = "ProdId"
        ddlProduct.DataTextField = "ProductCode"
        'If ddlProduct.Items.FindByValue(ddlProduct.SelectedValue) Is Nothing Then ddlProduct.SelectedValue = Nothing
        ddlProduct.DataBind()

        Dim dtproductsStrict As DataTable = reports.GetProductsStict(LocationId, imisgen.getUserId(Session("User")), True)
        ddlProductStrict.DataSource = dtproductsStrict
        ddlProductStrict.DataValueField = "ProdID"
        ddlProductStrict.DataTextField = "ProductCode"
        'If ddlProductStrict.Items.FindByValue(ddlProductStrict.SelectedValue) Is Nothing Then ddlProductStrict.SelectedValue = Nothing
        Try
            ddlProductStrict.DataBind()
            'If dtproductsStrict.Rows.Count = 1 Then
            '    getCapitationDetails()
            'End If
        Catch ex As Exception

        End Try





        'Catch ex As Exception
        'Throw ex
        'End Try
    End Sub
    Private Sub FillALLProducts()
        Dim dtproducts As DataTable = reports.GetAllProducts()
        ddlAllProducts.DataSource = dtproducts
        ddlAllProducts.DataValueField = "ProdId"
        ddlAllProducts.DataTextField = "ProductCode"
        ddlAllProducts.DataBind()

        'ddlProduct.Attributes.Add("style", "display:hidden;")
        'lblProducts.Attributes.Add("style", "display:hidden;")
    End Sub
    Private Sub FillHF(Sender As Object)

        Dim LocationId As Integer = Val(Sender.SelectedValue)

        If (Sender.Id = ddlDistrict.ID Or Sender.Id = ddlDistrictWoNational.ID) And Val(Sender.selectedValue) = 0 Then
            If hfVisibleRegion.Value = ddlRegion.ClientID Then
                LocationId = Val(ddlRegion.SelectedValue)
            ElseIf hfVisibleRegion.Value = ddlRegionWoNational.ClientID Then
                LocationId = Val(ddlRegionWoNational.SelectedValue)
            End If
        End If


        Dim dtHF As DataTable = reports.GetHFCodes(imisgen.getUserId(Session("User")), If(LocationId = 0, -1, LocationId))
        ddlHF.SelectedValue = Nothing
        ddlHF.DataSource = dtHF
        ddlHF.DataValueField = "HFID"
        ddlHF.DataTextField = "HFCode"
        ddlHF.DataBind()

        'ddlHF.Attributes.Add("style", "display:hidden;")
        'lblHFCode.Attributes.Add("style", "display:hidden;")

    End Sub
    Private Sub FillMonth()

        ddlMonth.DataSource = reports.GetMonths(1, 12)
        ddlMonth.DataValueField = "MonthNum"
        ddlMonth.DataTextField = "MonthName"
        ddlMonth.DataBind()

        ddlMonthPOIEnd.DataSource = reports.GetMonths(1, 12)
        ddlMonthPOIEnd.DataValueField = "MonthNum"
        ddlMonthPOIEnd.DataTextField = "MonthName"
        ddlMonthPOIEnd.DataBind()

        'ddlMonth.Attributes.Add("style", "display:hidden;")
        'lblMonth.Attributes.Add("style", "display:hidden;")
    End Sub
    Private Sub FillMonthPOI(ByVal StartMonth As Int16, ByVal EndMonth As Int16)

        ddlMonthPOI.DataSource = reports.GetMonths(StartMonth, EndMonth)
        ddlMonthPOI.DataValueField = "MonthNum"
        ddlMonthPOI.DataTextField = "MonthName"
        ddlMonthPOI.DataBind()
    End Sub
    Private Sub FillQuarter()
        ddlQuarter.DataSource = reports.GetPeriodNo("Q")
        ddlQuarter.DataValueField = "Value"
        ddlQuarter.DataTextField = "Period"
        ddlQuarter.DataBind()

    End Sub
    Private Sub FillEnrolmentOfficer(districtSender As DropDownList)
        Dim LocationId As Integer
        If Val(districtSender.SelectedValue) = 0 Then
            LocationId = Val(ddlRegionWoNational.SelectedValue)
        Else
            LocationId = Val(districtSender.SelectedValue)
        End If
        ' Val(districtSender.SelectedValue)
        ddlEnrolmentOfficer.DataSource = reports.GetOfficers(LocationId, True, Val(ddlVillages.SelectedValue))
        ddlEnrolmentOfficer.DataValueField = "OfficerId"
        ddlEnrolmentOfficer.DataTextField = "Code"
        ddlEnrolmentOfficer.DataBind()

    End Sub
    Private Sub FillYear()
        ddlYear.DataSource = reports.GetYears(2010, Year(Now()) + 5)
        ddlYear.DataValueField = "Year"
        ddlYear.DataTextField = "Year"
        ddlYear.DataBind()
        ddlYear.SelectedValue = Year(Now())

        ddlYearEnd.DataSource = reports.GetYears(2010, Year(Now()) + 5)
        ddlYearEnd.DataValueField = "Year"
        ddlYearEnd.DataTextField = "Year"
        ddlYearEnd.DataBind()
        ddlYearEnd.SelectedValue = Year(Now())
        'ddlYear.Attributes.Add("style", "display:hidden;")
        'lblYear.Attributes.Add("style", "display:hidden;")
    End Sub
    Private Sub FillPaymentType()
        ddlPaymentType.DataSource = reports.GetTypeOfPayment(True)
        ddlPaymentType.DataTextField = "PayType"
        ddlPaymentType.DataValueField = "Code"
        ddlPaymentType.DataBind()

        'ddlPaymentType.Attributes.Add("style", "display:hidden;")
        'lblPaymentType.Attributes.Add("style", "display:hidden;")
    End Sub
    Private Sub FillPayer(Region As Object, District As Object)
        ddlPayer.DataSource = reports.GetPayers(Val(Region.SelectedValue), Val(District.SelectedValue), imisgen.getUserId(Session("User")), True)
        ddlPayer.DataValueField = "PayerID"
        ddlPayer.DataTextField = "PayerName"
        ddlPayer.DataBind()
    End Sub
    Private Sub FillPreviousReportsDate()
        Dim dt As DataTable = reports.GetPreviousMatchingFundsReportDates(imisgen.getUserId(Session("User")), If(Val(ddlDistrictWoNational.SelectedValue) = 0, -1, ddlDistrictWoNational.SelectedValue), Nothing)
        ddlPreviousReportDate.DataSource = dt
        ddlPreviousReportDate.DataValueField = "ReportingId"
        ddlPreviousReportDate.DataTextField = "Display"
        ddlPreviousReportDate.DataBind()
    End Sub
    Private Sub FillClaimStatus()
        Dim dt As DataTable = reports.GetClaimStatus(63)
        ddlClaimStatus.DataSource = dt
        ddlClaimStatus.DataTextField = "Status"
        ddlClaimStatus.DataValueField = "Code"
        ddlClaimStatus.DataBind()
        ddlClaimStatus.SelectedValue = 0
    End Sub
    Private Sub FillSorting()
        Dim dt As DataTable = reports.GetSorting
        With ddlSorting
            .DataSource = dt
            .DataTextField = "Sorting"
            .DataValueField = "Code"
            .DataBind()
        End With
    End Sub
    Private Sub GetPremiumCollectionReport(ByVal which As Integer) '  3 - premium collection, 4 - product sales

        Dim RangeFrom As DateTime
        Dim RangeTo As DateTime
        Dim LocationId As Integer?
        If Val(ddlDistrictWoNational.SelectedValue) > 0 Then
            LocationId = Val(ddlDistrictWoNational.SelectedValue)
        Else
            LocationId = Val(ddlRegionWoNational.SelectedValue)
        End If
        If IsDate(Date.ParseExact(txtSTARTData.Text, "dd/MM/yyyy", Nothing)) Then
            RangeFrom = Date.ParseExact(txtSTARTData.Text, "dd/MM/yyyy", Nothing)
        End If

        If IsDate(Date.ParseExact(txtENDData.Text, "dd/MM/yyyy", Nothing)) Then
            RangeTo = Date.ParseExact(txtENDData.Text, "dd/MM/yyyy", Nothing)
        End If

        Dim sSubTitle As String = imisgen.getMessage("L_DATEFROM") & " " & txtSTARTData.Text & " " & imisgen.getMessage("L_TO") & " " & txtENDData.Text

        Dim PaymentType As String = ""
        Select Case ddlPaymentType.SelectedValue
            Case ""
                PaymentType = ""
            Case "C"
                PaymentType = imisgen.getMessage("T_CASH")
            Case "B"
                PaymentType = imisgen.getMessage("T_BANK")
            Case "M"
                PaymentType = imisgen.getMessage("T_MOBILE")
        End Select

        'If Len(Trim(ddlPaymentType.SelectedValue)) > 0 Or ddlDistrict.SelectedValue > 0 Or ddlProduct.SelectedValue > 0 Then
        'sSubTitle = sSubTitle & " Filter("
        If Not Val(ddlRegionWoNational.SelectedValue) = 0 Then
            If Not sSubTitle.EndsWith(" ") Then sSubTitle += ", "
            sSubTitle += imisgen.getMessage("L_REGION") & ": " & ddlRegionWoNational.SelectedItem.Text
        End If

        If Not Val(ddlDistrictWoNational.SelectedValue) = 0 Then
            If Not sSubTitle.EndsWith(" ") Then sSubTitle += ", "
            sSubTitle += imisgen.getMessage("L_District") & ": " & ddlDistrictWoNational.SelectedItem.Text
        End If


        If Not ddlProduct.SelectedValue = 0 Then
            If Not sSubTitle.EndsWith(" ") Then sSubTitle += ", "
            Dim dtProdDetails As DataTable = reports.GetProductName_Account(ddlProduct.SelectedValue)
            Dim ProductName As String = ""
            Dim AccountCode As String = ""
            If Not dtProdDetails Is Nothing AndAlso dtProdDetails.Rows.Count > 0 Then
                ProductName = dtProdDetails(0)("ProductName").ToString
                AccountCode = If(dtProdDetails(0)("AccCodePremiums").ToString.Trim.Length = 0, "...", dtProdDetails(0)("AccCodePremiums").ToString)
            End If
            sSubTitle += imisgen.getMessage("L_PRODUCT") & ": " & ddlProduct.SelectedItem.Text & " - " & ProductName & ", " & imisgen.getMessage("L_ACCCODE") & ": " & AccountCode
        End If
        If Not Len(Trim(PaymentType)) = 0 Then
            If Not sSubTitle.EndsWith(" ") Then sSubTitle += ", "
            sSubTitle += imisgen.getMessage("L_PAYMENTTYPE") & ": " & PaymentType
        End If
        'sSubTitle = sSubTitle & ")"
        ' End If

        IMIS_EN.eReports.SubTitle = sSubTitle

        'Dim dt As New DataTable

        If which = 4 Then
            'Premium Collection
            dt = reports.GetPremiumCollection(LocationId, ddlProduct.SelectedValue, ddlPaymentType.SelectedValue, RangeFrom, RangeTo)
        ElseIf which = 5 Then
            'Policy Sold
            dt = reports.GetPolicySold(LocationId, ddlProduct.SelectedValue, RangeFrom, RangeTo)
        ElseIf which = 24 Then
            'Program Details
            dt = reports.GetProgramDetails(LocationId, ddlProduct.SelectedValue, RangeFrom, RangeTo)
        End If
    End Sub
    Private Sub GetPremiumDistributionReport()

        Dim sSubTitle As String = imisgen.getMessage("L_YEAR") & ": " & ddlYear.SelectedItem.Text
        Dim LocationId As Integer?
        If Val(ddlDistrict.SelectedValue) > 0 Then
            LocationId = Val(ddlDistrict.SelectedValue)
        Else
            LocationId = Val(ddlRegion.SelectedValue)
        End If
        If Val(ddlRegion.SelectedValue) > 0 Or Val(ddlRegion.SelectedValue) = -1 Or ddlProduct.SelectedValue > 0 Then
            If Not Val(ddlRegion.SelectedValue) = 0 Then
                If Not sSubTitle.EndsWith(" ") Then sSubTitle += ", "
                sSubTitle += LocationName
            End If
            If Not ddlProduct.SelectedValue = 0 Then
                If Not sSubTitle.EndsWith(" ") Then sSubTitle += ", "
                sSubTitle += imisgen.getMessage("L_PRODUCT") & ": " & ddlProductStrict.SelectedItem.Text
            End If
        End If
        IMIS_EN.eReports.SubTitle = sSubTitle
        dt = reports.GetPremiumDistribution(LocationId, ddlProductStrict.SelectedValue, ddlMonth.SelectedValue, ddlYear.SelectedValue)



    End Sub
    Private Function GetIndicatorsReportData(ByVal Mode As Int16)
        Dim MonthFrom As Integer = ddlMonthPOI.SelectedValue
        Dim MonthTo As Integer = ddlQuarter.SelectedValue
        Dim Year As Integer = ddlYear.SelectedValue
        Dim sSubTitle As String = ""

        Dim LocationId As Integer?
        If Val(ddlDistrictWoNational.SelectedValue) > 0 Then
            LocationId = Val(ddlDistrictWoNational.SelectedValue)
        Else
            LocationId = Val(ddlRegionWoNational.SelectedValue)
        End If
        'Prepare SubTitle
        Dim ReportPeriod As String = ""

        If MonthFrom = 0 Then
            If MonthTo = 1 Then
                MonthFrom = 1
                MonthTo = 3
                ReportPeriod = imisgen.getMessage("M_QUARTER1")
            ElseIf MonthTo = 2 Then
                MonthFrom = 4
                MonthTo = 6
                ReportPeriod = imisgen.getMessage("M_QUARTER2")
            ElseIf MonthTo = 3 Then
                MonthFrom = 7
                MonthTo = 9
                ReportPeriod = imisgen.getMessage("M_QUARTER3")
            ElseIf MonthTo = 4 Then
                MonthFrom = 10
                MonthTo = 12
                ReportPeriod = imisgen.getMessage("M_QUARTER4")
            Else
                ReportPeriod = imisgen.getMessage("L_YEAR")
            End If
        Else
            MonthTo = MonthFrom
            ReportPeriod = MonthName(MonthFrom)

        End If

        sSubTitle = String.Format(imisgen.getMessage("L_PERIOD") & " : {0} {1}  |   " & imisgen.getMessage("L_REGION") & ": {2}   |   " & imisgen.getMessage("L_DISTRICT") & ": {3}   |   " & imisgen.getMessage("L_PRODUCT") & ": {4}",
                                  ReportPeriod,
                                  Year,
                                  If(Val(ddlRegionWoNational.SelectedValue) = 0, imisgen.getMessage("T_ALL"), ddlRegionWoNational.SelectedItem.Text),
                                  If(Val(ddlDistrictWoNational.SelectedValue) = 0, imisgen.getMessage("T_ALL"), ddlDistrictWoNational.SelectedItem.Text),
                                  ddlProduct.SelectedItem.Text)

        IMIS_EN.eReports.SubTitle = sSubTitle

        dt = reports.GetPrimaryIndicators1(LocationId, ddlProduct.SelectedValue, MonthFrom, Year, Mode, MonthTo)
        Dim ContainsData As Boolean = False
        Dim columns() As String = {"Quarter", "NameOfTheMonth", "ProductCode", "ProductName", "MonthId"}
        For Each c As DataColumn In dt.Columns
            If Not columns.Contains(c.ColumnName) Then
                If dt(0)(c.ColumnName) IsNot DBNull.Value Then
                    ContainsData = True
                    Exit For
                End If
            End If
        Next


        If ContainsData = False Then
            lblMsg.Text = imisgen.getMessage("M_NODATAFORREPORT")
            hfCompleted.Value = 0
            Return False
        End If

        Return True
    End Function
    Private Sub GetPrimaryIndicatorsClaimsData()
        Dim MonthFrom As Integer = ddlMonthPOI.SelectedValue
        Dim MonthTo As Integer = ddlQuarter.SelectedValue

        Dim ReportPeriod As String = ""

        If MonthFrom = 0 Then
            If MonthTo = 1 Then
                MonthFrom = 1
                MonthTo = 3
                ReportPeriod = imisgen.getMessage("M_QUARTER1")
            ElseIf MonthTo = 2 Then
                MonthFrom = 4
                MonthTo = 6
                ReportPeriod = imisgen.getMessage("M_QUARTER2")
            ElseIf MonthTo = 3 Then
                MonthFrom = 7
                MonthTo = 9
                ReportPeriod = imisgen.getMessage("M_QUARTER3")
            ElseIf MonthTo = 4 Then
                MonthFrom = 10
                MonthTo = 12
                ReportPeriod = imisgen.getMessage("M_QUARTER4")
            Else
                ReportPeriod = imisgen.getMessage("L_YEAR")
            End If
        Else
            MonthTo = MonthFrom
            ReportPeriod = MonthName(MonthFrom)

        End If

        Dim Year As Integer = ddlYear.SelectedValue
        Dim sSubTitle As String = ""
        Dim LocationId As Integer?
        If Val(ddlDistrictWoNational.SelectedValue) > 0 Then
            LocationId = Val(ddlDistrictWoNational.SelectedValue)
        Else
            LocationId = Val(ddlRegionWoNational.SelectedValue)
        End If
        ' Dim ReportPeriod As String = ""
        sSubTitle = String.Format(imisgen.getMessage("L_REGION") & " : {0}   |   " & imisgen.getMessage("L_DISTRICT") & " : {1}   |   " & imisgen.getMessage("L_PRODUCT") & ": {2}", ddlRegionWoNational.SelectedItem.Text, ddlDistrictWoNational.SelectedItem.Text, If(Val(ddlAllProducts.SelectedValue) = 0, imisgen.getMessage("T_ALL"), ddlAllProducts.SelectedItem.Text))
        IMIS_EN.eReports.SubTitle = sSubTitle

        dt = reports.GetPrimaryIndicators2(LocationId, ddlAllProducts.SelectedValue, if(ddlHF.SelectedValue = "", 0, ddlHF.SelectedValue), Year, MonthFrom, MonthTo)
    End Sub
    Private Sub CreateDerivedIndicators()
        Dim Month As Integer = ddlMonth.SelectedValue
        Dim Year As Integer = ddlYear.SelectedValue
        Dim sSubTitle As String = ""

        Dim LocationId As Integer?
        If Val(ddlDistrict.SelectedValue) > 0 Then
            LocationId = Val(ddlDistrict.SelectedValue)
        Else
            LocationId = Val(ddlRegion.SelectedValue)
        End If
        
        sSubTitle = If(LocationName.Length > 0, LocationName & " | ", "") & imisgen.getMessage("L_PRODUCT") & ": " & ddlProductStrict.SelectedItem.Text
        If ddlHF.SelectedIndex > 0 Then sSubTitle += " | " & imisgen.getMessage("L_OFFLINEHFID") & " : " & ddlHF.SelectedItem.Text
        IMIS_EN.eReports.SubTitle = sSubTitle
        ds = reports.GetDerivedIndicators(LocationId, ddlProductStrict.SelectedValue, if(ddlHF.SelectedValue.Trim = String.Empty, 0, ddlHF.SelectedValue), Month, Year)
    End Sub
    Private Sub FillUserName()
        ddlUserName.DataSource = reports.GetUsers
        ddlUserName.DataValueField = "UserID"
        ddlUserName.DataTextField = "UserName"
        ddlUserName.DataBind()

    End Sub
    Private Sub GetUserActivityData()
        Dim StartDate As DateTime = txtSTARTData.Text
        Dim EndDate As DateTime = txtENDData.Text
        Dim Action As String = ddlAction.SelectedValue
        Dim Entity As String = ddlEntity.SelectedValue
        Dim sSubTitle As String = ""
        sSubTitle = imisgen.getMessage("L_PERIOD") & " " & imisgen.getMessage("L_FROM") & " " & StartDate & " " & imisgen.getMessage("L_TO") & " " & EndDate & "   " & If(ddlUserName.SelectedIndex <> 0, " | " & imisgen.getMessage("M_FOR") & " : " & ddlUserName.SelectedItem.Text, "") & "   " & If(ddlAction.SelectedIndex = 0, "", " | " & imisgen.getMessage("L_ACTION") & " : " & ddlAction.SelectedItem.Text) & "  " & If(ddlEntity.SelectedIndex = 0, "", " | " & imisgen.getMessage("L_ENTITY") & " : " & ddlEntity.SelectedItem.Text)
        IMIS_EN.eReports.SubTitle = sSubTitle

        dt = reports.GetUserActivityData(ddlUserName.SelectedValue, StartDate, EndDate, Action, Entity)

        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            lblMsg.Text = imisgen.getMessage("M_NODATAFORREPORT")
            hfCompleted.Value = 0
        End If

    End Sub

    Private Sub GetStatusofRegistersData()
        Dim sSubTitle As String = "" 'imisgen.getMessage("R_ALL") & " " & imisgen.getMessage("L_DISTRICT")
        'If ddlRegion.SelectedIndex > 0 Then sSubTitle = LocationName

        If Val(ddlAllProducts.SelectedValue) = 0 Then
            lblMsg.Text = imisgen.getMessage("V_PRODUCTNAME")
            Return
        End If

        Dim PragramCode As String
        If Val(ddlAllProducts.SelectedValue) > 0 Then
            PragramCode = ddlAllProducts.SelectedItem.Text
        End If

        If Val(ddlDistrictWoNational.SelectedValue) > 0 Then
            sSubTitle += PragramCode + " Report"
        End If

        IMIS_EN.eReports.SubTitle = sSubTitle

        dt = reports.GetStatusofRegisters(PragramCode)
    End Sub
    Private Sub GetInsureesWithoutphotos()
        Dim sSubTitle As String = ""
        Dim LocationId As Integer?
        If Val(ddlDistrictWoNational.SelectedValue) > 0 Then
            LocationId = Val(ddlDistrictWoNational.SelectedValue)
        Else
            LocationId = Val(ddlRegionWoNational.SelectedValue)
        End If

        'Create Subtitle
        If ddlRegionWoNational.SelectedValue <> 0 Then
            sSubTitle = imisgen.getMessage("L_REGION") & ": " & ddlRegionWoNational.SelectedItem.Text
        End If

        If Val(ddlDistrictWoNational.SelectedValue) > 0 Then
            sSubTitle += "  |   " & imisgen.getMessage("L_DISTRICT") & ": " & ddlDistrictWoNational.SelectedItem.Text
        End If
        If Val(ddlEnrolmentOfficer.SelectedValue) > 0 Then
            sSubTitle += "  |   " & imisgen.getMessage("L_ENROLMENTOFFICERS") & ": " & ddlEnrolmentOfficer.SelectedItem.Text
        End If

        IMIS_EN.eReports.SubTitle = sSubTitle
        dt = reports.GetInsureesWithoutPhotos(Val(ddlEnrolmentOfficer.SelectedValue), LocationId)

    End Sub
    Private Sub GetProgramBeefiaciaryListData()
        Dim sSubTitle As String = "" 'imisgen.getMessage("R_ALL") & " " & imisgen.getMessage("L_DISTRICT")
        'If ddlRegion.SelectedIndex > 0 Then sSubTitle = LocationName

        If Val(ddlProduct.SelectedValue) = 0 Then
            lblMsg.Text = imisgen.getMessage("V_PRODUCTNAME")
            Return
        End If

        Dim PragramCode As String
        If Val(ddlProduct.SelectedValue) > 0 Then
            PragramCode = ddlProduct.SelectedItem.Text
        End If

        If Val(ddlDistrictWoNational.SelectedValue) > 0 Then
            sSubTitle += PragramCode + " Report"
        End If

        IMIS_EN.eReports.SubTitle = sSubTitle

        dt = reports.GetBeneficiaryList(PragramCode)
    End Sub
    Private Sub GetPaymentCategoryOverview()
        Dim DateFrom As DateTime = txtSTARTData.Text
        Dim DateTo As DateTime = txtENDData.Text

        Dim LocationId As Integer?
        If Val(ddlDistrictWoNational.SelectedValue) > 0 Then
            LocationId = Val(ddlDistrictWoNational.SelectedValue)
        Else
            LocationId = Val(ddlRegionWoNational.SelectedValue)
        End If
        'changed by amani 22/02/2018 to fix isuue with missing space
        Dim sSubTitle As String = If(LocationName.Length > 0, LocationName & " | ", "") & imisgen.getMessage("L_DATEFROM") & " " & DateFrom & " " & imisgen.getMessage("L_DATETO") & " " & DateTo
        IMIS_EN.eReports.SubTitle = sSubTitle
        dt = reports.GetPaymentCategoryOverview(DateFrom, DateTo, LocationId, ddlProduct.SelectedValue)
    End Sub

    Private Function GetMatchingFunds() As Boolean
        Dim DistrictID As Integer?
        Dim ProdID As Integer?
        Dim PayerID As Integer?
        Dim StartDate As Date?
        Dim EndDate As Date?

        Dim LocationId As Integer?
       

        Dim ReportingID As Integer? = if(ddlPreviousReportDate.SelectedValue > 0, CInt(ddlPreviousReportDate.SelectedValue), Nothing)
        If ReportingID = 0 Then
            DistrictID = If(Val(ddlDistrictWoNational.SelectedValue) > 0, CInt(Val(ddlDistrictWoNational.SelectedValue)), Nothing)
            ProdID = If(ddlProduct.SelectedValue > 0, CInt(ddlProduct.SelectedValue), Nothing)
            If ddlPayer.SelectedIndex > 0 Then PayerID = ddlPayer.SelectedValue
            StartDate = If(IsDate(txtSTARTData.Text), Date.ParseExact(txtSTARTData.Text, "dd/MM/yyyy", Nothing), Nothing)
            EndDate = If(IsDate(txtENDData.Text), Date.ParseExact(txtENDData.Text, "dd/MM/yyyy", Nothing), Nothing)
            If Val(ddlDistrictWoNational.SelectedValue) > 0 Then
                LocationId = Val(ddlDistrictWoNational.SelectedValue)
            Else
                LocationId = Val(ddlRegionWoNational.SelectedValue)
            End If
        End If
        Dim ErrorMessage As String = ""
        Dim oReturn As Integer = -1
        dt = reports.GetMatchingFunds(LocationId, ProdID, PayerID, StartDate, EndDate, ReportingID, ErrorMessage, oReturn)
        If oReturn = -1 Then
            imisgen.Alert(imisgen.getMessage("M_ERRORMESSAGE"), pnlTop, alertPopupTitle:="NAFA IMIS")
            Return False
        ElseIf oReturn = 1 Then
            imisgen.Alert(imisgen.getMessage("M_PLEASESELECTADISTRICT"), pnlTop, alertPopupTitle:="NAFA IMIS")
            Return False
        ElseIf oReturn = 2 Then
            imisgen.Alert(imisgen.getMessage("M_PLEASESELECTAPRODUCT"), pnlTop, alertPopupTitle:="NAFA IMIS")
            Return False
        ElseIf oReturn = 3 Then
            imisgen.Alert(imisgen.getMessage("M_INVALIDDATE"), pnlTop, alertPopupTitle:="NAFA IMIS")
            Return False
        ElseIf oReturn = 4 Then
            imisgen.Alert(imisgen.getMessage("M_INVALIDDATE"), pnlTop, alertPopupTitle:="NAFA IMIS")
            Return False
        End If
        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
            If ReportingID IsNot Nothing Then
                Dim dtRep = reports.GetPreviousMatchingFundsReportDates(imisgen.getUserId(Session("User")), 0, ReportingID)
                If dtRep IsNot Nothing AndAlso dtRep.Rows.Count > 0 Then
                    StartDate = dtRep.Rows(0)("StartDate")
                    EndDate = dtRep.Rows(0)("EndDate")
                End If
            End If
            IMIS_EN.eReports.SubTitle = imisgen.getMessage("L_PRODUCT") & " : " & dt.Rows(0)("ProductCode") & " - " & dt.Rows(0)("ProductName") & " | " & imisgen.getMessage("L_REGION") & " : " & ddlRegionWoNational.SelectedItem.Text & " | " & imisgen.getMessage("L_DISTRICT") & " : " & dt.Rows(0)("DistrictName") & " | " & imisgen.getMessage("L_PERIOD") & " " & imisgen.getMessage("L_FROM") & " " & StartDate & " " & imisgen.getMessage("L_TO") & " " & EndDate
        Else
            lblMsg.Text = imisgen.getMessage("M_NODATAFORREPORT")
            hfCompleted.Value = 0
            Return False
        End If
        Return True
    End Function
    Private Function GetClaimOverview() As Boolean
        Dim DistrictID As Integer?
        Dim ProdID As Integer?
        Dim HfID As Integer?
        Dim StartDate As Date?
        Dim EndDate As Date?
        Dim ClaimStatus As Integer?
        DistrictID = If(Val(ddlDistrictWoNational.SelectedValue) > 0, CInt(Val(ddlDistrictWoNational.SelectedValue)), Nothing)
        ProdID = If(Val(ddlAllProducts.SelectedValue) > 0, CInt(ddlAllProducts.SelectedValue), Nothing)
        HfID = If(Val(ddlHF.SelectedValue) > 0, CInt(ddlHF.SelectedValue), Nothing)
        StartDate = If(IsDate(txtSTARTData.Text), Date.ParseExact(txtSTARTData.Text, "dd/MM/yyyy", Nothing), Nothing)
        EndDate = If(IsDate(txtENDData.Text), Date.ParseExact(txtENDData.Text, "dd/MM/yyyy", Nothing), Nothing)
        If ddlClaimStatus.SelectedIndex > 0 Then ClaimStatus = ddlClaimStatus.SelectedValue
        'ClaimStatus = If(ddlClaimStatus.SelectedIndex > 0, ddlClaimStatus.SelectedValue, Nothing)


        dt = reports.GetClaimOverview(DistrictID, ProdID, HfID, StartDate, EndDate, ClaimStatus)
        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
            IMIS_EN.eReports.SubTitle = imisgen.getMessage("L_HFACILITY") & " : " & dt.Rows(0)("HFCode") & " - " & dt.Rows(0)("HFName") & If(ddlAllProducts.SelectedValue > 0, " | " & imisgen.getMessage("L_PRODUCT") & " : " & ddlAllProducts.SelectedItem.Text, "") & " | " & LocationName & " | " & imisgen.getMessage("L_PERIOD") & "  " & imisgen.getMessage("L_FROM") & " " & StartDate & " " & imisgen.getMessage("L_TO") & " " & EndDate
        Else
            lblMsg.Text = imisgen.getMessage("M_NODATAFORREPORT")
            hfCompleted.Value = 0
            Return False
        End If
        Return True
    End Function
    Private Function GetPercentageReferral() As Boolean
        ' Dim DistrictId As Integer?
        Dim StartDate As Date?
        Dim EndDate As Date?

         
        'DistrictId = if(ddlDistrict1.SelectedValue > 0, CInt(ddlDistrict1.SelectedValue), Nothing)
        StartDate = if(IsDate(txtSTARTData.Text), Date.ParseExact(txtSTARTData.Text, "dd/MM/yyyy", Nothing), Nothing)
        EndDate = if(IsDate(txtENDData.Text), Date.ParseExact(txtENDData.Text, "dd/MM/yyyy", Nothing), Nothing)

        dt = reports.GetPercentageReferral(Val(ddlRegion.SelectedValue), Val(ddlDistrict.SelectedValue), StartDate, EndDate)

        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
            'IMIS_EN.eReports.SubTitle = imisgen.getMessage("L_DISTRICT") & " : " & dt.Rows(0)("HFCode") & " - " & dt.Rows(0)("HFName") & " | " & imisgen.getMessage("L_PRODUCT") & " : " & ddlProduct.SelectedItem.Text & " | " & imisgen.getMessage("L_DISTRICT") & " : " & ddlDistrict.SelectedItem.Text & " | " & imisgen.getMessage("L_PERIOD") & " " & imisgen.getMessage("L_FROM") & " " & StartDate & " " & imisgen.getMessage("L_TO") & " " & EndDate
            IMIS_EN.eReports.SubTitle = If(Val(ddlRegion.SelectedValue) > 0 Or Val(ddlRegion.SelectedValue) = -1, imisgen.getMessage("L_REGION") & " : " & ddlRegion.SelectedItem.Text & " | ", "") & If(Val(ddlDistrict.SelectedValue) > 0, imisgen.getMessage("L_DISTRICT") & " : " & ddlDistrict.SelectedItem.Text & " | ", "") & imisgen.getMessage("L_PERIOD") & " " & imisgen.getMessage("L_FROM") & " " & StartDate & " " & imisgen.getMessage("L_TO") & " " & EndDate
        Else
            lblMsg.Text = imisgen.getMessage("M_NODATAFORREPORT")
            hfCompleted.Value = 0
            Return False
        End If
        Return True

    End Function
    Private Function GetFamiliesInsureesOverview() As Boolean
        Dim StartDate As Date
        Dim EndDate As Date
        Dim PolicyStatus As Integer?
        Dim LocationId As Integer? = Nothing
        If Val(ddlRegionWoNational.SelectedValue) > 0 Then
            LocationId = ddlRegionWoNational.SelectedValue
        End If
        If Val(ddlDistrictWoNational.SelectedValue) > 0 Then
            LocationId = ddlDistrictWoNational.SelectedValue
        End If
        If Val(ddlWards.SelectedValue) > 0 Then
            LocationId = ddlWards.SelectedValue
        End If
        If Val(ddlVillages.SelectedValue) > 0 Then
            LocationId = ddlVillages.SelectedValue
        End If

        'DistrictId = if(Val(ddlDistrict.SelectedValue) > 0, CInt(Val(ddlDistrict.SelectedValue)), Nothing)
        'If ddlWards.Items.Count > 0 Then WardId = if(ddlWards.SelectedValue > 0, CInt(ddlWards.SelectedValue), Nothing)
        'If ddlVillages.Items.Count > 0 Then VillageId = if(ddlVillages.SelectedValue > 0, CInt(ddlVillages.SelectedValue), Nothing)
        StartDate = if(IsDate(txtSTARTData.Text), Date.ParseExact(txtSTARTData.Text, "dd/MM/yyyy", Nothing), Nothing)
        EndDate = if(IsDate(txtENDData.Text), Date.ParseExact(txtENDData.Text, "dd/MM/yyyy", Nothing), Nothing)

        If ddlPolicyStatus.SelectedValue > 0 Then PolicyStatus = ddlPolicyStatus.SelectedValue

        dt = reports.GetEnroledFamilies(LocationId, StartDate, EndDate, PolicyStatus)

        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
            IMIS_EN.eReports.SubTitle = LocationName & "   " & If(PolicyStatus Is Nothing, "", imisgen.getMessage("L_STATUS") & " : " & ddlPolicyStatus.SelectedItem.Text) & "   " & imisgen.getMessage("L_PERIOD") & " " & imisgen.getMessage("L_FROM") & " " & StartDate & " " & imisgen.getMessage("L_TO") & " " & EndDate
            Return True
        Else
            lblMsg.Text = imisgen.getMessage("M_NODATAFORREPORT")
            hfCompleted.Value = 0
            Return False
        End If

    End Function
    Private Function GetPendingInsurees()
        ' Dim DistrictId As Integer
        Dim OfficerId As Integer?
        Dim StartDate As Date
        Dim EndDate As Date
        Dim LocationId As Integer?

        If Val(ddlDistrictWoNational.SelectedValue) > 0 Then
            LocationId = Val(ddlDistrictWoNational.SelectedValue)
        Else
            LocationId = Val(ddlRegionWoNational.SelectedValue)
        End If

        'DistrictId = if(Val(ddlDistrict.SelectedValue) > 0, CInt(Val(ddlDistrict.SelectedValue)), Nothing)
        OfficerId = If(Val(ddlEnrolmentOfficer.SelectedValue) > 0, CInt(ddlEnrolmentOfficer.SelectedValue), Nothing)
        StartDate = if(IsDate(txtSTARTData.Text), Date.ParseExact(txtSTARTData.Text, "dd/MM/yyyy", Nothing), Nothing)
        EndDate = if(IsDate(txtENDData.Text), Date.ParseExact(txtENDData.Text, "dd/MM/yyyy", Nothing), Nothing)

        dt = reports.GetPendingInsurees(LocationId, OfficerId, StartDate, EndDate)

        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
            IMIS_EN.eReports.SubTitle = LocationName & " | " & imisgen.getMessage("L_PERIOD") & " " & imisgen.getMessage("L_FROM") & " " & StartDate & " " & imisgen.getMessage("L_TO") & " " & EndDate
            Return True
        Else
            lblMsg.Text = imisgen.getMessage("M_NODATAFORREPORT")
            hfCompleted.Value = 0
            Return False
        End If

        Return dt

    End Function
    Private Function GetRenewals()
        Dim DistrictId As Integer
        Dim ProductId As Integer
        Dim OfficerId As Integer?
        Dim StartDate As Date
        Dim EndDate As Date
        Dim ProductName As String = ""
        Dim subTitle As String = ""

        Dim LocationId As Integer?
        If Val(ddlDistrictWoNational.SelectedValue) > 0 Then
            LocationId = Val(ddlDistrictWoNational.SelectedValue)
        Else
            LocationId = Val(ddlRegionWoNational.SelectedValue)
        End If

        DistrictId = If(Val(ddlDistrictWoNational.SelectedValue) > 0, CInt(Val(ddlDistrictWoNational.SelectedValue)), Nothing)
        ProductId = If(ddlProduct.SelectedValue > 0, CInt(ddlProduct.SelectedValue), Nothing)

        If Val(ddlEnrolmentOfficer.SelectedValue) > 0 Then
            OfficerId = ddlEnrolmentOfficer.SelectedValue
        Else
            OfficerId = Nothing
        End If
        StartDate = If(IsDate(txtSTARTData.Text), Date.ParseExact(txtSTARTData.Text, "dd/MM/yyyy", Nothing), Nothing)
        EndDate = If(IsDate(txtENDData.Text), Date.ParseExact(txtENDData.Text, "dd/MM/yyyy", Nothing), Nothing)

        dt = reports.GetRenewals(LocationId, ProductId, OfficerId, StartDate, EndDate, ddlSorting.SelectedValue)

        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            lblMsg.Text = imisgen.getMessage("M_NODATAFORREPORT")
            hfCompleted.Value = 0
            Return False
        End If

        If ProductId > 0 Then
            Dim dtProdDetails As DataTable = reports.GetProductName_Account(ddlProduct.SelectedValue)
            If Not dtProdDetails Is Nothing AndAlso dtProdDetails.Rows.Count > 0 Then
                ProductName = dtProdDetails(0)("ProductName").ToString
            End If
            subTitle += imisgen.getMessage("L_PRODUCT") & ": " & ddlProduct.SelectedItem.Text & " - " & ProductName
        End If


        subTitle += If(LocationName.Length > 0, "   |   " & LocationName, "")


        subTitle += " | " & imisgen.getMessage("L_PERIOD") & " " & imisgen.getMessage("L_FROM") & " " & StartDate & " " & imisgen.getMessage("L_TO") & " " & EndDate

        IMIS_EN.eReports.SubTitle = subTitle

        Return True
    End Function

    Private Function getRejectedPhoto()

        Dim StartDate As Date
        Dim EndDate As Date


        StartDate = If(IsDate(txtSTARTData.Text), Date.ParseExact(txtSTARTData.Text, "dd/MM/yyyy", Nothing), Nothing)
        EndDate = If(IsDate(txtENDData.Text), Date.ParseExact(txtENDData.Text, "dd/MM/yyyy", Nothing), Nothing)

        dt = reports.getRejectedPhoto(StartDate, EndDate)

        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
            IMIS_EN.eReports.SubTitle = imisgen.getMessage("L_FROM") & " " & StartDate & " " & imisgen.getMessage("L_TO") & " " & EndDate
            Return True
        Else
            lblMsg.Text = imisgen.getMessage("M_NODATAFORREPORT")
            hfCompleted.Value = 0
            Return False
        End If

        Return dt
    End Function

    Private Function getCapitationPayment()
        Dim sSubTitle As String = ""
        Dim dtCapitation As DataTable
        ' Dim dtHFLevel As New DataTable
        '  \dtHFLevel.Columns.Add("Code")
        'dtHFLevel.Columns.Add("Name")

        If Val(ddlProductStrict.SelectedValue) > 0 Then
            dtCapitation = reports.getProductCapitationDetails(ddlProductStrict.SelectedValue)

            If dtCapitation.Rows.Count Then
                IMIS_EN.eReports.Level1 = getHFName(dtCapitation.Rows(0)("Level1").ToString)
                IMIS_EN.eReports.Sublevel1 = dtCapitation.Rows(0)("HFSublevel1").ToString
                IMIS_EN.eReports.Level2 = getHFName(dtCapitation.Rows(0)("Level2").ToString)
                IMIS_EN.eReports.Sublevel2 = dtCapitation.Rows(0)("HFSublevel2").ToString
                IMIS_EN.eReports.Level3 = getHFName(dtCapitation.Rows(0)("Level3").ToString)
                IMIS_EN.eReports.Sublevel3 = dtCapitation.Rows(0)("HFSublevel3").ToString
                IMIS_EN.eReports.Level4 = getHFName(dtCapitation.Rows(0)("Level4").ToString)
                IMIS_EN.eReports.Sublevel4 = dtCapitation.Rows(0)("HFSublevel4").ToString
                IMIS_EN.eReports.ShareContribution = If(dtCapitation.Rows(0)("ShareContribution") Is DBNull.Value, 0, dtCapitation.Rows(0)("ShareContribution"))
                IMIS_EN.eReports.WeightPopulation = If(dtCapitation.Rows(0)("WeightPopulation") Is DBNull.Value, 0, dtCapitation.Rows(0)("WeightPopulation"))
                IMIS_EN.eReports.WeightNumberFamilies = If(dtCapitation.Rows(0)("WeightNumberFamilies") Is DBNull.Value, 0, dtCapitation.Rows(0)("WeightNumberFamilies"))
                IMIS_EN.eReports.WeightInsuredPopulation = If(dtCapitation.Rows(0)("WeightInsuredPopulation") Is DBNull.Value, 0, dtCapitation.Rows(0)("WeightInsuredPopulation"))
                IMIS_EN.eReports.WeightNumberInsuredFamilies = If(dtCapitation.Rows(0)("WeightNumberInsuredFamilies") Is DBNull.Value, 0, dtCapitation.Rows(0)("WeightNumberInsuredFamilies"))
                IMIS_EN.eReports.WeightNumberVisits = If(dtCapitation.Rows(0)("WeightNumberVisits") Is DBNull.Value, 0, dtCapitation.Rows(0)("WeightNumberVisits"))
                IMIS_EN.eReports.WeightAdjustedAmount = If(dtCapitation.Rows(0)("WeightAdjustedAmount") Is DBNull.Value, 0, dtCapitation.Rows(0)("WeightAdjustedAmount"))

            End If

            '   lblCatchmentArea.Text = dt.Rows(0)("Catchment")
        End If
        dt = reports.getCatchmentArea(Val(ddlRegion.SelectedValue), Val(ddlDistrict.SelectedValue), Val(ddlProductStrict.SelectedValue), ddlYear.SelectedValue, ddlMonth.SelectedValue)
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            lblMsg.Text = imisgen.getMessage("M_NODATAFORREPORT")
            hfCompleted.Value = 0
            Return False
        End If
        If Val(ddlRegion.SelectedValue) > 0 Or Val(ddlRegion.SelectedValue) = -1 Or ddlProductStrict.SelectedValue > 0 Then
            If Not Val(ddlRegion.SelectedValue) = 0 Then
                'If Not sSubTitle.EndsWith(" ") Then sSubTitle += ", "
                sSubTitle += LocationName
            End If
        End If
        If Not ddlProductStrict.SelectedValue = 0 Then
            If Not sSubTitle.EndsWith(" ") Then sSubTitle += ", "
            Dim dtProdDetails As DataTable = reports.GetProductName_Account(Val(ddlProductStrict.SelectedValue))
            Dim ProductName As String = ""

            If Not dtProdDetails Is Nothing AndAlso dtProdDetails.Rows.Count > 0 Then
                ProductName = dtProdDetails(0)("ProductName").ToString
            End If
            sSubTitle += imisgen.getMessage("L_PRODUCT") & ": " & ddlProductStrict.SelectedItem.Text & " - " & ProductName & ", " '& imisgen.getMessage("L_PRODUCTCODE") & ": " & AccountCode
            'sSubTitle += imisgen.getMessage("L_CODE") & ": " & ddlProductStrict.SelectedItem.Text
        End If


        If Not ddlMonth.SelectedValue = 0 Then
            If Not sSubTitle.EndsWith(" ") Then sSubTitle += ", "
            sSubTitle += imisgen.getMessage("L_MONTH") & ": " & ddlMonth.SelectedItem.Text
        End If
        If Not ddlYear.SelectedValue = 0 Then
            If Not sSubTitle.EndsWith(" ") Then sSubTitle += ", "
            sSubTitle += imisgen.getMessage("L_YEAR") & ": " & ddlYear.SelectedItem.Text
        End If


        IMIS_EN.eReports.SubTitle = sSubTitle
        Return True
    End Function
    Private Function getHFName(ByVal Code As String)
        Select Case Code
            Case "H"
                Return imisgen.getMessage("T_HOSPITAL")
            Case "C"
                Return imisgen.getMessage("T_HEALTHCENTRE")
            Case "D"
                Return imisgen.getMessage("T_DISPENSARY")
            Case Else
                Return ""
        End Select

    End Function

    Private Sub CacheCriteria()
        If Not hfCriteriaCache.Value = "#" Then
            Session("CriteriaCache") = hfCriteriaCache.Value
        End If

    End Sub
    Private Sub btnProcess_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnPreview.Click
        RunPageSecurity()
        Dim url As String = ""
        Try

            CacheCriteria()

            If (lstboxReportSelector.SelectedValue.Length <= 0) Then
                lstboxReportSelector.SelectedValue = 1
            End If
            Dim SelectedValueID As Integer = lstboxReportSelector.SelectedValue

            'If SelectedValueID = 3 Then
            '    If Val(ddlRegion.SelectedValue) = 0 Then
            '        lblMsg.Text = imisgen.getMessage("M_PLEASESELECTAREGION")
            '        Return
            '    End If
            'End If

            'If SelectedValueID = 2 Then
            '    If Val(ddlRegionWoNational.SelectedValue) = 0 Then
            '        lblMsg.Text = imisgen.getMessage("M_PLEASESELECTAREGION")
            '        Return
            '    End If
            'End If

            '  Dim LocationId As Integer?

            '********************

            If hfVisibleRegion.Value = ddlRegion.ClientID Then
                If (Val(ddlRegion.SelectedValue) > 0 Or Val(ddlRegion.SelectedValue) = -1) Then LocationName = imisgen.getMessage("L_REGION") & ": " & ddlRegion.SelectedItem.Text
                If Val(ddlDistrict.SelectedValue) > 0 Then LocationName += " | " & imisgen.getMessage("L_DISTRICT") & ": " & ddlDistrict.SelectedItem.Text

            ElseIf hfVisibleRegion.Value = ddlRegionWoNational.ClientID Or SelectedValueID = 0 Or SelectedValueID = 1 Then
                If Val(ddlRegionWoNational.SelectedValue) > 0 Then LocationName = imisgen.getMessage("L_REGION") & " : " & ddlRegionWoNational.SelectedItem.Text
                If Val(ddlDistrictWoNational.SelectedValue) > 0 Then LocationName += " | " & imisgen.getMessage("L_DISTRICT") & " : " & ddlDistrictWoNational.SelectedItem.Text

            End If

            '****************


            If SelectedValueID = 1 Then
                GetProgramBeefiaciaryListData()
                Session("Report") = dt
                url = "Report.aspx?r=br&tid=5"
            ElseIf SelectedValueID = 2 Then
                GetPostPaymentPayrollReportData(1)
                Session("Report") = dt
                url = "Report.aspx?r=ppr&tid=3"
            ElseIf SelectedValueID = 3 Then
                GetPostPaymentPayrollReportData(0)
                Session("Report") = dt
                url = "Report.aspx?r=pprUC&tid=4"
            ElseIf SelectedValueID = 4 Then
                GetConsolidatedReportData(1)
                Session("Report") = dt
                url = "Report.aspx?r=crp&tid=5"
            ElseIf SelectedValueID = 5 Then
                GetConsolidatedReportData(0)
                Session("Report") = dt
                url = "Report.aspx?r=dcrp&tid=6"
            ElseIf SelectedValueID = 6 Then
                GetConsolidatedApprovedReportData(0)
                Session("Report") = dt
                url = "Report.aspx?r=carp&tid=7"
            ElseIf SelectedValueID = 7 Then
                GetConsolidatedApprovedReportData(0)
                Session("Report") = dt
                url = "Report.aspx?r=abrp&tid=8"
            ElseIf SelectedValueID = 8 Then
                GetUserActivityData()
                Session("Report") = dt
                url = "Report.aspx?r=ua&tid=9"

            ElseIf SelectedValueID = 9 Then
                GetStatusofRegistersData()
                Session("Report") = dt
                url = "Report.aspx?r=benR&tid=9"
            ElseIf SelectedValueID = 10 Then
                GetInsureesWithoutphotos()
                Session("Report") = dt
                url = "Report.aspx?r=iwp&tid=10"
            ElseIf SelectedValueID = 11 Then
                GetPaymentCategoryOverview()
                Session("Report") = dt
                url = "Report.aspx?r=pco&tid=11"
            ElseIf SelectedValueID = 12 Then
                If Not GetMatchingFunds() Then Exit Sub
                Session("Report") = dt
                url = "Report.aspx?r=mf&tid=12"
            ElseIf SelectedValueID = 13 Then
                If Not GetClaimOverview() Then Exit Sub
                Session("Report") = dt
                url = "Report.aspx?r=co&tid=13"
            ElseIf SelectedValueID = 14 Then
                If Not GetPercentageReferral() Then Exit Sub
                Session("Report") = dt
                url = "Report.aspx?r=pr&tid=14"
            ElseIf SelectedValueID = 15 Then
                If Not GetFamiliesInsureesOverview() Then Exit Sub
                Session("Report") = dt
                url = "Report.aspx?r=fio&tid=15"
            ElseIf SelectedValueID = 16 Then
                If Not GetPendingInsurees() Then Exit Sub
                Session("Report") = dt
                url = "Report.aspx?r=pi&tid=16"
            ElseIf SelectedValueID = 17 Then
                If Not GetRenewals() Then Exit Sub
                Session("Report") = dt
                url = "Report.aspx?r=rnw&tid=17"
            ElseIf SelectedValueID = 18 Then
                If Not getCapitationPayment() Then Exit Sub
                Session("Report") = dt
                url = "Report.aspx?r=ca&tid=18"
            ElseIf SelectedValueID = 19 Then
                If Not getRejectedPhoto() Then Exit Sub
                Session("Report") = dt
                url = "Report.aspx?r=rp&tid=19"
            ElseIf SelectedValueID = 20 Then
                GetBeneficiaryReportData()
                Session("Report") = dt
                url = "Report.aspx?r=sr&tid=9"
                'GetBeneficiaryList()
                'Session("Report") = dt
                'url = "Report.aspx?r=brl&tid=20"
            End If


        Catch ex As Exception
            lblMsg.Text = ex.Message
            If Not ex.Message = imisgen.getMessage("M_NODATAFORREPORT") Then
                'lblMsg.Text = imisgen.getMessage("M_ERRORMESSAGE")
                imisgen.Alert(imisgen.getMessage("M_ERRORMESSAGE"), pnlButtons, alertPopupTitle:="NAFA IMIS")
                'EventLog.WriteEntry("IMIS", Page.Title & " : " & imisgen.getLoginName(Session("User")) & " : " & ex.Message, EventLogEntryType.Error, 999)
            End If
            hfCompleted.Value = 0
            Return
        End Try
        Response.Redirect(url)
    End Sub
    Private Sub ddlDistrict_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlDistrict.SelectedIndexChanged
        FillProducts()
        FillHF(ddlDistrict)
        FillEnrolmentOfficer(sender)
        FillPayer(ddlRegion, ddlDistrict)
        'FillPreviousReportsDate()
        HideCriteriaControls()
        '  FillWards()
    End Sub
    Private Sub ddlQuarter_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlQuarter.SelectedIndexChanged
        Try
            QuarterSelector()
        Catch ex As Exception
            'EventLog.WriteEntry("IMIS", imisgen.getUserId(Session("User")) & " : " & ex.Message, EventLogEntryType.Information, 5, 3)
            imisgen.Alert(imisgen.getMessage("M_ERRORMESSAGE"), pnlButtons, alertPopupTitle:="NAFA IMIS")
        End Try
    End Sub
    Private Sub QuarterSelector()
        If ddlQuarter.SelectedValue = 1 Then
            FillMonthPOI(1, 3)
        ElseIf ddlQuarter.SelectedValue = 2 Then
            FillMonthPOI(4, 6)
        ElseIf ddlQuarter.SelectedValue = 3 Then
            FillMonthPOI(7, 9)
        ElseIf ddlQuarter.SelectedValue = 4 Then
            FillMonthPOI(10, 12)
        Else
            FillMonthPOI(1, 12)
        End If
    End Sub

    Private Sub B_CANCEL_Click(sender As Object, e As EventArgs) Handles B_CANCEL.Click
        Response.Redirect("Home.aspx")
    End Sub

    Private Sub ddlWards_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlWards.SelectedIndexChanged
            FillVillages()
    End Sub
    Private Sub FillPolicyStatus()
        ddlPolicyStatus.DataSource = reports.GetPolicyStatus(True)
        ddlPolicyStatus.DataValueField = "Code"
        ddlPolicyStatus.DataTextField = "Status"
        ddlPolicyStatus.DataBind()
    End Sub
    Private Sub FillActions()
        Dim dt As DataTable = reports.GetActions
        With ddlAction
            .DataSource = dt
            .DataTextField = "Action"
            .DataValueField = "ActionId"
            .DataBind()
            .SelectedIndex = 0
        End With
    End Sub

    Private Sub FillEntities()
        Dim dt As DataTable = reports.GetEntities
        With ddlEntity
            .DataSource = dt
            .DataTextField = "Entity"
            .DataValueField = "EntityId"
            .DataBind()
            .SelectedIndex = 0
        End With
    End Sub

  
    Private Sub ddlVillages_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlVillages.SelectedIndexChanged
        'FillEnrolmentOfficer()
    End Sub

    Private Sub ddlRegion_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlRegion.SelectedIndexChanged
        FillDistricts()
        FillHF(sender)
    End Sub

    Private Sub ddlRegionWoNational_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlRegionWoNational.SelectedIndexChanged
        FillDistrictsWoNational()
        FillHF(sender)
    End Sub

    Private Sub ddlDistrictWoNational_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlDistrictWoNational.SelectedIndexChanged
        FillPreviousReportsDate()
        FillEnrolmentOfficer(sender)
        FillWards()
        FillHF(sender)
        FillProducts()
    End Sub

    Private Sub ddlProductStrict_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlProductStrict.SelectedIndexChanged
        Try
            'getCapitationDetails()
        Catch ex As Exception

        End Try
    End Sub


    Private Sub GetBeneficiaryReportData()
        Dim sSubTitle As String = "" 'imisgen.getMessage("R_ALL") & " " & imisgen.getMessage("L_DISTRICT")
        'If ddlRegion.SelectedIndex > 0 Then sSubTitle = LocationName

        Dim SartDate As String = ddlMonth.SelectedItem.Text + " " + ddlYear.SelectedItem.Text
        Dim EndDate As String = ddlMonthPOIEnd.SelectedItem.Text + " " + ddlYearEnd.SelectedItem.Text
        Dim programCode As String = ddlAllProducts.SelectedItem.Text

        Dim LocationId As Integer?
        If Val(ddlRegion.SelectedValue) > 0 Then
            LocationId = Val(ddlRegion.SelectedValue)
        Else
            LocationId = Val(ddlRegion.SelectedValue)
        End If

        If Val(ddlRegionWoNational.SelectedValue) = 0 Then
            sSubTitle = imisgen.getMessage("L_REGION") & ": " & imisgen.getMessage("T_ALL")
        End If

        If Val(ddlRegionWoNational.SelectedValue) <> 0 Then
            sSubTitle = imisgen.getMessage("L_REGION") & ": " & ddlRegionWoNational.SelectedItem.Text
        End If
        If Val(ddlDistrictWoNational.SelectedValue) > 0 Then
            sSubTitle += "  |   " & imisgen.getMessage("L_DISTRICT") & ": " & SartDate + " " + EndDate
        End If

        Dim subtitleMsg = ddlRegion.SelectedItem.Text + " : " + programCode + ": " & SartDate + " TO " + EndDate
        IMIS_EN.eReports.SubTitle = subtitleMsg
        'dt = reports.GetStatusofRegisters(LocationId) 

        dt = reports.GetBeneficiaryReport(LocationId, SartDate, EndDate, programCode)

    End Sub

    Private Sub GetPostPaymentPayrollReportData(ByVal creditType As Integer)
        Dim sSubTitle As String = ""

        Dim SartDate As String = ddlMonth.SelectedItem.Text + " " + ddlYear.SelectedItem.Text
        Dim EndDate As String = ddlMonthPOIEnd.SelectedItem.Text + " " + ddlYearEnd.SelectedItem.Text
        Dim programCode As String = ddlAllProducts.SelectedItem.Text

        Dim LocationId As Integer?
        If Val(ddlRegion.SelectedValue) > 0 Then
            LocationId = Val(ddlRegion.SelectedValue)
        Else
            LocationId = Val(ddlRegion.SelectedValue)
        End If

        If Val(ddlRegionWoNational.SelectedValue) = 0 Then
            sSubTitle = imisgen.getMessage("L_REGION") & ": " & imisgen.getMessage("T_ALL")
        End If

        If Val(ddlRegionWoNational.SelectedValue) <> 0 Then
            sSubTitle = imisgen.getMessage("L_REGION") & ": " & ddlRegionWoNational.SelectedItem.Text
        End If



        Dim subtitleMsg = ddlRegion.SelectedItem.Text + " : " + programCode + ": " & SartDate + " TO " + EndDate
        IMIS_EN.eReports.SubTitle = subtitleMsg

        dt = reports.GetConsolidatedReportData(LocationId, SartDate, EndDate, programCode, creditType)

    End Sub

    Private Sub GetConsolidatedReportData(ByVal creditType As Integer)
        Dim sSubTitle As String = ""

        Dim SartDate As String = ddlMonth.SelectedItem.Text + " " + ddlYear.SelectedItem.Text
        Dim EndDate As String = ddlMonthPOIEnd.SelectedItem.Text + " " + ddlYearEnd.SelectedItem.Text
        Dim programCode As String = ddlAllProducts.SelectedItem.Text

        Dim LocationId As Integer?
        If Val(ddlRegion.SelectedValue) > 0 Then
            LocationId = Val(ddlRegion.SelectedValue)
        Else
            LocationId = Val(ddlRegion.SelectedValue)
        End If

        If Val(ddlRegionWoNational.SelectedValue) = 0 Then
            sSubTitle = imisgen.getMessage("L_REGION") & ": " & imisgen.getMessage("T_ALL")
        End If

        If Val(ddlRegionWoNational.SelectedValue) <> 0 Then
            sSubTitle = imisgen.getMessage("L_REGION") & ": " & ddlRegionWoNational.SelectedItem.Text
        End If

        Dim subtitleMsg = ddlRegion.SelectedItem.Text + " : " + programCode + ": " & SartDate + " TO " + EndDate
        IMIS_EN.eReports.SubTitle = subtitleMsg

        dt = reports.GetConsolidatedReportData(LocationId, SartDate, EndDate, programCode, creditType)
    End Sub

    Private Sub GetApprovedConsolidatedReportData(ByVal creditType As Integer)
        Dim sSubTitle As String = ""

        Dim SartDate As String = ddlMonth.SelectedItem.Text + " " + ddlYear.SelectedItem.Text
        Dim EndDate As String = ddlMonthPOIEnd.SelectedItem.Text + " " + ddlYearEnd.SelectedItem.Text
        Dim programCode As String = ddlAllProducts.SelectedItem.Text

        Dim LocationId As Integer?
        If Val(ddlRegion.SelectedValue) > 0 Then
            LocationId = Val(ddlRegion.SelectedValue)
        Else
            LocationId = Val(ddlRegion.SelectedValue)
        End If

        If Val(ddlRegionWoNational.SelectedValue) = 0 Then
            sSubTitle = imisgen.getMessage("L_REGION") & ": " & imisgen.getMessage("T_ALL")
        End If

        If Val(ddlRegionWoNational.SelectedValue) <> 0 Then
            sSubTitle = imisgen.getMessage("L_REGION") & ": " & ddlRegionWoNational.SelectedItem.Text
        End If

        Dim subtitleMsg = ddlRegion.SelectedItem.Text + " : " + programCode + ": " & SartDate + " TO " + EndDate
        IMIS_EN.eReports.SubTitle = subtitleMsg

        dt = reports.GetConsolidatedReportData(LocationId, SartDate, EndDate, programCode, creditType)
    End Sub

    Private Sub GetConsolidatedApprovedReportData(ByVal creditType As Integer)
        Dim sSubTitle As String = ""

        Dim SartDate As String = ddlMonth.SelectedItem.Text + " " + ddlYear.SelectedItem.Text
        Dim EndDate As String = ddlMonthPOIEnd.SelectedItem.Text + " " + ddlYearEnd.SelectedItem.Text
        Dim programCode As String = ddlAllProducts.SelectedItem.Text

        Dim LocationId As Integer?
        If Val(ddlRegion.SelectedValue) > 0 Then
            LocationId = Val(ddlRegion.SelectedValue)
        Else
            LocationId = Val(ddlRegion.SelectedValue)
        End If

        If Val(ddlRegionWoNational.SelectedValue) = 0 Then
            sSubTitle = imisgen.getMessage("L_REGION") & ": " & imisgen.getMessage("T_ALL")
        End If

        If Val(ddlRegionWoNational.SelectedValue) <> 0 Then
            sSubTitle = imisgen.getMessage("L_REGION") & ": " & ddlRegionWoNational.SelectedItem.Text
        End If

        Dim subtitleMsg = ddlRegion.SelectedItem.Text + " : " + programCode + ": " & SartDate + " TO " + EndDate
        IMIS_EN.eReports.SubTitle = subtitleMsg

        dt = reports.GetConsolidatedApprovedReportData(LocationId, SartDate, EndDate, programCode, creditType)
    End Sub

    Private Sub FillPayrollCycle()
        payrollcycleDropDownList.DataSource = GenPayment.GetPayrollCycleGridView()
        payrollcycleDropDownList.DataValueField = "PayrollCycleID"
        payrollcycleDropDownList.DataTextField = "name"
        payrollcycleDropDownList.DataBind()
    End Sub

    Protected Sub payrollcycleDropDownList_SelectedIndexChanged(sender As Object, e As EventArgs)
        ePayment.PayrollCycleID = Val(payrollcycleDropDownList.SelectedValue)
        GenPayment.GetPayrollCycleDetails(ePayment)
        ddlRegion.SelectedValue = ePayment.RegionId
        ddlMonth.SelectedValue = getMonthID(ePayment.StartMonth)
        ddlYear.SelectedValue = ePayment.EndYear
        ddlMonthPOIEnd.SelectedValue = getMonthID(ePayment.EndMonth)
        ddlYearEnd.SelectedValue = ePayment.EndYear
        ddlAllProducts.SelectedValue = ePayment.ProdId
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

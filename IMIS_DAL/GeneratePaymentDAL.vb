Imports System.Data.SqlClient

Public Class GeneratePaymentDAL
    Dim data As New ExactSQL
    Public Function GeneratePayNational(ByVal ePayment As IMIS_EN.tblPayment, ByVal eProduct As IMIS_EN.tblPolicy, ByVal isExceptional As Boolean) As DataTable
        Dim data As New ExactSQL
        Dim sSQL As String = ""
        If isExceptional Then
            sSQL = " insert into tblPaymentDetails (PaymentID,ProductCode,InsuranceNumber,Amount,StartDate,EndDate,
                 ExpectedAmount, PaymentStatus,RegionId,rundate,payrollCycleID)
                 select 3,p.ProductCode,i.CHFID,p.LumpSum,@DateFrom,@DateTo,p.LumpSum,0,a.RegionId,GetDate(),@payrollCycleID from tblFamilies  f,tblalllocations  a,tblInsuree i, tblPolicy pl,
                 tblproduct p where f.LocationId=a.villageid and i.InsureeID=f.InsureeID and pl.FamilyID=f.FamilyID 
                 and pl.insureestatus = 1  and i.insureestatus = 1  and i.IsHead=1 and p.ProdID=pl.ProdID and p.ProductCode=@Pcode and i.TempID is not null "
        Else
            sSQL = " insert into tblPaymentDetails (PaymentID,ProductCode,InsuranceNumber,Amount,StartDate,EndDate,
                 ExpectedAmount, PaymentStatus,RegionId,rundate,payrollCycleID)
                 select 3,p.ProductCode,i.CHFID,p.LumpSum,@DateFrom,@DateTo,p.LumpSum,0,a.RegionId,GetDate(),@payrollCycleID from tblFamilies  f,tblalllocations  a,tblInsuree i, tblPolicy pl,
                 tblproduct p where f.LocationId=a.villageid and i.InsureeID=f.InsureeID and pl.FamilyID=f.FamilyID 
                 and pl.insureestatus = 1  and i.insureestatus = 1  and i.IsHead=1 and p.ProdID=pl.ProdID and p.ProductCode=@Pcode "
        End If
        data.setSQLCommand(sSQL, CommandType.Text)

        data.params("@Pcode", SqlDbType.NVarChar, 50, ePayment.ProductCode)
        data.params("@DateFrom", SqlDbType.NVarChar, 50, ePayment.StartMonth + " " + ePayment.StartYear)
        data.params("@DateTo", SqlDbType.NVarChar, 50, ePayment.EndMonth + " " + ePayment.EndYear)
        data.params("@payrollCycleID", SqlDbType.Int, ePayment.PayrollCycleID)
        Return data.Filldata
    End Function
    Public Function GeneratePayRegion(ByVal ePayment As IMIS_EN.tblPayment, ByVal eProduct As IMIS_EN.tblPolicy, ByVal isExceptional As Boolean) As DataTable
        Dim data As New ExactSQL
        Dim sSQL As String = ""

        If isExceptional Then
            sSQL = "insert into tblPaymentDetails (PaymentID,ProductCode,InsuranceNumber,Amount,StartDate,EndDate,
                ExpectedAmount, PaymentStatus, RegionId,rundate,payrollCycleID)
                select 3,p.ProductCode,i.CHFID,p.LumpSum,@DateFrom,@DateTo,p.LumpSum,0,a.RegionId,GetDate(),@payrollCycleID from tblFamilies  f,tblalllocations  a,tblInsuree i, tblPolicy pl,
                tblproduct p where f.LocationId=a.villageid and i.InsureeID=f.InsureeID and pl.FamilyID=f.FamilyID 
                and pl.insureestatus = 1 and i.insureestatus = 1  and i.IsHead=1 and p.ProdID=pl.ProdID and p.ProductCode=@ProdID and  a.regionid=@rid and i.TempID is not null "
        Else
            sSQL = "insert into tblPaymentDetails (PaymentID,ProductCode,InsuranceNumber,Amount,StartDate,EndDate,
                ExpectedAmount, PaymentStatus, RegionId,rundate,payrollCycleID)
                select 3,p.ProductCode,i.CHFID,p.LumpSum,@DateFrom,@DateTo,p.LumpSum,0,a.RegionId,GetDate(),@payrollCycleID from tblFamilies  f,tblalllocations  a,tblInsuree i, tblPolicy pl,
                tblproduct p where f.LocationId=a.villageid and i.InsureeID=f.InsureeID and pl.FamilyID=f.FamilyID 
                and pl.insureestatus = 1 and i.insureestatus = 1  and i.IsHead=1 and p.ProdID=pl.ProdID and p.ProductCode=@ProdID and  a.regionid=@rid "
        End If

        data.setSQLCommand(sSQL, CommandType.Text)

        data.params("@ProdID", SqlDbType.NVarChar, 8, ePayment.ProductCode)
        data.params("@rid", SqlDbType.Int, ePayment.RegionId)
        data.params("@DateFrom", SqlDbType.NVarChar, 50, ePayment.StartMonth + " " + ePayment.StartYear)
        data.params("@DateTo", SqlDbType.NVarChar, 50, ePayment.EndMonth + " " + ePayment.EndYear)
        data.params("@payrollCycleID", SqlDbType.Int, ePayment.PayrollCycleID)
        Return data.Filldata
    End Function

    Public Function GetPaymentNationalGridView(ByVal ePayment As IMIS_EN.tblPayment, ByVal isExceptional As Boolean) As DataTable

        Dim data As New ExactSQL
        Dim sSQL As String = ""
        'Dim startdate = ePayment.DateFrom.ToString()
        'Dim EndDate As Date
        If isExceptional Then
            sSQL = "select p.PaymentDetailsID, p.InsuranceNumber, ins.OtherNames , ins.LastName,ins.phone,ins.Gender, p.Amount, loc.RegionName,
                loc.DistrictName, loc.WardName,loc.VillageName,p.ProductCode, p.StartDate,p.EndDate
                from tblPaymentDetails p, tblInsuree ins, tblAllLocations loc, tblFamilies family
                where p.InsuranceNumber = ins.CHFID and family.LocationId = loc.villageId and ins.insureestatus = 1   and
                family.InsureeID = ins.InsureeID and ins.IsHead = 1 and p.StartDate=@DateFrom and p.EndDate=@DateTo and p.ProductCode=@Pcode and ins.TempID is not null "
        Else
            sSQL = "select p.PaymentDetailsID, p.InsuranceNumber, ins.OtherNames , ins.LastName,ins.phone,ins.Gender, p.Amount, loc.RegionName,
                loc.DistrictName, loc.WardName,loc.VillageName,p.ProductCode, p.StartDate,p.EndDate
                from tblPaymentDetails p, tblInsuree ins, tblAllLocations loc, tblFamilies family
                where p.InsuranceNumber = ins.CHFID and family.LocationId = loc.villageId and ins.insureestatus = 1   and
                family.InsureeID = ins.InsureeID and ins.IsHead = 1 and p.StartDate=@DateFrom and p.EndDate=@DateTo and p.ProductCode=@Pcode "
        End If
        If ePayment.PayrollCycleID >= 1 Then
            sSQL += "and p.payrollCycleID = @payrollCycleID "
        End If

        data.setSQLCommand(sSQL, CommandType.Text)


        data.params("@DateFrom", SqlDbType.NVarChar, 50, ePayment.StartMonth + " " + ePayment.StartYear.ToString())
        data.params("@DateTo", SqlDbType.NVarChar, 50, ePayment.EndMonth + " " + ePayment.EndYear.ToString())
        data.params("@Pcode", SqlDbType.NVarChar, 8, ePayment.ProductCode)
        data.params("@payrollCycleID", SqlDbType.Int, ePayment.PayrollCycleID)
        Return data.Filldata
    End Function




    Public Function GetPayrollCycle() As DataTable
        Dim data As New ExactSQL
        Dim sSQL As String = ""
        sSQL = "  select distinct PayrollCycleID,concat(name ,' ', StartDate, ' - ', EndDate) as name from tblPayrollCycle"
        data.setSQLCommand(sSQL, CommandType.Text)
        Return data.Filldata
    End Function

    Public Function IfGetNationalExists(ByVal ePayment As IMIS_EN.tblPayment, ByVal isExceptional As Boolean) As Boolean
        Dim data As New ExactSQL
        Dim str As String = ""
        If isExceptional Then
            str = "select count(1) as count  from tblPaymentDetails pd join tblInsuree i on pd.InsuranceNumber = i.CHFID where ProductCode=@Pcode and StartDate=@DateFrom and EndDate=@DateTo and i.TempID is not null "
        Else
            str = "select count(1) as count  from tblPaymentDetails  where ProductCode=@Pcode and StartDate=@DateFrom and EndDate=@DateTo "
        End If

        If ePayment.PayrollCycleID >= 1 Then
            str += "and payrollCycleID = @payrollCycleID "
        End If

        data.setSQLCommand(str, CommandType.Text)
        data.params("@DateFrom", SqlDbType.NVarChar, 50, ePayment.StartMonth + " " + ePayment.StartYear)
        data.params("@DateTo", SqlDbType.NVarChar, 50, ePayment.EndMonth + " " + ePayment.EndYear)
        data.params("@Pcode", SqlDbType.NVarChar, 8, ePayment.ProductCode)
        data.params("@payrollCycleID", SqlDbType.Int, ePayment.PayrollCycleID)
        Return data.ExecuteScalar()
    End Function
    Public Function IfNationalPayrollApproved(ByVal ePayment As IMIS_EN.tblPayment, ByVal isExceptional As Boolean) As Boolean
        Dim data As New ExactSQL
        Dim str As String = ""
        If isExceptional Then
            str = "select count(1) as count  from tblPaymentDetails pd join tblInsuree i on pd.InsuranceNumber = i.CHFID where ProductCode=@Pcode and StartDate=@DateFrom and EndDate=@DateTo and approvalstatus = 1 and i.TempID is not null "
        Else
            str = "select count(1) as count  from tblPaymentDetails   where ProductCode=@Pcode and StartDate=@DateFrom and EndDate=@DateTo and approvalstatus = 1 "
        End If

        If ePayment.PayrollCycleID >= 1 Then
            str += "and payrollCycleID = @payrollCycleID "
        End If

        data.setSQLCommand(str, CommandType.Text)
        data.params("@DateFrom", SqlDbType.NVarChar, 50, ePayment.StartMonth + " " + ePayment.StartYear)
        data.params("@DateTo", SqlDbType.NVarChar, 50, ePayment.EndMonth + " " + ePayment.EndYear)
        data.params("@Pcode", SqlDbType.NVarChar, 8, ePayment.ProductCode)
        data.params("@payrollCycleID", SqlDbType.Int, ePayment.PayrollCycleID)
        Return data.ExecuteScalar()
    End Function
    Public Function IfRegionalPayrollApproved(ByVal ePayment As IMIS_EN.tblPayment, ByVal isExceptional As Boolean) As Boolean
        Dim data As New ExactSQL
        Dim str As String = ""
        If isExceptional Then
            str = "select count(1) as count  from tblPaymentDetails pd join tblInsuree i on pd.InsuranceNumber = i.CHFID where regionid = @regionid and ProductCode=@Pcode and StartDate=@DateFrom and EndDate=@DateTo and approvalstatus = 1 and i.TempID is not null "
        Else
            str = "select count(1) as count  from tblPaymentDetails   where regionid = @regionid and ProductCode=@Pcode and StartDate=@DateFrom and EndDate=@DateTo and approvalstatus = 1 "
        End If

        If ePayment.PayrollCycleID >= 1 Then
            str += "and payrollCycleID = @payrollCycleID "
        End If

        data.setSQLCommand(str, CommandType.Text)
        data.params("@DateFrom", SqlDbType.NVarChar, 50, ePayment.StartMonth + " " + ePayment.StartYear)
        data.params("@DateTo", SqlDbType.NVarChar, 50, ePayment.EndMonth + " " + ePayment.EndYear)
        data.params("@Pcode", SqlDbType.NVarChar, 8, ePayment.ProductCode)
        data.params("@regionid", SqlDbType.Int, ePayment.RegionId)
        data.params("@payrollCycleID", SqlDbType.Int, ePayment.PayrollCycleID)
        Return data.ExecuteScalar()
    End Function
    Public Function IfGetRegionExists(ByVal ePayment As IMIS_EN.tblPayment, ByVal isExceptional As Boolean) As Boolean
        Dim data As New ExactSQL
        Dim str As String = ""
        If isExceptional Then
            str = "select count(1) as count  from tblPaymentDetails pd join tblInsuree i on pd.InsuranceNumber = i.CHFID where ProductCode=@Pcode and StartDate=@DateFrom and EndDate=@DateTo and RegionId=@rid and i.TempID is not null "
        Else
            str = "select count(1) as count  from tblPaymentDetails where ProductCode=@Pcode and StartDate=@DateFrom and EndDate=@DateTo and RegionId=@rid"
        End If

        If ePayment.PayrollCycleID >= 1 Then
            str += "and payrollCycleID = @payrollCycleID "
        End If

        data.setSQLCommand(str, CommandType.Text)
        data.params("@DateFrom", SqlDbType.VarChar, 50, ePayment.StartMonth + " " + ePayment.StartYear)
        data.params("@DateTo", SqlDbType.VarChar, 50, ePayment.EndMonth + " " + ePayment.EndYear)
        data.params("@Pcode", SqlDbType.NVarChar, 8, ePayment.ProductCode)
        data.params("@rid", SqlDbType.Int, ePayment.RegionId)
        data.params("@payrollCycleID", SqlDbType.Int, ePayment.PayrollCycleID)
        Return data.ExecuteScalar()
    End Function
    Public Function DeleteNationalExists(ByVal ePayment As IMIS_EN.tblPayment) As DataTable
        Dim data As New ExactSQL
        '  Dim str As String = "Select Count(*) from tblHF where HFName = @HFName AND HFAddress = @HFAddress AND LegacyID <> @HfID AND ValidityTo IS NULL"
        '
        Dim str As String = "Delete  from tblPaymentDetails   where ProductCode=@Pcode and StartDate=@DateFrom and EndDate=@DateTo "

        data.setSQLCommand(str, CommandType.Text)
        data.params("@DateFrom", SqlDbType.NVarChar, 50, ePayment.StartMonth + " " + ePayment.StartYear)
        data.params("@DateTo", SqlDbType.NVarChar, 50, ePayment.EndMonth + " " + ePayment.EndYear)
        data.params("@Pcode", SqlDbType.NVarChar, 8, ePayment.ProductCode)



        Return data.Filldata()


    End Function
    Public Function DeleteRegionExists(ByVal ePayment As IMIS_EN.tblPayment) As DataTable
        Dim data As New ExactSQL
        '  Dim str As String = "Select Count(*) from tblHF where HFName = @HFName AND HFAddress = @HFAddress AND LegacyID <> @HfID AND ValidityTo IS NULL"
        '
        Dim str As String = "Delete  from tblPaymentDetails  where ProductCode=@Pcode and StartDate=@DateFrom and EndDate=@DateTo and RegionId=@rid"

        data.setSQLCommand(str, CommandType.Text)
        data.params("@DateFrom", SqlDbType.NVarChar, 50, ePayment.StartMonth + " " + ePayment.StartYear)
        data.params("@DateTo", SqlDbType.NVarChar, 50, ePayment.EndMonth + " " + ePayment.EndYear)
        data.params("@Pcode", SqlDbType.NVarChar, 8, ePayment.ProductCode)
        data.params("@rid", SqlDbType.Int, ePayment.RegionId)



        Return data.Filldata()


    End Function
    Public Function GetPaymentRegionGridview(ByVal ePayment As IMIS_EN.tblPayment, ByVal isExceptional As Boolean) As DataTable
        Dim data As New ExactSQL
        Dim sSQL As String = ""
        Dim msg As String = ""

        If isExceptional Then
            sSQL = " select p.PaymentDetailsID, p.InsuranceNumber, ins.OtherNames , ins.LastName,ins.phone,ins.Gender, p.Amount, loc.RegionName,
            loc.DistrictName, loc.WardName,loc.VillageName,p.ProductCode, p.StartDate,p.EndDate
            from tblPaymentDetails p, tblInsuree ins, tblAllLocations loc, tblFamilies family
            where p.InsuranceNumber = ins.CHFID and family.LocationId = loc.villageId  and ins.insureestatus = 1  and
            family.InsureeID = ins.InsureeID and ins.IsHead = 1 and p.StartDate=@DateFrom and p.EndDate=@DateTo and p.ProductCode=@Pcode and p.RegionId=@rid and ins.TempID is not null "
        Else
            sSQL = " select p.PaymentDetailsID, p.InsuranceNumber, ins.OtherNames , ins.LastName,ins.phone,ins.Gender, p.Amount, loc.RegionName,
            loc.DistrictName, loc.WardName,loc.VillageName,p.ProductCode, p.StartDate,p.EndDate
            from tblPaymentDetails p, tblInsuree ins, tblAllLocations loc, tblFamilies family
            where p.InsuranceNumber = ins.CHFID and family.LocationId = loc.villageId  and ins.insureestatus = 1  and
            family.InsureeID = ins.InsureeID and ins.IsHead = 1 and p.StartDate=@DateFrom and p.EndDate=@DateTo and p.ProductCode=@Pcode and p.RegionId=@rid"
        End If

        If ePayment.PayrollCycleID >= 1 Then
            sSQL += "and p.payrollCycleID = @payrollCycleID "
        End If

        data.setSQLCommand(sSQL, CommandType.Text)
        data.params("@rid", SqlDbType.Int, ePayment.RegionId)
        data.params("@DateFrom", SqlDbType.NVarChar, 50, ePayment.StartMonth + " " + ePayment.StartYear)
        data.params("@DateTo", SqlDbType.NVarChar, 50, ePayment.EndMonth + " " + ePayment.EndYear)
        data.params("@Pcode", SqlDbType.NVarChar, 8, ePayment.ProductCode)
        data.params("@payrollCycleID", SqlDbType.Int, ePayment.PayrollCycleID)
        Return data.Filldata
    End Function
    Public Function GetProducts() As DataTable
        Dim data As New ExactSQL
        Dim sSQL As String = ""
        sSQL = "select distinct ProdId,ProductCode from tblProduct order by ProductCode"

        data.setSQLCommand(sSQL, CommandType.Text)

        Return data.Filldata
    End Function

    Public Sub InsertPayrollCycle(ByVal ePayment As IMIS_EN.tblPayment)
        Dim data As New ExactSQL
        data.setSQLCommand(" INSERT INTO tblPayrollCycle (name,prodID,StartDate,EndDate,Status,regionid,startMonth,endmonth,endyear,startyear)" &
     " VALUES(@name,@prodID,@StartDate,@EndDate,@Status,@regionid,@startMonth,@endmonth,@endyear,@startyear)", CommandType.Text)

        data.params("@name", SqlDbType.NVarChar, 50, ePayment.PayCycleName)
        data.params("@prodID", SqlDbType.Int, ePayment.ProdId)
        data.params("@StartDate", SqlDbType.NVarChar, 50, ePayment.StartMonth + " " + ePayment.StartYear.ToString())
        data.params("@EndDate", SqlDbType.NVarChar, 50, ePayment.EndMonth + " " + ePayment.EndYear.ToString())
        data.params("@Status", SqlDbType.NVarChar, 10, "Active")
        data.params("@regionid", SqlDbType.Int, ePayment.RegionId)
        data.params("@startMonth", SqlDbType.NVarChar, 50, ePayment.StartMonth)
        data.params("@endmonth", SqlDbType.NVarChar, 50, ePayment.EndMonth)
        data.params("@startyear", SqlDbType.NVarChar, 50, ePayment.StartYear)
        data.params("@endyear", SqlDbType.NVarChar, 50, ePayment.EndYear)

        data.ExecuteCommand()
    End Sub

    Public Sub UpdatePayrollCycle(ByVal ePayment As IMIS_EN.tblPayment)
        Dim data As New ExactSQL
        data.setSQLCommand(" update tblPayrollCycle set name = @name,prodID = @prodID,StartDate=@StartDate,EndDate=@EndDate,Status=@Status,regionid=@regionid,startMonth=@startMonth,endmonth=@endmonth, " +
                  " endyear=@endyear,startyear=@startyear where PayrollCycleID =  @PayrollCycleID  ", CommandType.Text)

        data.params("@PayrollCycleID", SqlDbType.Int, ePayment.PayrollCycleID)
        data.params("@name", SqlDbType.NVarChar, 50, ePayment.PayCycleName)
        data.params("@prodID", SqlDbType.Int, ePayment.ProdId)
        data.params("@StartDate", SqlDbType.NVarChar, 50, ePayment.StartMonth + " " + ePayment.StartYear.ToString())
        data.params("@EndDate", SqlDbType.NVarChar, 50, ePayment.EndMonth + " " + ePayment.EndYear.ToString())
        data.params("@Status", SqlDbType.NVarChar, 10, ePayment.PaymentStatusName)
        data.params("@regionid", SqlDbType.Int, ePayment.RegionId)
        data.params("@startMonth", SqlDbType.NVarChar, 50, ePayment.StartMonth)
        data.params("@endmonth", SqlDbType.NVarChar, 50, ePayment.EndMonth)
        data.params("@startyear", SqlDbType.NVarChar, 50, ePayment.StartYear)
        data.params("@endyear", SqlDbType.NVarChar, 50, ePayment.EndYear)

        data.ExecuteCommand()
    End Sub

    Public Sub GetPayrollCycleDetails(ByVal ePayment As IMIS_EN.tblPayment)
        Dim data As New ExactSQL
        Dim sSQL As String = ""
        sSQL = "SELECT name,prodID,StartDate,EndDate,Status,regionid,startMonth,endmonth,endyear,startyear  from tblPayrollCycle where PayrollCycleID = @PayrollCycleID  "

        Try

            data.setSQLCommand(sSQL, CommandType.Text)
            data.params("@PayrollCycleID", SqlDbType.Int, ePayment.PayrollCycleID)

            Dim dr As DataRow = data.Filldata()(0)

            Dim ePayments As New IMIS_EN.tblPayment
            ePayment.PayCycleName = dr("name")
            ePayment.ProdId = dr("prodID")
            ePayment.StartMonth = dr("startMonth")
            ePayment.EndMonth = dr("endmonth")
            ePayment.StartYear = dr("startyear")
            ePayment.EndYear = dr("endyear")
            ePayment.PaymentStatusName = dr("Status")
            ePayment.RegionId = dr("regionid")
        Catch ex As Exception
            Dim errMsg As String = ex.Message
        End Try

    End Sub

    Public Function GetPayrollCycleGridView() As DataTable
        Dim data As New ExactSQL
        Dim sSQL As String = ""
        sSQL = "  select distinct PayrollCycleID,name,pd.productCode,StartDate,EndDate,Status, CASE  WHEN p.regionid = -1 THEN 'National'  ELSE loc.RegionName END as RegionName " +
            " from tblPayrollCycle p  left join tblproduct pd on p.prodID = pd.prodID  left join tblAllLocations loc on loc.regionid = p.regionid "
        data.setSQLCommand(sSQL, CommandType.Text)
        Return data.Filldata
    End Function
    Public Function ifPayrollCycleExist(ByVal ePayment As IMIS_EN.tblPayment) As Boolean
        Dim data As New ExactSQL
        Dim str As String = "select count(1) as count  from tblpayrollcycle where name=@name and prodID=@prodID and regionid=@regionid and startMonth=@startMonth  " +
            " and endmonth=@endmonth and startyear=@startyear  and endyear=@endyear  "
        data.setSQLCommand(str, CommandType.Text)

        data.params("@name", SqlDbType.NVarChar, 50, ePayment.PayCycleName)
        data.params("@prodID", SqlDbType.Int, ePayment.ProdId)
        data.params("@regionid", SqlDbType.Int, ePayment.RegionId)
        data.params("@startMonth", SqlDbType.NVarChar, 50, ePayment.StartMonth)
        data.params("@endmonth", SqlDbType.NVarChar, 50, ePayment.EndMonth)
        data.params("@startyear", SqlDbType.NVarChar, 50, ePayment.StartYear)
        data.params("@endyear", SqlDbType.NVarChar, 50, ePayment.EndYear)
        Return data.ExecuteScalar()
    End Function


End Class

Public Class PaymentProviderPaymentsDAL
    Private errMsg As String = ""

    Public Function GetPaymentForApproval(ByVal ePayment As IMIS_EN.tblPayment, ByVal isExceptional As Boolean) As DataTable
        Dim data As New ExactSQL
        Dim sSQL As String = ""
        If isExceptional Then
            sSQL = " select distinct p.PaymentDetailsID,pd.productname, p.InsuranceNumber,concat(ins.OtherNames ,' ', ins.LastName) as Insuree,ins.phone,ins.Gender, p.Amount, loc.RegionName,  "
            sSQL += " loc.DistrictName, loc.WardName,loc.VillageName,p.ProductCode, p.StartDate,p.EndDate,p.PaymentStatus,p.PaymentDate, CONCAT( P.startdate, ' to ', P.enddate) AS Validity "
            sSQL += " , p.approvalStatus,p.TransactionNo from tblPaymentDetails p, tblInsuree ins, tblAllLocations loc, tblFamilies family,tblproduct pd "
            sSQL += "  where  p.InsuranceNumber = ins.CHFID and family.LocationId = loc.villageId "
            sSQL += "  and family.InsureeID = ins.InsureeID and ins.IsHead = 1 and pd.productcode = p.productcode and ins.TempID is not null "
        Else
            sSQL = " select distinct p.PaymentDetailsID,pd.productname, p.InsuranceNumber,concat(ins.OtherNames ,' ', ins.LastName) as Insuree,ins.phone,ins.Gender, p.Amount, loc.RegionName,  "
            sSQL += " loc.DistrictName, loc.WardName,loc.VillageName,p.ProductCode, p.StartDate,p.EndDate,p.PaymentStatus,p.PaymentDate, CONCAT( P.startdate, ' to ', P.enddate) AS Validity "
            sSQL += " , p.approvalStatus,p.TransactionNo from tblPaymentDetails p, tblInsuree ins, tblAllLocations loc, tblFamilies family,tblproduct pd "
            sSQL += "  where  p.InsuranceNumber = ins.CHFID and family.LocationId = loc.villageId "
            sSQL += "  and family.InsureeID = ins.InsureeID and ins.IsHead = 1 and pd.productcode = p.productcode "
        End If

        If ePayment.RegionId >= 1 Then
            sSQL += " AND loc.RegionId= @RegionId "
        End If
        If ePayment.ProductCode IsNot Nothing Then
            sSQL += " AND P.productcode = @productCode "
        End If
        If ePayment.StartMonth IsNot Nothing Then
            sSQL += " AND p.startdate = @DateFrom "
        End If
        If ePayment.EndMonth IsNot Nothing Then
            sSQL += " AND p.enddate = @DateTo "
        End If


        Try
            data.setSQLCommand(sSQL, CommandType.Text)
            data.params("@RegionId", SqlDbType.Int, ePayment.RegionId)
            data.params("@productCode", SqlDbType.NVarChar, 50, ePayment.ProductCode)
            data.params("@DateFrom", SqlDbType.NVarChar, 50, ePayment.StartMonth)
            data.params("@DateTo", SqlDbType.NVarChar, 50, ePayment.EndMonth)


        Catch ex As Exception
            errMsg = ex.Message
        End Try

        Return data.Filldata
    End Function

    Public Sub GetPaymentApprovalCounts(ByVal ePayment As IMIS_EN.tblPayment)
        Dim data As New ExactSQL
        Dim sSQL As String = ""
        sSQL = "SELECT  (select count(*) as approved  from tblpaymentdetails where startdate = @DateFrom and enddate = @DateTo and approvalstatus = 1 and regionid = @RegionId  and productcode = @productCode) as approvedCount,  "
        sSQL += " (select count(*) as approved  from tblpaymentdetails where startdate = @DateFrom and enddate = @DateTo and regionid = @RegionId and productcode = @productCode) as totalcount "

        Try

            data.setSQLCommand(sSQL, CommandType.Text)
            data.params("@RegionId", SqlDbType.Int, ePayment.RegionId)
            data.params("@productCode", SqlDbType.NVarChar, 50, ePayment.ProductCode)
            data.params("@DateFrom", SqlDbType.NVarChar, 50, ePayment.StartMonth)
            data.params("@DateTo", SqlDbType.NVarChar, 50, ePayment.EndMonth)

            Dim dr As DataRow = data.Filldata()(0)

            Dim ePayments As New IMIS_EN.tblPayment
            ePayment.ApprovedCount = dr("approvedCount")
            ePayment.TotalCount = dr("totalcount")

        Catch ex As Exception
            errMsg = ex.Message
        End Try

    End Sub
    Public Function GetPayment(ByVal ePayment As IMIS_EN.tblPayment) As DataTable
        Dim data As New ExactSQL
        Dim sSQL As String = ""
        sSQL = " select p.PaymentDetailsID,pd.productname, p.InsuranceNumber,concat(ins.OtherNames ,' ', ins.LastName) as Insuree,ins.phone,ins.Gender, p.Amount, loc.RegionName,  "
        sSQL += " loc.DistrictName, loc.WardName,loc.VillageName,p.ProductCode, p.StartDate,p.EndDate,p.PaymentStatus,p.PaymentDate, CONCAT( P.startdate, ' to ', P.enddate) AS Validity "
        sSQL += " , p.approvalStatus,p.TransactionNo from tblPaymentDetails p, tblInsuree ins, tblAllLocations loc, tblFamilies family,tblproduct pd "
        sSQL += "  where  p.InsuranceNumber = ins.CHFID and p.approvalstatus = 1 and family.LocationId = loc.villageId "
        sSQL += "  and family.InsureeID = ins.InsureeID and ins.IsHead = 1 and pd.productcode = p.productcode "
        Dim StartDate As String = ePayment.StartMonth + " " + ePayment.StartYear
        Dim EndDate As String = ePayment.EndMonth + " " + ePayment.EndYear
        If ePayment.RegionId >= 1 Then
            sSQL += " AND loc.RegionId= @RegionId "
        End If
        If ePayment.ProductCode IsNot Nothing Then
            sSQL += " AND P.productcode = @productCode "
        End If
        If StartDate IsNot Nothing Then
            sSQL += " AND p.startdate = @DateFrom "
        End If
        If EndDate IsNot Nothing Then
            sSQL += " AND p.enddate = @DateTo "
        End If


        Try
            data.setSQLCommand(sSQL, CommandType.Text)
            data.params("@RegionId", SqlDbType.Int, ePayment.RegionId)
            data.params("@productCode", SqlDbType.NVarChar, 8, ePayment.ProductCode)
            data.params("@DateFrom", SqlDbType.NVarChar, 50, StartDate)
            data.params("@DateTo", SqlDbType.NVarChar, 50, EndDate)


        Catch ex As Exception
            errMsg = ex.Message
        End Try

        Return data.Filldata
    End Function

    Public Function GetPaymentToExport(ByVal ePayment As IMIS_EN.tblPayment, ByVal Export As String) As DataTable
        Dim data As New ExactSQL
        Dim sSQL As String = ""
        If Export = "Draft" Then
            sSQL = " select distinct p.InsuranceNumber as 'Social Registry No',ins.TempID,concat(ins.OtherNames ,' ', ins.LastName) as 'Beneficiary Name',ins.phone as 'Telephone Number', " +
            " loc.RegionName as 'Region',loc.DistrictName as 'District', loc.WardName as 'Ward',pd.productname as 'Program',loc.VillageName as 'Settlement',   p.Amount, p.TransactionNo as 'Transaction Number'  " +
            " from tblPaymentDetails p, tblInsuree ins, tblAllLocations loc, tblFamilies family,tblproduct pd "
            sSQL += "  where  p.InsuranceNumber = ins.CHFID And family.LocationId = loc.villageId "
            sSQL += "  And family.InsureeID = ins.InsureeID And ins.IsHead = 1 And pd.productcode = p.productcode And payrollCycleID = @PayrollCycleID"

        Else
            sSQL = " select distinct p.InsuranceNumber as 'Social Registry No',ins.TempID,concat(ins.OtherNames ,' ', ins.LastName) as 'Beneficiary Name',ins.phone as 'Telephone Number', " +
            " loc.RegionName as 'Region',loc.DistrictName as 'District', loc.WardName as 'Ward',pd.productname as 'Program',loc.VillageName as 'Settlement',   p.Amount, p.TransactionNo as 'Transaction Number'  " +
            " from tblPaymentDetails p, tblInsuree ins, tblAllLocations loc, tblFamilies family,tblproduct pd "
            sSQL += "  where p. approvalStatus = 1 and  p.InsuranceNumber = ins.CHFID And family.LocationId = loc.villageId "
            sSQL += "  And family.InsureeID = ins.InsureeID And ins.IsHead = 1 And pd.productcode = p.productcode And  payrollCycleID = @PayrollCycleID And paymentstatus !=1"
        End If
        If ePayment.RegionId >= 1 Then
            sSQL += " And loc.RegionId= @RegionId "
        End If
        If ePayment.ProductCode IsNot Nothing Then
            sSQL += " And P.productcode = @productCode "
        End If
        If ePayment.StartMonth IsNot Nothing Then
            sSQL += " And p.startdate = @DateFrom "
        End If
        If ePayment.EndMonth IsNot Nothing Then
            sSQL += " And p.enddate = @DateTo "
        End If


        Try
            data.setSQLCommand(sSQL, CommandType.Text)
            data.params("@RegionId", SqlDbType.Int, ePayment.RegionId)
            data.params("@PayrollCycleID", SqlDbType.Int, ePayment.PayrollCycleID)
            data.params("@productCode", SqlDbType.NVarChar, 50, ePayment.ProductCode)
            data.params("@DateFrom", SqlDbType.NVarChar, 50, ePayment.StartMonth)
            data.params("@DateTo", SqlDbType.NVarChar, 50, ePayment.EndMonth)


        Catch ex As Exception
            errMsg = ex.Message
        End Try

        Return data.Filldata
    End Function

    Public Function UpdatePaymentStatus(ByVal ePayment As IMIS_EN.tblPayment) As String
        Dim data As New ExactSQL
        Dim sSQL As String = ""
        sSQL = "UPDATE tblPaymentDetails SET PaymentStatus = @PaymentStatus, PaymentDate = GETDATE() WHERE PaymentDetailsID = @PaymentID "
        Try
            data.setSQLCommand(sSQL, CommandType.Text)
            data.params("@PaymentID", SqlDbType.Int, ePayment.PaymentID)
            data.params("@PaymentStatus", SqlDbType.Int, ePayment.PaymentStatus)
            data.ExecuteCommand()
            Return "Record successfuly updated"

        Catch ex As Exception

            Return ex.Message

        End Try

    End Function

    Public Function UpdateAllPaymentStatus(ByVal ePayment As IMIS_EN.tblPayment) As String
        Dim data As New ExactSQL
        Dim sSQL As String = ""
        sSQL = " UPDATE tblPaymentDetails SET PaymentStatus = @PaymentStatus, PaymentDate = GETDATE() WHERE regionid = @RegionId and startdate = @DateFrom and  enddate = @DateTo  and productcode = @productCode "
        Try
            Dim StartDate As String = ePayment.StartMonth + " " + ePayment.StartYear
            Dim EndDate As String = ePayment.EndMonth + " " + ePayment.EndYear
            data.setSQLCommand(sSQL, CommandType.Text)
            data.params("@RegionId", SqlDbType.Int, ePayment.RegionId)
            data.params("@productCode", SqlDbType.NVarChar, 8, ePayment.ProductCode)
            data.params("@DateFrom", SqlDbType.NVarChar, 50, StartDate)
            data.params("@DateTo", SqlDbType.NVarChar, 50, EndDate)
            data.params("@PaymentStatus", SqlDbType.Int, ePayment.PaymentStatus)
            data.ExecuteCommand()
            Return "Record successfuly updated"

        Catch ex As Exception

            Return ex.Message

        End Try

    End Function

    Public Function UpdatePaymentApprovalStatus(ByVal ePayment As IMIS_EN.tblPayment) As String
        Dim data As New ExactSQL
        Dim sSQL As String = ""
        sSQL = "UPDATE tblPaymentDetails SET ApprovalStatus = @ApprovalStatus, PaymentDate = GETDATE(), TransactionNo = @transactionNumber WHERE PaymentDetailsID = @PaymentID and ApprovalStatus = 0  "
        Try
            data.setSQLCommand(sSQL, CommandType.Text)
            data.params("@PaymentID", SqlDbType.Int, ePayment.PaymentID)
            data.params("@ApprovalStatus", SqlDbType.Int, ePayment.ApprovalStatus)
            data.params("@transactionNumber", ePayment.TransactionNumber)
            data.ExecuteCommand()
            Return "Record successfuly updated"

        Catch ex As Exception

            Return ex.Message

        End Try

    End Function

End Class

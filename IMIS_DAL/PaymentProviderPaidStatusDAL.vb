Public Class PaymentProviderPaidStatusDAL
    Private errMsg As String = ""

    Public Function GetPayment(ByVal ePayment As IMIS_EN.tblPayment) As DataTable
        Dim data As New ExactSQL
        Dim sSQL As String = ""
        sSQL = " select p.InsuranceNumber, ins.OtherNames , ins.LastName,ins.phone,ins.Gender, p.Amount, loc.RegionName, "
        sSQL += " loc.DistrictName, loc.WardName,loc.VillageName,p.ProductCode, p.StartDate,p.EndDate,p.PaymentStatus,p.PaymentDate "
        sSQL += " from tblPaymentDetails p, tblInsuree ins, tblAllLocations loc, tblFamilies family "
        sSQL += " where p.InsuranceNumber = ins.CHFID and family.LocationId = loc.villageId "
        sSQL += " and family.InsureeID = ins.InsureeID and ins.IsHead = 1 and p.PaymentStatus = @PaymentStatus "

        Dim StartDate As String = ePayment.StartMonth + " " + ePayment.StartYear
        Dim EndDate As String = ePayment.EndMonth + " " + ePayment.EndYear

        If ePayment.RegionId >= 1 Then
            sSQL += " AND loc.RegionId= @RegionId "
        End If

        If ePayment.ProductCode IsNot Nothing Then

            sSQL += " AND P.productcode = @productCode "
        End If
        'If Not ePayment.PaymentStatus = 0 Then
        '    sSQL += " AND .HfID = @HfID"
        'End If
        If StartDate IsNot Nothing Then
            sSQL += " AND p.StartDate = @DateFrom "
        End If

        If EndDate IsNot Nothing Then
            sSQL += " AND p.EndDate = @DateTo "
        End If

        'If ePayment.TransactionNumber IsNot Nothing Then
        '    sSQL += " AND PD.TransactionNo LIKE @TransactionNo"
        'End If

        'If ePayment.ReceiptNo IsNot Nothing Then
        '    sSQL += " AND PD.ReceiptNo LIKE @ReceiptNo"
        'End If

        'If ePayment.HfName IsNot Nothing Then
        '    sSQL += " AND HF.HFName LIKE @HfName"
        'End If

        'sSQL += " GROUP BY  P.ProductName, PD.PaymentID, PD.ExpectedAmount, PD.ReceiptNo, PD.TransactionNo, PD.PaymentDate, "
        'sSQL += "PD.ReceivedDate, PD.ReceivedAmount, PD.ValidityFrom, PD.ValidityTo , I.CHFID, I.LastName, I.OtherNames, L.RegionId, "
        'sSQL += "L.RegionName, HF.HFCode, HF.HFName, PD.PaymentStatus, PD.PaymentDetailsID "
        'sSQL += " ORDER BY PaymentID DESC"

        Try
            data.setSQLCommand(sSQL, CommandType.Text)
            data.params("@PaymentStatus", SqlDbType.Int, ePayment.PaymentStatus)
            data.params("@RegionId", SqlDbType.Int, ePayment.RegionId)
            data.params("@productCode", SqlDbType.NVarChar, 8, ePayment.ProductCode)
            'data.params("@HfID", SqlDbType.Int, ePayment.HfId)
            data.params("@DateFrom", SqlDbType.NVarChar, 50, StartDate)
            data.params("@DateTo", SqlDbType.NVarChar, 50, EndDate)
            'data.params("@TransactionNo", SqlDbType.NVarChar, 50, ePayment.TransactionNumber + "%")
            'data.params("@ReceiptNo", SqlDbType.NVarChar, 50, ePayment.ReceiptNo + "%")
            'data.params("@HfName", SqlDbType.NVarChar, 50, "%" + ePayment.HfName + "%")

        Catch ex As Exception
            errMsg = ex.Message
        End Try

        Return data.Filldata
    End Function
    Public Function UpdatePaymentStatus(ByVal ePayment As IMIS_EN.tblPayment) As String
        Dim data As New ExactSQL
        Dim sSQL As String = ""
        sSQL = "UPDATE tblPaymentDetails SET PaymentStatus = @PaymentStatus, PaymentDate = GETDATE(), RegionId=@rid WHERE PaymentDetailsID = @PaymentID "
        Try
            data.setSQLCommand(sSQL, CommandType.Text)
            data.params("@PaymentID", SqlDbType.Int, ePayment.PaymentID)
            data.params("@PaymentStatus", SqlDbType.Int, ePayment.PaymentStatus)
            data.params("@rid", SqlDbType.Int, ePayment.RegionId)
            data.ExecuteCommand()
            Return "Record successfuly updated"

        Catch ex As Exception

            Return ex.Message

        End Try

    End Function
    Public Function GetPaymentGroupReport(ByVal ePayment As IMIS_EN.tblPayment, ByRef statusType As Integer) As DataTable
        Dim data As New ExactSQL
        Dim sSQL As String = ""
        If (statusType = 1) Then
            sSQL = "  select p.productcode,p.startdate,p.enddate,l.LocationName,p.regionid,pc.name as payrollCycleName,p.payrollcycleid from tblpaymentdetails p,tbllocations l,tblpayrollcycle pc where paymentstatus = 0 and approvalstatus = 0 " +
           " and l.locationid = p.regionid and p.payrollcycleid = pc.payrollcycleid group by p.productcode,p.startdate,p.enddate,p.regionid,l.LocationName,pc.name,p.payrollcycleid  order by  productcode,startdate,enddate "
        ElseIf (statusType = 2) Then
            sSQL = "  select p.productcode,p.startdate,p.enddate,l.LocationName,p.regionid,pc.name as payrollCycleName,p.payrollcycleid from tblpaymentdetails p,tbllocations l,tblpayrollcycle pc where paymentstatus = 0 and approvalstatus = 1 " +
           " and l.locationid = p.regionid and p.payrollcycleid = pc.payrollcycleid group by p.productcode,p.startdate,p.enddate,p.regionid,l.LocationName,pc.name,p.payrollcycleid  order by  productcode,startdate,enddate "
        End If


        data.setSQLCommand(sSQL, CommandType.Text)
        Return data.Filldata
    End Function

End Class

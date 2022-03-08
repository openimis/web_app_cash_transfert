Imports System.Data.SqlClient

Public Class BeneficiaryListReportDAL
    Dim data As New ExactSQL



    Public Function GetBeneficiaryReportGridview(ByVal ePayment As IMIS_EN.tblPayment) As DataTable
        Dim data As New ExactSQL
        Dim sSQL As String = ""
        Dim msg As String = ""
        sSQL = " select p.PaymentDetailsID, p.InsuranceNumber, ins.OtherNames , ins.LastName,ins.phone,ins.Gender, p.Amount, loc.RegionName,
loc.DistrictName, loc.WardName,loc.VillageName,p.ProductCode, p.ValidityFrom,p.ValidityTo
from tblPaymentDetails p, tblInsuree ins, tblAllLocations loc, tblFamilies family
where p.InsuranceNumber = ins.CHFID and family.LocationId = loc.villageId and
family.InsureeID = ins.InsureeID and ins.IsHead = 1 and p.ValidityFrom>=@DateFrom and p.ValidityTo<=@DateTo and p.ProductCode=@Pcode and  p.RegionId=@rid "

        data.setSQLCommand(sSQL, CommandType.Text)


        data.params("@rid", SqlDbType.Int, ePayment.RegionId)
        data.params("@DateFrom", SqlDbType.SmallDateTime, ePayment.DateFrom)
            data.params("@DateTo", SqlDbType.SmallDateTime, ePayment.DateTo)
            data.params("@Pcode", SqlDbType.NVarChar, 8, ePayment.ProductCode)
            Return data.Filldata


    End Function

End Class

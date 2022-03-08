Public Class ImportPaymentsBI
    Public Function ImportPaymentsData(dtPayments As DataTable, ByRef ePayment As IMIS_EN.tblPayment, Optional ByRef isExceptional As Boolean = False) As DataTable
        Dim ImpPaymentsBL As New IMIS_BL.ImportPaymentsBL
        Return ImpPaymentsBL.ImportPaymentsData(dtPayments, ePayment, isExceptional)
    End Function

    Public Function GettPaymentStatusCode(ByRef PaymentStatus As String) As Integer
        Dim ImpPaymentsBL As New IMIS_BL.ImportPaymentsBL
        Return ImpPaymentsBL.GettPaymentStatusCode(PaymentStatus)
    End Function

    Public Function GETLocationID(ByRef LocationType As String, ByVal RegionName As String, Optional VillageName As String = "", Optional WardName As String = "", Optional DistrictName As String = "") As DataTable
        Dim ImpPaymentsBL As New IMIS_BL.ImportPaymentsBL
        Return ImpPaymentsBL.GETLocationID(LocationType, RegionName, VillageName, WardName, DistrictName)
    End Function

    Public Function GETIdentificationTypeCode(ByRef IdentificationType As String) As DataTable
        Dim ImpPaymentsBL As New IMIS_BL.ImportPaymentsBL
        Return ImpPaymentsBL.GETIdentificationTypeCode(IdentificationType)
    End Function

    Public Function GETPayPointId(ByRef PayPointCode As String) As DataTable
        Dim ImpPaymentsBL As New IMIS_BL.ImportPaymentsBL
        Return ImpPaymentsBL.GETPayPointId(PayPointCode)
    End Function

    Public Function GETProgramCode(ByRef Program As String) As DataTable
        Dim ImpPaymentsBL As New IMIS_BL.ImportPaymentsBL
        Return ImpPaymentsBL.GETProgramCode(Program)
    End Function


End Class

Public Class ImportPaymentsBL
    Public Function ImportPaymentsData(ByRef dtPayments As DataTable, ByRef ePayment As IMIS_EN.tblPayment, ByRef isExceptional As Boolean) As DataTable
        Dim ImpPaymentsDl As New IMIS_DAL.ImportPaymentsDL
        Return ImpPaymentsDl.ImportPaymentsData(dtPayments, ePayment, isExceptional)
    End Function

    Public Function GettPaymentStatusCode(ByRef PaymentStatus As String) As Integer
        If PaymentStatus = "Paid" Then
            Return 1
        ElseIf PaymentStatus = "Not Paid" Then
            Return 0
        Else
            Return -1
        End If
    End Function

    Public Function GETLocationID(ByRef LocationType As String, ByVal RegionName As String, Optional VillageName As String = "", Optional WardName As String = "", Optional DistrictName As String = "") As DataTable
        Dim ImpPaymentsDl As New IMIS_DAL.ImportPaymentsDL
        Return ImpPaymentsDl.GETLocationID(LocationType, RegionName, VillageName, WardName, DistrictName)
    End Function

    Public Function GETIdentificationTypeCode(ByRef IdentificationType As String) As DataTable
        Dim ImpPaymentsDl As New IMIS_DAL.ImportPaymentsDL
        Return ImpPaymentsDl.GETIdentificationTypeCode(IdentificationType)
    End Function

    Public Function GETPayPointId(ByRef PayPointCode As String) As DataTable
        Dim ImpPaymentsDl As New IMIS_DAL.ImportPaymentsDL
        Return ImpPaymentsDl.GETPayPointId(PayPointCode)
    End Function

    Public Function GETProgramCode(ByRef Program As String) As DataTable
        Dim ImpPaymentsDl As New IMIS_DAL.ImportPaymentsDL
        Return ImpPaymentsDl.GETProgramCode(Program)
    End Function

End Class

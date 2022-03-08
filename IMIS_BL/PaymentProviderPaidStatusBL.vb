Public Class PaymentProviderPaidStatusBL

    Public Function GetPayment(ByRef ePayment As IMIS_EN.tblPayment) As DataTable
        Dim Payment As New IMIS_DAL.PaymentProviderPaidStatusDAL
        Return Payment.GetPayment(ePayment)
    End Function

    Public Function UpdatePaymentStatus(ByRef ePayment As IMIS_EN.tblPayment) As String
        Dim Payment As New IMIS_DAL.PaymentProviderPaidStatusDAL
        Return Payment.UpdatePaymentStatus(ePayment)
    End Function
    Public Function GetPaymentGroupRep(ByRef ePayment As IMIS_EN.tblPayment, ByRef statusType As Integer) As DataTable
        Dim Payment As New IMIS_DAL.PaymentProviderPaidStatusDAL
        Return Payment.GetPaymentGroupReport(ePayment, statusType)
    End Function
End Class

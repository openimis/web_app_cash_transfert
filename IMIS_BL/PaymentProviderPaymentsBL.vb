Public Class PaymentProviderPaymentsBL

    Public Function GetPayment(ByRef ePayment As IMIS_EN.tblPayment) As DataTable
        Dim Payment As New IMIS_DAL.PaymentProviderPaymentsDAL
        Return Payment.GetPayment(ePayment)
    End Function

    Public Function GetPaymentToExport(ByRef ePayment As IMIS_EN.tblPayment, ByRef Export As String) As DataTable
        Dim Payment As New IMIS_DAL.PaymentProviderPaymentsDAL
        Return Payment.GetPaymentToExport(ePayment, Export)
    End Function

    Public Function GetPaymentForApproval(ByRef ePayment As IMIS_EN.tblPayment, ByRef isExceptional As Boolean) As DataTable
        Dim Payment As New IMIS_DAL.PaymentProviderPaymentsDAL
        Return Payment.GetPaymentForApproval(ePayment, isExceptional)
    End Function

    Public Sub GetPaymentApprovalCounts(ByRef ePayment As IMIS_EN.tblPayment)
        Dim Payment As New IMIS_DAL.PaymentProviderPaymentsDAL
        Payment.GetPaymentApprovalCounts(ePayment)
    End Sub

    Public Function UpdatePaymentStatus(ByRef ePayment As IMIS_EN.tblPayment) As String
        Dim Payment As New IMIS_DAL.PaymentProviderPaymentsDAL
        Return Payment.UpdatePaymentStatus(ePayment)
    End Function

    Public Function UpdatePaymentApprovalStatus(ByRef ePayment As IMIS_EN.tblPayment) As String
        Dim Payment As New IMIS_DAL.PaymentProviderPaymentsDAL
        Return Payment.UpdatePaymentApprovalStatus(ePayment)
    End Function

    Public Function UpdateAllPaymentStatus(ByRef ePayment As IMIS_EN.tblPayment) As String
        Dim Payment As New IMIS_DAL.PaymentProviderPaymentsDAL
        Return Payment.UpdateAllPaymentStatus(ePayment)
    End Function

End Class

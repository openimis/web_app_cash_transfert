Public Class generatePay
    Public Function GetPayNational(ByRef ePayment As IMIS_EN.tblPayment, ByVal eProduct As IMIS_EN.tblPolicy, ByRef isExceptional As Boolean) As DataTable
        Dim Payment As New IMIS_DAL.GeneratePaymentDAL
        Return Payment.GeneratePayNational(ePayment, eProduct, isExceptional)
    End Function
    Public Function GetGridPaymentNational(ByVal ePayment As IMIS_EN.tblPayment, ByRef isExceptional As Boolean) As DataTable
        Dim Payment As New IMIS_DAL.GeneratePaymentDAL
        Return Payment.GetPaymentNationalGridView(ePayment, isExceptional)
    End Function

    Public Function GetPayrollCycleGridView() As DataTable
        Dim Payment As New IMIS_DAL.GeneratePaymentDAL
        Return Payment.GetPayrollCycleGridView()
    End Function

    Public Function GetPayrollCycle() As DataTable
        Dim Payment As New IMIS_DAL.GeneratePaymentDAL
        Return Payment.GetPayrollCycle()
    End Function

    Public Sub InsertPayrollCycle(ByVal ePayment As IMIS_EN.tblPayment)
        Dim Payment As New IMIS_DAL.GeneratePaymentDAL
        Payment.InsertPayrollCycle(ePayment)
    End Sub

    Public Sub UpdatePayrollCycle(ByVal ePayment As IMIS_EN.tblPayment)
        Dim Payment As New IMIS_DAL.GeneratePaymentDAL
        Payment.UpdatePayrollCycle(ePayment)
    End Sub

    Public Sub GetPayrollCycleDetails(ByVal ePayment As IMIS_EN.tblPayment)
        Dim Payment As New IMIS_DAL.GeneratePaymentDAL
        Payment.GetPayrollCycleDetails(ePayment)
    End Sub

    Public Function IfNationalExist(ByVal ePayment As IMIS_EN.tblPayment, ByRef isExceptional As Boolean) As Boolean
        Dim Payment As New IMIS_DAL.GeneratePaymentDAL
        Return Payment.IfGetNationalExists(ePayment, isExceptional)
    End Function
    Public Function IfNationalPayrollApproved(ByVal ePayment As IMIS_EN.tblPayment, ByRef isExceptional As Boolean) As Boolean
        Dim Payment As New IMIS_DAL.GeneratePaymentDAL
        Return Payment.IfNationalPayrollApproved(ePayment, isExceptional)
    End Function
    Public Function IfRegionalPayrollApproved(ByVal ePayment As IMIS_EN.tblPayment, ByRef isExceptional As Boolean) As Boolean
        Dim Payment As New IMIS_DAL.GeneratePaymentDAL
        Return Payment.IfRegionalPayrollApproved(ePayment, isExceptional)
    End Function
    Public Function IfRegionalExist(ByRef ePayment As IMIS_EN.tblPayment, ByRef isExceptional As Boolean) As Boolean
        Dim Payment As New IMIS_DAL.GeneratePaymentDAL
        Return Payment.IfGetRegionExists(ePayment, isExceptional)
    End Function
    Public Function ifPayrollCycleExist(ByRef ePayment As IMIS_EN.tblPayment) As Boolean
        Dim Payment As New IMIS_DAL.GeneratePaymentDAL
        Return Payment.ifPayrollCycleExist(ePayment)
    End Function

    Public Function DeleteNationalPays(ByRef ePayment As IMIS_EN.tblPayment) As DataTable
        Dim Payment As New IMIS_DAL.GeneratePaymentDAL
        Return Payment.DeleteNationalExists(ePayment)
    End Function
    Public Function DeleteRegionalPays(ByRef ePayment As IMIS_EN.tblPayment) As DataTable
        Dim Payment As New IMIS_DAL.GeneratePaymentDAL

        Return Payment.DeleteRegionExists(ePayment)
    End Function

    Public Function GetPayRegional(ByRef ePayment As IMIS_EN.tblPayment, ByVal eProduct As IMIS_EN.tblPolicy, ByRef isExceptional As Boolean) As DataTable
        Dim Payment As New IMIS_DAL.GeneratePaymentDAL

        Return Payment.GeneratePayRegion(ePayment, eProduct, isExceptional)
    End Function
    Public Function GetGridPaymentRegional(ByRef ePayment As IMIS_EN.tblPayment, ByRef isExceptional As Boolean) As DataTable
        Dim Payment As New IMIS_DAL.GeneratePaymentDAL
        Return Payment.GetPaymentRegionGridview(ePayment, isExceptional)
    End Function
    Public Function GetGridBeneficiaryProduct(ByRef ePayment As IMIS_EN.tblPayment) As DataTable
        Dim Payment As New IMIS_DAL.BeneficiaryListReportDAL

        Return Payment.GetBeneficiaryReportGridview(ePayment)
    End Function
End Class

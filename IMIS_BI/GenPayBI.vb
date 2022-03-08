Public Class GenPayBI
    Public Function GetRegions(UserId As Integer, Optional ShowSelect As Boolean = True, Optional IncludeNational As Boolean = True) As DataTable
        Dim BL As New IMIS_BL.LocationsBL
        Return BL.GetRegions(UserId, ShowSelect, IncludeNational:=IncludeNational)
    End Function
    Public Function GetPayerIdByUUID(ByVal uuid As Guid) As Integer
        Dim Payer As New IMIS_BL.PayersBL
        Return Payer.GetPayerIdByUUID(uuid)
    End Function
    Public Function GetProducts() As DataTable
        Dim Districts As New IMIS_BL.LocationsBL
        Return Districts.GetProducts()
    End Function

    Public Function getPayNational(ePayment As IMIS_EN.tblPayment, ByVal eProduct As IMIS_EN.tblPolicy, Optional isExceptional As Boolean = False) As DataTable
        Dim PaymentBL As New IMIS_BL.generatePay
        Return PaymentBL.GetPayNational(ePayment, eProduct, isExceptional)
    End Function
    Public Function getPayRegional(ePayment As IMIS_EN.tblPayment, ByVal eProduct As IMIS_EN.tblPolicy, Optional isExceptional As Boolean = False) As DataTable
        Dim PaymentBL As New IMIS_BL.generatePay
        Return PaymentBL.GetPayRegional(ePayment, eProduct, isExceptional)
    End Function
    Public Function getGridPayNational(ePayment As IMIS_EN.tblPayment, Optional isExceptional As Boolean = False) As DataTable
        Dim PaymentBL As New IMIS_BL.generatePay
        Return PaymentBL.GetGridPaymentNational(ePayment, isExceptional)
    End Function
    Public Function GetPayrollCycleGridView() As DataTable
        Dim PaymentBL As New IMIS_BL.generatePay
        Return PaymentBL.GetPayrollCycleGridView()
    End Function
    Public Function GetPayrollCycle() As DataTable
        Dim PaymentBL As New IMIS_BL.generatePay
        Return PaymentBL.GetPayrollCycle()
    End Function

    Public Sub InsertPayrollCycle(ByVal ePayment As IMIS_EN.tblPayment)
        Dim PaymentBL As New IMIS_BL.generatePay
        PaymentBL.InsertPayrollCycle(ePayment)
    End Sub

    Public Sub UpdatePayrollCycle(ByVal ePayment As IMIS_EN.tblPayment)
        Dim PaymentBL As New IMIS_BL.generatePay
        PaymentBL.UpdatePayrollCycle(ePayment)
    End Sub

    Public Sub GetPayrollCycleDetails(ByVal ePayment As IMIS_EN.tblPayment)
        Dim PaymentBL As New IMIS_BL.generatePay
        PaymentBL.GetPayrollCycleDetails(ePayment)
    End Sub


    Public Function getGridPayRegional(ePayment As IMIS_EN.tblPayment, Optional isExceptional As Boolean = False) As DataTable
        Dim PaymentBL As New IMIS_BL.generatePay
        Return PaymentBL.GetGridPaymentRegional(ePayment, isExceptional)
    End Function

    Public Function IfExistNational(ePayment As IMIS_EN.tblPayment, Optional isExceptional As Boolean = False) As Boolean
        Dim PaymentBL As New IMIS_BL.generatePay
        Return PaymentBL.IfNationalExist(ePayment, isExceptional)
    End Function

    Public Function IfNationalPayrollApproved(ePayment As IMIS_EN.tblPayment, Optional isExceptional As Boolean = False) As Boolean
        Dim PaymentBL As New IMIS_BL.generatePay
        Return PaymentBL.IfNationalPayrollApproved(ePayment, isExceptional)
    End Function
    Public Function IfRegionalPayrollApproved(ePayment As IMIS_EN.tblPayment, Optional isExceptional As Boolean = False) As Boolean
        Dim PaymentBL As New IMIS_BL.generatePay
        Return PaymentBL.IfRegionalPayrollApproved(ePayment, isExceptional)
    End Function
    Public Function IfExistRegional(ePayment As IMIS_EN.tblPayment, Optional isExceptional As Boolean = False) As Boolean
        Dim PaymentBL As New IMIS_BL.generatePay
        Return PaymentBL.IfRegionalExist(ePayment, isExceptional)
    End Function
    Public Function ifPayrollCycleExist(ePayment As IMIS_EN.tblPayment) As Boolean
        Dim PaymentBL As New IMIS_BL.generatePay
        Return PaymentBL.ifPayrollCycleExist(ePayment)
    End Function

    Public Function DeleteNationalPay(ePayment As IMIS_EN.tblPayment) As DataTable
        Dim PaymentBL As New IMIS_BL.generatePay
        Return PaymentBL.DeleteNationalPays(ePayment)
    End Function
    Public Function DeleteRegionalPay(ePayment As IMIS_EN.tblPayment) As DataTable
        Dim PaymentBL As New IMIS_BL.generatePay
        Return PaymentBL.DeleteRegionalPays(ePayment)
    End Function
    Public Function getBeneficiaryReport(ePayment As IMIS_EN.tblPayment) As DataTable
        Dim PaymentBL As New IMIS_BL.generatePay
        Return PaymentBL.GetGridBeneficiaryProduct(ePayment)
    End Function
End Class


Public Class FindPaymentProviderPaymentsBI

    'Return regions from IMIS_BL LocationsBL
    Public Function GetRegions(UserId As Integer, Optional ShowSelect As Boolean = True) As DataTable
        Dim BL As New IMIS_BL.LocationsBL
        Return BL.GetRegions(UserId, ShowSelect)
    End Function

    'Return district from IMIS_BL LocationsBL
    Public Function GetDistricts(ByVal userId As Integer, Optional ByVal showSelect As Boolean = False, Optional ByVal RegionId As Integer = 0) As DataTable
        Dim Districts As New IMIS_BL.LocationsBL
        Return Districts.GetDistricts(userId, True, RegionId)
    End Function

    'Return payment providers codes from IMIS_BL HealthFacilityBL
    Public Function GetHFCodes(ByVal UserId As Integer, ByVal LocationId As Integer) As DataTable
        Dim hf As New IMIS_BL.HealthFacilityBL
        Return hf.GetHFCodes(UserId, LocationId)
    End Function

    'Return payment from IMIS_BL PaymentProviderPaymentsBL
    Public Function getPayment(ePayment As IMIS_EN.tblPayment) As DataTable
        Dim PaymentBL As New IMIS_BL.PaymentProviderPaymentsBL
        Return PaymentBL.GetPayment(ePayment)
    End Function

    Public Function GetPaymentToExport(ePayment As IMIS_EN.tblPayment, ByRef Export As String) As DataTable
        Dim PaymentBL As New IMIS_BL.PaymentProviderPaymentsBL
        Return PaymentBL.GetPaymentToExport(ePayment, Export)
    End Function

    Public Function GetPaymentForApproval(ePayment As IMIS_EN.tblPayment, Optional isExceptional As Boolean = False) As DataTable
        Dim PaymentBL As New IMIS_BL.PaymentProviderPaymentsBL
        Return PaymentBL.GetPaymentForApproval(ePayment, isExceptional)
    End Function

    Public Sub GetPaymentApprovalCounts(ePayment As IMIS_EN.tblPayment)
        Dim PaymentBL As New IMIS_BL.PaymentProviderPaymentsBL
        PaymentBL.GetPaymentApprovalCounts(ePayment)
    End Sub

    Public Function GetProducts(ByVal UserId As Integer, Optional ByVal showSelect As Boolean = True, Optional ByVal RegionId As Integer = 0, Optional ByVal DistrictID As Integer = 0) As DataTable
        Dim BL As New IMIS_BL.ProductsBL
        Return BL.GetProducts(UserId, showSelect, RegionId, DistrictID)
    End Function

    Public Function UpdatePaymentStatus(ePayment As IMIS_EN.tblPayment) As String
        Dim PaymentBL As New IMIS_BL.PaymentProviderPaymentsBL
        Return PaymentBL.UpdatePaymentStatus(ePayment)
    End Function


    Public Function UpdatePaymentApprovalStatus(ePayment As IMIS_EN.tblPayment) As String
        Dim PaymentBL As New IMIS_BL.PaymentProviderPaymentsBL
        Return PaymentBL.UpdatePaymentApprovalStatus(ePayment)
    End Function

    Public Function GetPayementStatus(ByVal StatusID As Integer) As String
        Dim gen As New IMIS_BL.GeneralBL
        Return gen.GetPPaymentStatusName(StatusID)
    End Function

    Public Function GetApprovalStatus(ByVal StatusID As Integer) As String
        Dim gen As New IMIS_BL.GeneralBL
        Return gen.GetApprovalStatusName(StatusID)
    End Function

    Public Function UpdateAllPaymentStatus(ePayment As IMIS_EN.tblPayment) As String
        Dim PaymentBL As New IMIS_BL.PaymentProviderPaymentsBL
        Return PaymentBL.UpdateAllPaymentStatus(ePayment)
    End Function


End Class

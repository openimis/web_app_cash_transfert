Public Class FindPaymentBI
    Public UserRights As New IMIS_BL.UsersBL
    Public Function checkRights(ByVal Right As IMIS_EN.Enums.Rights, ByVal UserID As Integer) As Boolean
        Return UserRights.CheckRights(Right, UserID)
    End Function
    Public Function GetRegions(UserId As Integer, Optional ShowSelect As Boolean = True, Optional IncludeNational As Boolean = True) As DataTable
        Dim BL As New IMIS_BL.LocationsBL
        Return BL.GetRegions(UserId, ShowSelect, IncludeNational:=IncludeNational)
    End Function
End Class

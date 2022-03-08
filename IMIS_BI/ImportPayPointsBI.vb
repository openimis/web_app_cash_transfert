Public Class ImportPayPointsBI
    Public Function ImportPayPointsData(dtPayPoints As DataTable) As DataTable
        Dim ImpPayPointsBL As New IMIS_BL.ImportPayPointsBL
        Return ImpPayPointsBL.ImportPayPointsData(dtPayPoints)
    End Function
    Public Function GETLocationID(ByRef Settlement As String) As DataTable
        Dim ImpPayPointsBL As New IMIS_BL.ImportPayPointsBL
        Return ImpPayPointsBL.GETLocationID(Settlement)
    End Function
    Public Function GETHFID(ByRef HFName As String) As DataTable
        Dim ImpPayPointsBL As New IMIS_BL.ImportPayPointsBL
        Return ImpPayPointsBL.GETHFID(HFName)
    End Function
End Class

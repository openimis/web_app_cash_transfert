Public Class ImportPayPointsBL
    Public Function ImportPayPointsData(ByRef dtPayPoints As DataTable) As DataTable
        Dim ImpPayPointsDl As New IMIS_DAL.ImportPaypointsDL
        Return ImpPayPointsDl.ImportPayPointData(dtPayPoints)
    End Function

    Public Function GETLocationID(ByRef Settlement As String) As DataTable
        Dim ImpPayPointsDl As New IMIS_DAL.ImportPaypointsDL
        Return ImpPayPointsDl.GETLocationID(Settlement)
    End Function

    Public Function GETHFID(ByRef HFName As String) As DataTable
        Dim ImpPayPointsDl As New IMIS_DAL.ImportPaypointsDL
        Return ImpPayPointsDl.GETHFID(HFName)
    End Function

End Class

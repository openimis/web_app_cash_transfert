Public Class ImportFalmilyBL
    Public Function ImportFamilyData(ByRef dtFamily As DataTable, Optional isExceptional As Boolean = False) As DataTable
        Dim ImpFamilyDl As New IMIS_DAL.ImportFalmilyDAL
        Return ImpFamilyDl.ImportFamilyData(dtFamily, isExceptional)
    End Function

    Public Function GETLocationCode(ByRef LocationName As String) As DataTable
        Dim ImpFamilyDl As New IMIS_DAL.ImportFalmilyDAL
        Return ImpFamilyDl.GETLocationCode(LocationName)
    End Function

    Public Function GETGenderCode(ByRef Gender As String) As DataTable
        Dim ImpFamilyDl As New IMIS_DAL.ImportFalmilyDAL
        Return ImpFamilyDl.GETGenderCode(Gender)
    End Function

    Public Function GETFamilyTypeCode(ByRef FamilyType As String) As DataTable
        Dim ImpFamilyDl As New IMIS_DAL.ImportFalmilyDAL
        Return ImpFamilyDl.GETFamilyTypeCode(FamilyType)
    End Function

End Class

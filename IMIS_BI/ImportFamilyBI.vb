Public Class ImportFamilyBI
    Public Function ImportFamilyData(dtFamily As DataTable, Optional isExceptional As Boolean = False) As DataTable
        Dim ImpFamilyBL As New IMIS_BL.ImportFalmilyBL
        Return ImpFamilyBL.ImportFamilyData(dtFamily, isExceptional)
    End Function

    Public Function GETLocationCode(ByRef LocationName As String) As DataTable
        Dim ImpFamilyBL As New IMIS_BL.ImportFalmilyBL
        Return ImpFamilyBL.GETLocationCode(LocationName)
    End Function

    Public Function GETGenderCode(ByRef Gender As String) As DataTable
        Dim ImpFamilyBL As New IMIS_BL.ImportFalmilyBL
        Return ImpFamilyBL.GETGenderCode(Gender)
    End Function

    Public Function GETFamilyTypeCode(ByRef FamilyType As String) As DataTable
        Dim ImpFamilyBL As New IMIS_BL.ImportFalmilyBL
        Return ImpFamilyBL.GETFamilyTypeCode(FamilyType)
    End Function

End Class

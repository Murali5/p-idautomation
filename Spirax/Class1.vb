Imports SolidWorks.Interop.swdocumentmgr
Imports System.Drawing
Imports System
Imports System.Diagnostics
Imports System.Windows.Forms
Public Class Class1

End Class

Module DocMgrPreview
    Function GetFolder() As String
        Dim FoldBrow As New FolderBrowserDialog
        GetFolder = FoldBrow.SelectedPath
        Dim Result = FoldBrow.ShowDialog()
        If Result = Windows.Forms.DialogResult.OK Then
            Dim Foldpath = FoldBrow.SelectedPath
        End If
    End Function

    Function DocPreview(ByVal DocPath As String, ByVal FileName As Integer) As Drawing.Image

        Const sLicenseKey As String = "EgsComputersIndiaPvtLtd:swdocmgr_general-11785-02051-00064-50177-08535-34307-00007-57016-22240-62621-45846-24488-61275-02451-51205-61488-33911-04670-46853-11778-04158-23363-40212-03533-46525-54721-38353-52681-47397-42385-16773-53721-53553-00401-25656-23150-57538-23268-24676-28770-7,swdocmgr_previews-11785-02051-00064-50177-08535-34307-00007-19536-48115-11433-26986-08326-65389-30260-08196-32250-39730-28041-08173-07527-60972-22873-40212-03533-46525-54721-38353-52681-47397-42385-16773-53721-53553-00401-25656-23150-57538-23268-24676-28770-2,swdocmgr_dimxpert-11785-02051-00064-50177-08535-34307-00007-22280-08843-16814-54179-05348-15120-62331-28677-19562-29513-44848-37847-60283-48579-23866-40212-03533-46525-54721-38353-52681-47397-42385-16773-53721-53553-00401-25656-23150-57538-23268-24676-28770-0,swdocmgr_geometry-11785-02051-00064-50177-08535-34307-00007-48944-57862-56464-60324-43254-08625-52506-08192-47426-65067-21404-00528-46280-24953-24118-40212-03533-46525-54721-38353-52681-47397-42385-16773-53721-53553-00401-25656-23150-57538-23268-24676-28770-2,swdocmgr_xml-11785-02051-00064-50177-08535-34307-00007-41424-26308-24490-07686-30143-26745-02425-43008-62901-13398-25975-00701-58240-38653-24242-40212-03533-46525-54721-38353-52681-47397-42385-16773-53721-53553-00401-25656-23150-57538-23268-24676-28770-5,swdocmgr_tessellation-11785-02051-00064-50177-08535-34307-00007-04784-46623-47800-47706-28892-58918-08077-04102-28354-64049-40647-59477-42515-14100-23899-40212-03533-46525-54721-38353-52681-47397-42385-16773-53721-53553-00401-25656-23150-57538-23268-24676-28770-4"

        Dim sDocFileName As String = DocPath   '"C:\Users\egs6.EGSNETWORK\Desktop\HKT_.SLDDRW"
        Const sExtractDir As String = "C:\temp\Spirax\"
        Dim sFilename As String = FileName
        Dim swClassFact As SwDMClassFactory
        Dim swDocMgr As SwDMApplication
        Dim swDoc As SwDMDocument
        Dim swDoc10 As SwDMDocument10
        Dim nDocType As Integer
        Dim nRetVal As Integer

        ' Determine type of SOLIDWORKS file based on file extension
        If InStr(LCase(sDocFileName), "sldprt") > 0 Then
            nDocType = SwDmDocumentType.swDmDocumentPart
        ElseIf InStr(LCase(sDocFileName), "sldasm") > 0 Then
            nDocType = SwDmDocumentType.swDmDocumentAssembly
        ElseIf InStr(LCase(sDocFileName), "slddrw") > 0 Then
            nDocType = SwDmDocumentType.swDmDocumentDrawing
        ElseIf InStr(LCase(sDocFileName), "sldblk") > 0 Then
            nDocType = SwDmDocumentType.swDmDocumentUnknown
        Else
            ' Probably not a SOLIDWORKS file,
            ' so cannot open
            nDocType = SwDmDocumentType.swDmDocumentUnknown
        End If

        swClassFact = CreateObject("SwDocumentMgr.SwDMClassFactory")
        swDocMgr = swClassFact.GetApplication(sLicenseKey)
        swDoc = swDocMgr.GetDocument(sDocFileName, nDocType, True, nRetVal)

        swDoc10 = swDoc
        Dim imgPreview As System.Drawing.Image
        Try
            Dim objBitMap As Object = swDoc10.GetPreviewBitmap(nRetVal)
            imgPreview = PictureDispConverter.Convert(objBitMap)
            imgPreview.Save(sExtractDir + sFilename + ".bmp", Drawing.Imaging.ImageFormat.Bmp)
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try

        Return imgPreview
    End Function

End Module
Public Class PictureDispConverter

    Inherits System.Windows.Forms.AxHost

    Public Sub New()
        MyBase.New("56174C86-1546-4778-8EE6-B6AC606875E7")
    End Sub

    Public Shared Function Convert(ByVal objIDispImage As Object) As System.Drawing.Image
        Dim objPicture As System.Drawing.Image
        objPicture = CType(System.Windows.Forms.AxHost.GetPictureFromIPicture(objIDispImage), System.Drawing.Image)
        Return objPicture
    End Function

End Class
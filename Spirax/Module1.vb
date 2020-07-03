Imports System
Imports System.IO
Imports SolidWorks.Interop.sldworks
Imports SolidWorks.Interop.swconst
Imports SolidWorks.Interop.swpublished

Module Module1
    Public TableName As String
    '    Dim WithEvents swApp As SldWorks
    Private swModel As ModelDoc2
    Private swSelMgr As SelectionMgr
    Private swSelObj As Object
    Private swSelBlkInst As SketchBlockInstance
    Private swSelBlkDef As SketchBlockDefinition
    Private run As Boolean
    Private swDraw As DrawingDoc
    Private swSkMgr As SketchManager

    Private myPath As String = "E:\Murali Kannan\Projeccts\Spirax\Blocks"
    Private myBlocks As IList
    Private DirInf As DirectoryInfo
    Private FileInf() As FileInfo

    Public MPtArry As ArrayList = New ArrayList()
    Public ReadyToDropBlk As String

    Public MouseSelectedBlkBtn As Integer
    Public MouseSelectDrop As Boolean = False
    Public MouseSelBlkFileName As String = ""

    Public InsMPt As MathPoint
    Function GetActiveDoc(ByVal sw As SldWorks) As ModelDoc2
        GetActiveDoc = sw.ActiveDoc

    End Function

    Function GetBlkInst(ByVal sw As SldWorks) As SketchBlockInstance
        swModel = GetActiveDoc(sw)
        If Not swModel Is Nothing And swModel.GetType = swDocumentTypes_e.swDocDRAWING Then
            swSelMgr = swModel.SelectionManager
            If swSelMgr.GetSelectedObjectCount2(0) > 0 Then

                Debug.Print("swselObjtype2 " & swSelMgr.GetSelectedObjectType2(1))
                swSelBlkInst = swSelMgr.GetSelectedObject5(1)
                If swSelBlkInst Is Nothing Then
                    swSelObj = swSelMgr.GetSelectedObject5(1)
                    swSelBlkInst = CType(swSelObj, SketchBlockInstance)
                    Return swSelBlkInst
                Else
                    Return swSelBlkInst
                End If
            End If
        End If
    End Function

    Function CheckBlk(ByVal sw As SldWorks) As Boolean
        swModel = GetActiveDoc(sw)
        If Not swModel Is Nothing And swModel.GetType = swDocumentTypes_e.swDocDRAWING Then
            swSelMgr = swModel.SelectionManager
            If swSelMgr.GetSelectedObjectCount2(0) > 0 Then

                For i = 1 To swSelMgr.GetSelectedObjectCount2(0)
                    Debug.Print("swselObj Type 3:- " & swSelMgr.GetSelectedObjectType3(1, 0))
                Next

                If swSelMgr.GetSelectedObjectType2(1) = swSelectType_e.swSelSUBSKETCHINST Then
                    Return True
                Else
                    Return False
                End If
            End If
        End If

    End Function

    Function GetBlkNumFromLib(ByVal path As String) As Integer
        Return ReadBlocksFromLib(path).Length
    End Function

    Function ReadBlocksFromLib(ByVal path As String) As String()
        Dim out() As String = New String() {}
        If Directory.Exists(path) Then
            DirInf = New DirectoryInfo(path)
            FileInf = DirInf.GetFiles("*.SLDBLK")
            Dim i As Integer = 0

            ReDim out(FileInf.Length - 1)

            For Each fi As FileInfo In FileInf
                out(i) = fi.Name.Replace(fi.Extension, "")
                ' myBlocks.Add(fi.Name.Replace(fi.Extension, ""))
                i += 1
            Next
        Else
            out = New String() {}
            MsgBox("Directory Doesn't Exist. Please Check the directory.")
        End If
        Return out
    End Function

    Function ReadBlocksFromLibWithFullName(ByVal path As String) As String()
        Dim out() As String = New String() {}
        If Directory.Exists(path) Then
            DirInf = New DirectoryInfo(path)
            FileInf = DirInf.GetFiles("*.SLDBLK")
            Dim i As Integer = 0

            ReDim out(FileInf.Length - 1)

            For Each fi As FileInfo In FileInf
                out(i) = fi.FullName  '.Replace(fi.Extension, "")
                ' myBlocks.Add(fi.Name.Replace(fi.Extension, ""))
                i += 1
            Next
        Else
            out = New String() {}
            MsgBox("Directory Doesn't Exist. Please Check the directory.")
        End If
        Return out
    End Function

    Function SetDefaultAttributeVal(ByVal blkInst As SketchBlockInstance) As Boolean
        If Not blkInst Is Nothing Then
            run = blkInst.SetAttributeValue("Name", "")
            run = blkInst.SetAttributeValue("Type", RemovePath(blkInst.Definition.FileName))
            run = blkInst.SetAttributeValue("Size", "1" & Chr(34))
            run = blkInst.SetAttributeValue("Material", "AISI")
            run = blkInst.SetAttributeValue("ByPass", "No")
            run = blkInst.SetAttributeValue("Mode", "Mechanical")
            Return True
        Else
            Return False
        End If
    End Function

    Function GetBlkAttribute(ByVal sw As SldWorks, ByVal blkInst As SketchBlockInstance) As String()
        Dim AttrStr(5) As String
        If Not blkInst Is Nothing Then
            Dim swNote As Object
            Dim swAttrCnt As Integer
            swAttrCnt = blkInst.GetAttributeCount
            If swAttrCnt > 0 Then
                swNote = blkInst.GetAttributes()
                For i = 0 To swAttrCnt - 1
                    AttrStr(i) = swNote(i).gettext
                Next

            End If
        End If
        Return AttrStr
    End Function

    Function AddPath(ByVal filename As String) As String
        Return "E:\Murali Kannan\Projeccts\Spirax\Blocks\" & filename & ".SLDBLK"
    End Function

    Function SetBlkAttribute(ByVal sw As SldWorks, ByVal blkinst As SketchBlockInstance, ByVal value As String()) As Boolean
        If Not blkinst Is Nothing Then
            If value(0) <> "" Then
                run = blkinst.SetAttributeValue("Name", value(0).ToString)
            End If

            If value(1) <> "" Then
                run = blkinst.SetAttributeValue("Type", value(1).ToString)
            End If

            If value(2) <> "" Then
                run = blkinst.SetAttributeValue("Size", value(2).ToString)
            End If

            If value(3) <> "" Then
                run = blkinst.SetAttributeValue("Material", value(3).ToString)
            End If

            If value(4) <> "" Then
                run = blkinst.SetAttributeValue("ByPass", value(4).ToString)
            End If

            If value(5) <> "" Then
                run = blkinst.SetAttributeValue("Mode", value(5).ToString)
            End If
            Return True
        Else
            Return False
        End If
    End Function

    Function InsrtBlkInstFile(ByVal InsrtPt As MathPoint, ByVal BlkFile As String, ByVal myModeldoc As ModelDoc2) As SketchBlockInstance
        swSkMgr = myModeldoc.SketchManager
        If Not CheckBlkDefExist(BlkFile, swSkMgr) Then
            Debug.Print("BlockDef Doesn't Exist")
            swSelBlkDef = swSkMgr.MakeSketchBlockFromFile(InsrtPt, BlkFile, False, 1, 0)
            Dim InstCnt = swSelBlkDef.GetInstanceCount
            swSelBlkInst = swSelBlkDef.GetInstances(InstCnt - 1)
            Return swSelBlkInst
        Else
            Debug.Print("BlockDef Already Exist")
            swSelBlkDef = GetBlkDef(swSkMgr, BlkFile)
            swSelBlkInst = swSkMgr.InsertSketchBlockInstance(swSelBlkDef, InsrtPt, 1, 0)
            Return swSelBlkInst
        End If
    End Function

    Function GetBlkDef(ByVal mySkMgr As SketchManager, ByVal BlkFile As String) As SketchBlockDefinition
        Dim SkBlkDefCnt As Integer
        SkBlkDefCnt = swSkMgr.GetSketchBlockDefinitionCount()
        Dim skBlkDefs As Object = swSkMgr.GetSketchBlockDefinitions
        For i = 0 To SkBlkDefCnt - 1
            Dim TempBlkDef As SketchBlockDefinition = skBlkDefs(i)
            If TempBlkDef.FileName = BlkFile Then
                Return TempBlkDef
            End If
        Next
    End Function

    Function ReplaceBlkInst(ByVal TmModel As ModelDoc2, ByVal ExistInst As SketchBlockInstance, ByVal ReplaceInst As String) As SketchBlockInstance
        Dim NewBlkInst As SketchBlockInstance
        Dim OldMPt As MathPoint = ExistInst.InstancePosition
        Dim OldLen As Double = DeleteBlock(ExistInst, TmModel, False)
        Dim NewLen As Double = CheckSizeofBlk(ReplaceInst)
        If OldLen = NewLen Then
            'do nothing
        ElseIf OldLen < NewLen Then

        ElseIf OldLen > NewLen Then

        End If
        NewBlkInst = InsrtBlkInstFile(OldMPt, AddPath(ReplaceInst), TmModel)
        TableName = UpdateBOM(TableName, TmModel)
        TmModel.Extension.SelectByID2(NewBlkInst.Name, "SUBSKETCHINST", 0, 0, 0, False, 0, Nothing, 0)
        Return NewBlkInst
    End Function

    Function DeleteBlock(ByVal ExistInst As SketchBlockInstance, ByVal TmModel As ModelDoc2, Optional ByVal InsertLine As Boolean = False) As Double
        Dim TmMpt As MathPoint
        Dim TmName As String
        Dim Run As Boolean
        Dim TmDef As SketchBlockDefinition

        TmMpt = ExistInst.InstancePosition
        TmName = ExistInst.Name
        TmDef = ExistInst.Definition
        TmModel.ClearSelection2(True)
        Run = TmModel.Extension.SelectByID2(TmName, "SUBSKETCHINST", 0, 0, 0, False, 0, Nothing, 0)
        TmModel.EditDelete()
        DeleteBlkDef(TmModel, TmDef)
        Dim TmSize As Double = CheckSizeofBlk(TmName)
        If InsertLine Then
            Dim TmSkmgr As SketchManager = TmModel.SketchManager
            TmSize = InsrtLine(TmSkmgr, TmMpt, TmSize)
        End If
        TableName = UpdateBOM(TableName, TmModel)
        Return TmSize
    End Function

    'Function RemoveBlkNumber(ByVal NameOfBlk As String) As String
    '    NameOfBlk = NameOfBlk.Replace("-*", "")
    '    Return NameOfBlk.Trim
    'End Function

    Function CheckBlkDefExist(ByVal BlkFileName As String, ByVal swSkMgr As SketchManager) As Boolean
        Dim SkBlkDefCnt As Integer
        SkBlkDefCnt = swSkMgr.GetSketchBlockDefinitionCount()
        Dim skBlkDefs As Object = swSkMgr.GetSketchBlockDefinitions

        For i = 0 To SkBlkDefCnt - 1
            Dim TempBlkDef As SketchBlockDefinition = skBlkDefs(i)
            If TempBlkDef.FileName = BlkFileName Then
                Return True
            End If
        Next
        Return False
    End Function

    Function CheckSizeofBlk(ByVal filename As String) As Double
        If filename.Contains("Tank") Or filename.Contains("Strainer") Or filename.Contains("Dampener") Or filename.Contains("DiverterValve") Or filename.Contains("Indicator") Or filename.Contains("Pressure") Then
            Return 0.01
        ElseIf filename.Contains("Junction") Then
            Return 0.002
        ElseIf filename.Contains("Flange") Then
            Return 0.003
        Else
            Return 0.02
        End If
    End Function

    Function InsrtLine(ByVal SkMgr As SketchManager, ByVal PrevPt As MathPoint, Optional ByVal lenth As Double = 0.02) As Double
        Dim poi As Object
        Dim Pt(2) As Double
        poi = PrevPt.ArrayData
        Pt(0) = CDbl(poi(0))
        Pt(1) = CDbl(poi(1))
        Pt(2) = CDbl(poi(2))
        Dim skSegment As Object
        skSegment = SkMgr.CreateLine(Pt(0), Pt(1), Pt(2), Pt(0) + lenth, Pt(1), Pt(2))
        Return Pt(0) + lenth
    End Function

    Function RemovePath(ByVal fileName As String) As String
        fileName = fileName.Replace("E:\Murali Kannan\Projeccts\Spirax\Blocks\", "")
        fileName = fileName.Replace(".SLDBLK", "")
        Return fileName
    End Function

    Function NextInsrtPt(ByVal sw As SldWorks, ByVal PrevBlkInst As SketchBlockInstance, Optional ByVal line As Double = 0.02, Optional ByVal lineSt As Boolean = False) As MathPoint
        If lineSt Then line = 0
        Dim prevPt As MathPoint
        prevPt = PrevBlkInst.InstancePosition
        Dim PrevPt1 As Object = prevPt.ArrayData
        Dim newPt(2) As Double
        newPt(0) = PrevPt1(0)
        newPt(1) = PrevPt1(1)
        newPt(2) = PrevPt1(2)
        newPt(0) += CheckSizeofBlk(RemovePath(PrevBlkInst.Definition.FileName)) + line
        Dim pioi As Object
        pioi = newPt
        Dim MUtil As MathUtility
        MUtil = sw.GetMathUtility
        prevPt = MUtil.CreatePoint(pioi)

        Return prevPt
    End Function

    Function DeleteBlkDef(ByVal myModel As ModelDoc2, ByVal TmBlkDef As SketchBlockDefinition) As Boolean
        Try
            If TmBlkDef.GetInstanceCount() = 0 Then
                run = myModel.Extension.SelectByID2(RemovePath(TmBlkDef.FileName), "SUBSKETCHDEF", 0, 0, 0, False, 0, Nothing, 0)
                myModel.EditDelete()
            End If
        Catch ex As Exception
            Return False
        End Try
        Return True
    End Function

    Function DefaultMathPoint(ByVal sw As SldWorks) As MathPoint
        Dim swMUtil As MathUtility = sw.GetMathUtility
        Dim vnPt(2) As Double
        vnPt(0) = 0.04#
        vnPt(1) = 0.1485#
        vnPt(2) = 0.0#
        Return swMUtil.CreatePoint(CObj(vnPt))
    End Function

    Function GetSelectedPoint(ByVal sw As SldWorks) As MathPoint
        Dim myModel As ModelDoc2 = sw.ActiveDoc
        Dim mySkPt As SketchPoint
        Dim mySelMgr As SelectionMgr = myModel.SelectionManager
        Dim myMUtil As MathUtility = sw.GetMathUtility
        If mySelMgr.GetSelectedObjectType3(1, 0) = 11 Then
            mySkPt = mySelMgr.GetSelectedObject6(1, 0)
            Dim vmPt(2) As Double
            vmPt(0) = mySkPt.X
            vmPt(1) = mySkPt.Y
            vmPt(2) = mySkPt.Z
            Return myMUtil.CreatePoint(CObj(vmPt))
        End If
    End Function

    Function InsertBOM(ByVal myModel As ModelDoc2) As String
        If myModel.GetType = swDocumentTypes_e.swDocDRAWING Then
            Dim myDraw As DrawingDoc = myModel
            Dim skMgr As SketchManager = myModel.SketchManager
            Dim myTableAnno As TableAnnotation
            Dim BlkDefCnt As Integer = skMgr.GetSketchBlockDefinitionCount()
            Dim BlkDefs As Object = skMgr.GetSketchBlockDefinitions()
            Try
                myTableAnno = myDraw.InsertTableAnnotation2(False, 0.41, 0.287, swBOMConfigurationAnchorType_e.swBOMConfigurationAnchor_TopRight, "E:\Murali Kannan\Projeccts\Spirax\TableTemplate\Spirax.sldtbt", BlkDefCnt, 3)

                '     myTableAnno.Text(0, 0) = "S.No."
                '    myTableAnno.Text(0, 1) = "Description"
                '   myTableAnno.Text(0, 2) = "Qty"

                For i As Integer = 1 To BlkDefCnt - 1
                    For j As Integer = 0 To 2
                        If j = 0 Then
                            myTableAnno.Text(i, j) = i.ToString
                        ElseIf j = 1 Then
                            myTableAnno.Text(i, j) = RemovePath(BlkDefs(i).filename)
                        ElseIf j = 2 Then
                            myTableAnno.Text(i, j) = BlkDefs(i).getinstancecount()
                        End If

                    Next
                Next
                Dim TbFeat As Feature = myTableAnno.GeneralTableFeature.GetFeature
                Return TbFeat.Name
            Catch ex As Exception
                MsgBox(ex.Message)
            End Try
        End If
    End Function

    Function UpdateBOM(ByVal BomName As String, ByVal myModel As ModelDoc2) As String
        Dim myTableAnno As TableAnnotation
        Dim mySelMgr As SelectionMgr = myModel.SelectionManager
        Dim mySkMgr As SketchManager = myModel.SketchManager

        'Dim mymodview As ModelView = myModel.ActiveView
        'Dim mos As Mouse = mymodview.GetMouse
        'mos.

        myModel.ClearSelection2(True)
        run = myModel.Extension.SelectByID2(BomName, "GENERALTABLEFEAT", 0.405, 0.282, 0, False, 0, Nothing, 0)
        If run Then
            ' MsgBox("Bom is selected")
            Dim TbFeat As GeneralTableFeature
            TbFeat = mySelMgr.GetSelectedObject6(1, 0)
            Dim tabAnno As Object = TbFeat.GetTableAnnotations
            myTableAnno = tabAnno(0)
            Dim cnt As Integer = myTableAnno.RowCount
            For i = cnt To 1 Step -1
                myTableAnno.DeleteRow(i)
            Next
            cnt = mySkMgr.GetSketchBlockDefinitionCount()
            For i = 1 To cnt - 1
                myTableAnno.InsertRow(swTableItemInsertPosition_e.swTableItemInsertPosition_Last, i)
            Next
            Dim BlkDefs As Object = mySkMgr.GetSketchBlockDefinitions()
            For i As Integer = 1 To cnt - 1
                For j As Integer = 0 To 2
                    If j = 0 Then
                        myTableAnno.Text(i, j) = i.ToString
                    ElseIf j = 1 Then
                        myTableAnno.Text(i, j) = RemovePath(BlkDefs(i).filename)
                    ElseIf j = 2 Then
                        myTableAnno.Text(i, j) = BlkDefs(i).getinstancecount()
                    End If
                Next
            Next
            Dim TbFeat1 As Feature = myTableAnno.GeneralTableFeature.GetFeature
            Return TbFeat1.Name
        Else
            '  MsgBox("Bom not found")
        End If
    End Function

    Function FindNearestPoint(ByVal CurMPt As MathPoint) As MathPoint
        Dim cnt As Integer = MPtArry.Count
        Dim DistArray(cnt) As Double

        For i As Integer = 0 To cnt - 1
            Dim LastMPt As MathPoint = MPtArry.Item(i)
            Dim x1 As Double = CDbl(LastMPt.ArrayData(0))
            Dim y1 As Double = CDbl(LastMPt.ArrayData(1))
            Dim x2 As Double = CDbl(CurMPt.ArrayData(0))
            Dim y2 As Double = CDbl(CurMPt.ArrayData(1))
            'Dim Dist As Double = Math.Sqrt(((x1 - x2) ^ 2) + ((y1 - y2) ^ 2))
            DistArray(i) = Math.Sqrt(((x1 - x2) ^ 2) + ((y1 - y2) ^ 2))
        Next
        Dim maxDist As Double = DistArray(0)
        Dim maxIdx As Integer = 0
        For i = 1 To cnt - 1
            If maxDist < DistArray(i) Then
                maxDist = DistArray(i)
                maxIdx = i
            End If
        Next
        Return MPtArry.Item(maxIdx)
    End Function
    Function MakeMouseClick(ByVal sw As SldWorks, ByVal mousepos As Integer()) As Double()

    End Function
End Module

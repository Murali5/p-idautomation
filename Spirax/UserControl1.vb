Imports System
Imports System.IO
Imports System.Diagnostics
Imports System.Runtime.InteropServices
'Imports SolidWorks.Interop
Imports SolidWorks.Interop.sldworks
Imports SolidWorks.Interop.swconst
Imports SolidWorks.Interop.swpublished
Imports SolidWorks.Interop.swcommands
Imports System.Windows.Forms
Imports System.Drawing
'Imports System.Diagnostics
<ComVisible(True)> <ProgId("Spirax.SWTaskPane_SwAddin")> _
Public Class UserControl1
    Dim PPage As UserPMPage
    Dim swapp As SldWorks
    Dim swModel As ModelDoc2
    Dim swdraw As DrawingDoc
    Dim swSheet As Sheet
    Dim swMathPoint As MathPoint
    Dim swMPt As Object
    Dim vnPt(2) As Double
    Dim skMgr As SketchManager
    Dim swMUtil As MathUtility

    Dim swBlkDef As SketchBlockDefinition
    Dim swBlkInst As SketchBlockInstance

    Dim AttrCnt As Integer
    Dim AttrName As String
    Dim AttrVal As String
    Dim Attr(,) As String
    Dim FreshFile As Boolean = True
    ' <DllImport("user32.dll")> _
    ' Private Shared Sub mouse_event(ByVal dwFlags As UInteger, ByVal dx As UInteger, ByVal dy As UInteger, ByVal dwData As UInteger, ByVal dwExtraInfo As Integer)
    ' End Sub
    ' Public Declare Sub mouse_event Lib "user32" Alias "mouse_event" (ByVal dwFlags As Long, ByVal dx As Long, ByVal dy As Long, ByVal cButtons As Long, ByVal dwExtraInfo As Long)
    'Public Const MOUSEEVENTF_LEFTDOWN = &H2
    'Public Const MOUSEEVENTF_LEFTUP = &H4
    'Public Const MOUSEEVENTF_MIDDLEDOWN = &H20
    'Public Const MOUSEEVENTF_MIDDLEUP = &H40
    'Public Const MOUSEEVENTF_RIGHTDOWN = &H8
    'Public Const MOUSEEVENTF_RIGHTUP = &H10
    'Public Const MOUSEEVENTF_MOVE = &H1


    Friend Sub GetswApp(ByRef swappIn As SldWorks)
        swapp = swappIn
    End Sub

    Public Sub New()
        '  MyBase.New()
        ' This call is required by the designer.
        InitializeComponent()
        'Me.DataContext = New MainWindowViewModel
        AddHandler MyBase.Load, AddressOf UserControl1_Load
#If (Debug) Then
        '      System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical
#End If
        ' Add any initialization after the InitializeComponent() call.
    End Sub
    Sub setAttrVal(ByVal blkInst As SketchBlockInstance)
        Dim run As Boolean
        If Not blkInst Is Nothing Then
            run = blkInst.SetAttributeValue("Name", Me.TextBox1.Text)
            run = blkInst.SetAttributeValue("Type", RemovePath(blkInst.Definition.FileName))
            run = blkInst.SetAttributeValue("Size", Me.ComboBox3.Text)
            run = blkInst.SetAttributeValue("Material", Me.ComboBox4.Text)
            run = blkInst.SetAttributeValue("ByPass", "No")
            run = blkInst.SetAttributeValue("Mode", Me.ComboBox5.Text)
        End If
    End Sub
    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        'Open New template and refresh
        Dim Tmplate As String = swapp.GetUserPreferenceStringValue(swUserPreferenceStringValue_e.swDefaultTemplateDrawing)
        Debug.Print("Drawing template - " & Tmplate)
        Me.swModel = swapp.NewDocument("C:\ProgramData\SolidWorks\SOLIDWORKS 2016\templates\Drawing.drwdot", 2, 0.2794, 0.4318) '"C:\ProgramData\SolidWorks\SOLIDWORKS 2016\templates\Drawing.drwdot"
        Dim tit As String = Me.swModel.GetTitle
        Dim longstatus As Long
        swapp.ActivateDoc2(tit, False, longstatus)
        Me.swModel = swapp.ActiveDoc
        Me.swdraw = Me.swModel
        Dim swSheetWidth As Double
        Dim swSheetHeight As Double
        swSheetHeight = 0.297
        swSheetWidth = 0.42
        swSheet = Me.swdraw.GetCurrentSheet
        swSheet.SetProperties2(12, 12, 1, 1, False, swSheetWidth, swSheetHeight, True)
        swSheet.SetTemplateName("C:\ProgramData\SolidWorks\SOLIDWORKS 2016\lang\english\sheetformat\a3 - iso.slddrt")
        swSheet.ReloadTemplate(True)
        FreshFile = True
        Dim sheetname As String = swSheet.GetName

        Me.swMUtil = swapp.GetMathUtility
        Me.vnPt(0) = 0.04#
        Me.vnPt(1) = 0.1485#
        Me.vnPt(2) = 0.0#
        Me.swMPt = Me.vnPt
        Me.swMathPoint = swMUtil.CreatePoint(swMPt)
        swdraw.ActivateSheet(sheetname)
        Dim Fi As String = "E:\Murali Kannan\Projeccts\Spirax\StartFlow\FlowStarter.SLDBLK"
        swBlkInst = InsrtBlkInstFile(swMathPoint, Fi, swModel)
        TableName = InsertBOM(swModel)
        FreshFile = False
    End Sub
    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        'insert at selected point
        Me.swModel = Me.swapp.ActiveDoc
        If Not swModel Is Nothing Then
            If swModel.GetType = swDocumentTypes_e.swDocDRAWING Then
                swMathPoint = GetSelectedPoint(swapp)
                If Not swMathPoint Is Nothing Then
                    swBlkInst = InsrtBlkInstFile(swMathPoint, AddPath(Me.ComboBox2.Text), swModel)
                    InsrtLine(Me.swModel.SketchManager, NextInsrtPt(swapp, swBlkInst, 0, True))
                    swMathPoint = NextInsrtPt(swapp, swBlkInst)
                    SetDefaultAttributeVal(swBlkInst)
                    setAttrVal(swBlkInst)
                    TableName = UpdateBOM(TableName, swModel)
                    ' MsgBox(TableName)
                Else
                    MsgBox("Please select a Point and try Again")
                End If
            End If
        End If
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        '   CreateObject("WScript.Shell").Popup("Welcome", 1, "Title")
        '"E:\Murali Kannan\Projeccts\Spirax\StartFlow\FlowStarter.SLDBLK"
        Me.swModel = Me.swapp.ActiveDoc
        If Not swModel Is Nothing Then
            If Me.swModel.GetType = swDocumentTypes_e.swDocDRAWING Then
                If FreshFile Then
                    'inserting flowstarter
                    Dim Fi As String = "E:\Murali Kannan\Projeccts\Spirax\StartFlow\FlowStarter.SLDBLK"
                    Me.swMUtil = swapp.GetMathUtility
                    Me.vnPt(0) = 0.04#
                    Me.vnPt(1) = 0.1485#
                    Me.vnPt(2) = 0.0#
                    Me.swMPt = Me.vnPt
                    Me.swMathPoint = swMUtil.CreatePoint(swMPt)
                    swBlkInst = InsrtBlkInstFile(swMathPoint, Fi, swModel)
                    FreshFile = False
                    TableName = InsertBOM(swModel)
                    'MsgBox(TableName)
                End If

                'inserting blocks
                swBlkInst = InsrtBlkInstFile(swMathPoint, AddPath(Me.ComboBox2.Text), swModel)
                InsrtLine(Me.swModel.SketchManager, NextInsrtPt(swapp, swBlkInst, 0, True))
                swMathPoint = NextInsrtPt(swapp, swBlkInst)
                SetDefaultAttributeVal(swBlkInst)
                setAttrVal(swBlkInst)
                TableName = UpdateBOM(TableName, swModel)
                '  MsgBox(TableName)
                'Try
                '    Dim fi As String = "E:\Murali Kannan\Projeccts\Spirax\Blocks\3WayValve.SLDBLK"
                '    Me.swMUtil = Me.swapp.GetMathUtility
                '    skMgr = swModel.SketchManager
                '    Me.vnPt(0) = 0.04#
                '    Me.vnPt(1) = 0.1485#
                '    Me.vnPt(2) = 0.0#
                '    Me.swMPt = Me.vnPt

                '    Me.swMathPoint = Me.swMUtil.CreatePoint(swMPt)
                '    Me.swBlkDef = Me.skMgr.MakeSketchBlockFromFile(swMathPoint, fi, False, 1, 0)
                '    Dim instcnt As Integer
                '    instcnt = swBlkDef.GetInstanceCount
                '    Me.swBlkInst = CType(swBlkDef.IGetInstances(instcnt - 1), SketchBlockInstance)

                '    Me.AttrCnt = Me.swBlkInst.GetAttributeCount()
                '    MsgBox(Me.swBlkInst.GetAttributeCount().ToString)

                '    ReDim Me.Attr(Me.AttrCnt - 1, 1)
                '    Dim attrmsg As String = ""
                '    Dim swNote As Object = swBlkInst.GetAttributes()
                '    For i = 0 To AttrCnt - 1
                '        attrmsg &= swNote(i).GetText & vbNewLine
                '    Next
                '    CreateObject("WScript.Shell").popup(attrmsg, 1, "Attributes Of current Block")
                'Catch ex As Exception
                '    MsgBox(ex.Message)
                'End Try

                '  Me.swMUtil = Nothing
            Else
                MsgBox("Please Open a Drawing document to Continue", MsgBoxStyle.Exclamation, "Document Type Mismatch")
            End If
        Else
            'CreateObject("WScript.Shell").popup("Opening Default...", 1, "P $ I Diagram")
            Me.swModel = swapp.NewDocument("C:\ProgramData\SolidWorks\SOLIDWORKS 2016\templates\Drawing.drwdot", 2, 0.2794, 0.4318)
            Dim tit As String = Me.swModel.GetTitle
            Dim longstatus As Long
            swapp.ActivateDoc2(tit, False, longstatus)
            Me.swModel = swapp.ActiveDoc
            Me.swdraw = Me.swModel
            Dim swSheetWidth As Double
            Dim swSheetHeight As Double
            swSheetHeight = 0.27
            swSheetWidth = 0.42
            swSheet = Me.swdraw.GetCurrentSheet
            swSheet.SetProperties2(12, 12, 1, 1, False, swSheetWidth, swSheetHeight, True)
            swSheet.SetTemplateName("C:\ProgramData\SolidWorks\SOLIDWORKS 2016\lang\english\sheetformat\a3 - iso.slddrt")
            swSheet.ReloadTemplate(True)
            FreshFile = True
            Call Button1_Click(sender, e)
            'TableName = InsertBOM(swModel)
            ' MsgBox(TableName)
        End If
    End Sub

    Function PMPEnable() As Integer
        If swapp.ActiveDoc Is Nothing Then
            PMPEnable = 0
        Else
            PMPEnable = 1
        End If
    End Function

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        PMPEnable()
        PPage = New UserPMPage
        Dim swadim As New SwAddin
        PPage.Init(swapp, swadim)
        If Not PPage Is Nothing Then
            PPage.Show()
        Else
            MsgBox("pmp is nothing")
        End If
    End Sub

    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click
        'insert BOM
        'InsertBOM(swapp.ActiveDoc)
        Me.ComboBox2.DataSource = ReadBlocksFromLib("E:\Murali Kannan\Projeccts\Spirax\Blocks")
        Dim BlkLib() As String = ReadBlocksFromLib("E:\Murali Kannan\Projeccts\Spirax\Blocks")
        FlowLayoutPanel1.AutoScroll = True
        For i As Integer = 0 To BlkLib.Length
            Dim imgBox As New PictureBox
            imgBox.Name = i.ToString
            imgBox.Image = DocPreview(BlkLib(i), i)
            FlowLayoutPanel1.Controls.Add(imgBox)
            '        imgBox.Size = Drawing.Size
            ' Dim Imgsze As Drawing.Size = Drawing.Size.new(100, 100)
            'imgBox.Size = 100
            'imgBox.Size.Width = 100
        Next
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

    Private Sub UserControl1_Load(ByVal sender As Object, ByVal e As System.EventArgs) 'Handles Me.Load
        ' Me.ComboBox3.DataSource = New String() {"0.25""", "0.5""", "1""", "1.5""", "2"""}
        ' Me.ComboBox4.DataSource = New String() {"Carbon Steel", "Cast Iron", "Stainless Steel", "SG Iron", "AISI 304", "Alloy Steel", "Gray Cast Iron", "3060 Alloy", "Brass", "Copper"}
        ' Me.ComboBox5.DataSource = New String() {"Mechanical", "Electrical"}
    End Sub

    Private Sub CheckBox1_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox1.CheckedChanged
        If CheckBox1.Checked = True Then
            Me.GroupBox1.Enabled = True
            Me.ComboBox2.DataSource = ReadBlocksFromLib("E:\Murali Kannan\Projeccts\Spirax\Blocks")
            Me.ComboBox3.DataSource = New String() {"0.25""", "0.5""", "1""", "1.5""", "2"""}
            Me.ComboBox4.DataSource = New String() {"Carbon Steel", "Cast Iron", "Stainless Steel", "SG Iron", "AISI 304", "Alloy Steel", "Gray Cast Iron", "3060 Alloy", "Brass", "Copper"}
            Me.ComboBox5.DataSource = New String() {"Mechanical", "Electrical"}

            Me.FlowLayoutPanel1.Enabled = True
            Me.FlowLayoutPanel1.AutoScroll = True
            Me.FlowLayoutPanel1.Controls.Clear()

            Dim myBlkFiles() As String = ReadBlocksFromLibWithFullName("E:\Murali Kannan\Projeccts\Spirax\Blocks")
            Dim BlkCnt As Integer = myBlkFiles.Length
            For i = 0 To BlkCnt - 1
                Debug.Print("BlkCnt - " & i.ToString)
                Dim btn As New Button
                btn.Name = i.ToString
                btn.FlatStyle = FlatStyle.Flat
                btn.TextAlign = Drawing.ContentAlignment.BottomCenter
                btn.ForeColor = Drawing.Color.White
                btn.BackgroundImage = DocPreview(myBlkFiles(i), i.ToString)
                btn.Text = RemovePath(myBlkFiles(i))
                btn.BackgroundImageLayout = ImageLayout.Zoom
                btn.Size = New System.Drawing.Size(150, 125)
                btn.FlatAppearance.BorderSize = 0
                AddHandler btn.MouseDown, AddressOf MouseDown_Event
                AddHandler btn.DragDrop, AddressOf MouseDragDrop_Event
                AddHandler btn.MouseUp, AddressOf MouseUp_Event
                Me.FlowLayoutPanel1.Controls.Add(btn)
            Next
        Else
            Me.GroupBox1.Enabled = False
            Me.FlowLayoutPanel1.Controls.Clear()
            Me.FlowLayoutPanel1.Enabled = False
        End If
    End Sub

    Sub MouseDragDrop_Event(ByVal sender As Object, ByVal e As DragEventArgs)
        MouseSelectDrop = False
        MsgBox("Mouse Drop")
    End Sub

    Sub MouseUp_Event(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        '   PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical
        If Not sender Is Nothing Then
            Dim flowlayoutloc = Me.FlowLayoutPanel1.Location
            Dim btn As Button = sender
            CreateObject("WScript.Shell").Popup("Inserting " & btn.Text, 1, "")
            '            MsgBox(Me.FlowLayoutPanel1.PointToScreen(btn.Location).X + e.X & "  " & Me.FlowLayoutPanel1.PointToScreen(btn.Location).Y + e.Y)
            '    MsgBox(e.X & " " & e.Y)

            ' Try
            '     mypos = Windows.Forms.Cursor.Position
            'If mypos = Nothing Then
            '    Do Until mypos <> Nothing
            '        mypos = Windows.Forms.Cursor.Position
            '    Loop
            'End If
            ' mypos = Windows.Forms.Cursor.Current.HotSpot
            Dim mymod As ModelDoc2 = swapp.ActiveDoc
            Dim myModView As ModelView = mymod.ActiveView
            Dim myModExt As ModelDocExtension = mymod.Extension
            '  myModExt.HideFeatureManager(True)
            Dim mymouse As Mouse = myModView.GetMouse
            '   Dim btns As Button = sender
            MouseSelBlkFileName = btn.Text
            '   CreateObject("WScript.Shell").Popup("Inserting " & btns.Text, 1, "")
            'MsgBox("Inserting ")
            If MouseSelectDrop Then
                'MsgBox("select drop true")
                'Threading.Thread.Sleep(500)
                'mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0)
                ' MsgBox("down")
                ' mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0)
                'MsgBox("up")
                Dim LimitLoc As Object = myModView.GetVisibleBox()
                Dim minX As Integer = LimitLoc(0)
                Dim miny As Integer = LimitLoc(1)
                Dim maxX As Integer = LimitLoc(2)
                Dim maxY As Integer = LimitLoc(3)
                'Threading.Thread.Sleep(500)
                Dim a1 As Integer = Me.FlowLayoutPanel1.PointToScreen(btn.Location).X + e.X
                Dim b1 As Integer = Me.FlowLayoutPanel1.PointToScreen(btn.Location).Y + e.Y
                Dim mypos As Drawing.Point
                mypos = New Point(a1, b1)
                If mypos.IsEmpty Then
                    '   MsgBox("Empty")
                Else
                    '  MsgBox("X" & mypos.X & " - y" & mypos.Y)
                End If
                If mypos.X < maxX And mypos.X > minX And mypos.Y < maxY And mypos.Y > miny Then
                    ' If mypos = Windows.Forms.Cursor.Position Then
                    If mymouse.Move(mypos.X - minX, mypos.Y - miny, swMouse_e.swMouse_Click) Then
                        '    Threading.Thread.Sleep(250)
                        'MsgBox("Move done")
                        '   If mymouse.Move(mypos.X - minX, mypos.Y - miny, swMouse_e.swMouse_Click) Then
                        ' MsgBox("Mouse click")
                        '  TableName = UpdateBOM(TableName, swModel)
                        'End If
                    End If
                    'End If
                    MouseSelectDrop = False
                Else
                    'MsgBox("out of location")
                End If
                '  MsgBox("out of line")
            End If
            'Catch ex As Exception
            '    MsgBox(ex.Message)
            '    mypos = Windows.Forms.Cursor.Current.HotSpot
            'End Try
            '    MsgBox("end")
            'AddHandler mymouse.MouseSelectNotify, AddressOf MouseEventHandler.MouseSelectNotify
            'RaiseEvent MouseSelectNotify()
        Else
            MsgBox("sender empty")
        End If
       
    End Sub
    Sub MouseDown_Event(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        Dim Btn As Button = sender
        MouseSelectedBlkBtn = CInt(Btn.Name)
        MouseSelectDrop = True
    End Sub

    Private Sub FlowLayoutPanel1_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs) Handles FlowLayoutPanel1.MouseEnter
        Me.FlowLayoutPanel1.Focus()
    End Sub

    Private Sub FlowLayoutPanel1_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles FlowLayoutPanel1.MouseLeave
    End Sub

    Private Sub FlowLayoutPanel1_MouseWheel(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles FlowLayoutPanel1.MouseWheel
        Dim Sensitivity As Integer = 20
        If FlowLayoutPanel1.Bounds.Contains(e.Location) Then
            Dim vScrollPosition As Integer = FlowLayoutPanel1.VerticalScroll.Value
            vScrollPosition -= Math.Sign(e.Delta) * Sensitivity
            vScrollPosition = Math.Max(0, vScrollPosition)
            vScrollPosition = Math.Min(vScrollPosition, Me.VerticalScroll.Maximum)
            FlowLayoutPanel1.AutoScrollPosition = New System.Drawing.Point(FlowLayoutPanel1.AutoScrollPosition.X, vScrollPosition)
            Panel1.Invalidate()
        End If
    End Sub

    Private Sub FlowLayoutPanel1_Scroll(ByVal sender As Object, ByVal e As System.Windows.Forms.ScrollEventArgs) Handles FlowLayoutPanel1.Scroll
    End Sub
End Class

'Namespace ConsoleApplication1
'    Class Program
'        Private Shared Sub Main(ByVal args As String())
'            Dim StringArray As String() = Cast(GetStringArray(), New With {Key .MyList = New String() {}}).MyList

'            For Each item As String In StringArray
'                Console.WriteLine(item)
'            Next

'            Console.ReadLine()
'        End Sub

'        Private Shared Function GetStringArray() As Object
'            Return New With {Key .MyList = New String() {"Code", "Project", "Question", "Answer"}}
'        End Function

'        Private Shared Function Cast(Of T)(ByVal obj As Object, ByVal Type As T) As T
'            Return CType(obj, T)
'        End Function
'    End Class
'End Namespace

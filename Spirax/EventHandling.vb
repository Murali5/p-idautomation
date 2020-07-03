Imports SolidWorks.Interop.sldworks
Imports SolidWorks.Interop.swconst
Imports SolidWorks.Interop.swcommands

'Base class for model event handlers
Public Class DocumentEventHandler
    Protected openModelViews As New Hashtable()
    Protected userAddin As SwAddin
    Protected iDocument As ModelDoc2
    Protected iSwApp As SldWorks
    Overridable Function Init(ByVal sw As SldWorks, ByVal addin As SwAddin, ByVal model As ModelDoc2) As Boolean
        userAddin = addin
        iSwApp = sw
        iDocument = model
    End Function
    Overridable Function AttachEventHandlers() As Boolean
    End Function
    Overridable Function DetachEventHandlers() As Boolean
    End Function

    Function ConnectModelViews() As Boolean
        Dim iModelView As ModelView
        iModelView = iDocument.GetFirstModelView()
        While (Not iModelView Is Nothing)
            If Not openModelViews.Contains(iModelView) Then
                Dim mView As New DocView()
                mView.Init(userAddin, iModelView, Me)
                mView.AttachEventHandlers()
                openModelViews.Add(iModelView, mView)
            End If
            iModelView = iModelView.GetNext
        End While
    End Function

    Function ConnectMouse() As Boolean
        '   Dim iModelView As ModelView
        '  iModelView = iDocument.GetFirstModelView()
        If (Not iDocument Is Nothing) Then
            Dim mMouse As New MouseEventHandler()
            mMouse.Init(iSwApp, userAddin, iDocument, Me)
            mMouse.AttachEventHandlers()
        End If
    End Function
    Function DisConnectMouse() As Boolean

    End Function
    Function DisconnectModelViews() As Boolean
        'Close events on all currently open docs
        Dim mView As DocView
        Dim key As ModelView
        Dim numKeys As Integer
        numKeys = openModelViews.Count
        Dim keys() As Object = New Object(numKeys - 1) {}
        'Remove all ModelView event handlers
        openModelViews.Keys.CopyTo(keys, 0)
        For Each key In keys
            mView = openModelViews.Item(key)
            mView.DetachEventHandlers()
            openModelViews.Remove(key)
            mView = Nothing
            key = Nothing
        Next
    End Function
    Sub DetachModelViewEventHandler(ByVal mView As ModelView)
        Dim docView As DocView
        If openModelViews.Contains(mView) Then
            docView = openModelViews.Item(mView)
            openModelViews.Remove(mView)
            mView = Nothing
            docView = Nothing
        End If

    End Sub
End Class

'Class to listen for Part Events
Public Class PartEventHandler
    Inherits DocumentEventHandler

    Dim WithEvents iPart As PartDoc

    Overrides Function Init(ByVal sw As SldWorks, ByVal addin As SwAddin, ByVal model As ModelDoc2) As Boolean
        userAddin = addin
        iPart = model
        iDocument = iPart
        iSwApp = sw
    End Function

    Overrides Function AttachEventHandlers() As Boolean
        AddHandler iPart.DestroyNotify, AddressOf Me.PartDoc_DestroyNotify
        AddHandler iPart.NewSelectionNotify, AddressOf Me.PartDoc_NewSelectionNotify
        ConnectModelViews()
    End Function

    Overrides Function DetachEventHandlers() As Boolean
        RemoveHandler iPart.DestroyNotify, AddressOf Me.PartDoc_DestroyNotify
        RemoveHandler iPart.NewSelectionNotify, AddressOf Me.PartDoc_NewSelectionNotify

        DisconnectModelViews()

        userAddin.DetachModelEventHandler(iDocument)
    End Function

    Function PartDoc_DestroyNotify() As Integer
        DetachEventHandlers()
    End Function

    Function PartDoc_NewSelectionNotify() As Integer

    End Function
End Class

'Class to listen for Assembly Events
Public Class AssemblyEventHandler
    Inherits DocumentEventHandler

    Dim WithEvents iAssembly As AssemblyDoc
    Dim swAddin As SwAddin

    Overrides Function Init(ByVal sw As SldWorks, ByVal addin As SwAddin, ByVal model As ModelDoc2) As Boolean
        userAddin = addin
        iAssembly = model
        iDocument = iAssembly
        iSwApp = sw
        swAddin = addin

    End Function

    Overrides Function AttachEventHandlers() As Boolean
        AddHandler iAssembly.DestroyNotify, AddressOf Me.AssemblyDoc_DestroyNotify
        AddHandler iAssembly.NewSelectionNotify, AddressOf Me.AssemblyDoc_NewSelectionNotify
        AddHandler iAssembly.ComponentStateChangeNotify, AddressOf Me.AssemblyDoc_ComponentStateChangeNotify
        AddHandler iAssembly.ComponentStateChangeNotify2, AddressOf Me.AssemblyDoc_ComponentStateChangeNotify2
        AddHandler iAssembly.ComponentVisualPropertiesChangeNotify, AddressOf Me.AssemblyDoc_ComponentVisiblePropertiesChangeNotify
        AddHandler iAssembly.ComponentDisplayStateChangeNotify, AddressOf Me.AssemblyDoc_ComponentDisplayStateChangeNotify

        ConnectModelViews()
    End Function

    Overrides Function DetachEventHandlers() As Boolean
        RemoveHandler iAssembly.DestroyNotify, AddressOf Me.AssemblyDoc_DestroyNotify
        RemoveHandler iAssembly.NewSelectionNotify, AddressOf Me.AssemblyDoc_NewSelectionNotify
        RemoveHandler iAssembly.ComponentStateChangeNotify, AddressOf Me.AssemblyDoc_ComponentStateChangeNotify
        RemoveHandler iAssembly.ComponentStateChangeNotify2, AddressOf Me.AssemblyDoc_ComponentStateChangeNotify2
        RemoveHandler iAssembly.ComponentVisualPropertiesChangeNotify, AddressOf Me.AssemblyDoc_ComponentVisiblePropertiesChangeNotify
        RemoveHandler iAssembly.ComponentDisplayStateChangeNotify, AddressOf Me.AssemblyDoc_ComponentDisplayStateChangeNotify

        DisconnectModelViews()
        userAddin.DetachModelEventHandler(iDocument)
    End Function

    Function AssemblyDoc_DestroyNotify() As Integer
        DetachEventHandlers()
    End Function

    Function AssemblyDoc_NewSelectionNotify() As Integer

    End Function

    Protected Function ComponentStateChange(ByVal componentModel As Object, Optional ByVal newCompState As Short = swComponentSuppressionState_e.swComponentResolved) As Integer

        Dim modDoc As ModelDoc2 = componentModel
        Dim newState As swComponentSuppressionState_e = newCompState


        Select Case newState

            Case swComponentSuppressionState_e.swComponentFullyResolved, swComponentSuppressionState_e.swComponentResolved

                If ((Not modDoc Is Nothing) AndAlso Not Me.swAddin.OpenDocumentsTable.Contains(modDoc)) Then
                    Me.swAddin.AttachModelDocEventHandler(modDoc)
                End If

                Exit Select

        End Select

    End Function

    'attach events to a component if it becomes resolved
    Public Function AssemblyDoc_ComponentStateChangeNotify(ByVal componentModel As Object, ByVal oldCompState As Short, ByVal newCompState As Short) As Integer

        Return ComponentStateChange(componentModel, newCompState)

    End Function

    'attach events to a component if it becomes resolved
    Public Function AssemblyDoc_ComponentStateChangeNotify2(ByVal componentModel As Object, ByVal CompName As String, ByVal oldCompState As Short, ByVal newCompState As Short) As Integer

        Return ComponentStateChange(componentModel, newCompState)

    End Function


    Public Function AssemblyDoc_ComponentVisiblePropertiesChangeNotify(ByVal swObject As Object) As Integer

        Dim component As Component2
        Dim modDoc As ModelDoc2

        component = swObject

        modDoc = component.GetModelDoc

        Return ComponentStateChange(modDoc)

    End Function


    Public Function AssemblyDoc_ComponentDisplayStateChangeNotify(ByVal swObject As Object) As Integer

        Dim component As Component2
        Dim modDoc As ModelDoc2

        component = swObject

        modDoc = component.GetModelDoc

        Return ComponentStateChange(modDoc)

    End Function


End Class

'Class to listen for Drawing Events
Public Class DrawingEventHandler
    Inherits DocumentEventHandler
    Dim PPage As UserPMPage
    Dim WithEvents iDrawing As DrawingDoc

    Overrides Function Init(ByVal sw As SldWorks, ByVal addin As SwAddin, ByVal model As ModelDoc2) As Boolean
        userAddin = addin
        iDrawing = model
        iDocument = iDrawing
        iSwApp = sw
    End Function

    Overrides Function AttachEventHandlers() As Boolean
        AddHandler iDrawing.DestroyNotify, AddressOf Me.DrawingDoc_DestroyNotify
        AddHandler iDrawing.NewSelectionNotify, AddressOf Me.DrawingDoc_NewSelectionNotify
        AddHandler iDrawing.DynamicHighlightNotify, AddressOf Me.DrawingDoc_DynamicHighlight
        AddHandler iDrawing.DeleteItemNotify, AddressOf Me.DrawingDoc_DeleteItemNotify
        AddHandler iDrawing.ClearSelectionsNotify, AddressOf Me.DrawingDoc_clearselectionNotify
        AddHandler iDrawing.AddItemNotify, AddressOf Me.DrawingDoc_AdditemNotify
        AddHandler iDrawing.NewSelectionNotify, AddressOf Me.DrawingDoc_NewSelectionNotify
        AddHandler iDrawing.UserSelectionPostNotify, AddressOf Me.UserSelectionPostNotify
        AddHandler iDrawing.UserSelectionPreNotify, AddressOf Me.UserselectionPreNotify
        AddHandler iDrawing.ActivateSheetPostNotify, AddressOf Me.ActivateSheetPostNotify
        AddHandler iDrawing.SketchSolveNotify, AddressOf DrawingDoc_SketchSolveNotify

        ConnectModelViews()
        ConnectMouse()
    End Function

    Overrides Function DetachEventHandlers() As Boolean
        RemoveHandler iDrawing.DestroyNotify, AddressOf Me.DrawingDoc_DestroyNotify
        RemoveHandler iDrawing.NewSelectionNotify, AddressOf Me.DrawingDoc_NewSelectionNotify
        RemoveHandler iDrawing.DynamicHighlightNotify, AddressOf Me.DrawingDoc_DynamicHighlight
        RemoveHandler iDrawing.DeleteItemNotify, AddressOf Me.DrawingDoc_DeleteItemNotify
        RemoveHandler iDrawing.ClearSelectionsNotify, AddressOf Me.DrawingDoc_clearselectionNotify
        RemoveHandler iDrawing.AddItemNotify, AddressOf Me.DrawingDoc_AdditemNotify
        RemoveHandler iDrawing.NewSelectionNotify, AddressOf Me.DrawingDoc_NewSelectionNotify
        RemoveHandler iDrawing.UserSelectionPostNotify, AddressOf Me.UserSelectionPostNotify
        RemoveHandler iDrawing.UserSelectionPreNotify, AddressOf Me.UserselectionPreNotify
        RemoveHandler iDrawing.ActivateSheetPostNotify, AddressOf Me.ActivateSheetPostNotify
        RemoveHandler iDrawing.SketchSolveNotify, AddressOf DrawingDoc_SketchSolveNotify

        DisconnectModelViews()
        userAddin.DetachModelEventHandler(iDocument)
    End Function

    Function ActivateSheetPostNotify(ByVal sheetname As String) As Integer
        ' MsgBox("Activate sheet post notify")
    End Function

    Function DrawingDoc_DynamicHighlight() As Boolean
        '  MsgBox("Drawing Doc Highlight notify")
    End Function

    Function DrawingDoc_SketchSolveNotify(ByVal featName As System.String) As Integer
        '   MsgBox("sketch solve notify -" & featName)
    End Function

    Function DrawingDoc_DestroyNotify() As Integer
        DetachEventHandlers()
    End Function

    Function DrawingDoc_DeleteItemNotify() As Integer
        '  MsgBox("Delete Item notify")
    End Function

    Function DrawingDoc_clearselectionNotify() As Integer
        ' MsgBox("clear selection notify")
    End Function

    Function DrawingDoc_NewSelectionNotify() As Integer
        '  MsgBox("new selection notify")
        Dim swapp = iSwApp
        Dim swModel As ModelDoc2
        Dim swSelMgr As SelectionMgr
        swModel = GetActiveDoc(swapp)
        If Not swModel Is Nothing And swModel.GetType = swDocumentTypes_e.swDocDRAWING Then
            swSelMgr = swModel.SelectionManager
            If swSelMgr.GetSelectedObjectCount2(0) > 0 Then
                For i = 1 To swSelMgr.GetSelectedObjectCount2(0)
                    Debug.Print("swselObj Type 3:- " & swSelMgr.GetSelectedObjectType3(1, 0))
                Next
                'If swSelMgr.GetSelectedObjectType2(1) = swSelectType_e.swSelSUBSKETCHINST Then
                '    Return True
                'Else
                '    Return False
                'End If
            End If
        End If
    End Function

    Function ShowPPage() As Boolean
        If CheckBlk(iSwApp) Then
            Debug.Print("Block")
            Dim ppage1 As New PropBox
            ppage1.PropStr = GetBlkAttribute(iSwApp, GetBlkInst(iSwApp))
            ppage1.swapp = iSwApp
            ppage1.Focus()
            ppage1.ShowDialog()
            '' Dim swAttr(5) As String
            'PPage = New UserPMPage
            'Dim swadin As New SwAddin
            'PPage.Init(iSwApp, swadin)
            'If Not PPage Is Nothing Then
            '    ' Threading.Thread.Sleep(1000)
            '    PPage.Show()
            '    Debug.Print("PMP is valid")
            ''    PPage.Show()
            Return True
            ' Else
            '    Debug.Print("PMP is nothing")
            '   Return False
            'End If
            '  MsgBox("Block")
        Else
            Debug.Print("not a block")
            'MsgBox("Not a Block")
            Return False
        End If
    End Function

    Function UserselectionPreNotify() As Integer
        '   MsgBox("User Selection Pre-Notify")
        '  ShowPPage()
    End Function

    Function UserSelectionPostNotify() As Integer
        'MsgBox("User selection Post_notify")
        Debug.Print("UserselectionNotify")

        If ShowPPage() Then
            Debug.Print("Pmp shown")
        Else
            Debug.Print("Pmp Hidden")
        End If
        ' ShowPPage()
    End Function

    Function DrawingDoc_AdditemNotify() As Integer
        '     MsgBox("Add item notify")
        ' ShowPPage()
    End Function
End Class

'class for handling mouse events
Public Class MouseEventHandler

    Dim WithEvents iMouse As Mouse
    Dim iModelDoc As IModelDoc2
    Dim userAddin As SwAddin
    '  Dim parentDoc As DocumentEventHandler
    Dim iswapp As SldWorks
    Function Init(ByVal sw As SldWorks, ByVal addin As SwAddin, ByVal model As ModelDoc2, ByVal parent As DocumentEventHandler) As Boolean
        iswapp = sw
        userAddin = addin
        iModelDoc = model
        Dim MyModelView As IModelView = iModelDoc.ActiveView
        iMouse = MyModelView.GetMouse
    End Function

    Function AttachEventHandlers() As Boolean
        AddHandler iMouse.MouseLBtnUpNotify, AddressOf MouseLBtnUpNotify
        AddHandler iMouse.MouseSelectNotify, AddressOf MouseSelectNotify

        'Return MyBase.AttachEventHandlers()
    End Function
    Function DetachEventHandlers() As Boolean
        RemoveHandler iMouse.MouseLBtnUpNotify, AddressOf MouseLBtnUpNotify
        RemoveHandler iMouse.MouseSelectNotify, AddressOf MouseSelectNotify
        ' userAddin.DetachModelEventHandler(iModelDoc)
        '    Return MyBase.DetachEventHandlers()
    End Function
    Function MouseLBtnDownNotify(ByVal x As Integer, ByVal y As Integer, ByVal wParam As Integer) As Integer
        'MsgBox("Mouse Down")
    End Function
    'waste
    Function MouseLBtnUpNotify(ByVal x As Integer, ByVal y As Integer, ByVal WParam As Integer) As Integer
        'Mouse Left button up
        '   MsgBox("Mouse up " & x.ToString & " - " & y.ToString & " - " & WParam.ToString)
    End Function

    Function MouseSelectNotify(ByVal Ix As Integer, ByVal Iy As Integer, ByVal x As Double, ByVal y As Double, ByVal z As Double) As Integer
        'mouse select notify
        If MouseSelectDrop Then
            iModelDoc.ClearSelection2(True)
            Dim mpt As MathUtility = iswapp.IGetMathUtility
            InsMPt = mpt.CreatePoint({x, y, z})
            '            Dim BlkLists As String() = ReadBlocksFromLib("E:\Murali Kannan\Projeccts\Spirax\Blocks")
            InsrtBlkInstFile(InsMPt, AddPath(MouseSelBlkFileName), iModelDoc)
            '  TableName = UpdateBOM(TableName, iModelDoc)
        End If
        
        '  MsgBox("Mouse select x=" & x.ToString & " - y=" & y.ToString & " - z=" & z.ToString & " - ix=" & Ix.ToString & " - iy=" & Iy.ToString)
        ' Dim ui As IModelView = iModelDoc.ActiveView
        '  Dim sizewin As Object = ui.GetVisibleBox()
        '  MsgBox("modelview  " & sizewin(0) & " * " & sizewin(1) & " * " & sizewin(2) & " * " & sizewin(3))
    End Function
End Class

'Public Delegate Function mouseclik(ByVal x As Integer, ByVal y As Integer) As Integer

'Class for handling ModelView events
Public Class DocView
    Dim WithEvents iModelView As ModelView
    Dim userAddin As SwAddin
    Dim parentDoc As DocumentEventHandler
    Function Init(ByVal addin As SwAddin, ByVal mView As ModelView, ByVal parent As DocumentEventHandler) As Boolean
        userAddin = addin
        iModelView = mView
        parentDoc = parent
    End Function

    Function AttachEventHandlers() As Boolean
        AddHandler iModelView.DestroyNotify2, AddressOf Me.ModelView_DestroyNotify2
        AddHandler iModelView.RepaintNotify, AddressOf Me.ModelView_RepaintNotify
    End Function

    Function DetachEventHandlers() As Boolean
        RemoveHandler iModelView.DestroyNotify2, AddressOf Me.ModelView_DestroyNotify2
        RemoveHandler iModelView.RepaintNotify, AddressOf Me.ModelView_RepaintNotify
        parentDoc.DetachModelViewEventHandler(iModelView)
    End Function

    Function ModelView_DestroyNotify2(ByVal destroyTYpe As Integer) As Integer
        DetachEventHandlers()
    End Function

    Function ModelView_RepaintNotify(ByVal repaintTYpe As Integer) As Integer
    End Function
End Class

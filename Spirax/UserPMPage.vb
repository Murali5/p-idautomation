Imports SolidWorks.Interop.sldworks
Imports SolidWorks.Interop.swconst
Imports SolidWorks.Interop.swpublished

Public Class UserPMPage
    Dim iSwApp As SldWorks
    Dim userAddin As SwAddin
    Dim handler As PMPageHandler
    Dim ppage As PropertyManagerPage2

    Public PageName As String
    Public swattr(5) As String


#Region "Property Manager Page Controls"
    'Groups
    Dim group1 As PropertyManagerPageGroup
    '  Dim group2 As PropertyManagerPageGroup

    'Controls
    Dim lbl1 As PropertyManagerPageLabel
    Dim lbl2 As PropertyManagerPageLabel
    Dim lbl3 As PropertyManagerPageLabel
    Dim lbl4 As PropertyManagerPageLabel
    Dim lbl5 As PropertyManagerPageLabel
    Dim lbl6 As PropertyManagerPageLabel
    Dim txt1 As PropertyManagerPageTextbox
    Dim cmb1 As PropertyManagerPageCombobox
    Dim cmb2 As PropertyManagerPageCombobox
    Dim cmb3 As PropertyManagerPageCombobox
    Dim cmb4 As PropertyManagerPageCombobox
    Dim cmb5 As PropertyManagerPageCombobox

    'Control IDs
    Dim group1ID As Integer = 0
    Dim lbl1ID As Integer = 1
    Dim txt1ID As Integer = 2
    Dim lbl2ID As Integer = 3
    Dim cmb1ID As Integer = 4
    Dim lbl3ID As Integer = 5
    Dim cmb2ID As Integer = 6
    Dim lbl4ID As Integer = 7
    Dim cmb3ID As Integer = 8
    Dim lbl5ID As Integer = 9
    Dim cmb4ID As Integer = 10
    Dim lbl6ID As Integer = 11
    Dim cmb5ID As Integer = 12

   
#End Region

    Sub Init(ByVal sw As SldWorks, ByVal addin As SwAddin)
        iSwApp = sw
        userAddin = addin
        CreatePage()
        AddControls()
    End Sub

    Sub Show()
        ppage.Show()
    End Sub

    Sub CreatePage()
        handler = New PMPageHandler()
        handler.Init(iSwApp, userAddin)
        Dim options As Integer
        Dim errors As Integer
        options = swPropertyManagerPageOptions_e.swPropertyManagerOptions_OkayButton + swPropertyManagerPageOptions_e.swPropertyManagerOptions_CancelButton
        If PageName = "" Then PageName = "Properties"
        ppage = iSwApp.CreatePropertyManagerPage(PageName, options, handler, errors)
    End Sub

    Sub AddControls()
        Dim options As Integer
        Dim leftAlign As Integer
        Dim controlType As Integer

        'Add Groups
        options = swAddGroupBoxOptions_e.swGroupBoxOptions_Expanded + swAddGroupBoxOptions_e.swGroupBoxOptions_Visible
        group1 = ppage.AddGroupBox(group1ID, "Properties", options)

        'options = swAddGroupBoxOptions_e.swGroupBoxOptions_Checkbox + swAddGroupBoxOptions_e.swGroupBoxOptions_Visible
        'group2 = ppage.AddGroupBox(group2ID, "Sample Group II", options)

        'Add Controls to Group1 
        'lbl1
        controlType = swPropertyManagerPageControlType_e.swControlType_Label
        leftAlign = swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge
        options = swAddControlOptions_e.swControlOptions_Enabled + swAddControlOptions_e.swControlOptions_Visible '+ swAddControlOptions_e.swControlOptions_SmallGapAbove
        lbl1 = group1.AddControl(lbl1ID, controlType, "Name", leftAlign, options, "")

        'txt1
        controlType = swPropertyManagerPageControlType_e.swControlType_Textbox
        leftAlign = swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge
        options = swAddControlOptions_e.swControlOptions_Enabled + swAddControlOptions_e.swControlOptions_Visible
        txt1 = group1.AddControl(txt1ID, controlType, "", leftAlign, options, "Name of the Instrument")

        'lbl2
        controlType = swPropertyManagerPageControlType_e.swControlType_Label
        leftAlign = swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge
        options = swAddControlOptions_e.swControlOptions_Enabled + swAddControlOptions_e.swControlOptions_Visible '+ swAddControlOptions_e.swControlOptions_SmallGapAbove
        lbl1 = group1.AddControl(lbl1ID, controlType, "Type", leftAlign, options, "")

        'cmb1
        controlType = swPropertyManagerPageControlType_e.swControlType_Combobox
        leftAlign = swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge
        options = swAddControlOptions_e.swControlOptions_Enabled + swAddControlOptions_e.swControlOptions_Visible
        cmb1 = group1.AddControl(cmb1ID, controlType, "", leftAlign, options, "Type of the Instrument")
        If Not cmb1 Is Nothing Then
            Dim items() As String = New String() {""}
            cmb1.Height = 40
            cmb1.Style = swPropMgrPageComboBoxStyle_e.swPropMgrPageComboBoxStyle_EditableText
            cmb1.AddItems(items)
        End If

        'lbl3
        controlType = swPropertyManagerPageControlType_e.swControlType_Label
        leftAlign = swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge
        options = swAddControlOptions_e.swControlOptions_Enabled + swAddControlOptions_e.swControlOptions_Visible '+ swAddControlOptions_e.swControlOptions_SmallGapAbove
        lbl1 = group1.AddControl(lbl1ID, controlType, "Size", leftAlign, options, "")

        'cmb2
        controlType = swPropertyManagerPageControlType_e.swControlType_Combobox
        leftAlign = swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge
        options = swAddControlOptions_e.swControlOptions_Enabled + swAddControlOptions_e.swControlOptions_Visible
        cmb1 = group1.AddControl(cmb1ID, controlType, "", leftAlign, options, "Size of the Instrument")
        If Not cmb2 Is Nothing Then
            Dim items() As String = New String() {"0.25", "0.5", "1", "1.5", "2"}
            cmb2.Height = 40
            cmb2.Style = swPropMgrPageComboBoxStyle_e.swPropMgrPageComboBoxStyle_EditableText
            cmb2.AddItems(items)
        End If

        'lbl4
        controlType = swPropertyManagerPageControlType_e.swControlType_Label
        leftAlign = swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge
        options = swAddControlOptions_e.swControlOptions_Enabled + swAddControlOptions_e.swControlOptions_Visible '+ swAddControlOptions_e.swControlOptions_SmallGapAbove
        lbl1 = group1.AddControl(lbl1ID, controlType, "Material", leftAlign, options, "")

        'cmb3
        controlType = swPropertyManagerPageControlType_e.swControlType_Combobox
        leftAlign = swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge
        options = swAddControlOptions_e.swControlOptions_Enabled + swAddControlOptions_e.swControlOptions_Visible
        cmb3 = group1.AddControl(cmb3ID, controlType, "", leftAlign, options, "Material of the Instrument")
        If Not cmb3 Is Nothing Then
            Dim items() As String = New String() {"AISI 304", "Alloy Steel", "Gray Cast Iron", "3060 Alloy", "Brass", "Copper"}
            cmb3.Height = 40
            cmb3.Style = swPropMgrPageComboBoxStyle_e.swPropMgrPageComboBoxStyle_EditableText
            cmb3.AddItems(items)
        End If

        'lbl5
        controlType = swPropertyManagerPageControlType_e.swControlType_Label
        leftAlign = swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge
        options = swAddControlOptions_e.swControlOptions_Enabled + swAddControlOptions_e.swControlOptions_Visible '+ swAddControlOptions_e.swControlOptions_SmallGapAbove
        lbl1 = group1.AddControl(lbl1ID, controlType, "ByPass", leftAlign, options, "")

        'cmb4
        controlType = swPropertyManagerPageControlType_e.swControlType_Combobox
        leftAlign = swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge
        options = swAddControlOptions_e.swControlOptions_Enabled + swAddControlOptions_e.swControlOptions_Visible
        cmb4 = group1.AddControl(cmb4ID, controlType, "", leftAlign, options, "Instrument connected in ByPass")
        If Not cmb4 Is Nothing Then
            Dim items() As String = New String() {"Yes", "No"}
            cmb4.Height = 40
            cmb4.Style = swPropMgrPageComboBoxStyle_e.swPropMgrPageComboBoxStyle_EditableText
            cmb4.AddItems(items)
        End If

        'lbl6
        controlType = swPropertyManagerPageControlType_e.swControlType_Label
        leftAlign = swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge
        options = swAddControlOptions_e.swControlOptions_Enabled + swAddControlOptions_e.swControlOptions_Visible '+ swAddControlOptions_e.swControlOptions_SmallGapAbove
        lbl1 = group1.AddControl(lbl1ID, controlType, "Mode", leftAlign, options, "")

        'cmb5
        controlType = swPropertyManagerPageControlType_e.swControlType_Combobox
        leftAlign = swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge
        options = swAddControlOptions_e.swControlOptions_Enabled + swAddControlOptions_e.swControlOptions_Visible
        cmb5 = group1.AddControl(cmb5ID, controlType, "", leftAlign, options, "Type of the Instrument")
        If Not cmb1 Is Nothing Then
            Dim items() As String = New String() {"Mechanical", "Electrical"}
            cmb5.Height = 40
            cmb5.Style = swPropMgrPageComboBoxStyle_e.swPropMgrPageComboBoxStyle_EditableText
            cmb5.AddItems(items)
        End If
    End Sub

    Function ValidateAttribute(ByVal swAttr() As String) As String()
        If swAttr(0) = "" Then
            swAttr(0) = "3W-01"
        End If
        If swAttr(1) = "" Then
            swAttr(1) = "3Way Valve"
        End If
        If swAttr(2) = "" Then
            swAttr(2) = "1" & Chr(34)
        End If
        If swAttr(3) = "" Then
            swAttr(3) = "AISI 304"
        End If
        If swAttr(4) = "" Then
            swAttr(4) = "No"
        End If
        If swAttr(5) = "" Then
            swAttr(5) = "Mechanical"
        End If
        Return swAttr
    End Function

End Class


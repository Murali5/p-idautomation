Imports System.Drawing
Imports SolidWorks.Interop.sldworks
Imports System
Imports System.Windows.Forms

Public Class PropBox
    Public PropStr(5) As String
    Public swapp As SldWorks
    Private Replace As Boolean = False
    Private Sub PropBox_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.Escape Then
            ' sender.Close()
        End If
    End Sub

    Private Sub PropBox_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles MyBase.KeyPress
        '  MsgBox("")
        '  MsgBox(sender.GetType.Name)
        If e.KeyChar = (ChrW(Keys.Escape)) Then
            '    MsgBox("")
            '  sender.Close()
        End If
    End Sub

    Private Sub PropBox_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyUp
        '  MsgBox("")
        MsgBox(sender.GetType.Name)
        If e.KeyData = Keys.Escape Then
            ' MsgBox("")
            '   sender.Close()
        End If
    End Sub

    Private Sub PropBox_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.Location = Windows.Forms.Cursor.Position

        Dim FillCmbStr() As String
        FillCmbStr = New String() {"No", "Yes"}
        cmbBypass.DataSource = FillCmbStr
        FillCmbStr = New String() {"Carbon Steel", "Cast Iron", "Stainless Steel", "SG Iron", "AISI 304", "Alloy Steel", "Gray Cast Iron", "3060 Alloy", "Brass", "Copper"}
        cmbMat.DataSource = FillCmbStr
        FillCmbStr = New String() {"0.25" & Chr(34), "0.5" & Chr(34), "1" & Chr(34), "1.5" & Chr(34), "2" & Chr(34)}
        cmbSize.DataSource = FillCmbStr
        FillCmbStr = New String() {"Mechanical", "Electrical"}
        cmbMode.DataSource = FillCmbStr

        FillCmbStr = ReadBlocksFromLib("E:\Murali Kannan\Projeccts\Spirax\Blocks")
        cmbType.DataSource = FillCmbStr

        AssignFromArray()
        Replace = True
    End Sub

    Sub AssignFromArray()
        txtName.Text = PropStr(0)
        cmbType.Text = PropStr(1)
        cmbSize.Text = PropStr(2)
        cmbMat.Text = PropStr(3)
        cmbBypass.Text = PropStr(4)
        cmbMode.Text = PropStr(5)
    End Sub

    Sub AssignToArray()
        PropStr(0) = txtName.Text
        PropStr(1) = cmbType.Text
        PropStr(2) = cmbSize.Text
        PropStr(3) = cmbMat.Text
        PropStr(4) = cmbBypass.Text
        PropStr(5) = cmbMode.Text
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        AssignToArray()
        If SetBlkAttribute(swapp, GetBlkInst(swapp), PropStr) Then
            Me.Close()
        Else
            MsgBox("Error in Setting Values of valve")
        End If
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Me.Close()
    End Sub

    Private Sub PropBox_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.LostFocus
        ' MsgBox("lost focus")
        'sender.Close()
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        Dim Result = MsgBox("Do you really Wanna Delete this instance?", MsgBoxStyle.YesNoCancel + MsgBoxStyle.Critical, "Alert!")
        If Result = MsgBoxResult.Yes Then
            Dim TmBlkInst = GetBlkInst(swapp)
            DeleteBlock(TmBlkInst, GetActiveDoc(swapp), True)
        End If
    End Sub

    Private Sub cmbType_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbType.SelectedIndexChanged
        'replace block
        If Replace Then
            'MsgBox("Cmb Change")
            Dim ReBlkInst As SketchBlockInstance
            ReBlkInst = ReplaceBlkInst(GetActiveDoc(swapp), GetBlkInst(swapp), cmbType.Text)

        End If
    End Sub
End Class
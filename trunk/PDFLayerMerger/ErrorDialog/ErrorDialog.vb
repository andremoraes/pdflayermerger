Imports System.Windows.Forms

Public Class ErrorDialog
    Private ParentFormke As Form
    Private TypeOfDialog As ErrorDialogType

    Public Sub New(ByVal TypeOfMessage As String, ByVal message As String, ByVal ex As Exception, Optional ByVal ParentForm As Form = Nothing)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Select Case LCase(TypeOfMessage)

            Case "warning"
                Me.PictureBox1.Image = Me.ImageList1.Images(0)
                TypeOfDialog = ErrorDialogType.WarningMessage
            Case "error"
                Me.PictureBox1.Image = Me.ImageList1.Images(1)
                TypeOfDialog = ErrorDialogType.ErrorMessage
            Case "information"
                Me.PictureBox1.Image = Me.ImageList1.Images(2)
                TypeOfDialog = ErrorDialogType.InformationMessage
            Case "question"
                Me.PictureBox1.Image = Me.ImageList1.Images(3)
                Me.btnCancel.Visible = True
                Me.btnCancel.Text = "No"
                Me.btnOK.Text = "Yes"
                TypeOfDialog = ErrorDialogType.QuestionMessage
        End Select

        Me.lblMessage.Text = message
        Me.txtMessage.Text = GatherErrorInfo(ex)
        Me.ParentFormke = ParentForm

        If Not Me.ParentFormke Is Nothing Then
            Me.Text = Me.ParentFormke.Text
        Else
            Me.Text = My.Application.Info.Title
        End If
    End Sub

    Public Sub New(ByVal TypeOfMessage As ErrorDialogType, ByVal message As String, ByVal ex As Exception, Optional ByVal ParentForm As Form = Nothing)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Select Case TypeOfMessage

            Case ErrorDialogType.WarningMessage
                Me.PictureBox1.Image = Me.ImageList1.Images(0)
                TypeOfDialog = ErrorDialogType.WarningMessage
            Case ErrorDialogType.ErrorMessage
                Me.PictureBox1.Image = Me.ImageList1.Images(1)
                TypeOfDialog = ErrorDialogType.ErrorMessage
            Case ErrorDialogType.InformationMessage
                Me.PictureBox1.Image = Me.ImageList1.Images(2)
                TypeOfDialog = ErrorDialogType.InformationMessage
            Case ErrorDialogType.QuestionMessage
                Me.PictureBox1.Image = Me.ImageList1.Images(3)
                Me.btnCancel.Visible = True
                Me.btnCancel.Text = "No"
                Me.btnOK.Text = "Yes"
                TypeOfDialog = ErrorDialogType.QuestionMessage
        End Select

        Me.lblMessage.Text = message
        Me.txtMessage.Text = GatherErrorInfo(ex)
        Me.ParentFormke = ParentForm

        If Not Me.ParentFormke Is Nothing Then
            Me.Text = Me.ParentFormke.Text
        Else
            Me.Text = My.Application.Info.Title
        End If
    End Sub

    Public Sub New(ByVal TypeOfMessage As String, ByVal message As String, ByVal errMessage As String, Optional ByVal ParentForm As Form = Nothing)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Select Case LCase(TypeOfMessage)

            Case "warning"
                Me.PictureBox1.Image = Me.ImageList1.Images(0)
                TypeOfDialog = ErrorDialogType.WarningMessage
            Case "error"
                Me.PictureBox1.Image = Me.ImageList1.Images(1)
                TypeOfDialog = ErrorDialogType.ErrorMessage
            Case "information"
                Me.PictureBox1.Image = Me.ImageList1.Images(2)
                TypeOfDialog = ErrorDialogType.InformationMessage
            Case "question"
                Me.PictureBox1.Image = Me.ImageList1.Images(3)
                Me.btnCancel.Visible = True
                Me.btnCancel.Text = "No"
                Me.btnOK.Text = "Yes"
                TypeOfDialog = ErrorDialogType.QuestionMessage
        End Select

        Me.lblMessage.Text = message
        Me.txtMessage.Text = errMessage
        Me.ParentFormke = ParentForm
        If Not Me.ParentFormke Is Nothing Then
            Me.Text = Me.ParentFormke.Text
        Else
            Me.Text = My.Application.Info.Title
        End If

    End Sub

    Public Sub New(ByVal TypeOfMessage As ErrorDialogType, ByVal message As String, ByVal errMessage As String, Optional ByVal ParentForm As Form = Nothing)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Select Case LCase(TypeOfMessage)
            Case ErrorDialogType.WarningMessage
                Me.PictureBox1.Image = Me.ImageList1.Images(0)
                TypeOfDialog = ErrorDialogType.WarningMessage
            Case ErrorDialogType.ErrorMessage
                Me.PictureBox1.Image = Me.ImageList1.Images(1)
                TypeOfDialog = ErrorDialogType.ErrorMessage
            Case ErrorDialogType.InformationMessage
                Me.PictureBox1.Image = Me.ImageList1.Images(2)
                TypeOfDialog = ErrorDialogType.InformationMessage
            Case ErrorDialogType.QuestionMessage
                Me.PictureBox1.Image = Me.ImageList1.Images(3)
                Me.btnCancel.Visible = True
                Me.btnCancel.Text = "No"
                Me.btnOK.Text = "Yes"
                TypeOfDialog = ErrorDialogType.QuestionMessage
        End Select

        Me.lblMessage.Text = message
        Me.txtMessage.Text = errMessage
        Me.ParentFormke = ParentForm
        If Not Me.ParentFormke Is Nothing Then
            Me.Text = Me.ParentFormke.Text
        Else
            Me.Text = My.Application.Info.Title
        End If
    End Sub

    Public Overloads Shared Function ShowDialog(ByVal TypeOfMessage As String, ByVal message As String, ByVal ex As Exception, Optional ByVal ParentForm As Form = Nothing) As MsgBoxResult
        Dim d As New ErrorDialog(TypeOfMessage, message, ex, ParentForm)
        Dim msgresult As New MsgBoxResult
        msgresult = d.ShowDialog()
        Return msgresult
    End Function
    Public Overloads Shared Function ShowDialog(ByVal TypeOfMessage As String, ByVal message As String, ByVal errormessage As String, Optional ByVal ParentForm As Form = Nothing) As MsgBoxResult
        Dim d As New ErrorDialog(TypeOfMessage, message, errormessage, ParentForm)
        Dim msgresult As New MsgBoxResult
        msgresult = d.ShowDialog()
        Return msgresult
    End Function
    Public Overloads Shared Function ShowDialog(ByVal TypeOfMessage As ErrorDialogType, ByVal message As String, ByVal errormessage As String, Optional ByVal ParentForm As Form = Nothing) As MsgBoxResult
        Dim d As New ErrorDialog(TypeOfMessage, message, errormessage, ParentForm)
        Dim msgresult As New MsgBoxResult
        msgresult = d.ShowDialog()
        Return msgresult
    End Function
    Public Overloads Shared Function ShowDialog(ByVal TypeOfMessage As ErrorDialogType, ByVal message As String, ByVal ex As Exception, Optional ByVal ParentForm As Form = Nothing) As MsgBoxResult
        Dim d As New ErrorDialog(TypeOfMessage, message, ex, ParentForm)
        Dim msgresult As New MsgBoxResult
        msgresult = d.ShowDialog()
        Return msgresult
    End Function

    Private Function GatherErrorInfo(ByVal ex As Exception) As String
        Dim sb As New System.Text.StringBuilder()
        Dim st As New System.Diagnostics.StackTrace(ex, True)
        Dim FileName As String = ""
        Dim Method As String = ""
        Dim LineNumber As String = ""

        sb.AppendLine(ex.Message)
        sb.AppendLine("")
        For Each frame As System.Diagnostics.StackFrame In st.GetFrames()
            FileName = frame.GetFileName
            Method = frame.GetMethod().ToString
            LineNumber = frame.GetFileLineNumber
            If FileName <> "" Then sb.AppendLine("Filename : " + IO.Path.GetFileName(FileName))
            If Method <> "" Then sb.AppendLine("Method : " + Method)
            If LineNumber <> "" Then sb.AppendLine("Line N° : " + LineNumber)
            sb.AppendLine()
        Next
        Return sb.ToString()
    End Function

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOK.Click
        If TypeOfDialog = ErrorDialogType.QuestionMessage Then
            Me.DialogResult = System.Windows.Forms.DialogResult.Yes
        Else
            Me.DialogResult = System.Windows.Forms.DialogResult.OK
        End If

        Me.Close()
    End Sub

    Private Sub ErrorDialog_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Me.ParentFormke Is Nothing Then
            Dim boundWidth As Integer = (Me.ParentFormke.Location.X) + (Me.ParentFormke.Size.Width / 2)
            Dim boundHeight As Integer = (Me.ParentFormke.Location.Y) + (Me.ParentFormke.Size.Height / 2)

            Dim boundWidthMe As Integer = (Me.Size.Width / 2)
            Dim boundHeightMe As Integer = (Me.Size.Height / 2)
            Me.Left = boundWidth - boundWidthMe
            Me.Top = boundHeight - boundHeightMe

        Else
            Dim Monitor As Drawing.Rectangle = Screen.PrimaryScreen.Bounds
            Me.Location = New Point((Monitor.Width - Me.Width) / 2, (Monitor.Height - Me.Height) / 2)
        End If
    End Sub

    Private Sub ErrorDialog_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
        Me.btnOK.Focus()
    End Sub


    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        If TypeOfDialog = ErrorDialogType.QuestionMessage Then
            Me.DialogResult = System.Windows.Forms.DialogResult.No
        Else
            Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        End If

        Me.Close()
    End Sub
End Class

Public Enum ErrorDialogType
    InformationMessage = 0
    WarningMessage = 1
    ErrorMessage = 2
    QuestionMessage = 3
End Enum
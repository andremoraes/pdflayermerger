Public Class frmMain

    Private Sub GlassButton1_Click(sender As System.Object, e As System.EventArgs) Handles GlassButton1.Click 'first pdf
        If Me.OpenFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
            Dim filename As String = OpenFileDialog1.FileName
            If Not filename = "" Then
                If IO.File.Exists(filename) Then
                    Me.txtOriginal.Text = filename
                End If
            End If
        End If
    End Sub

    Private Sub GlassButton2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GlassButton2.Click
        If Me.OpenFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
            Dim filename As String = OpenFileDialog1.FileName
            If Not filename = "" Then
                If IO.File.Exists(filename) Then
                    Me.TextBox2.Text = filename
                End If
            End If
        End If
    End Sub

    Private Sub GlassButton3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GlassButton3.Click
        If Me.SaveFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
            Dim filename As String = SaveFileDialog1.FileName
            If Not filename = "" Then
                If IO.File.Exists(filename) Then
                    Me.TextBox3.Text = filename
                End If
            End If
        End If
    End Sub

    Private Sub btnCombinePDF_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCombinePDF.Click
        Dim layername As String = ""
        Dim transform As String = ""
        If txtOriginal.Text <> "" Then
            If TextBox2.Text <> "" Then
                If TextBox3.Text <> "" Then
                    'execute Merging
                    '  If CheckBox1.Checked Then
                    If TextBox4.Text = "" Then layername = GetLayerNameFromSettingsFile()
                    If TextBox5.Text = "" Then transform = GetTransformationFromSettingsFile()
                    If PDFCombineLayer(Replace(txtOriginal.Text, Chr(34), ""), Replace(TextBox2.Text, Chr(34), ""), Replace(TextBox3.Text, Chr(34), ""), TextBox4.Text, TextBox5.Text) Then
                        Dim pr As New Process
                        With pr.StartInfo
                            .FileName = TextBox3.Text
                        End With
                        pr.Start()
                    Else
                        MsgBox("An error occured!!!")
                    End If
                End If
            End If
        End If
    End Sub
End Class

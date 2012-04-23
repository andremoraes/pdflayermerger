#Region "   ApplicationEntryPoint   "
''' <summary>
''' Class with program entry point.
''' </summary>
Friend NotInheritable Class AppLaunch

    ''' <summary>
    ''' Program entry point.
    ''' </summary>
    <STAThread()> _
    Friend Shared Sub Main(ByVal commandLineArgs() As String)
        AddHandler CurAppdomain.UnhandledException, AddressOf ExceptionHandlerke
        AddHandler Application.ThreadException, AddressOf ThreadErrorHandler

        If commandLineArgs.Length > 0 Then
            Dim source1 As String = Nothing '1st commandline argument, obligated, original pdf-file
            Dim source2 As String = Nothing '2nd commandline argument, obligated, markup pdf-file
            Dim destination As String = Nothing '3rd commandline argument, obligated, combined pdf-file
            Dim layername As String = Nothing '4th commandline argument, optional, layername for markup layers
            Dim transform As String = Nothing '5th commandline argument, optional, tranformation of the orginal pdf (rotation, scaleX, scaleY, offsetX, offsetY

            'Validate input
            Dim validated As Boolean = True
            For i As Integer = 0 To commandLineArgs.Length - 1
                Select Case i
                    Case 0
                        source1 = RemoveLeadingTrailingQuotes(commandLineArgs(i))
                        If source1 = "" Then : validated = False
                        Else : If Not IO.File.Exists(source1) Then validated = False
                        End If
                    Case 1
                        source2 = RemoveLeadingTrailingQuotes(commandLineArgs(i))
                        If source2 = "" Then : validated = False
                        Else : If Not IO.File.Exists(source2) Then validated = False
                        End If
                    Case 2
                        destination = RemoveLeadingTrailingQuotes(commandLineArgs(i))
                        If destination = "" Then : validated = False
                        Else
                            If IO.File.Exists(destination) Then
                                Try
                                    IO.File.Delete(destination)
                                Catch ex As Exception
                                    ErrorDialog.ShowDialog(ErrorDialogType.ErrorMessage, "Unable to remove the combined PDF File!", "Please check if you have the access rights and the file is not opened." + vbCrLf + destination + GetErrorInfo(ex))
                                    validated = False
                                End Try
                            End If
                        End If
                    Case 3
                        layername = RemoveLeadingTrailingQuotes(commandLineArgs(i))
                        If layername = "" Then layername = GetLayerNameFromSettingsFile()
                        'If no layername is defined by user or ini-file, the application will give a default name.
                    Case 4
                        transform = RemoveLeadingTrailingQuotes(commandLineArgs(i))
                        If transform = "" Then transform = GetTransformationFromSettingsFile()
                        'If no transformation is defined by user or ini-file, the application will be executed without a transformation.
                End Select
            Next
            'Start writing pdf if commandline had valid information.
            If validated Then
                Try
                    'get autoopen 
                    If PDFCombineLayer(source1, source2, destination, layername, transform) Then
                        If GetAutoOpenFromSettingsFile() Then
                            Dim pr As New Process
                            With pr.StartInfo
                                .FileName = destination
                            End With
                            pr.Start()
                        End If
                    Else
                        ErrorDialog.ShowDialog(ErrorDialogType.ErrorMessage, "An error occured while trying to combine the PDF files!", "An unexpected error occured while trying to combine : " + vbCrLf + source1 + vbCrLf + source2 + "The destination file : " + destination + " could not be created!")
                    End If
                Catch ex As Exception
                    ErrorDialog.ShowDialog(ErrorDialogType.ErrorMessage, "An error occured while trying to combine the PDF files!", "An unexpected error occured while trying to combine : " + vbCrLf + source1 + vbCrLf + source2 + "The destination file : " + destination + " could not be created!" + GetErrorInfo(ex))
                End Try
            End If
        Else
            'If no information was added to commandline, open Form.
            Application.EnableVisualStyles()
            Application.SetCompatibleTextRenderingDefault(False)
            Application.VisualStyleState = VisualStyles.VisualStyleState.ClientAndNonClientAreasEnabled
            Application.Run(New frmMain())
        End If
    End Sub
End Class

#End Region
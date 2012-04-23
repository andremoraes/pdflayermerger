Module GlobalFunctions

    Public CurAppdomain As AppDomain = AppDomain.CurrentDomain

    Public Sub ExceptionHandlerke(ByVal sender As Object, ByVal e As System.UnhandledExceptionEventArgs)
        Dim Excep As Exception
        Excep = e.ExceptionObject
        ErrorDialog.ShowDialog(ErrorDialogType.ErrorMessage, "Unhandled error occured in application: " + vbCrLf + "Application aborted on " + Now, "Error : Application Crashed" + GetErrorInfo(Excep))
        GC.Collect()
        End
    End Sub

    Public Sub ThreadErrorHandler(ByVal sender As Object, ByVal e As Threading.ThreadExceptionEventArgs)
        Dim Excep As Exception
        Excep = e.Exception
        ErrorDialog.ShowDialog(ErrorDialogType.ErrorMessage, "Unhandled error occured in application: " + vbCrLf + "Application aborted on " + Now, "Error : Application Crashed" + GetErrorInfo(Excep))
        GC.Collect()
        End
    End Sub

    Public Function GetErrorInfo(ByVal ex As Exception) As String
        Dim sb As New System.Text.StringBuilder()
        Dim st As New System.Diagnostics.StackTrace(ex, True)
        Dim FileName As String = ""
        Dim Method As String = ""
        Dim LineNumber As String = ""
        sb.AppendLine("")
        sb.AppendLine(ex.Message)
        sb.AppendLine("")
        For Each frame As System.Diagnostics.StackFrame In st.GetFrames()
            FileName = IO.Path.GetFileName(frame.GetFileName)
            Method = frame.GetMethod().ToString
            LineNumber = frame.GetFileLineNumber
            If FileName <> "" Then sb.AppendLine("Filename : " + FileName)
            If Method <> "" Then sb.AppendLine("Method : " + Method)
            If LineNumber <> "" Then sb.AppendLine("Line N° : " + LineNumber)
        Next
        Return sb.ToString()
    End Function

    Public Function RemoveLeadingTrailingQuotes(ByVal str As String) As String
        If str.Contains(Chr(34)) Then
            str.Replace(Chr(34), "")
            Return str
        Else
            Return str
        End If
    End Function

    ''' <summary>
    ''' Combines two source pdf's to one single pdf.
    ''' </summary>
    ''' <param name="sourceOriginal">The file-location incl. the name of the original pdf.</param>
    ''' <param name="sourceMarkup">The file-location incl. the name of the mark-up pdf.</param>
    ''' <param name="destination">The file-location incl. the name of the generated combined pdf.</param>
    ''' <param name="NewLayer">The name which the added mark-up layer will have in the new pdf.</param>
    ''' <param name="Transform">The transformation which is executed on the original pdf: rotation, scaleX, scaleY, offsetX, offsetY</param>
    ''' <returns>Return boolean indication whether the combination of the pdf's was successful.</returns>
    ''' <remarks></remarks>
    Public Function PDFCombineLayer(ByVal sourceOriginal As String, ByVal sourceMarkup As String, ByVal destination As String, Optional ByVal NewLayer As String = "SOURCE-DRAWING", Optional ByVal Transform As String = "0;1;1;0;0") As Boolean
        Dim oPdfOriginal As iTextSharp.text.pdf.PdfReader = Nothing
        Dim oPdfMarkup As iTextSharp.text.pdf.PdfReader = Nothing
        Dim oPdfStamper As iTextSharp.text.pdf.PdfStamper = Nothing
        Dim PDFDirectContent As iTextSharp.text.pdf.PdfContentByte
        Dim iNumberOfPages As Integer = Nothing
        Dim iPage As Integer = 0
        Dim matrix1 As Drawing2D.Matrix = Nothing
        Dim matrix2 As Drawing2D.Matrix = Nothing

        Try
            'Read the source pdf-files and prepare new file.
            oPdfOriginal = New iTextSharp.text.pdf.PdfReader(sourceOriginal)
            oPdfMarkup = New iTextSharp.text.pdf.PdfReader(sourceMarkup)
            oPdfStamper = New iTextSharp.text.pdf.PdfStamper(oPdfMarkup, New IO.FileStream(destination, IO.FileMode.Create, IO.FileAccess.Write))

            'Count pages in original pdf and create new pdf wih same amount of pages.
            iNumberOfPages = oPdfOriginal.NumberOfPages
            oPdfStamper.GetPdfLayers()

            'Make transformation matrix
            If Transform = "" Then Transform = "0;1;1;0;0" 'Note transform: rotation(degrees), scaleX, scaleY, offsetX, offsetY
            Dim ar() As String = Split(Transform, ";")
            If Not ar.Length = 5 Then ar = New String() {0, 1, 1, 0, 0}
            If ar(1) <= 0 Or ar(2) <= 0 Then Throw New ArgumentOutOfRangeException("transform", "Make sure that scaling factors of the transformation are larger than zero.")
            matrix1 = New Drawing2D.Matrix
            matrix1.Rotate(-CSng(ar(0)))
            matrix1.Scale(CSng(ar(1)), CSng(ar(2)))
            matrix1.Translate(CSng(ar(3)), CSng(ar(4)))
            matrix2 = New Drawing2D.Matrix

            'Add the new layer to each of the pdf-pages.
            If NewLayer = "" Then NewLayer = "SOURCE-DRAWING"
            Dim PDFNewLayer As New iTextSharp.text.pdf.PdfLayer(NewLayer, oPdfStamper.Writer)
            Dim PDFLayerMem As New iTextSharp.text.pdf.PdfLayerMembership(oPdfStamper.Writer)
            PDFLayerMem.AddMember(PDFNewLayer)
            Do While (iPage < iNumberOfPages)
                'Get new page
                iPage += 1
                Dim iPageOriginal As iTextSharp.text.pdf.PdfImportedPage = oPdfStamper.Writer.GetImportedPage(oPdfOriginal, iPage)
                PDFDirectContent = oPdfStamper.GetUnderContent(iPage) 'add page under existing pdf in new layer
                matrix2 = matrix1.Clone 'Cloning is necessary, otherwise all adaptations to matrix2 are also done in matrix1.

                'Compensate for the orientation of the original pdf-file.
                Dim iRotation As Single = oPdfOriginal.GetPageRotation(iPage)
                Select Case iRotation
                    Case Is = 90
                        matrix2.Rotate(-iRotation)
                        matrix2.Translate(-oPdfOriginal.GetPageSize(iPage).Width, 0)
                    Case Is = 180 'Not tested yet.
                        matrix2.Rotate(-iRotation)
                        matrix2.Translate(-oPdfOriginal.GetPageSize(iPage).Width, -oPdfOriginal.GetPageSize(iPage).Height)
                    Case Is = 270 'Not tested yet.
                        matrix2.Rotate(-iRotation)
                        matrix2.Translate(0, oPdfOriginal.GetPageSize(iPage).Height)
                    Case Else
                End Select

                'Add the new layer ot this page.
                PDFDirectContent.BeginLayer(PDFNewLayer)
                PDFDirectContent.AddTemplate(iPageOriginal, matrix2)
                PDFDirectContent.EndLayer()
            Loop
            oPdfStamper.SetFullCompression()
            Return True
        Catch ex As Exception
            Return False
        Finally
            oPdfMarkup.Close()
            oPdfOriginal.Close()
            oPdfStamper.Close()
            matrix1.Dispose()
            matrix2.Dispose()
        End Try
    End Function

    ''' <summary>
    ''' Gets the layername to be used for the mark-ups in the new pdf-file.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetLayerNameFromSettingsFile() As String
        Dim Layername As String = ""
        Try
            Dim SettingsFile As New IniFile(Application.StartupPath + "\" + IO.Path.GetFileNameWithoutExtension(Application.ExecutablePath) + ".ini")
            SettingsFile.GetValue("LayerName", "GENERAL", Layername)
            Return Layername
        Catch ex As Exception
            Return ""
        End Try
    End Function

    ''' <summary>
    ''' Gets the AutoOpen setting as boolean.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetAutoOpenFromSettingsFile() As Boolean
        Dim autoopen As String = ""
        Try
            Dim SettingsFile As New IniFile(Application.StartupPath + "\" + IO.Path.GetFileNameWithoutExtension(Application.ExecutablePath) + ".ini")
            SettingsFile.GetValue("AutoOpen", "GENERAL", autoopen)
            If LCase(autoopen) = "true" Then
                Return True
            Else
                Return False
            End If

        Catch ex As Exception
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Gets the transformation to be used on the original pdf-pages before adding them in the new pdf-file.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetTransformationFromSettingsFile() As String
        Dim Transform As String = ""
        Try
            Dim SettingsFile As New IniFile(Application.StartupPath + "\" + IO.Path.GetFileNameWithoutExtension(Application.ExecutablePath) + ".ini")
            SettingsFile.GetValue("Transformation", "GENERAL", Transform)
            Return Transform
        Catch ex As Exception
            Return ""
        End Try
    End Function

End Module



Imports System.IO
Imports System.Collections
Imports Microsoft.VisualBasic
Imports System.Text

Public Class IniFile
    Private Sections As New ArrayList
    Public CommentString As String = ";"
    Private _FileName As String

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Returns the contents of the IniFile in a text or HTML format
    ''' </summary>
    ''' <param name="ReturnAsHTML">Optional, defaults to false. Carriage return/linefeeds are converted to HTML.</param>
    ''' <value></value>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[AcketK]	30/09/2010	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public ReadOnly Property Text(Optional ByVal ReturnAsHTML As Boolean = False) As String
        Get
            Return GetText(ReturnAsHTML)
        End Get
    End Property

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Contructor called when creating a new IniFile. Requires a filename value.
    ''' </summary>
    ''' <param name="FileName">The path to the file to be edited.</param>
    ''' <param name="CreateIfNotExist">Optional, defaults to true. If the file does not exist, it is created.</param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[AcketK]	30/09/2010	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Sub New(ByVal FileName As String, Optional ByVal CreateIfNotExist As Boolean = True)
        Read(FileName, CreateIfNotExist)
        _FileName = FileName
    End Sub

    Private Function Read(ByVal Filename As String, Optional ByVal CreateIfNotExist As Boolean = True) As Boolean
        If Not File.Exists(Filename) Then ' verify that the file exists
            If CreateIfNotExist Then 'if it does not exist, check to see if we should create it
                Try
                    Dim fs As FileStream = File.Create(Filename) 'create it
                    fs.Close()
                Catch ex As Exception
                    MsgBox("Error: Cannot create file " & Filename & vbCrLf & ex.ToString(), MsgBoxStyle.Critical, "Error creating IniFile")
                    Return False
                End Try
            Else
                MsgBox("Error: File " & Filename & " does not exist. Cannot create IniFile.", MsgBoxStyle.Critical, "Critical Error")
                Return False
            End If
        End If

        Sections.Clear() 'Clear the arraylist

        Try
            Dim sr As New System.IO.StreamReader(Filename) 'Declare a streamreader
            Dim CurrentSection As String = "" 'Flag to track what section we are currently in
            Dim ThisLine As String = sr.ReadLine() 'Read in the first line
            Dim hasComment As Boolean = False
            Dim comment() As String = Nothing
            Do
                Select Case Eval(ThisLine) 'Evalue the contents of the line
                    Case "Section"
                        If hasComment Then
                            AddSection(RemoveBrackets(RemoveComment(ThisLine)), IsCommented(ThisLine), comment)
                            ReDim comment(0)
                            comment = Nothing
                            hasComment = False
                        Else
                            AddSection(RemoveBrackets(RemoveComment(ThisLine)), IsCommented(ThisLine)) 'Add the section to the sections arraylist
                        End If
                        CurrentSection = RemoveBrackets(RemoveComment(ThisLine)) 'Make this the current section, so we know where to keys to
                    Case "Key"
                        If hasComment Then
                            AddKey(GetKeyName(ThisLine), GetKeyValue(ThisLine), CurrentSection, IsCommented(ThisLine), , comment)
                            ReDim comment(0)
                            comment = Nothing
                            hasComment = False
                        Else
                            AddKey(GetKeyName(ThisLine), GetKeyValue(ThisLine), CurrentSection, IsCommented(ThisLine))
                        End If

                    Case "Comment"
                        'AddComment(ThisLine, CurrentSection)
                        'if next line is a section this comment needs to be grouped with that section
                        hasComment = True
                        If comment Is Nothing Then
                            ReDim comment(0)
                            comment(0) = RemoveComment(ThisLine)
                        Else
                            ReDim Preserve comment(comment.Length)
                            comment(comment.Length - 1) = RemoveComment(ThisLine)
                        End If

                    Case "Blank"
                        'TODO: Should we create a blank object to handle blanks?
                        If hasComment Then
                            GetSection(CurrentSection).Comment = comment
                            ReDim comment(0)
                            comment = Nothing
                            hasComment = False
                        Else
                            'do nothing
                        End If
                    Case ""
                        'We hit something unknown - ignore it
                End Select
                ThisLine = sr.ReadLine() 'Get the next line
            Loop Until ThisLine Is Nothing 'continue until the end of the file
            sr.Close() 'close the file

            Return True

        Catch ex As Exception
            MsgBox("Error: " & ex.ToString, MsgBoxStyle.Critical, "Error")
            Return False
        End Try


    End Function

    Private Function Eval(ByVal value As String) As String
        value = Trim(value) 'Remove any leading/trailing spaces, just in case they exist

        'If the value is blank, then it is a blank line
        If value = "" Then Return "Blank"

        'If the value is surrounded by brackets, then it is a section
        If Microsoft.VisualBasic.Left(RemoveComment(value), 1) = "[" And Microsoft.VisualBasic.Right(value, 1) = "]" Then Return "Section"

        'If the value contains an equals sign (=), then it is a value. This test can be fooled by 
        'comment with an equals sign in it, but it is the best test we have. We test for this before
        'testing for a comment in case the key is commented out. It is still a key.
        If InStr(value, "=", CompareMethod.Text) > 0 Then Return "Key"

        'If the value is preceeded by the comment string, then it is a pure comment
        If IsCommented(value) Then Return "Comment"

        Return ""
    End Function

    Private Function GetKeyName(ByVal Value As String) As String
        'If the value is commented out, then remove the comment string so we can get the name
        If IsCommented(Value) Then Value = RemoveComment(Value)
        Dim Equals As Integer = InStr(Value, "=", CompareMethod.Text) 'Locate the equals sign
        If Equals > 0 Then 'It should be, but just to be safe
            Return Trim(Microsoft.VisualBasic.Left(Value, Equals - 1)) 'Return everything before the equals sign
        Else : Return ""
        End If
    End Function

    Private Function GetKeyValue(ByVal value As String) As String
        Dim Equals As Integer = InStr(value, "=", CompareMethod.Text) 'Locate the equals sign
        If Equals > 0 Then 'It should be, but just to be safe
            Return Trim(Microsoft.VisualBasic.Right(value, Len(value) - Equals)) 'Return everything after the equals sign
        Else : Return ""
        End If
    End Function

    Private Function IsCommented(ByVal value As String) As Boolean
        'Return true if the passed value starts with a comment string
        If Microsoft.VisualBasic.Left(value, Len(CommentString)) = CommentString Then Return True
        Return False
    End Function

    Private Function RemoveComment(ByVal value As String) As String
        'Return the value with the comment string stripped
        Return IIf(IsCommented(value), value.Remove(0, Len(CommentString)), value)
    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Adds a key/value to a given section. If the section does not exist, it is created.
    ''' </summary>
    ''' <param name="KeyName">The name of the key to add. If the key alreadys exists, then no action is taken.</param>
    ''' <param name="KeyValue">The value to assign to the new key.</param>
    ''' <param name="SectionName">The section to add the new key to. If it does not exist, it is created.</param>
    ''' <param name="IsCommented">Optional, defaults to false. Will create the key in commented state.</param>
    ''' <param name="InsertBefore">Optional. Will insert the new key prior to the specified key.</param>
    ''' <returns></returns>
    ''' <remarks>If the section does not exist, it will be created. If the 'IsCommented' option is true, then the newly created section will also be commented. If the 'InsertBefore' option is used, the specified key does not exist, then the new key is simply added to the section. If the section the key is being added to is commented, then the key will be commented as well.
    ''' </remarks>
    ''' <history>
    ''' 	[AcketK]	30/09/2010	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function AddKey(ByVal KeyName As String, ByVal KeyValue As String, ByVal SectionName As String, Optional ByVal IsCommented As Boolean = False, Optional ByVal InsertBefore As String = Nothing, Optional ByVal Comments() As String = Nothing) As Boolean
        Dim ThisSection As Section = GetSection(SectionName) 'verify that the section exists
        If ThisSection Is Nothing Then AddSection(SectionName, IsCommented)
        If ThisSection.IsCommented Then IsCommented = True 'If the section is commented out, then this key must be too
        If Not GetKey(KeyName, SectionName) Is Nothing Then Return False 'verify that the key does *not* exist
        Dim ThisKey As New Key(KeyName, KeyValue, IsCommented, Comments) 'create a new key
        If InsertBefore Is Nothing Then 'if no insertbefore is required
            ThisSection.Add(ThisKey) 'then add the new key to the bottom of the section
        Else
            Dim KeyIndex = GetKeyIndex(InsertBefore, SectionName) 'locate the key to insert prior to
            If KeyIndex > -1 Then 'if the key exists
                ThisSection.Insert(KeyIndex, ThisKey) 'then do the insert
                Return True
            Else
                ThisSection.Add(ThisKey) 'the key to insert prior to wasn't found, so just add it
                Return False 'the key to insert prior to was not found
            End If
        End If
        Return True
    End Function

    Private Function GetKeyIndex(ByVal KeyName As String, ByVal SectionName As String) As Integer
        'returns the index of a given key
        'Dim ThisKey As Key = GetKey(KeyName, SectionName)
        'If ThisKey Is Nothing Then Return -1
        'Dim ThisSection As Section = GetSection(SectionName) 
        'Return ThisSection.IndexOf(ThisKey.Name)

        Dim SectionEnumerator As System.Collections.IEnumerator = Sections.GetEnumerator()
        While SectionEnumerator.MoveNext()
            If SectionEnumerator.Current.Name = SectionName Then
                Dim KeyEnumerator As System.Collections.IEnumerator = SectionEnumerator.Current.GetEnumerator()
                While KeyEnumerator.MoveNext()
                    If KeyEnumerator.Current.Name = KeyName Then Return SectionEnumerator.Current.indexof(KeyEnumerator.Current)
                End While
            End If
        End While
        Return -1
    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Adds a section to the IniFile. If the section already exists, then no action is taken.
    ''' </summary>
    ''' <param name="SectionName">The name of the section to add.</param>
    ''' <param name="IsCommented">Optional, defaults to false. Will add the section in a commented state.</param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[AcketK]	30/09/2010	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    ''' 
    Public Sub AddSection(ByVal SectionName As String, Optional ByVal IsCommented As Boolean = False, Optional ByVal Comments() As String = Nothing)
        If GetSection(SectionName) Is Nothing Then Sections.Add(New Section(SectionName, IsCommented, Comments)) 'Add the section to the sections arraylist
    End Sub

    Private Function GetSection(ByVal SectionName As String) As Section
        'Return the given section object
        Dim myEnumerator As System.Collections.IEnumerator = Sections.GetEnumerator()
        While myEnumerator.MoveNext()
            Dim CurrentSection As Section = myEnumerator.Current
            If LCase(CurrentSection.Name) = LCase(SectionName) Then Return myEnumerator.Current
        End While
        Return Nothing
    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Return the sections in the IniFile.
    ''' </summary>
    ''' <returns>Returns an ArrayList of Section objects.</returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[AcketK]	30/09/2010	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function GetSections() As ArrayList
        'returns an arraylist of the sections in the inifile
        Dim ListOfSections As New ArrayList
        Dim myEnumerator As System.Collections.IEnumerator = Sections.GetEnumerator()
        While myEnumerator.MoveNext()
            ListOfSections.Add(myEnumerator.Current)
        End While
        Return ListOfSections
    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Returns an arraylist of Key objects in a given Section. Section must exist.
    ''' </summary>
    ''' <param name="SectionName">The name of the Section to retrieve the keys from.</param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[AcketK]	30/09/2010	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function GetKeys(ByVal SectionName As String) As ArrayList
        'returns an arraylist of the keys in a given section
        Dim ListOfKeys As New ArrayList
        Dim ThisSection As Section = GetSection(SectionName)
        If ThisSection Is Nothing Then Return Nothing
        Dim KeyEnumerator As System.Collections.IEnumerator = ThisSection.GetEnumerator()
        While KeyEnumerator.MoveNext()
            'ListOfKeys.Add(Trim(KeyEnumerator.Current))
            ListOfKeys.Add(KeyEnumerator.Current)
        End While

        Return ListOfKeys
    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Comments a given section, including all of the keys contained in the section.
    ''' </summary>
    ''' <param name="SectionName">The name of the Section to comment.</param>
    ''' <returns></returns>
    ''' <remarks>Keys that are already commented will <b>not</b> preserve their comment status if 'UnCommentSection' is used later on.
    ''' </remarks>
    ''' <history>
    ''' 	[AcketK]	30/09/2010	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function CommentSection(ByVal SectionName As String) As Boolean
        'Comments a given section and all of its keys
        Dim ThisSection As Section = GetSection(SectionName)
        If ThisSection Is Nothing Then Return False
        ThisSection.IsCommented = True
        Dim myEnumerator As System.Collections.IEnumerator = ThisSection.GetEnumerator()
        While myEnumerator.MoveNext()
            myEnumerator.Current.IsCommented = True
        End While
        Return True
    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Uncomments a given section, and all of its keys.
    ''' </summary>
    ''' <param name="SectionName">The name of the Section to uncomment.</param>
    ''' <returns></returns>
    ''' <remarks>Any keys in the section that were previously commented will be uncommented after this function.
    ''' </remarks>
    ''' <history>
    ''' 	[AcketK]	30/09/2010	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function UnCommentSection(ByVal SectionName As String) As Boolean
        'Uncomments a given section and all of its keys
        Dim ThisSection As Section = GetSection(SectionName)
        If ThisSection Is Nothing Then Return False
        ThisSection.IsCommented = False
        Dim myEnumerator As System.Collections.IEnumerator = ThisSection.GetEnumerator()
        While myEnumerator.MoveNext()
            myEnumerator.Current.IsCommented = False
        End While
        Return True
    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Comments a given key in a given section. Both the key and the section must exist. 
    ''' </summary>
    ''' <param name="KeyName">The name of the key to comment.</param>
    ''' <param name="SectionName">The name of the section the key is in.</param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[AcketK]	30/09/2010	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function CommentKey(ByVal KeyName As String, ByVal SectionName As String) As Boolean
        'Comments a given a key
        Dim ThisKey As Key = GetKey(KeyName, SectionName)
        If ThisKey Is Nothing Then Return False
        ThisKey.IsCommented = True
        Return True
    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Uncomments a given key in a given section. Both the key and section must exist.
    ''' </summary>
    ''' <param name="KeyName">The name of the key to uncomment.</param>
    ''' <param name="SectionName">The name of the section the key is in.</param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[AcketK]	30/09/2010	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function UnCommentKey(ByVal KeyName As String, ByVal SectionName As String) As Boolean
        'Uncomments a given key
        Dim ThisKey As Key = GetKey(KeyName, SectionName)
        If ThisKey Is Nothing Then Return False
        ThisKey.IsCommented = False
        Return True
    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Renames a section. The section must exist.
    ''' </summary>
    ''' <param name="SectionName">The name of the section to be renamed.</param>
    ''' <param name="NewSectionName">The new name of the section.</param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[AcketK]	30/09/2010	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function RenameSection(ByVal SectionName As String, ByVal NewSectionName As String) As Boolean
        Dim ThisSection As Section = GetSection(SectionName)
        If ThisSection Is Nothing Then Return False
        ThisSection.Name = NewSectionName
        Return True
    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Renames a given key key in a given section. Both they key and the section must exist. The value is not altered.
    ''' </summary>
    ''' <param name="KeyName">The name of the key to be renamed.</param>
    ''' <param name="SectionName">The name of the section the key exists in.</param>
    ''' <param name="NewKeyName">The new name of the key.</param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[AcketK]	30/09/2010	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function RenameKey(ByVal KeyName As String, ByVal SectionName As String, ByVal NewKeyName As String) As Boolean
        Dim ThisKey As Key = GetKey(KeyName, SectionName)
        If ThisKey Is Nothing Then Return False
        ThisKey.Name = NewKeyName
        Return True
    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Changes the value of a given key in a given section. Both the key and the section must exist.
    ''' </summary>
    ''' <param name="KeyName">The name of the key whose value should be changed.</param>
    ''' <param name="SectionName">The name of the section the key exists in.</param>
    ''' <param name="NewValue">The new value to assign to the key.</param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[AcketK]	30/09/2010	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function ChangeValue(ByVal KeyName As String, ByVal SectionName As String, ByVal NewValue As String)
        Dim ThisSection As Section = GetSection(SectionName)
        If ThisSection Is Nothing Then Return False
        Dim ThisKey As Key = GetKey(KeyName, SectionName)
        If ThisKey Is Nothing Then Return False
        ThisKey.Value = NewValue
        Return True
    End Function

    Public Function GetValue(ByVal KeyName As String, ByVal SectionName As String, ByRef Value As String, Optional ByVal DefaultValue As String = "") As Boolean
        Dim ThisSection As Section = GetSection(SectionName)
        If ThisSection Is Nothing Then Return False
        Dim ThisKey As Key = GetKey(KeyName, SectionName)
        If ThisKey Is Nothing Then Return False
        Value = ThisKey.Value
        Return True
    End Function

    Public Function GetValue(ByVal KeyName As String, ByVal SectionName As String) As String
        Dim ThisSection As Section = GetSection(SectionName)

        If ThisSection Is Nothing Then
            Return Nothing
        End If

        Dim ThisKey As Key = GetKey(KeyName, SectionName)
        If ThisKey Is Nothing Then
            Return Nothing
        End If
        'Value = ThisKey.Value
        Return ThisKey.Value
    End Function

    Private Function GetKey(ByVal KeyName As String, ByVal SectionName As String) As Key
        Dim ThisSection As Section = GetSection(SectionName)
        If ThisSection Is Nothing Then Return Nothing
        Dim myEnumerator As System.Collections.IEnumerator = ThisSection.GetEnumerator()
        While myEnumerator.MoveNext()
            If LCase(myEnumerator.Current.Name) = LCase(KeyName) Then
                Return myEnumerator.Current
            End If
        End While
        Return Nothing
    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Deletes a given section. The section must exist. All the keys in the section will also be deleted.
    ''' </summary>
    ''' <param name="SectionName">The name of the section to be deleted.</param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[AcketK]	30/09/2010	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function DeleteSection(ByVal SectionName As String) As Boolean
        Dim ThisSection As Section = GetSection(SectionName)
        If ThisSection Is Nothing Then Return False
        Sections.Remove(ThisSection)
        Return True
    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Deletes a given key in a given section. Both the key and the section must exist.
    ''' </summary>
    ''' <param name="KeyName">The name of the key to be deleted.</param>
    ''' <param name="SectionName">The name of the section the key exists in.</param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[AcketK]	30/09/2010	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function DeleteKey(ByVal KeyName As String, ByVal SectionName As String) As Boolean
        Dim ThisSection As Section = GetSection(SectionName)
        If ThisSection Is Nothing Then Return False
        Dim ThisKey As Key = GetKey(KeyName, SectionName)
        If ThisKey Is Nothing Then Return False
        ThisSection.Remove(ThisKey)
        Return True
    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Moves a key from one section to another. Both the key and the section must exist, as must the section to move the key to.
    ''' </summary>
    ''' <param name="KeyName">The name of the key to be moved.</param>
    ''' <param name="SectionName">The name of the section the key exists in.</param>
    ''' <param name="NewSectionName">The name of the section to move the key to.</param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[AcketK]	30/09/2010	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function MoveKey(ByVal KeyName As String, ByVal SectionName As String, ByVal NewSectionName As String) As Boolean
        Dim ThisSection As Section = GetSection(SectionName)
        If ThisSection Is Nothing Then Return False
        Dim ThisNewSection As Section = GetSection(NewSectionName)
        If ThisNewSection Is Nothing Then Return False
        Dim ThisKey As Key = GetKey(KeyName, SectionName)
        If ThisKey Is Nothing Then Return False
        If Not GetKey(KeyName, NewSectionName) Is Nothing Then Return False 'Verifiy that the key doesn't already exist in the new section
        ThisSection.Remove(ThisKey)
        ThisNewSection.Add(ThisKey)
        Return True
    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Sorts all of the sections, and all of the keys within the sections.
    ''' </summary>
    ''' <remarks>There is no undo feature for this operation.
    ''' </remarks>
    ''' <history>
    ''' 	[AcketK]	30/09/2010	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Sub Sort()
        Dim mySC As SectionComparer = New SectionComparer
        Sections.Sort(mySC)
        Dim myEnumerator As System.Collections.IEnumerator = Sections.GetEnumerator()
        While myEnumerator.MoveNext()
            myEnumerator.Current.Sort(mySC)
        End While
    End Sub

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Saves the IniFile to the specified filename.
    ''' </summary>
    ''' <param name="FileName">The filename to save the inifile to.</param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[AcketK]	30/09/2010	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Sub Save(ByVal FileName As String)
        Dim sw As System.IO.StreamWriter = Nothing
        Try
            If File.Exists(FileName) Then File.Delete(FileName) ' Remove the existing file

            'Loop through the arraylist (Content) and write each line to the file
            sw = New System.IO.StreamWriter(FileName)
            Dim i As Integer = 1
            Dim SectionEnumerator As System.Collections.IEnumerator = Sections.GetEnumerator()
            While SectionEnumerator.MoveNext()
                If i = 1 Then
                    If Not SectionEnumerator.Current.Comment Is Nothing Then
                        For Each comment As String In SectionEnumerator.Current.Comment
                            sw.Write(CommentString + comment + vbCrLf)
                        Next
                    End If
                    sw.Write(AddBrackets(SectionEnumerator.Current.Name) & vbCrLf)
                    i = i + 1
                Else
                    If Not SectionEnumerator.Current.Comment Is Nothing Then
                        Dim j As Integer = 0
                        For Each comment As String In SectionEnumerator.Current.Comment
                            If j = 0 Then
                                sw.Write(vbCrLf + CommentString + comment + vbCrLf)
                            Else
                                sw.Write(CommentString + comment + vbCrLf)
                            End If
                            j = j + 1
                        Next
                        sw.Write(AddBrackets(SectionEnumerator.Current.Name) & vbCrLf)
                    Else
                        sw.Write(vbCrLf & AddBrackets(SectionEnumerator.Current.Name) & vbCrLf)
                    End If

                End If

                Dim KeyEnumerator As System.Collections.IEnumerator = SectionEnumerator.Current.GetEnumerator()
                While KeyEnumerator.MoveNext()
                    If Not KeyEnumerator.Current.Comment Is Nothing Then
                        For Each comment As String In KeyEnumerator.Current.Comment
                            sw.Write(CommentString + comment + vbCrLf)
                        Next
                    End If
                    sw.Write(KeyEnumerator.Current.Name & "=" & KeyEnumerator.Current.Value & vbCrLf)
                End While
            End While
            sw.Close()
        Catch ex As Exception
            Throw ex
        Finally
            If Not sw Is Nothing Then
                sw.Close()
                sw = Nothing
            End If
        End Try

    End Sub

    Private Function GetText(Optional ByVal ReturnAsHTML As Boolean = False) As String
        Dim CrLf As String = IIf(ReturnAsHTML, "<br>", vbCrLf)
        Dim sb As New StringBuilder
        Dim SectionEnumerator As System.Collections.IEnumerator = Sections.GetEnumerator()
        While SectionEnumerator.MoveNext()
            sb.Append(AddBrackets(SectionEnumerator.Current.Name) & CrLf)
            Dim KeyEnumerator As System.Collections.IEnumerator = SectionEnumerator.Current.GetEnumerator()
            While KeyEnumerator.MoveNext()
                sb.Append(KeyEnumerator.Current.Name & "=" & KeyEnumerator.Current.Value & CrLf)
            End While
        End While
        Return sb.ToString
    End Function

    Private Function RemoveBrackets(ByVal Value As String) As String
        Dim chArr() As Char = {"[", "]", " "}
        Value = Value.TrimStart(chArr)
        Value = Value.TrimEnd(chArr)
        Return Value
    End Function

    Private Function AddBrackets(ByVal Value As String) As String
        Return "[" & Trim(Value) & "]"
    End Function

End Class

Public Class Section
    Inherits ArrayList
    Public Name As String
    Public IsCommented As Boolean
    Public Comment() As String

    Public Sub New(ByVal SectionName As String, Optional ByVal SectionIsCommented As Boolean = False, Optional ByVal Comments() As String = Nothing)
        Name = SectionName
        IsCommented = SectionIsCommented
        Comment = Comments
    End Sub

    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If obj Is Nothing Or Not Me.GetType() Is obj.GetType() Then
            Return False
        End If
        Dim s As Section = CType(obj, Section)
        Return Me.Name = s.Name And Me.IsCommented = s.IsCommented
    End Function

    Public Overrides Function GetHashCode() As Integer
        Dim s As String = Name
        Return s.GetHashCode()
    End Function
End Class

Public Class SectionComparer
    Implements IComparer

    Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer Implements System.Collections.IComparer.Compare
        Dim s1 As String = LCase(x.Name)
        Dim s2 As String = LCase(y.Name)
        Return s1.CompareTo(s2)
    End Function
End Class

Public Class Key
    Public Name As String
    Public Value As String
    Public IsCommented As Boolean
    Public Comment() As String

    Public Sub New(ByVal KeyName As String, ByVal KeyValue As String, Optional ByVal KeyIsCommented As Boolean = False, Optional ByVal Comments() As String = Nothing)
        Name = KeyName
        Value = KeyValue
        IsCommented = KeyIsCommented
        Comment = Comments
    End Sub

    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If obj Is Nothing Or Not Me.GetType() Is obj.GetType() Then
            Return False
        End If
        Dim k As Key = CType(obj, Key)
        Return Me.Name = k.Name And Me.Value = k.Value And Me.IsCommented = k.IsCommented
    End Function

    Public Overrides Function GetHashCode() As Integer
        Dim s As String = Name + Value + IsCommented
        Return s.GetHashCode()
    End Function
End Class

Public Class KeyComparer
    Implements IComparer

    Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer Implements System.Collections.IComparer.Compare
        Dim s1 As String = LCase(x.Name)
        Dim s2 As String = LCase(y.Name)
        Return s1.CompareTo(s2)
    End Function
End Class

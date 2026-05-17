Imports System.IO
Imports System.IO.Compression

Public Class Index

    Private SuitList As New List(Of SuitInfo)

    ' ---------------- FORM LOAD ----------------
    Private Sub Index_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.AutoScaleMode = AutoScaleMode.Dpi

        AllowDrop = True

        Dim dllPath As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "7z.dll")
        SevenZip.SevenZipExtractor.SetLibraryPath(dllPath)

        If statusDropdown IsNot Nothing Then
            statusDropdown.Items.Clear()
            statusDropdown.Items.AddRange({"Enabled/Disabled", "Enabled", "Disabled"})
            statusDropdown.SelectedIndex = 0
        End If

        If typeDropdown IsNot Nothing Then
            typeDropdown.Items.Clear()
            typeDropdown.Items.AddRange({
                "All",
                "Batman",
                "Batmobile",
                "Robin",
                "Catwoman",
                "Nightwing",
                "Harley Quinn",
                "Red Hood"
            })
            typeDropdown.SelectedIndex = 0
        End If

        LoadSuits()
        RefreshSuitList()
        Me.PerformAutoScale()
        Me.PerformLayout()
    End Sub

    Private Sub Index_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        ResizeSuitCards()
    End Sub

    ' ---------------- DRAG & DROP ----------------
    Private Sub Index_DragEnter(sender As Object, e As DragEventArgs) Handles Me.DragEnter
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            e.Effect = DragDropEffects.Copy
        End If
    End Sub

    Private Sub Index_DragDrop(sender As Object, e As DragEventArgs) Handles Me.DragDrop
        Dim files() As String = CType(e.Data.GetData(DataFormats.FileDrop), String())

        For Each file In files
            Dim ext = Path.GetExtension(file).ToLower()
            If {".zip", ".rar", ".7z"}.Contains(ext) Then
                ImportArchiveMod(file)
            End If
        Next

        LoadSuits()
        RefreshSuitList()
    End Sub

    ' ---------------- IMPORT ARCHIVE (ZIP / RAR / 7Z) ----------------
    Private Sub ImportArchiveMod(archivePath As String)
        Dim modName As String = Path.GetFileNameWithoutExtension(archivePath)
        Dim enabledPath As String = Path.Combine(My.Settings.GameRoot, "DLC", "313100", "[ENABLED MODS]")
        Dim tempExtract As String = Path.Combine(Path.GetTempPath(), "AKSuitManager_" & Guid.NewGuid().ToString())

        Directory.CreateDirectory(tempExtract)

        ' Extract archive using SevenZipSharp
        Try
            ExtractArchive(archivePath, tempExtract)
        Catch ex As Exception
            MessageBox.Show("Failed to extract archive: " & ex.Message,
                        "Import Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error)
            Directory.Delete(tempExtract, True)
            Exit Sub
        End Try

        ' Detect real mod root
        Dim entries = Directory.GetDirectories(tempExtract)
        Dim modRoot As String = tempExtract

        If entries.Length = 1 Then
            modRoot = entries(0)
        End If

        Dim finalModFolder As String = Path.Combine(enabledPath, modName)

        Try
            Directory.CreateDirectory(finalModFolder)

            ' Copy contents of modRoot into finalModFolder
            For Each item In Directory.GetFileSystemEntries(modRoot)
                Dim dest = Path.Combine(finalModFolder, Path.GetFileName(item))

                If Directory.Exists(item) Then
                    CopyDirectory(item, dest)
                Else
                    File.Copy(item, dest, True)
                End If
            Next

        Catch ex As Exception
            MessageBox.Show("Failed to prepare mod folder: " & ex.Message,
                        "Import Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error)
            Directory.Delete(tempExtract, True)
            Exit Sub
        End Try

        Directory.Delete(tempExtract, True)

        ' Validate mod structure
        If Not ValidateModStructure(finalModFolder) Then
            MessageBox.Show("Invalid mod structure. The mod has been removed.",
                        "Invalid Mod",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error)
            Directory.Delete(finalModFolder, True)
        End If
    End Sub

    ' ---------------- EXTRACT ARCHIVE (SevenZipSharp) ----------------
    Private Sub ExtractArchive(archivePath As String, outputPath As String)
        Try
            Using extractor As New SevenZip.SevenZipExtractor(archivePath)
                extractor.ExtractArchive(outputPath)
            End Using
        Catch ex As Exception
            Throw
        End Try
    End Sub


    ' ---------------- COPY DIRECTORY (CORRECT VERSION) ----------------
    Private Sub CopyDirectory(source As String, destination As String)
        Directory.CreateDirectory(destination)

        ' Copy files
        For Each filePath In Directory.GetFiles(source)
            Dim destFile As String = Path.Combine(destination, Path.GetFileName(filePath))
            File.Copy(filePath, destFile, True)
        Next

        ' Copy subdirectories
        For Each dirPath In Directory.GetDirectories(source)
            Dim destDir As String = Path.Combine(destination, Path.GetFileName(dirPath))
            CopyDirectory(dirPath, destDir)
        Next
    End Sub





    ' ---------------- VALIDATE STRUCTURE ----------------
    Private Function ValidateModStructure(modFolder As String) As Boolean
        Dim configPath = Path.Combine(modFolder, "Config")
        Dim cookedPath = Path.Combine(modFolder, "CookedPCConsole")

        ' Two possible localization paths:
        Dim locPathINT = Path.Combine(modFolder, "Localization", "INT")
        Dim locPathFlat = Path.Combine(modFolder, "Localization")

        Dim locFileINT = Path.Combine(locPathINT, "GFxUI.int")
        Dim locFileFlat = Path.Combine(locPathFlat, "GFxUI.int")

        ' Check required folders
        If Not Directory.Exists(configPath) Then Return False
        If Not Directory.Exists(cookedPath) Then Return False

        ' Check localization folder (either format)
        Dim locValid As Boolean =
        (Directory.Exists(locPathINT) AndAlso File.Exists(locFileINT)) OrElse
        (Directory.Exists(locPathFlat) AndAlso File.Exists(locFileFlat))

        If Not locValid Then Return False

        ' Check required files
        If Not File.Exists(Path.Combine(configPath, "BmGame.ini")) Then Return False

        Return True
    End Function


    ' ---------------- READ METADATA ----------------
    Private Function ReadModMetadata(modFolder As String) As (Name As String, Description As String)
        Dim locFileINT As String = Path.Combine(modFolder, "Localization", "INT", "GFxUI.int")
        Dim locFileFlat As String = Path.Combine(modFolder, "Localization", "GFxUI.int")

        Dim locFile As String = Nothing

        If File.Exists(locFileINT) Then
            locFile = locFileINT
        ElseIf File.Exists(locFileFlat) Then
            locFile = locFileFlat
        Else
            Return (Path.GetFileName(modFolder), "")
        End If

        Dim modName As String = Path.GetFileName(modFolder)
        Dim modDesc As String = ""

        For Each line In File.ReadLines(locFile)
            If line.Contains("=") Then
                Dim parts = line.Split("="c, 2)
                Dim key = parts(0).Trim()
                Dim value = parts(1).Trim()

                If key.EndsWith("_Desc") Then
                    modDesc = value
                Else
                    modName = value
                End If
            End If
        Next

        Return (modName, modDesc)
    End Function

    Private Sub ResizeSuitCards()
        If suitView Is Nothing OrElse suitView.Controls.Count = 0 Then Exit Sub

        ' Card height scales with window height
        Dim newHeight As Integer = Math.Max(90, Me.Height \ 12)

        For Each card As Panel In suitView.Controls
            card.Height = newHeight

            ' Reposition icons vertically centered
            For Each ctrl As Control In card.Controls
                If TypeOf ctrl Is PictureBox Then
                    Dim pb = DirectCast(ctrl, PictureBox)
                    Dim yCenter As Integer = (newHeight \ 2) - (pb.Height \ 2)

                    Select Case CStr(pb.Tag)
                        Case "browse"
                            pb.Location = New Point(card.ClientSize.Width - 120, yCenter)
                        Case "toggle"
                            pb.Location = New Point(card.ClientSize.Width - 80, yCenter)
                        Case "remove"
                            pb.Location = New Point(card.ClientSize.Width - 40, yCenter)
                    End Select
                End If
            Next
        Next
    End Sub





    ' ---------------- SETTINGS BUTTON ----------------
    Private Sub settingsButton_Click(sender As Object, e As EventArgs) Handles settingsButton.Click
        Settings.StartPosition = FormStartPosition.Manual
        Settings.Location = Me.Location
        Settings.Size = Me.Size

        Settings.Show()
        Me.Hide()
    End Sub

    Private Sub Form_Closed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed
        Application.Exit()
    End Sub

    ' ---------------- DETECT SUIT TYPE ----------------
    Private Function DetectSuitType(suitFolder As String) As String
        Dim cookedPath As String = Path.Combine(suitFolder, "CookedPCConsole")

        If Not Directory.Exists(cookedPath) Then
            Return "Unknown"
        End If

        Dim files = Directory.EnumerateFiles(cookedPath, "*.*", SearchOption.AllDirectories)

        For Each f In files
            Dim name = Path.GetFileName(f).ToLower()

            If {"playable_batman", "playable_dlcbatman"}.Any(Function(s) name.Contains(s)) Then Return "Batman"
            If {"playable_batmobile", "playable_dlcbatmobile"}.Any(Function(s) name.Contains(s)) Then Return "Batmobile"
            If {"playable_robin", "playable_dlcrobin"}.Any(Function(s) name.Contains(s)) Then Return "Robin"
            If {"playable_catwoman", "playable_dlccatwoman"}.Any(Function(s) name.Contains(s)) Then Return "Catwoman"
            If {"playable_nightwing", "playable_dlcnightwing"}.Any(Function(s) name.Contains(s)) Then Return "Nightwing"
            If {"playable_harleyquinn", "playable_harleyquinn"}.Any(Function(s) name.Contains(s)) Then Return "Harley Quinn"
            If {"playable_redhood", "playable_dlcredhood"}.Any(Function(s) name.Contains(s)) Then Return "Red Hood"
        Next

        Return "Unknown"
    End Function

    ' ---------------- LOAD SUITS ----------------
    Public Sub LoadSuits()
        SuitList.Clear()

        If String.IsNullOrWhiteSpace(My.Settings.GameRoot) Then Exit Sub

        Dim enabledPath As String = Path.Combine(My.Settings.GameRoot, "DLC", "313100", "[ENABLED MODS]")
        Dim disabledPath As String = Path.Combine(My.Settings.GameRoot, "DLC", "[DISABLED MODS]")

        If Directory.Exists(enabledPath) Then
            For Each suitDir In Directory.EnumerateDirectories(enabledPath)
                SuitList.Add(New SuitInfo With {
                    .Name = Path.GetFileName(suitDir),
                    .IsEnabled = True,
                    .FolderPath = suitDir,
                    .SuitType = DetectSuitType(suitDir)
                })
            Next
        End If

        If Directory.Exists(disabledPath) Then
            For Each suitDir In Directory.EnumerateDirectories(disabledPath)
                SuitList.Add(New SuitInfo With {
                    .Name = Path.GetFileName(suitDir),
                    .IsEnabled = False,
                    .FolderPath = suitDir,
                    .SuitType = DetectSuitType(suitDir)
                })
            Next
        End If
    End Sub

    ' ---------------- REFRESH UI ----------------
    Public Sub RefreshSuitList()
        suitView.SuspendLayout()
        suitView.Controls.Clear()

        Dim typeOrder As New Dictionary(Of String, Integer) From {
        {"Batman", 1},
        {"Batmobile", 2},
        {"Robin", 3},
        {"Catwoman", 4},
        {"Nightwing", 5},
        {"Harley Quinn", 6},
        {"Red Hood", 7}
    }

        Dim status As String = If(statusDropdown?.SelectedItem?.ToString(), "All")
        Dim suitType As String = If(typeDropdown?.SelectedItem?.ToString(), "All")

        Dim sorted = SuitList _
        .OrderBy(Function(s) If(typeOrder.ContainsKey(s.SuitType),
                                typeOrder(s.SuitType),
                                Integer.MaxValue)) _
        .ThenBy(Function(s) ReadModMetadata(s.FolderPath).Name) _
        .ToList()

        For Each suit In sorted
            If status = "Enabled" AndAlso Not suit.IsEnabled Then Continue For
            If status = "Disabled" AndAlso suit.IsEnabled Then Continue For
            If suitType <> "All" AndAlso suit.SuitType <> suitType Then Continue For

            Dim card As Panel = CreateSuitCard(suit)
            suitView.Controls.Add(card)
            suitView.Controls.SetChildIndex(card, 0)

            For Each ctrl As Control In card.Controls
                If TypeOf ctrl Is PictureBox Then
                    Dim pb = DirectCast(ctrl, PictureBox)
                    Select Case CStr(pb.Tag)
                        Case "browse"
                            pb.Location = New Point(card.ClientSize.Width - 120, 28)
                        Case "toggle"
                            pb.Location = New Point(card.ClientSize.Width - 80, 28)
                        Case "remove"
                            pb.Location = New Point(card.ClientSize.Width - 40, 28)
                    End Select
                End If
            Next
        Next

        suitView.ResumeLayout()
    End Sub




    ' ---------------- FORCE DELETE ----------------
    Private Sub ForceDeleteFolder(path As String)
        If Not Directory.Exists(path) Then Exit Sub

        For Each f In Directory.GetFiles(path, "*", SearchOption.AllDirectories)
            Try : File.SetAttributes(f, FileAttributes.Normal) : Catch : End Try
        Next

        For Each d In Directory.GetDirectories(path, "*", SearchOption.AllDirectories)
            Try : File.SetAttributes(d, FileAttributes.Normal) : Catch : End Try
        Next

        Directory.Delete(path, True)
    End Sub

    ' ---------------- CREATE CARD ----------------
    Private Function CreateSuitCard(suit As SuitInfo) As Panel
        Dim meta = ReadModMetadata(suit.FolderPath)

        ' --- CARD PANEL ---
        Dim card As New Panel()
        card.Height = 90
        card.BackColor = Color.FromArgb(40, 40, 40)
        card.Margin = New Padding(5, 5, 5, 0)
        card.MinimumSize = New Size(200, 90)

        ' KEY: let WinForms handle width
        card.Dock = DockStyle.Top

        ' --- NAME LABEL ---
        Dim lbl As New Label()
        lbl.Text = meta.Name
        lbl.ForeColor = Color.White
        lbl.Location = New Point(10, 10)
        lbl.AutoSize = True

        ' --- DESCRIPTION LABEL ---
        Dim descLbl As New Label()
        descLbl.Text = meta.Description
        descLbl.ForeColor = Color.LightGray
        descLbl.Location = New Point(10, 35)
        descLbl.AutoSize = True

        ' --- TYPE LABEL ---
        Dim typeLbl As New Label()
        typeLbl.Text = suit.SuitType
        typeLbl.ForeColor = Color.Gray
        typeLbl.Location = New Point(10, 60)
        typeLbl.AutoSize = True

        ' --- BROWSE ICON ---
        Dim browse As New PictureBox()
        browse.Size = New Size(32, 32)
        browse.SizeMode = PictureBoxSizeMode.Zoom
        browse.Cursor = Cursors.Hand
        browse.Image = My.Resources.Browse
        browse.Tag = "browse"
        browse.Anchor = AnchorStyles.Top Or AnchorStyles.Right

        AddHandler browse.Click,
        Sub()
            If Directory.Exists(suit.FolderPath) Then
                Process.Start("explorer.exe", suit.FolderPath)
            End If
        End Sub

        ' --- TOGGLE ICON ---
        Dim toggle As New PictureBox()
        toggle.Size = New Size(32, 32)
        toggle.SizeMode = PictureBoxSizeMode.Zoom
        toggle.Cursor = Cursors.Hand
        toggle.Image = If(suit.IsEnabled, My.Resources.ToggledOn, My.Resources.ToggledOff)
        toggle.Tag = "toggle"
        toggle.Anchor = AnchorStyles.Top Or AnchorStyles.Right

        AddHandler toggle.Click,
        Sub()
            ToggleSuit(suit)
            LoadSuits()
            RefreshSuitList()
        End Sub

        ' --- REMOVE ICON ---
        Dim remove As New PictureBox()
        remove.Size = New Size(32, 32)
        remove.SizeMode = PictureBoxSizeMode.Zoom
        remove.Cursor = Cursors.Hand
        remove.Image = My.Resources.Uninstall
        remove.Tag = "remove"
        remove.Anchor = AnchorStyles.Top Or AnchorStyles.Right

        AddHandler remove.Click,
        Sub()
            suitView.Controls.Remove(card)
            card.Dispose()

            GC.Collect()
            GC.WaitForPendingFinalizers()

            If Directory.Exists(suit.FolderPath) Then
                ForceDeleteFolder(suit.FolderPath)
            End If

            LoadSuits()
            RefreshSuitList()
        End Sub

        ' Add controls
        card.Controls.Add(lbl)
        card.Controls.Add(descLbl)
        card.Controls.Add(typeLbl)
        card.Controls.Add(browse)
        card.Controls.Add(toggle)
        card.Controls.Add(remove)

        ' Initial rough positions; they’ll stretch with Dock
        browse.Location = New Point(card.Width - 120, 28)
        toggle.Location = New Point(card.Width - 80, 28)
        remove.Location = New Point(card.Width - 40, 28)

        Return card
    End Function



    ' ---------------- TOGGLE SUIT ----------------
    Private Sub ToggleSuit(suit As SuitInfo)
        GC.Collect()
        GC.WaitForPendingFinalizers()

        Dim enabledPath As String = Path.Combine(My.Settings.GameRoot, "DLC", "313100", "[ENABLED MODS]")
        Dim disabledPath As String = Path.Combine(My.Settings.GameRoot, "DLC", "[DISABLED MODS]")

        If Not Directory.Exists(enabledPath) Then Directory.CreateDirectory(enabledPath)
        If Not Directory.Exists(disabledPath) Then Directory.CreateDirectory(disabledPath)

        Dim newPath As String =
            If(suit.IsEnabled,
               Path.Combine(disabledPath, suit.Name),
               Path.Combine(enabledPath, suit.Name))

        If Directory.Exists(suit.FolderPath) Then
            Directory.Move(suit.FolderPath, newPath)
        End If
    End Sub

    ' ---------------- DROPDOWN EVENTS ----------------
    Private Sub statusDropdown_SelectedIndexChanged(sender As Object, e As EventArgs) Handles statusDropdown.SelectedIndexChanged
        RefreshSuitList()
    End Sub

    Private Sub typeDropdown_SelectedIndexChanged(sender As Object, e As EventArgs) Handles typeDropdown.SelectedIndexChanged
        RefreshSuitList()
    End Sub

    ' ---------------- REFRESH BUTTON ----------------
    Private Sub refreshButton_Click(sender As Object, e As EventArgs) Handles refreshButton.Click
        LoadSuits()
        RefreshSuitList()
    End Sub
End Class

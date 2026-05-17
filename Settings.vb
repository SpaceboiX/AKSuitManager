Imports System.IO

Public Class Settings

    Private Sub Settings_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Load saved directory into the textbox
        rootLocation.Text = My.Settings.GameRoot
        Me.Scale(New SizeF(1.5F, 1.5F))
    End Sub

    Private Sub homeButton_Click(sender As Object, e As EventArgs) Handles homeButton.Click
        Index.StartPosition = FormStartPosition.Manual
        Index.Location = Me.Location
        Index.Size = Me.Size

        Index.Show()
        Me.Hide()
    End Sub
    Private Sub Form_Closed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed
        Application.Exit()
    End Sub

    Private Sub browse_Click(sender As Object, e As EventArgs) Handles browse.Click
        If rootDialog.ShowDialog() = DialogResult.OK Then

            Dim selectedPath As String = rootDialog.SelectedPath

            ' Validate folder exists
            If Not Directory.Exists(selectedPath) Then
                MessageBox.Show("The selected folder does not exist.", "Invalid Directory", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            ' Save to My.Settings
            My.Settings.GameRoot = selectedPath
            My.Settings.Save()

            ' Update textbox
            rootLocation.Text = selectedPath

            ' Refresh Index immediately if it's open
            If Index IsNot Nothing Then
                Index.LoadSuits()
                Index.RefreshSuitList()
            End If

        End If
    End Sub

End Class

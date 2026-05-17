<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Settings
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Settings))
        homeButton = New Button()
        rootLocation = New TextBox()
        browse = New Button()
        rootDialog = New FolderBrowserDialog()
        SuspendLayout()
        ' 
        ' homeButton
        ' 
        homeButton.BackColor = Color.Transparent
        homeButton.BackgroundImage = My.Resources.Resources.BackArrow
        homeButton.BackgroundImageLayout = ImageLayout.Stretch
        homeButton.FlatAppearance.BorderSize = 0
        homeButton.FlatStyle = FlatStyle.Flat
        homeButton.Location = New Point(12, 12)
        homeButton.Name = "homeButton"
        homeButton.Size = New Size(65, 65)
        homeButton.TabIndex = 1
        homeButton.TabStop = False
        homeButton.UseVisualStyleBackColor = False
        ' 
        ' rootLocation
        ' 
        rootLocation.Font = New Font("Segoe UI", 16F)
        rootLocation.Location = New Point(100, 41)
        rootLocation.Name = "rootLocation"
        rootLocation.Size = New Size(591, 36)
        rootLocation.TabIndex = 2
        ' 
        ' browse
        ' 
        browse.BackgroundImage = My.Resources.Resources.Browse
        browse.BackgroundImageLayout = ImageLayout.Zoom
        browse.FlatAppearance.BorderSize = 0
        browse.FlatStyle = FlatStyle.Flat
        browse.ForeColor = Color.Transparent
        browse.Location = New Point(697, 39)
        browse.Name = "browse"
        browse.Size = New Size(38, 38)
        browse.TabIndex = 3
        browse.TabStop = False
        browse.UseVisualStyleBackColor = True
        ' 
        ' Settings
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(741, 450)
        Controls.Add(browse)
        Controls.Add(rootLocation)
        Controls.Add(homeButton)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        Name = "Settings"
        Text = "AKSuitManager"
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents homeButton As Button
    Friend WithEvents rootLocation As TextBox
    Friend WithEvents browse As Button
    Friend WithEvents rootDialog As FolderBrowserDialog
End Class

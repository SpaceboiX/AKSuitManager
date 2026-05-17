<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Index
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(disposing As Boolean)
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Index))
        settingsButton = New Button()
        refreshButton = New Button()
        statusDropdown = New ComboBox()
        typeDropdown = New ComboBox()
        suitView = New Panel()
        header = New Panel()
        header.SuspendLayout()
        SuspendLayout()
        ' 
        ' settingsButton
        ' 
        settingsButton.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        settingsButton.AutoSize = True
        settingsButton.BackColor = Color.Transparent
        settingsButton.BackgroundImage = My.Resources.Resources.Settings
        settingsButton.BackgroundImageLayout = ImageLayout.Stretch
        settingsButton.FlatAppearance.BorderSize = 0
        settingsButton.FlatStyle = FlatStyle.Flat
        settingsButton.Location = New Point(360, 8)
        settingsButton.Name = "settingsButton"
        settingsButton.Size = New Size(40, 40)
        settingsButton.TabIndex = 0
        settingsButton.TabStop = False
        settingsButton.UseVisualStyleBackColor = False
        ' 
        ' refreshButton
        ' 
        refreshButton.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        refreshButton.AutoSize = True
        refreshButton.BackColor = Color.Transparent
        refreshButton.BackgroundImage = My.Resources.Resources.Refresh
        refreshButton.BackgroundImageLayout = ImageLayout.Zoom
        refreshButton.FlatAppearance.BorderSize = 0
        refreshButton.FlatStyle = FlatStyle.Flat
        refreshButton.Location = New Point(314, 5)
        refreshButton.Name = "refreshButton"
        refreshButton.Size = New Size(40, 40)
        refreshButton.TabIndex = 6
        refreshButton.TabStop = False
        refreshButton.UseVisualStyleBackColor = False
        ' 
        ' statusDropdown
        ' 
        statusDropdown.DropDownStyle = ComboBoxStyle.DropDownList
        statusDropdown.FlatStyle = FlatStyle.Flat
        statusDropdown.FormattingEnabled = True
        statusDropdown.Location = New Point(130, 13)
        statusDropdown.Name = "statusDropdown"
        statusDropdown.Size = New Size(121, 23)
        statusDropdown.TabIndex = 7
        ' 
        ' typeDropdown
        ' 
        typeDropdown.DropDownStyle = ComboBoxStyle.DropDownList
        typeDropdown.FlatStyle = FlatStyle.Flat
        typeDropdown.FormattingEnabled = True
        typeDropdown.Location = New Point(3, 13)
        typeDropdown.Name = "typeDropdown"
        typeDropdown.Size = New Size(121, 23)
        typeDropdown.TabIndex = 8
        ' 
        ' suitView
        ' 
        suitView.AutoScroll = True
        suitView.AutoSize = True
        suitView.BackColor = Color.Transparent
        suitView.Dock = DockStyle.Fill
        suitView.Location = New Point(0, 0)
        suitView.Name = "suitView"
        suitView.Padding = New Padding(5, 50, 5, 5)
        suitView.Size = New Size(736, 385)
        suitView.TabIndex = 9
        ' 
        ' header
        ' 
        header.AutoSize = True
        header.Controls.Add(typeDropdown)
        header.Controls.Add(settingsButton)
        header.Controls.Add(refreshButton)
        header.Controls.Add(statusDropdown)
        header.Dock = DockStyle.Top
        header.Location = New Point(0, 0)
        header.Name = "header"
        header.Padding = New Padding(5)
        header.Size = New Size(736, 56)
        header.TabIndex = 10
        ' 
        ' Index
        ' 
        AllowDrop = True
        AutoScaleDimensions = New SizeF(96F, 96F)
        AutoScaleMode = AutoScaleMode.Dpi
        BackColor = SystemColors.Desktop
        ClientSize = New Size(736, 385)
        Controls.Add(header)
        Controls.Add(suitView)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        Name = "Index"
        RightToLeftLayout = True
        Text = "AKSuitManager"
        header.ResumeLayout(False)
        header.PerformLayout()
        ResumeLayout(False)
        PerformLayout()
    End Sub
    Friend WithEvents settingsButton As Button
    Friend WithEvents refreshButton As Button
    Friend WithEvents statusDropdown As ComboBox
    Friend WithEvents typeDropdown As ComboBox
    Friend WithEvents suitView As Panel
    Friend WithEvents header As Panel

End Class

namespace ScreenSaver
{
    partial class SettingsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.displayNothingRadioButton = new System.Windows.Forms.RadioButton();
            this.displayCurrentTimeRadioButton = new System.Windows.Forms.RadioButton();
            this.displayWorldTimesRadioButton = new System.Windows.Forms.RadioButton();
            this.worldTimesListView = new System.Windows.Forms.ListView();
            this.cityColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.largeImageList = new System.Windows.Forms.ImageList(this.components);
            this.display24hrRadioButton = new System.Windows.Forms.RadioButton();
            this.display12hrRadioButton = new System.Windows.Forms.RadioButton();
            this.selectedScreenNameLabel = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.githubLinkLabel = new System.Windows.Forms.LinkLabel();
            this.scaleTrackBar = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.addLocationButton = new System.Windows.Forms.Button();
            this.removeLocationButton = new System.Windows.Forms.Button();
            this.locationComboBox = new System.Windows.Forms.ComboBox();
            this.editLocationButton = new System.Windows.Forms.Button();
            this.screensListBox = new System.Windows.Forms.ListBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scaleTrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(478, 13);
            this.okButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(87, 30);
            this.okButton.TabIndex = 4;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.CausesValidation = false;
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(478, 51);
            this.cancelButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(87, 30);
            this.cancelButton.TabIndex = 5;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // displayNothingRadioButton
            // 
            this.displayNothingRadioButton.AutoSize = true;
            this.displayNothingRadioButton.Location = new System.Drawing.Point(120, 187);
            this.displayNothingRadioButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.displayNothingRadioButton.Name = "displayNothingRadioButton";
            this.displayNothingRadioButton.Size = new System.Drawing.Size(73, 21);
            this.displayNothingRadioButton.TabIndex = 7;
            this.displayNothingRadioButton.TabStop = true;
            this.displayNothingRadioButton.Text = "Nothing";
            this.displayNothingRadioButton.UseVisualStyleBackColor = true;
            this.displayNothingRadioButton.CheckedChanged += new System.EventHandler(this.displayNothingRadioButton_CheckedChanged);
            // 
            // displayCurrentTimeRadioButton
            // 
            this.displayCurrentTimeRadioButton.AutoSize = true;
            this.displayCurrentTimeRadioButton.Location = new System.Drawing.Point(120, 216);
            this.displayCurrentTimeRadioButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.displayCurrentTimeRadioButton.Name = "displayCurrentTimeRadioButton";
            this.displayCurrentTimeRadioButton.Size = new System.Drawing.Size(98, 21);
            this.displayCurrentTimeRadioButton.TabIndex = 8;
            this.displayCurrentTimeRadioButton.TabStop = true;
            this.displayCurrentTimeRadioButton.Text = "Current time";
            this.displayCurrentTimeRadioButton.UseVisualStyleBackColor = true;
            this.displayCurrentTimeRadioButton.CheckedChanged += new System.EventHandler(this.displayCurrentTimeRadioButton_CheckedChanged);
            // 
            // displayWorldTimesRadioButton
            // 
            this.displayWorldTimesRadioButton.AutoSize = true;
            this.displayWorldTimesRadioButton.Location = new System.Drawing.Point(120, 245);
            this.displayWorldTimesRadioButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.displayWorldTimesRadioButton.Name = "displayWorldTimesRadioButton";
            this.displayWorldTimesRadioButton.Size = new System.Drawing.Size(99, 21);
            this.displayWorldTimesRadioButton.TabIndex = 10;
            this.displayWorldTimesRadioButton.TabStop = true;
            this.displayWorldTimesRadioButton.Text = "World Times";
            this.displayWorldTimesRadioButton.UseVisualStyleBackColor = true;
            this.displayWorldTimesRadioButton.CheckedChanged += new System.EventHandler(this.displayWorldTimesRadioButton_CheckedChanged);
            // 
            // worldTimesListView
            // 
            this.worldTimesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.cityColumnHeader});
            this.worldTimesListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.worldTimesListView.HideSelection = false;
            this.worldTimesListView.LabelEdit = true;
            this.worldTimesListView.Location = new System.Drawing.Point(225, 245);
            this.worldTimesListView.MultiSelect = false;
            this.worldTimesListView.Name = "worldTimesListView";
            this.worldTimesListView.Size = new System.Drawing.Size(247, 145);
            this.worldTimesListView.TabIndex = 11;
            this.worldTimesListView.UseCompatibleStateImageBehavior = false;
            this.worldTimesListView.View = System.Windows.Forms.View.Details;
            this.worldTimesListView.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.worldTimesListView_AfterLabelEdit);
            this.worldTimesListView.SelectedIndexChanged += new System.EventHandler(this.worldTimesListView_SelectedIndexChanged);
            // 
            // cityColumnHeader
            // 
            this.cityColumnHeader.Text = "Location";
            this.cityColumnHeader.Width = 202;
            // 
            // largeImageList
            // 
            this.largeImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("largeImageList.ImageStream")));
            this.largeImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.largeImageList.Images.SetKeyName(0, "screen.png");
            // 
            // display24hrRadioButton
            // 
            this.display24hrRadioButton.AutoSize = true;
            this.display24hrRadioButton.Location = new System.Drawing.Point(145, 7);
            this.display24hrRadioButton.Name = "display24hrRadioButton";
            this.display24hrRadioButton.Size = new System.Drawing.Size(56, 21);
            this.display24hrRadioButton.TabIndex = 1;
            this.display24hrRadioButton.TabStop = true;
            this.display24hrRadioButton.Text = "24 hr";
            this.display24hrRadioButton.UseVisualStyleBackColor = true;
            this.display24hrRadioButton.CheckedChanged += new System.EventHandler(this.display24hrRadioButton_CheckedChanged);
            // 
            // display12hrRadioButton
            // 
            this.display12hrRadioButton.AutoSize = true;
            this.display12hrRadioButton.Location = new System.Drawing.Point(73, 7);
            this.display12hrRadioButton.Name = "display12hrRadioButton";
            this.display12hrRadioButton.Size = new System.Drawing.Size(56, 21);
            this.display12hrRadioButton.TabIndex = 0;
            this.display12hrRadioButton.TabStop = true;
            this.display12hrRadioButton.Text = "12 hr";
            this.display12hrRadioButton.UseVisualStyleBackColor = true;
            this.display12hrRadioButton.CheckedChanged += new System.EventHandler(this.display12hrRadioButton_CheckedChanged);
            // 
            // selectedScreenNameLabel
            // 
            this.selectedScreenNameLabel.AutoSize = true;
            this.selectedScreenNameLabel.Location = new System.Drawing.Point(117, 157);
            this.selectedScreenNameLabel.Name = "selectedScreenNameLabel";
            this.selectedScreenNameLabel.Size = new System.Drawing.Size(47, 17);
            this.selectedScreenNameLabel.TabIndex = 14;
            this.selectedScreenNameLabel.Text = "Screen";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.display24hrRadioButton);
            this.panel1.Controls.Add(this.display12hrRadioButton);
            this.panel1.Location = new System.Drawing.Point(47, 40);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(243, 40);
            this.panel1.TabIndex = 15;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 17);
            this.label3.TabIndex = 2;
            this.label3.Text = "Display:";
            // 
            // githubLinkLabel
            // 
            this.githubLinkLabel.AutoSize = true;
            this.githubLinkLabel.Location = new System.Drawing.Point(165, 454);
            this.githubLinkLabel.Name = "githubLinkLabel";
            this.githubLinkLabel.Size = new System.Drawing.Size(212, 17);
            this.githubLinkLabel.TabIndex = 16;
            this.githubLinkLabel.TabStop = true;
            this.githubLinkLabel.Text = "https://github.com/phaselden/FlipIt";
            this.githubLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.githubLinkLabel_LinkClicked);
            // 
            // scaleTrackBar
            // 
            this.scaleTrackBar.LargeChange = 1;
            this.scaleTrackBar.Location = new System.Drawing.Point(111, 86);
            this.scaleTrackBar.Name = "scaleTrackBar";
            this.scaleTrackBar.Size = new System.Drawing.Size(170, 45);
            this.scaleTrackBar.TabIndex = 17;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(51, 83);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 17);
            this.label1.TabIndex = 18;
            this.label1.Text = "Size:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(16, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 17);
            this.label2.TabIndex = 19;
            this.label2.Text = "Current Time";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(16, 130);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(54, 17);
            this.label4.TabIndex = 20;
            this.label4.Text = "Screens";
            // 
            // addLocationButton
            // 
            this.addLocationButton.CausesValidation = false;
            this.addLocationButton.Location = new System.Drawing.Point(478, 392);
            this.addLocationButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.addLocationButton.Name = "addLocationButton";
            this.addLocationButton.Size = new System.Drawing.Size(87, 30);
            this.addLocationButton.TabIndex = 21;
            this.addLocationButton.Text = "Add...";
            this.addLocationButton.UseVisualStyleBackColor = true;
            this.addLocationButton.Click += new System.EventHandler(this.addLocationButton_Click);
            // 
            // removeLocationButton
            // 
            this.removeLocationButton.CausesValidation = false;
            this.removeLocationButton.Location = new System.Drawing.Point(478, 317);
            this.removeLocationButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.removeLocationButton.Name = "removeLocationButton";
            this.removeLocationButton.Size = new System.Drawing.Size(87, 30);
            this.removeLocationButton.TabIndex = 22;
            this.removeLocationButton.Text = "Remove";
            this.removeLocationButton.UseVisualStyleBackColor = true;
            this.removeLocationButton.Click += new System.EventHandler(this.removeLocationButton_Click);
            // 
            // locationComboBox
            // 
            this.locationComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.locationComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.locationComboBox.DisplayMember = "DisplayName";
            this.locationComboBox.FormattingEnabled = true;
            this.locationComboBox.Location = new System.Drawing.Point(225, 396);
            this.locationComboBox.Name = "locationComboBox";
            this.locationComboBox.Size = new System.Drawing.Size(247, 25);
            this.locationComboBox.Sorted = true;
            this.locationComboBox.TabIndex = 23;
            this.locationComboBox.SelectedIndexChanged += new System.EventHandler(this.locationComboBox_SelectedIndexChanged);
            this.locationComboBox.Enter += new System.EventHandler(this.locationComboBox_Enter);
            // 
            // editLocationButton
            // 
            this.editLocationButton.CausesValidation = false;
            this.editLocationButton.Location = new System.Drawing.Point(478, 279);
            this.editLocationButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.editLocationButton.Name = "editLocationButton";
            this.editLocationButton.Size = new System.Drawing.Size(87, 30);
            this.editLocationButton.TabIndex = 24;
            this.editLocationButton.Text = "Edit";
            this.editLocationButton.UseVisualStyleBackColor = true;
            this.editLocationButton.Click += new System.EventHandler(this.editLocationButton_Click);
            // 
            // screensListBox
            // 
            this.screensListBox.DisplayMember = "ShortDescription";
            this.screensListBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.screensListBox.FormattingEnabled = true;
            this.screensListBox.ItemHeight = 65;
            this.screensListBox.Location = new System.Drawing.Point(19, 157);
            this.screensListBox.Name = "screensListBox";
            this.screensListBox.Size = new System.Drawing.Size(85, 264);
            this.screensListBox.TabIndex = 25;
            this.screensListBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.screensListBox_DrawItem);
            this.screensListBox.SelectedIndexChanged += new System.EventHandler(this.screensListBox_SelectedIndexChanged);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(577, 480);
            this.Controls.Add(this.screensListBox);
            this.Controls.Add(this.editLocationButton);
            this.Controls.Add(this.locationComboBox);
            this.Controls.Add(this.removeLocationButton);
            this.Controls.Add(this.addLocationButton);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.scaleTrackBar);
            this.Controls.Add(this.githubLinkLabel);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.selectedScreenNameLabel);
            this.Controls.Add(this.worldTimesListView);
            this.Controls.Add(this.displayWorldTimesRadioButton);
            this.Controls.Add(this.displayCurrentTimeRadioButton);
            this.Controls.Add(this.displayNothingRadioButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.Name = "SettingsForm";
            this.Text = "FlipIt Settings";
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scaleTrackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.RadioButton displayNothingRadioButton;
        private System.Windows.Forms.RadioButton displayCurrentTimeRadioButton;
        private System.Windows.Forms.RadioButton displayWorldTimesRadioButton;
        private System.Windows.Forms.ListView worldTimesListView;
        private System.Windows.Forms.ColumnHeader cityColumnHeader;
        private System.Windows.Forms.ImageList largeImageList;
        private System.Windows.Forms.RadioButton display24hrRadioButton;
        private System.Windows.Forms.RadioButton display12hrRadioButton;
        private System.Windows.Forms.Label selectedScreenNameLabel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.LinkLabel githubLinkLabel;
        private System.Windows.Forms.TrackBar scaleTrackBar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button addLocationButton;
        private System.Windows.Forms.Button removeLocationButton;
        private System.Windows.Forms.ComboBox locationComboBox;
        private System.Windows.Forms.Button editLocationButton;
        private System.Windows.Forms.ListBox screensListBox;
    }
}
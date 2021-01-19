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
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("Coming soon...");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.displayNothingRadioButton = new System.Windows.Forms.RadioButton();
            this.displayCurrentTimeRadioButton = new System.Windows.Forms.RadioButton();
            this.displayWorldTimesRadioButton = new System.Windows.Forms.RadioButton();
            this.worldTimesListView = new System.Windows.Forms.ListView();
            this.cityColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.mainListView = new System.Windows.Forms.ListView();
            this.largeImageList = new System.Windows.Forms.ImageList(this.components);
            this.display24hrRadioButton = new System.Windows.Forms.RadioButton();
            this.display12hrRadioButton = new System.Windows.Forms.RadioButton();
            this.selectedScreenNameLabel = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(12, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 30);
            this.label1.TabIndex = 1;
            this.label1.Text = "Flip It";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(309, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(212, 17);
            this.label2.TabIndex = 2;
            this.label2.Text = "https://github.com/phaselden/FlipIt";
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(175, 440);
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
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.cancelButton.Location = new System.Drawing.Point(270, 440);
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
            this.displayNothingRadioButton.Location = new System.Drawing.Point(139, 209);
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
            this.displayCurrentTimeRadioButton.Location = new System.Drawing.Point(139, 238);
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
            this.displayWorldTimesRadioButton.Location = new System.Drawing.Point(139, 267);
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
            this.worldTimesListView.HideSelection = false;
            this.worldTimesListView.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1});
            this.worldTimesListView.Location = new System.Drawing.Point(261, 267);
            this.worldTimesListView.Name = "worldTimesListView";
            this.worldTimesListView.Size = new System.Drawing.Size(260, 132);
            this.worldTimesListView.TabIndex = 11;
            this.worldTimesListView.UseCompatibleStateImageBehavior = false;
            this.worldTimesListView.View = System.Windows.Forms.View.Details;
            // 
            // cityColumnHeader
            // 
            this.cityColumnHeader.Text = "City";
            this.cityColumnHeader.Width = 256;
            // 
            // mainListView
            // 
            this.mainListView.HideSelection = false;
            this.mainListView.LargeImageList = this.largeImageList;
            this.mainListView.Location = new System.Drawing.Point(22, 171);
            this.mainListView.MultiSelect = false;
            this.mainListView.Name = "mainListView";
            this.mainListView.Size = new System.Drawing.Size(99, 228);
            this.mainListView.TabIndex = 12;
            this.mainListView.UseCompatibleStateImageBehavior = false;
            this.mainListView.SelectedIndexChanged += new System.EventHandler(this.mainListView_SelectedIndexChanged);
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
            this.display24hrRadioButton.Location = new System.Drawing.Point(114, 45);
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
            this.display12hrRadioButton.Location = new System.Drawing.Point(114, 18);
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
            this.selectedScreenNameLabel.Location = new System.Drawing.Point(136, 171);
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
            this.panel1.Location = new System.Drawing.Point(12, 61);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(249, 76);
            this.panel1.TabIndex = 15;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(84, 17);
            this.label3.TabIndex = 2;
            this.label3.Text = "Time display:";
            // 
            // SettingsForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(552, 483);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.selectedScreenNameLabel);
            this.Controls.Add(this.mainListView);
            this.Controls.Add(this.worldTimesListView);
            this.Controls.Add(this.displayWorldTimesRadioButton);
            this.Controls.Add(this.displayCurrentTimeRadioButton);
            this.Controls.Add(this.displayNothingRadioButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.Name = "SettingsForm";
            this.Text = "FlipIt Settings";
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.RadioButton displayNothingRadioButton;
        private System.Windows.Forms.RadioButton displayCurrentTimeRadioButton;
        private System.Windows.Forms.RadioButton displayWorldTimesRadioButton;
        private System.Windows.Forms.ListView worldTimesListView;
        private System.Windows.Forms.ColumnHeader cityColumnHeader;
        private System.Windows.Forms.ListView mainListView;
        private System.Windows.Forms.ImageList largeImageList;
        private System.Windows.Forms.RadioButton display24hrRadioButton;
        private System.Windows.Forms.RadioButton display12hrRadioButton;
        private System.Windows.Forms.Label selectedScreenNameLabel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label3;
    }
}
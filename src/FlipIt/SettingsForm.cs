/* Originally based on project by Frank McCown in 2010 */

using System;
using System.IO;
using System.Windows.Forms;

namespace ScreenSaver
{
	public partial class SettingsForm : Form
    {
        private readonly FlipItSettings _settings;

        public SettingsForm(FlipItSettings settings)
		{
			InitializeComponent();
            _settings = settings;
        }
		
        private void SaveSettings()
        {
            var settingsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FlipIt");
            if (!Directory.Exists(settingsFolder))
            {
                Directory.CreateDirectory(settingsFolder);
            }
            var iniFilePath = Path.Combine(settingsFolder, "Settings.ini");
            if (!File.Exists(iniFilePath))
            {
                File.Create(iniFilePath).Dispose();
            }

            var iniFile = new IniFile(iniFilePath);
            iniFile.WriteBool("General", "Display24Hr", _settings.Display24HrTime);
            foreach (var screenSetting in _settings.ScreenSettings)
            {
                var sectionName = $"Screen {screenSetting.DeviceName}";
                iniFile.WriteInt(sectionName, "DisplayType", (int) screenSetting.DisplayType);
            }
        }

		private void okButton_Click(object sender, EventArgs e)
		{
            SaveSettings();
			Close();
		}

		private void cancelButton_Click(object sender, EventArgs e)
		{
			Close();
		}

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            mainListView.Items.Clear();

            if (_settings.Display24HrTime)
            {
                display24hrRadioButton.Checked = true;
            }
            else
            {
                display12hrRadioButton.Checked = true;
            }
            
			foreach (var screen in _settings.ScreenSettings)
            {
                var item = new ListViewItem(screen.ShortDescription)
                {
                    ImageIndex = 0, 
                    Tag = screen
                };
                mainListView.Items.Add(item);
            }
            mainListView.Items[0].Selected = true;
        }

        private void DisplayScreenDetails()
        {
            if (mainListView.SelectedIndices.Count == 0)
                return;

            var screenSettings = GetCurrentScreenSettings();
            selectedScreenNameLabel.Text = $"{screenSettings.Description}";
            switch (screenSettings.DisplayType)
            {
                case DisplayType.None:
                    displayNothingRadioButton.Checked = true;
                    break;
                case DisplayType.CurrentTime:
                    displayCurrentTimeRadioButton.Checked = true;
                    break;
                case DisplayType.WorldTime:
                    displayWorldTimesRadioButton.Checked = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            CheckWorldTimesControls();
        }

        private ScreenSetting GetCurrentScreenSettings()
        {
            var selectedItem = mainListView.Items[mainListView.SelectedIndices[0]];
            return (ScreenSetting) selectedItem.Tag;
        }

        private void CheckWorldTimesControls()
        {
            worldTimesListView.Enabled = false;// displayWorldTimesRadioButton.Checked;
        }

        private void mainListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            DisplayScreenDetails();
        }

        private void displayWorldTimesRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            SetScreenDisplayType(DisplayType.WorldTime);
            CheckWorldTimesControls();
        }

        private void SetScreenDisplayType(DisplayType displayType)
        {
            var screen = GetCurrentScreenSettings();
            screen.DisplayType = displayType;
        }

        private void displayNothingRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            SetScreenDisplayType(DisplayType.None);
            CheckWorldTimesControls();
        }

        private void displayCurrentTimeRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            SetScreenDisplayType(DisplayType.CurrentTime);
            CheckWorldTimesControls();
        }

        private void display12hrRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            _settings.Display24HrTime = false;
        }

        private void display24hrRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            _settings.Display24HrTime = true;
        }

        private void githubLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/phaselden/FlipIt");
        }
    }
}

/* Originally based on project by Frank McCown in 2010 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting.Messaging;
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
            _settings.Scale = scaleTrackBar.Value * 10;
            _settings.Save();
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

        // TODO Need to find a way to allow an Esc to close the dropdown, but not then also close the dialog. In the short term, 
        // Esc never closes the dialog

        // protected override bool ProcessDialogKey(Keys keyData)
        // {
        //     if (Form.ModifierKeys == Keys.None && keyData == Keys.Escape)
        //     {
        //         this.Close();
        //         return true;
        //     }
        //     return base.ProcessDialogKey(keyData);
        // }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            if (_settings.Display24HrTime)
            {
                display24hrRadioButton.Checked = true;
            }
            else
            {
                display12hrRadioButton.Checked = true;
            }

            scaleTrackBar.Value = _settings.Scale / 10;

            mainListView.Items.Clear();
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
            worldTimesListView.Items.Clear();
            foreach (var location in screenSettings.Locations)
            {
                AddLocationToListView(location);
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
            var allowLocationEditing = displayWorldTimesRadioButton.Checked;
            worldTimesListView.Enabled = allowLocationEditing;
            locationComboBox.Enabled = allowLocationEditing;
            addLocationButton.Enabled = allowLocationEditing && locationComboBox.SelectedItem != null;
            removeLocationButton.Enabled = allowLocationEditing && worldTimesListView.SelectedIndices.Count > 0;
            editLocationButton.Enabled = removeLocationButton.Enabled;
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

        private void editLocationButton_Click(object sender, EventArgs e)
        {
            if (worldTimesListView.SelectedItems.Count > 0)
            {
                worldTimesListView.SelectedItems[0].BeginEdit();
            }
        }

        private void worldTimesListView_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            SyncLocationsFromListViewToSettings();
        }

        private void removeLocationButton_Click(object sender, EventArgs e)
        {
            // We use foreach, but there will only be one selected at most
            foreach (ListViewItem item in worldTimesListView.SelectedItems)
            {
                item.Remove();
                SyncLocationsFromListViewToSettings();
            }
        }

        private void addLocationButton_Click(object sender, EventArgs e)
        {
            var location = (Location) locationComboBox.SelectedItem;
            if (location == null)
                return;

            AddLocationToListView(location);
            SyncLocationsFromListViewToSettings();
        }

        private void AddLocationToListView(Location simpleLocation)
        {
            AddLocationToListView(simpleLocation.DisplayName, simpleLocation.TimeZoneID);
        }

        private void AddLocationToListView(string location, string timeZoneID)
        {
            var listViewItem = new ListViewItem(location);
            listViewItem.Tag = timeZoneID;
            worldTimesListView.Items.Add(listViewItem);
        }

        private void locationComboBox_Enter(object sender, EventArgs e)
        {
            if (locationComboBox.Items.Count == 0)
            {
                try
                {
                    Cursor = Cursors.WaitCursor;
                    PopulateLocationsCombo();
                }
                finally
                {
                    Cursor = Cursors.Default;
                }
            }
        }

        private void PopulateLocationsCombo()
        {
            locationComboBox.Sorted = false;
            locationComboBox.Items.Clear();
            
            var timeZones = TimeZoneInfo.GetSystemTimeZones();

            foreach (var timeZone in timeZones)
            {
                var locationNames = ParseLocationNames(timeZone.DisplayName);
                foreach (var locationName in locationNames)
                {
                    var city = new Location(timeZone.Id, locationName.Trim());
                    locationComboBox.Items.Add(city);
                }
            }

            locationComboBox.Sorted = true;
        }

        private IEnumerable<string> ParseLocationNames(string timeZoneDisplayName)
        {
            var index = timeZoneDisplayName.IndexOf(')');
            if (index > 0)
            {
                timeZoneDisplayName = timeZoneDisplayName.Remove(0, index + 1).Trim();
            }
            // Handle exclusions eg UTC variants
            if (timeZoneDisplayName.StartsWith("Coordinated Universal Time-") || timeZoneDisplayName.StartsWith("Coordinated Universal Time+"))
            {
                return new string[] { };
            }

            // Handle manual expansions and explicit inclusions. e.g. Add some US cities, add UTC?
            switch (timeZoneDisplayName)
            {
                case "Coordinated Universal Time":
                    return new[] { timeZoneDisplayName, "UTC" };
                case "Pacific Time (US & Canada)":
                    return new[] { timeZoneDisplayName, "Los Angeles", "San Francisco", "Seattle", "Vancouver" };
                case "Mountain Time (US & Canada)":
                    return new[] { timeZoneDisplayName, "Denver" };
                case "Central Time (US & Canada)":
                    return new[] { timeZoneDisplayName, "Chicago", "Dallas" };
                case "Eastern Time (US & Canada)":
                    return new[] { timeZoneDisplayName, "New York", "Washington", "Toronto", "Quebec", "Montreal" };
            }

            return timeZoneDisplayName.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        }

        private void worldTimesListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckWorldTimesControls();
        }

        private void locationComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckWorldTimesControls();
        }

        private void SyncLocationsFromListViewToSettings()
        {
            var screen = GetCurrentScreenSettings();
            screen.Locations.Clear();
            foreach (ListViewItem item in worldTimesListView.Items)
            {
                screen.Locations.Add(new Location((string)item.Tag, item.Text));
            }
        }
    }
}

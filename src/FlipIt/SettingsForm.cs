/* Originally based on project by Frank McCown in 2010 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using ScreenSaver.Properties;

namespace ScreenSaver
{
	public partial class SettingsForm : Form
    {
        private readonly FlipItSettings _settings;
        private readonly List<Location> _availableCities = new List<Location>();
        private readonly AutoCompleteStringCollection _cityAutoCompleteSource = new AutoCompleteStringCollection();

        public SettingsForm(FlipItSettings settings)
		{
			InitializeComponent();
            _settings = settings;

            versionLabel.Text = $"Version {GetVersion()}";
        }

        private string GetVersion()
        {
            var version = typeof(SettingsForm).Assembly.GetName().Version;
            return $"{version.Major}.{version.Minor}.{version.Build}";
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

            screensListBox.Items.Clear();
            foreach (var screen in _settings.ScreenSettings)
            {
                screensListBox.Items.Add(screen);
            }
            screensListBox.SelectedIndex = 0;
        }

        private void DisplayScreenDetails()
        {
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
            return (ScreenSetting) screensListBox.SelectedItem;
        }

        private void CheckWorldTimesControls()
        {
            var allowLocationEditing = displayWorldTimesRadioButton.Checked;
            worldTimesListView.Enabled = allowLocationEditing;
            citySearchTextBox.Enabled = allowLocationEditing;
            addLocationButton.Enabled = allowLocationEditing && IsValidCity(citySearchTextBox.Text);
            removeLocationButton.Enabled = allowLocationEditing && worldTimesListView.SelectedIndices.Count > 0;
            editLocationButton.Enabled = removeLocationButton.Enabled;
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
            AddCityFromSearchBox();
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

        private void FillCityList()
        {
            if (_availableCities.Count == 0)
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
            _cityAutoCompleteSource.Clear();
            _availableCities.Clear();

            // LoadLocationsFromWindows();
            LoadLocationsFromFile();

            citySearchTextBox.AutoCompleteCustomSource = _cityAutoCompleteSource;
        }

        private void LoadLocationsFromFile()
        {
            var txt = Resources.TimeZoneCities;
            var lines = txt.Split(new[] {"\r\n"}, StringSplitOptions.None);
            foreach(var rawLine in lines)
            {
                var line = rawLine.Trim();
                if (String.IsNullOrEmpty(line) || line.StartsWith(";")) 
                    continue;
                var parts= line.Split('=');
                Debug.Assert(parts.Length == 2);
                if (parts.Length != 2) // Something strange happened. Just ignore it.
                    continue;
                var cities = parts[1].Split(',');
                foreach (var city in cities)
                {
                    // Exclude some names
                    if (city == "PST8PDT")
                        continue;
                    if (city.StartsWith("GMT") && city.Length > 3)
                        continue;

                    _availableCities.Add(new Location(parts[0], city));
                    _cityAutoCompleteSource.Add(city);
                }
            }
        }

        /*private void LoadLocationsFromWindows()
        {
            var timeZones = TimeZoneInfo.GetSystemTimeZones();

            foreach (var timeZone in timeZones)
            {
                var locationNames = ParseLocationNames(timeZone.DisplayName);
                foreach (var locationName in locationNames)
                {
                    var city = new Location(timeZone.Id, locationName.Trim());
                }
            }
        }*/

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

        private void SyncLocationsFromListViewToSettings()
        {
            // Note: We don't have to worry about any unsaved cities being in the list if the form
            // is redisplayed, because the app exists after the form closes.

            var screen = GetCurrentScreenSettings();
            screen.Locations.Clear();
            foreach (ListViewItem item in worldTimesListView.Items)
            {
                screen.Locations.Add(new Location((string)item.Tag, item.Text));
            }
        }

        private void screensListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index == -1)
                return;
            
            e.DrawBackground();
            var rect = new Rectangle(e.Bounds.X + 20, e.Bounds.Y + 4, 32, 32);
            e.Graphics.DrawImageUnscaled(Properties.Resources.screen, rect);
            
            var screen = (ScreenSetting) screensListBox.Items[e.Index];
            var isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;//screensListBox.SelectedIndex == e.Index;
            e.Graphics.DrawString(screen.ShortDescription, 
                e.Font, isSelected ? Brushes.White : Brushes.Black, 
                new Rectangle(e.Bounds.X + 10, e.Bounds.Y + 40, e.Bounds.Width, e.Bounds.Height),
                StringFormat.GenericDefault);
                
            // If the ListBox has focus, draw a focus rectangle around the selected item.
            e.DrawFocusRectangle();
        }

        private void screensListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            DisplayScreenDetails();
        }

        private void citySearchTextBox_Enter(object sender, EventArgs e)
        {
            FillCityList();
        }

        private void AddCityFromSearchBox()
        {
            var citySearch = citySearchTextBox.Text.Trim();
            var location = _availableCities.SingleOrDefault(s => s.DisplayName.HasSameText(citySearch));
            if (location != null)
            {
                if (WorldTimesListViewContainsCity(location.DisplayName))
                {
                    MessageBox.Show($"'{location.DisplayName}' is already in the list", "Duplicate City", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                AddLocationToListView(location);
                SyncLocationsFromListViewToSettings(); 
            }
        }

        private void citySearchTextBox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                AddCityFromSearchBox();
            }
        }

        private bool WorldTimesListViewContainsCity(string cityName)
        {
            foreach (ListViewItem item in worldTimesListView.Items)
            {
                if (item.Text.HasSameText(cityName))
                    return true;
            }
            return false;
        }

        private void citySearchTextBox_TextChanged(object sender, EventArgs e)
        {
            addLocationButton.Enabled = IsValidCity(citySearchTextBox.Text);
        }

        private bool IsValidCity(string cityName)
        {
            return cityName.Length > 0 && _availableCities.Exists(c => c.DisplayName.HasSameText(cityName));
        }
    }
}

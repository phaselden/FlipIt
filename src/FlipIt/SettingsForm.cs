/* Originally based on project by Frank McCown in 2010 */

using System;
using System.Windows.Forms;

namespace ScreenSaver
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
//            LoadSettings();
        }

        /// <summary>
        /// Load display text from the Registry
        /// </summary>
//        private void LoadSettings()
//        {
//            RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Demo_ScreenSaver");
//            if (key == null)
//                textBox.Text = "C# Screen Saver";
//            else
//                textBox.Text = (string)key.GetValue("text");
//        }

        /// <summary>
        /// Save text into the Registry.
        /// </summary>
//        private void SaveSettings()
//        {
//            // Create or get existing subkey
//            RegistryKey key = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Demo_ScreenSaver");
//
//            key.SetValue("text", textBox.Text);
//        }

        private void okButton_Click(object sender, EventArgs e)
        {
//            SaveSettings();
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}

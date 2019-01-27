using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace BingWallpaper
{
    public partial class FormOptions : Form
    {
        private static Dictionary<string, string> Locales = new Dictionary<string, string>
        {
            { "es-AR", "Argentina" },
            { "en-AU", "Australia" },
            { "de-AT", "Austria" },
            { "nl-BE", "Belgium - Dutch" },
            { "fr-BE", "Belgium - French" },
            { "pt-BR", "Brazil" },
            { "en-CA", "Canada - English" },
            { "fr-CA", "Canada - French" },
            { "es-CL", "Chile" },
            { "zh-CN", "China" },
            { "da-DK", "Denmark" },
            { "ar-EG", "Egypt" },
            { "fi-FI", "Finland" },
            { "fr-FR", "France" },
            { "de-DE", "Germany" },
            { "zh-HK", "Hong Kong SAR" },
            { "en-IN", "India" },
            { "en-ID", "Indonesia" },
            { "en-IE", "Ireland" },
            { "it-IT", "Italy" },
            { "ja-JP", "Japan" },
            { "ko-KR", "Korea" },
            { "en-MY", "ia" },
            { "es-MX", "Mexico" },
            { "nl-NL", "Netherlands" },
            { "en-NZ", "New Zealand" },
            { "nb-NO", "Norway" },
            { "en-PH", "Philippines" },
            { "pl-PL", "Poland" },
            { "pt-PT", "Portugal" },
            { "ru-RU", "Russia" },
            { "ar-SA", "Saudi Arabia" },
            { "en-SG", "Singapore" },
            { "en-ZA", "South Africa" },
            { "es-ES", "Spain" },
            { "sv-SE", "Sweden" },
            { "fr-CH", "Switzerland - French" },
            { "de-CH", "Switzerland - German" },
            { "zh-TW", "Taiwan" },
            { "tr-TR", "Turkey" },
            { "ar-AE", "United Arab Emirates" },
            { "en-GB", "United Kingdom" },
            { "en-US", "United States - English (Default)" },
            { "es-US", "United States - Spanish" }
        };

        private static readonly string StartupKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
        private static readonly string StartupValue = "BingWallpaper";

        public FormOptions()
        {
            InitializeComponent();
        }

        private void FormOptions_Load(object sender, EventArgs e)
        {
            comboLocale.DataSource = new BindingSource(Locales, null); ;
            comboLocale.DisplayMember = "Value";
            comboLocale.ValueMember = "Key";
            comboLocale.SelectedValue = Properties.Settings.Default.Locale;

            checkBoxRunAtStartup.Checked = Properties.Settings.Default.RunAtStartup;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            var subkey = Registry.CurrentUser.OpenSubKey(StartupKey, true);

            if(checkBoxRunAtStartup.Checked == false && Properties.Settings.Default.RunAtStartup == true)
                subkey.DeleteValue(StartupValue); 
            else if(checkBoxRunAtStartup.Checked == true && Properties.Settings.Default.RunAtStartup == false)
                subkey.SetValue(StartupValue, Application.ExecutablePath.ToString());

            subkey.Close();
            Properties.Settings.Default.Locale = comboLocale.SelectedValue.ToString();
            Properties.Settings.Default.RunAtStartup = checkBoxRunAtStartup.Checked;
            Properties.Settings.Default.Save();
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}

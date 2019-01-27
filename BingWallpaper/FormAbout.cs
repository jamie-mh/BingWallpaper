using System;
using System.Reflection;
using System.Windows.Forms;

namespace BingWallpaper
{
    public partial class FormAbout : Form
    {
        public FormAbout()
        {
            InitializeComponent();
        }

        private void FormAbout_Load(object sender, EventArgs e)
        {
            labelVersion.Text = $"{Application.ProductName} {Assembly.GetAssembly(typeof(Program)).GetName().Version}";
        }

        private void linkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/jamie-mh/BingWallpaper");
            Close();
        }
    }
}

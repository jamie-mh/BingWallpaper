using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BingWallpaper
{
    public partial class FormMain : Form
    {
        private BingClient _bingClient;

        private bool _exiting;
        private DateTime _lastOpen;
        private int _currentImage = 0;

        public FormMain()
        {
            InitializeComponent();

            _bingClient = new BingClient();
            _exiting = false;
            _lastOpen = DateTime.Now;
        }

        private void FormMain_Shown(object sender, EventArgs e)
        {
            SetFormLocation();
            AttemptUpdate();
           
            if(_bingClient.Images.Count > 0)
                LoadPreview();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(_exiting) return;

            e.Cancel = true;
            Visible = false;
        }

        private void AttemptUpdate()
        {
            try
            {
                _bingClient.Update();
            }
            catch
            {
                ShowNetworkError(AttemptUpdate, () => {
                    Visible = false;
                });
            }
        }

        private void SetFormLocation()
        {
            var screen = Screen.FromControl(this);
            Location = new Point(screen.WorkingArea.Width - Width, screen.WorkingArea.Height - Height);
        }

        private void ShowNetworkError(Action retry, Action cancel)
        {
            var result = MessageBox.Show(
                "Unable to fetch wallpaper data, please make sure you are connected a network.", 
                "Error", 
                MessageBoxButtons.RetryCancel, 
                MessageBoxIcon.Error
             );

            if(result == DialogResult.Retry)
                retry.Invoke();
            else
                cancel.Invoke();
        }

        private async Task SetWallpaper(int index, bool silent = false)
        {
            var image = _bingClient.Images[index];
            var path = Path.Combine(Path.GetTempPath(), "wallpaper.jpg");

            try
            {
                await _bingClient.DownloadImage(image, path);
            }
            catch
            {
                if(!silent)
                    ShowNetworkError(() => buttonSetWallpaper_Click(), () => {
                        buttonSetWallpaper.Enabled = true;
                    });

                return;
            }

            Wallpaper.SetStyle(Wallpaper.Style.Fill);
            Wallpaper.SetPath(path);
        }

        private void LoadPreview()
        {
            var image = _bingClient.Images[_currentImage];
            Text = $"Bing Wallpaper {_currentImage + 1} / {_bingClient.Images.Count}";

            progressBar.Value = 0;
            progressBar.Visible = true;
            pictureBox.LoadAsync(BingClient.Url + image.Url);

            labelTitle.Text = image.Title;
            labelCopyright.Text = image.Copyright;

            buttonPrevious.Enabled = _currentImage != 0;
            buttonNext.Enabled = _currentImage != (_bingClient.Images.Count - 1);
        }

        private void toolStripMenuItemExit_Click(object sender, EventArgs e)
        {
            _exiting = true;
            Application.Exit();
        }

        private void toolStripMenuItemShow_Click(object sender, EventArgs e)
        {
            Visible = true;
            SetFormLocation();
            AttemptUpdate();
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            _currentImage++;
            LoadPreview();
        }

        private void buttonPrevious_Click(object sender, EventArgs e)
        {
            _currentImage--;
            LoadPreview();
        }

        private async void buttonSetWallpaper_Click(object sender= null, EventArgs e = null)
        {
            buttonSetWallpaper.Enabled = false;
            await SetWallpaper(_currentImage);
            buttonSetWallpaper.Enabled = true;
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new FormOptions().ShowDialog();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new FormAbout().ShowDialog();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            var previousHash = _bingClient.Images[0].Hash;

            // Attempt to update in the background
            try
            {
                _bingClient.Update();
            } 
            catch { }

            if(Properties.Settings.Default.AutomaticallySetNewWallpaper &&
               _bingClient.Images[0].Hash != previousHash)
            {
                SetWallpaper(0, true);
            }
        }

        private void pictureBox_LoadProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }

        private void pictureBox_LoadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            progressBar.Visible = false;
        }
    }
}

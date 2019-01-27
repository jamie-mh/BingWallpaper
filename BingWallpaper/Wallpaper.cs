using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace BingWallpaper
{
    internal static class Wallpaper
    {
        private const int SPI_SETDESKWALLPAPER = 20;
        private const int SPIF_UPDATEINIFILE = 0x01;
        private const int SPIF_SENDWININICHANGE = 0x02;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        public enum Style
        {
            Centered = 0,
            Tiled = 1,
            Stretched = 2,
            Fit = 6,
            Fill = 10
        };

        public static void SetStyle(Style style)
        {
            var key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);

            if(style == Style.Tiled)
            {
                key.SetValue(@"WallpaperStyle", Style.Centered);
                key.SetValue(@"TileWallpaper", 1);
            }
            else
            {
                key.SetValue(@"WallpaperStyle", style);
                key.SetValue(@"TileWallpaper", 0);
            }

            key.Close();
        }

        public static void SetPath(string path)
        {
            SystemParametersInfo(
                SPI_SETDESKWALLPAPER,
                0,
                path,
                SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE
            );
        }
    }
}

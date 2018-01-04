using BingWallpaper.APITypes;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BingWallpaper
{
    class Program
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        const int SPI_SETDESKWALLPAPER = 20;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDWININICHANGE = 0x02;

        static void Main(string[] args)
        {
            // Get the archive object
            HPImageArchive archive = MakeApiRequest()
                                     .GetAwaiter()
                                     .GetResult();
            if (archive == null) return;

            // Download the image and save it in the temp folder
            string imagePath = DownloadBitmap("https://www.bing.com" + archive.Images[0].URL)
                               .GetAwaiter()
                               .GetResult();
            if (imagePath == null) return;

            // Set the new wallpaper
            SetCenteredWallpaper(imagePath);
        }

        private static async Task<HPImageArchive> MakeApiRequest()
        {
            // Make a request to the Bing api, fetch the URL of the wallpaper
            using (HttpClient http = new HttpClient())
            {
                Uri apiURL = new Uri(ConfigurationManager.AppSettings["apiURL"]);
                HttpResponseMessage response = await http.GetAsync(apiURL);

                if(response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    // Parse the JSON
                    return JsonConvert.DeserializeObject<HPImageArchive>(content);
                }
            }
            return null;
        }

        private static async Task<string> DownloadBitmap(string url)
        {
            using(HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                
                if(response.IsSuccessStatusCode)
                {
                    // Create an image from the stream
                    Stream fileStream = await response.Content.ReadAsStreamAsync();
                    Image image = Image.FromStream(fileStream);

                    // Save to the temporary directory
                    string savePath = Path.Combine(Path.GetTempPath(), "image.bmp");
                    image.Save(savePath, ImageFormat.Bmp);

                    return savePath;
                }
            }
            return null;
        }

        private static void SetCenteredWallpaper(string path)
        {
            // Use the registry to set the wallpaper to centered
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
            key.SetValue(@"WallpaperStyle", "1");
            key.SetValue(@"TileWallpaper", "0");

            // Set the wallpaper
            SystemParametersInfo(SPI_SETDESKWALLPAPER,
                                 0,
                                 path,
                                 SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }
    }
}

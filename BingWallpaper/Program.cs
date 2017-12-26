using BingWallpaper.APITypes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BingWallpaper
{
    class Program
    {
        static void Main(string[] args)
        {
            // Get the archive
            HPImageArchive archive = MakeApiRequest().GetAwaiter().GetResult();

            // Set the wallpaper
            Wallpaper.Set(new Uri("https://www.bing.com/" + archive.Images[0].URL), 
                          Wallpaper.Style.Centered);
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
                else
                {
                    return new HPImageArchive();
                }
            }
        }
    }
}

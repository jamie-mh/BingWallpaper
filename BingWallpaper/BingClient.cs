using RestSharp;
using RestSharp.Deserializers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace BingWallpaper
{
    internal class BingClient
    {
        public const string Url = "https://www.bing.com";

        public List<HPImageArchiveImage> Images { get; private set; }

        private RestClient _restClient;
        private HttpClient _httpClient;

        public BingClient()
        {
            Images = new List<HPImageArchiveImage>();
            _restClient = new RestClient(Url);
            _httpClient = new HttpClient();
        }

        public void Update()
        {
            var request = new RestRequest("HPImageArchive.aspx", Method.GET);
            request.AddParameter("format", "js");
            request.AddParameter("mkt", Properties.Settings.Default.Locale);
            request.AddParameter("n", 8);

            var response = _restClient.Execute<HPImageArchive>(request);

            if(response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception("Invalid Response");
            }

            Images = response.Data.Images;
        }

        public async Task DownloadImage(HPImageArchiveImage image, string downloadPath)
        {
            var response = await _httpClient.GetAsync(Url + image.Url);
            var stream = await response.Content.ReadAsStreamAsync();
            var result = Image.FromStream(stream);

            result.Save(downloadPath, ImageFormat.Jpeg);
        }
    }

    internal class HPImageArchive
    {
        [DeserializeAs(Name = "images")]
        public List<HPImageArchiveImage> Images { get; set; }
    }

    internal class HPImageArchiveImage
    {
        [DeserializeAs(Name = "url")]
        public string Url { get; set; }

        [DeserializeAs(Name = "title")]
        public string Title { get; set; }

        [DeserializeAs(Name = "copyright")]
        public string Copyright { get; set; }

        [DeserializeAs(Name = "hsh")]
        public string Hash { get; set; }
    }
}

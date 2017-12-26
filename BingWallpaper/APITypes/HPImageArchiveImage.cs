using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BingWallpaper.APITypes
{
    public class HPImageArchiveImage
    {
        [JsonProperty("url")]
        public string URL { get; set; }
    }
}

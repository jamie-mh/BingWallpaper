using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BingWallpaper.APITypes
{
    public class HPImageArchive
    {
        [JsonProperty("images")]
        public List<HPImageArchiveImage> Images { get; set; }

    }
}

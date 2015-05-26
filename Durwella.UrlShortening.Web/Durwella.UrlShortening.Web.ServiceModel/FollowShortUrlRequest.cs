using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durwella.UrlShortening.Web.ServiceModel
{
    [FallbackRoute("/{Key}")]
    public class FollowShortUrlRequest
    {
        public string Key { get; set; }
    }
}

using Durwella.UrlShortening.Web.ServiceModel;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durwella.UrlShortening.Web.ServiceInterface
{
    public class UrlShorteningService : Service
    {
        public UrlShortener UrlShortener { get; set; }

        public ShortUrlResponse Post(ShortUrlRequest shortUrlRequest)
        {
            var shortened = UrlShortener.Shorten(shortUrlRequest.Url);
            return new ShortUrlResponse(shortened);
        }
    }
}

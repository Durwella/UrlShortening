using ServiceStack;

namespace Durwella.UrlShortening.Web.ServiceModel
{
    [Route("/shorten/{Url}")]
    public class ShortUrlRequest
    {
        public string Url { get; set; }
    }
}

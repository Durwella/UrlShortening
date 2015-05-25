using ServiceStack;

namespace Durwella.UrlShortening.Web.ServiceModel
{
    [Route("/shorten")]
    public class ShortUrlRequest
    {
        public string Url { get; set; }
    }
}

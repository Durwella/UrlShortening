using ServiceStack;

namespace Durwella.UrlShortening.Web.ServiceModel
{
    [Route("/shorten", "POST", Summary = "Create a short URL that will redirect to the given URL.", Notes = "The given URL should be well-formatted and live/reachable.")]
    public class ShortUrlRequest
    {
        public string Url { get; set; }
    }
}

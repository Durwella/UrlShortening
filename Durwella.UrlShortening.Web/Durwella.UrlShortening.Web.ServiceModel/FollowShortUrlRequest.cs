using ServiceStack;

namespace Durwella.UrlShortening.Web.ServiceModel
{
    [FallbackRoute("/{Key}", "GET", Summary = "Returns the HTTP Redirect for the given key")]
    public class FollowShortUrlRequest
    {
        public string Key { get; set; }
    }
}

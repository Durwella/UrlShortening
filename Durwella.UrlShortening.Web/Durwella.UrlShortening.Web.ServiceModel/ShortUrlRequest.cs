using ServiceStack;
using ServiceStack.DataAnnotations;

namespace Durwella.UrlShortening.Web.ServiceModel
{
    [Api("URL Shortening")]
    [Route("/shorten", "POST", Summary = "Create a short URL that will redirect to the given URL.", Notes = "The given URL should be well-formatted and live or reachable.")]
    public class ShortUrlRequest : IReturn<ShortUrlResponse>
    {
        [Required]
        [ApiMember(IsRequired = true,
            Description = "The destination for the new short URL.")]
        public string Url { get; set; }

        [ApiMember(IsRequired = false, 
            Description = "If provided, creates short URL with given custom path. Throws error if path is already used.")]
        public string CustomPath { get; set; }
    }
}

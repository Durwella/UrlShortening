
namespace Durwella.UrlShortening.Web.ServiceModel
{
    public class ShortUrlResponse
    {
        public string Shortened { get; set; }

        public ShortUrlResponse()
        {
        }

        public ShortUrlResponse(string shortened)
        {
            Shortened = shortened;
        }
    }
}

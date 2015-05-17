using System.Net;

namespace Durwella.UrlShortening
{
    public class WebClientUrlUnwrapper : IUrlUnwrapper
    {
        public string GetDirectUrl(string url)
        {
            var request = WebRequest.Create(url);
            using (var response = request.GetResponse())
                return response.ResponseUri.ToString();
        }
    }
}

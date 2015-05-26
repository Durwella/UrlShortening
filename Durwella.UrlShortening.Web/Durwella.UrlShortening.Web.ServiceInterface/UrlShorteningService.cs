using Durwella.UrlShortening.Web.ServiceModel;
using ServiceStack;
using ServiceStack.Configuration;
using System;

namespace Durwella.UrlShortening.Web.ServiceInterface
{
    public class UrlShorteningService : Service
    {
        public IResolver Resolver { get; set; }

        // Can't have a persistent UrlShortener with current architecture because can't get absolute uri until have a request
        //public UrlShortener UrlShortener { get; set; }

        public ShortUrlResponse Post(ShortUrlRequest shortUrlRequest)
        {
            var aliasRepository = Resolver.TryResolve<IAliasRepository>() ?? new MemoryAliasRepository();
            var hashScheme = Resolver.TryResolve<IHashScheme>() ?? new DefaultHashScheme();
            var urlUnwrapper = Resolver.TryResolve<IUrlUnwrapper>() ?? new WebClientUrlUnwrapper();
            var uri = new Uri(Request.AbsoluteUri);
            var baseUri = uri.GetLeftPart(UriPartial.Authority);
            var urlShortener = new UrlShortener(baseUri, aliasRepository, hashScheme, urlUnwrapper);
            var shortened = urlShortener.Shorten(shortUrlRequest.Url);
            return new ShortUrlResponse(shortened);
        }

        public ShortUrlResponse Get(ShortUrlRequest shortUrlRequest)
        {
            return Post(shortUrlRequest);
        }
    }
}

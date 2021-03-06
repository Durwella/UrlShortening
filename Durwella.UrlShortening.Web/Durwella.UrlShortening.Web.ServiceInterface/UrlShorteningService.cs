﻿using Durwella.UrlShortening.Web.ServiceModel;
using ServiceStack;
using ServiceStack.Configuration;
using System;
using System.Net;

namespace Durwella.UrlShortening.Web.ServiceInterface
{
    public class UrlShorteningService : Service
    {
        public IResolver Resolver { get; set; }
        public IAliasRepository AliasRepository { get; set; }
        public IProtectedPathList ProtectedPathList { get; set; }

        // Can't have a persistent UrlShortener with current architecture because can't get absolute uri until have a request
        //public UrlShortener UrlShortener { get; set; }

        [Authenticate]
        public ShortUrlResponse Post(ShortUrlRequest shortUrlRequest)
        {
            return String.IsNullOrWhiteSpace(shortUrlRequest.CustomPath) ?
                MakeShortUrlResponse(shortener => shortener.Shorten(shortUrlRequest.Url)) : 
                MakeShortUrlResponse(shortener => shortener.ShortenWithCustomHash(shortUrlRequest.Url, shortUrlRequest.CustomPath));
        }

        public object Get(FollowShortUrlRequest request)
        {
            if (!AliasRepository.ContainsKey(request.Key))
                throw HttpError.NotFound("Short URL '{0}' does not exist".Fmt(request.Key));
            var destination = AliasRepository.GetValue(request.Key);
            return new HttpResult { StatusCode = HttpStatusCode.Redirect, Headers = { { HttpHeaders.Location, destination } } };
        }

        private ShortUrlResponse MakeShortUrlResponse(Func<UrlShortener, string> shorten)
        {
            var hashScheme = Resolver.TryResolve<IHashScheme>() ?? Default.HashScheme();
            var urlUnwrapper = Resolver.TryResolve<IUrlUnwrapper>() ?? new WebClientUrlUnwrapper();
            var uri = new Uri(Request.AbsoluteUri);
            var baseUri = uri.GetLeftPart(UriPartial.Authority);
            var urlShortener = new UrlShortener(baseUri, AliasRepository, hashScheme, urlUnwrapper)
            {
                ProtectedPaths = ProtectedPathList.ProtectedPaths
            };
            var shortened = shorten(urlShortener);
            return new ShortUrlResponse(shortened);
        }
    }
}

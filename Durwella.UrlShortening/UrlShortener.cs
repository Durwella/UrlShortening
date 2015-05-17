using System;

namespace Durwella.UrlShortening
{
    public class UrlShortener
    {
        public IHashScheme HashScheme { get; private set; }
        public IAliasRepository Repository { get; private set; }
        public string BaseUrl { get; private set; }

        public UrlShortener()
            : this( new DefaultHashScheme(), new MemoryAliasRepository(), "http://example.com")
        {
        }

        public UrlShortener(IHashScheme hashScheme, IAliasRepository repository, string baseUrl)
        {
            HashScheme = hashScheme;
            Repository = repository;
            BaseUrl = baseUrl;
        }

        public string Shorten(string url)
        {
            var key = HashScheme.GetKey(url);
            Repository.Add(key, url);
            var baseUri = new Uri(BaseUrl);
            var newUri = new Uri(baseUri, key);
            return newUri.ToString();
        }
    }
}

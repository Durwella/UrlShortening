using System;

namespace Durwella.UrlShortening
{
    public class UrlShortener
    {
        public IHashScheme HashScheme { get; private set; }
        public IAliasRepository Repository { get; private set; }
        public IUrlUnwrapper UrlUnwrapper { get; private set; }
        public string BaseUrl { get; private set; }
        public const int MaximumHashAttempts = 500;

        public UrlShortener()
            : this("http://example.com")
        {
        }

        public UrlShortener(string baseUrl)
            : this(baseUrl, new MemoryAliasRepository(), new DefaultHashScheme(), new WebClientUrlUnwrapper())
        {
        }

        public UrlShortener(string baseUrl, IAliasRepository repository, IHashScheme hashScheme)
            : this(baseUrl, repository, hashScheme, new WebClientUrlUnwrapper())
        {
        }

        public UrlShortener(string baseUrl, IAliasRepository repository, IHashScheme hashScheme, IUrlUnwrapper urlUnwrapper)
        {
            BaseUrl = baseUrl;
            Repository = repository;
            HashScheme = hashScheme;
            UrlUnwrapper = urlUnwrapper;
        }

        public string Shorten(string url)
        {
            var directUrl = UrlUnwrapper.GetDirectUrl(url);
            return ShortenDirect(directUrl);
        }

        public string ShortenWithCustomHash(string url, string customHash)
        {
            var directUrl = UrlUnwrapper.GetDirectUrl(url);
            if (Repository.ContainsKey(customHash))
                throw new ArgumentException("The given custom short URL is already in use.");
            var oldKey = Repository.GetKey(directUrl);
            Repository.Remove(oldKey);
            Repository.Add(customHash, directUrl);
            return CompleteUrl(customHash);
        }

        private string ShortenDirect(string url)
        {
            string key;
            if (Repository.ContainsValue(url))
                key = Repository.GetKey(url);
            else
            {
                key = HashScheme.GetKey(url);
                int permutation = 0;
                while (Repository.ContainsKey(key))
                {
                    if (permutation == MaximumHashAttempts)
                        throw new Exception(String.Format("Failed to find a unique hash for url <{0}> after {1} attempts", url, permutation));
                    key = HashScheme.GetKey(url, ++permutation);
                }
                Repository.Add(key, url);
            }
            return CompleteUrl(key);
        }

        private string CompleteUrl(string key)
        {
            var baseUri = new Uri(BaseUrl);
            var newUri = new Uri(baseUri, key);
            return newUri.ToString();
        }
    }
}

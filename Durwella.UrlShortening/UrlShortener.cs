using System;

namespace Durwella.UrlShortening
{
    public class UrlShortener
    {
        public IHashScheme HashScheme { get; private set; }
        public IAliasRepository Repository { get; private set; }
        public string BaseUrl { get; private set; }
        public const int MaximumHashAttempts = 500;

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
            var baseUri = new Uri(BaseUrl);
            var newUri = new Uri(baseUri, key);
            return newUri.ToString();
        }
    }
}

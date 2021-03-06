﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Durwella.UrlShortening
{
    public class UrlShortener
    {
        public IHashScheme HashScheme { get; private set; }
        public IAliasRepository Repository { get; private set; }
        public IUrlUnwrapper UrlUnwrapper { get; private set; }
        public string BaseUrl { get; private set; }
        public IList<string> ProtectedPaths { get; set; }

        public const int MaximumHashAttempts = 500;

        public UrlShortener()
            : this("http://example.com")
        {
        }

        public UrlShortener(string baseUrl)
            : this(baseUrl, new MemoryAliasRepository(), Default.HashScheme(), new WebClientUrlUnwrapper())
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
            ProtectedPaths = new string[]{};
        }

        public string Shorten(string url)
        {
            var directUrl = UrlUnwrapper.GetDirectUrl(url);
            return ShortenDirect(directUrl);
        }

        public string ShortenWithCustomHash(string url, string customHash)
        {
            if (String.IsNullOrWhiteSpace(customHash))
                throw new ArgumentException("The custom short URL cannot be empty.");
            if (customHash.Length > 100)
                throw new ArgumentException("The custom short URL must be no longer than 100 characters.");
            var unreserved = new Regex(@"[a-z]|[A-Z]|[0-9]|\-|\.|_|\~");
            if (customHash.EndsWith(".") || !customHash.All(c => unreserved.IsMatch(c.ToString()) ))
                throw new ArgumentException(
                    "The custom short URL must only contain letters A ... Z, numbers 0 ... 9 or " + 
                    "dash (-), underscore (_), dot(.), or tilde (~)");
            if (NotAllowed.Contains(customHash))
                throw new ArgumentException("That custom short URL is not available.");
            var directUrl = UrlUnwrapper.GetDirectUrl(url);
            if (Repository.ContainsKey(customHash))
                throw new ArgumentException("The given custom short URL is already in use.");
            if (Repository.ContainsValue(directUrl))
            {
                var oldKey = Repository.GetKey(directUrl);
                Repository.Remove(oldKey);
            }
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

        private IList<string> NotAllowed
        {
            get
            {
                return ProtectedPaths.Select(p => 
                        p.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries)
                        .First())
                    .Distinct()
                    .ToList();
            }
        }
    }
}

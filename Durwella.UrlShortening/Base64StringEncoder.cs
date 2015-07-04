using System;

namespace Durwella.UrlShortening
{
    public class Base64StringEncoder : IStringEncoder
    {
        public string GetString(byte[] bytes)
        {
            return Convert.ToBase64String(bytes)
                .Replace("=", "")
                .Replace("+", "-")
                .Replace("/", "_");
        }
    }
}
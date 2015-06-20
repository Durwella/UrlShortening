using System;

namespace Durwella.UrlShortening
{
    public class DefaultHashScheme : IHashScheme
    {
        public int LengthPreference { get; set; }

        public DefaultHashScheme()
        {
            LengthPreference = 6;
        }

        public string GetKey(string value)
        {
            var code = value.GetHashCode();
            return GetString(code);
        }
        
        public string GetKey(string value, int permutation)
        {
            return GetString(value.GetHashCode() + permutation);
        }

        private string GetString(int code)
        {
            var bytes = BitConverter.GetBytes(code);
            var key = Convert.ToBase64String(bytes)
                .Replace("=", "")
                .Replace("+", "-")
                .Replace("/", "_");
            return key.Substring(0, LengthPreference);
        }
    }
}

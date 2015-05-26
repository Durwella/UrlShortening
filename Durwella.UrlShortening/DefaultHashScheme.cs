using System;

namespace Durwella.UrlShortening
{
    public class DefaultHashScheme : IHashScheme
    {
        public string GetKey(string value)
        {
            var code = value.GetHashCode();
            return GetString(code);
            return value.GetHashCode().ToString();
        }
        
        public string GetKey(string value, int permutation)
        {
            return GetString(value.GetHashCode() + permutation);
        }

        private string GetString(int code)
        {
            var bytes = BitConverter.GetBytes(code);
            return Convert.ToBase64String(bytes).Replace("=", "");
        }
    }
}

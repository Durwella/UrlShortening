using System;

namespace Durwella.UrlShortening
{
    public class DotNetHashFunction : IHashFunction
    {
        public byte[] GetHashBytes(string s)
        {
            var code = s.GetHashCode();
            return BitConverter.GetBytes(code);
        }
    }
}
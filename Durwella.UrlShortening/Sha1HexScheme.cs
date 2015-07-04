using System.Security.Cryptography;
using System.Text;

namespace Durwella.UrlShortening
{
    internal class Sha1HexScheme : IHashScheme
    {
        public Sha1HexScheme()
        {
            LengthPreference = 8;
        }

        public int LengthPreference { get; set; }

        public string GetKey(string value)
        {
            return GetKey(value, 0);
        }

        public string GetKey(string value, int permutation)
        {
            return Sha1HashStringForUtf8String(value + permutation)
                .Substring(0, LengthPreference);
        }

        static string Sha1HashStringForUtf8String(string s)
        {
            var bytes = Encoding.UTF8.GetBytes(s);
            var sha1 = SHA1.Create();
            var hashBytes = sha1.ComputeHash(bytes);
            return HexStringFromBytes(hashBytes);
        }

        static string HexStringFromBytes(byte[] bytes)
        {
            var sb = new StringBuilder();
            foreach (var b in bytes)
            {
                var hex = b.ToString("x2");
                sb.Append(hex);
            }
            return sb.ToString();
        }
    }
}
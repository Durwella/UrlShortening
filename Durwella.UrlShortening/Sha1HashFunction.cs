using System.Security.Cryptography;
using System.Text;

namespace Durwella.UrlShortening
{
    public class Sha1HashFunction : IHashFunction
    {
        public byte[] GetHashBytes(string s)
        {
            var bytes = Encoding.UTF8.GetBytes(s);
            using (var sha1 = SHA1.Create())
                return sha1.ComputeHash(bytes);
        }
    }
}
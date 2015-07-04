using System.Text;

namespace Durwella.UrlShortening
{
    public class HexStringEncoder : IStringEncoder
    {
        public string GetString(byte[] bytes)
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

namespace Durwella.UrlShortening
{
    public class DefaultHashScheme : IHashScheme
    {
        public string GetKey(string value)
        {
            return value.GetHashCode().ToString();
        }
    }
}

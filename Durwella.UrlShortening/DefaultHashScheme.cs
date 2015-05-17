
namespace Durwella.UrlShortening
{
    public class DefaultHashScheme : IHashScheme
    {
        public string GetKey(string value)
        {
            return value.GetHashCode().ToString();
        }

        public string GetKey(string value, int permutation)
        {
            return (value.GetHashCode() + permutation).ToString();
        }
    }
}

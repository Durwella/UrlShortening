
namespace Durwella.UrlShortening
{
    public interface IHashScheme
    {
        string GetKey(string value);
    }
}

namespace Durwella.UrlShortening
{
    public interface IHashFunction
    {
        byte[] GetHashBytes(string s);
    }
}
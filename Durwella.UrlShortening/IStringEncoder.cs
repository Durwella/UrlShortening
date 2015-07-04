namespace Durwella.UrlShortening
{
    public interface IStringEncoder
    {
        string GetString(byte[] bytes);
    }
}
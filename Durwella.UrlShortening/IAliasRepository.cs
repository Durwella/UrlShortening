namespace Durwella.UrlShortening
{
    public interface IAliasRepository
    {
        void Add(string key, string value);
        bool ContainsKey(string key);
        string GetValue(string key);
    }
}

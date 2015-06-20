namespace Durwella.UrlShortening
{
    public interface IAliasRepository
    {
        void Add(string key, string value);
        bool Remove(string key);
        bool ContainsKey(string key);
        bool ContainsValue(string value);
        string GetKey(string value);
        string GetValue(string key);
    }
}

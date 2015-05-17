using System.Collections.Generic;

namespace Durwella.UrlShortening
{
    public class MemoryAliasRepository : Dictionary<string, string>, IAliasRepository
    {
        public string GetValue(string key)
        {
            return this[key];
        }
    }
}

using System.Collections.Generic;
using System.Linq;

namespace Durwella.UrlShortening
{
    public class MemoryAliasRepository : Dictionary<string, string>, IAliasRepository
    {
        public string GetValue(string key)
        {
            return this[key];
        }

        public string GetKey(string value)
        {
            return this.Single(t => t.Value == value).Key;
        }
    }
}

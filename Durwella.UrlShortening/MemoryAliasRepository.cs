using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Durwella.UrlShortening
{
    [SuppressMessage("Microsoft.Usage", "CA2237:MarkISerializableTypesWithSerializable", Justification = "Not relevant")]
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

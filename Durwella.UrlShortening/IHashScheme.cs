
namespace Durwella.UrlShortening
{
    public interface IHashScheme
    {
        /// <summary>
        /// Gets and sets the preferred number of characters for returned keys.
        /// </summary>
        int LengthPreference { get; set; }

        /// <summary>
        /// Get a probably-unique alias or 'hash' for the given string
        /// </summary>
        string GetKey(string value);

        /// <summary>
        /// Get a different probably-unique hash for each integer greater than 0
        /// </summary>
        string GetKey(string value, int permutation);
    }
}

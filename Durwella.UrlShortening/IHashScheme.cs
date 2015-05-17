
namespace Durwella.UrlShortening
{
    public interface IHashScheme
    {
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

namespace Durwella.UrlShortening
{
    public abstract class BaseHashScheme<TFunction, TEncoding> : IHashScheme 
        where TFunction : IHashFunction, new()
        where TEncoding : IStringEncoder, new()
    {
        public int LengthPreference { get; set; }

        public string GetKey(string value)
        {
            return GetKey(value, 0);
        }

        public string GetKey(string value, int permutation)
        {
            var hashBytes = _hashFunction.GetHashBytes(value + permutation);
            var hashString = _stringEncoder.GetString(hashBytes);
            while (hashString.Length < LengthPreference)
                hashString += hashString;
            return hashString.Substring(0, LengthPreference);
        }

        private readonly IHashFunction _hashFunction = new TFunction();
        private readonly IStringEncoder _stringEncoder = new TEncoding();
    }
}
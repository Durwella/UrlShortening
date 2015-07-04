namespace Durwella.UrlShortening
{
    public class Sha1HexScheme : 
        BaseHashScheme<Sha1HashFunction, HexStringEncoder>
    {
        public Sha1HexScheme()
        {
            LengthPreference = 8;
        }
    }
}
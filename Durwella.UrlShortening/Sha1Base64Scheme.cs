namespace Durwella.UrlShortening
{
    public class Sha1Base64Scheme :
        BaseHashScheme<Sha1HashFunction, Base64StringEncoder>
    {
        public Sha1Base64Scheme()
        {
            LengthPreference = 4;
        }
    }
}
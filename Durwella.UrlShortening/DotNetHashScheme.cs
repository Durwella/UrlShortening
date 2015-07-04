namespace Durwella.UrlShortening
{
    public class DotNetHashScheme :
        BaseHashScheme<DotNetHashFunction, Base64StringEncoder>
    {
        public DotNetHashScheme()
        {
            LengthPreference = 6;
        }
    }
}

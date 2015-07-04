namespace Durwella.UrlShortening
{
    public class DefaultHashScheme :
        BaseHashScheme<DefaultHashFunction, Base64StringEncoder>
    {
        public DefaultHashScheme()
        {
            LengthPreference = 6;
        }
    }
}

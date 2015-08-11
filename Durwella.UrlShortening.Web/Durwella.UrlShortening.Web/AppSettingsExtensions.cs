using System.Collections.Specialized;
using ServiceStack;

namespace Durwella.UrlShortening.Web
{
    public static class AppSettingsExtensions
    {
        public static bool UseCredentials(this NameValueCollection appSettings)
        {
            return !appSettings["AdminPassword"].IsNullOrEmpty();
        }

        public static bool UseAad(this NameValueCollection appSettings)
        {
            return !appSettings["oauth.aad.ClientId"].IsNullOrEmpty() &&
                   !appSettings["oauth.aad.ClientSecret"].IsNullOrEmpty();
        }
    }
}
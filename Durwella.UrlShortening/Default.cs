
namespace Durwella.UrlShortening
{
    public static class Default
    {
        public static IHashScheme HashScheme() {  return new Sha1Base64Scheme(); }
    }
}

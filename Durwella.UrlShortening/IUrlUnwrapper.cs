
namespace Durwella.UrlShortening
{
    public interface IUrlUnwrapper
    {
        /// <summary>
        /// Follow any redirects to return the URL directly referring to the resource
        /// </summary>
        string GetDirectUrl(string url);
    }
}

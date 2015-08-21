using System.Collections.Generic;
using System.Net;

namespace Durwella.UrlShortening
{
    public class WebClientUrlUnwrapper : IUrlUnwrapper
    {
        /// <summary>
        /// Set IgnoreErrorCodes to list of HTTP Status Codes that 
        /// should be ignored when testing the URL. For example,
        /// if you want to allow URLs to secure resources to be shortened 
        /// you can set IgnoreErrorCodes = new[] { HttpStatusCode.Unauthorized }
        /// </summary>
        public IList<HttpStatusCode> IgnoreErrorCodes { get; set; }

        public string GetDirectUrl(string url)
        {
            var request = WebRequest.Create(url);
            try
            {
                using (var response = request.GetResponse())
                    return response.ResponseUri.ToString();
            }
            catch (WebException ex)
            {
                var httpResponse = ex.Response as HttpWebResponse;
                if (httpResponse == null || 
                    IgnoreErrorCodes == null ||
                    !IgnoreErrorCodes.Contains(httpResponse.StatusCode))
                    throw;
                return url;
            }
        }
    }
}

using System.Collections.Generic;

namespace Durwella.UrlShortening.Web.ServiceInterface
{
    public interface IProtectedPathList
    {
        IList<string> ProtectedPaths { get; } 
    }
}
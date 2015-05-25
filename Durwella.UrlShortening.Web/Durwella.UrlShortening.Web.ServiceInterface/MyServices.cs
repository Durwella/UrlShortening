using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack;
using Durwella.UrlShortening.Web.ServiceModel;

namespace Durwella.UrlShortening.Web.ServiceInterface
{
    public class MyServices : Service
    {
        public object Any(Hello request)
        {
            return new HelloResponse { Result = "Hello, {0}!".Fmt(request.Name) };
        }
    }
}
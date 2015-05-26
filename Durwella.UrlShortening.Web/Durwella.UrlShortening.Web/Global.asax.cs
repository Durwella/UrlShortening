using ServiceStack.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace Durwella.UrlShortening.Web
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            // TODO: Setup logging for local debugging and azure hosting
            //LogManager.LogFactory = new LogFactory(); 
            new AppHost().Init();
        }
    }
}
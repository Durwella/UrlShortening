using Durwella.UrlShortening.Web.ServiceInterface;
using Funq;
using ServiceStack;
using ServiceStack.Configuration;
using ServiceStack.Logging;
using ServiceStack.Razor;
using System;
using System.Configuration;
using System.Diagnostics;

namespace Durwella.UrlShortening.Web
{
    public class AppHost : AppHostBase
    {
        /// Base constructor requires a name and assembly to locate web service classes. 
        public AppHost()
            : base("Durwella.UrlShortening.Web", typeof(HelloService).Assembly)
        {
        }

        /// Application specific configuration
        /// This method should initialize any IoC resources utilized by your web service classes.
        public override void Configure(Container container)
        {
            //var log = LogManager.GetLogger(this.GetType());
            //log.InfoFormat("Configuring {0}...", ServiceName);

            IAliasRepository aliasRepository = new MemoryAliasRepository();
            try
            {
                var connectionStringSetting = ConfigurationManager.ConnectionStrings["AzureStorage"];
                if (connectionStringSetting != null && !String.IsNullOrWhiteSpace(connectionStringSetting.ConnectionString))
                    aliasRepository = new AzureTableAliasRepository(connectionStringSetting.ConnectionString);
            }
            catch(Exception exception)
            {
                //log.Error("Failed to connect to Azure Storage for persistent short URLs", exception);
                Trace.TraceError("Failed to connect to Azure Storage for persistent short URLs. Exception: {0}", exception);
            }

            container.Register<IResolver>(container);
            container.Register<IAliasRepository>(aliasRepository);

            //Plugins:
            this.Plugins.Add(new RazorFormat());
            //this.Plugins.Add(new PostmanFeature());
            //this.Plugins.Add(new CorsFeature());
        }
    }
}
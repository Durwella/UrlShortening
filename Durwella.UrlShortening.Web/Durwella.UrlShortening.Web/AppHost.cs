using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using Durwella.UrlShortening.Web.ServiceInterface;
using Funq;
using ServiceStack;
using ServiceStack.Api.Swagger;
using ServiceStack.Auth;
using ServiceStack.Authentication.Aad;
using ServiceStack.Caching;
using ServiceStack.Configuration;
using ServiceStack.Razor;

namespace Durwella.UrlShortening.Web
{
    public class AppHost : AppHostBase, IProtectedPathList
    {
        public IList<string> ProtectedPaths { get { return RestPaths.Select(rp => rp.Path).ToList(); } }

        /// Base constructor requires a name and assembly to locate web service classes.
        public AppHost()
            : base("Durwella.UrlShortening.Web", typeof (UrlShorteningService).Assembly)
        {
        }

        /// Application specific configuration
        /// This method should initialize any IoC resources utilized by your web service classes.
        public override void Configure(Container container)
        {
            // TODO: Logging (compatible with VS console and Azure logs)
            //var log = LogManager.GetLogger(this.GetType());
            //log.InfoFormat("Configuring {0}...", ServiceName);

            SetupUrlShortening(container);
            SetupAuthentication(container);

            // Additional Plugins:
            Plugins.Add(new RazorFormat());
            Plugins.Add(new SwaggerFeature());
        }

        private void SetupUrlShortening(Container container)
        {
            container.Register<IResolver>(container);
            var aliasRepository = SetupAzureStorageAliasRepository() ?? new MemoryAliasRepository();
            container.Register(aliasRepository);
            container.Register<IProtectedPathList>(this);
            SetupPreferredHashLength(container);
        }

        private static IAliasRepository SetupAzureStorageAliasRepository()
        {
            IAliasRepository aliasRepository = null;
            try
            {
                var connectionStringSetting = ConfigurationManager.ConnectionStrings["AzureStorage"];
                if (connectionStringSetting != null &&
                    !String.IsNullOrWhiteSpace(connectionStringSetting.ConnectionString))
                {
                    var azureStorageRepo = new AzureTableAliasRepository(connectionStringSetting.ConnectionString);
                    aliasRepository = azureStorageRepo;
                    SetupLockUrlMinutes(azureStorageRepo);
                }
            }
            catch (Exception exception)
            {
                //log.Error("Failed to connect to Azure Storage for persistent short URLs", exception);
                Trace.TraceError("Failed to connect to Azure Storage for persistent short URLs. Exception: {0}",
                    exception);
            }
            return aliasRepository;
        }

        private static void SetupLockUrlMinutes(AzureTableAliasRepository azureStorageRepo)
        {
            try
            {
                var lockUrlMinutes = ConfigUtils.GetAppSetting("LockUrlMinutes",
                    (int) azureStorageRepo.LockAge.TotalMinutes);
                azureStorageRepo.LockAge = TimeSpan.FromMinutes(lockUrlMinutes);
            }
            catch (FormatException exception)
            {
                Trace.TraceError("Failed to parse LockUrlMinutes. Exception: {0}", exception);
            }
        }

        private static void SetupPreferredHashLength(Container container)
        {
            var preferredHashLengthString = ConfigurationManager.AppSettings["PreferredHashLength"];
            if (!String.IsNullOrWhiteSpace(preferredHashLengthString))
            {
                try
                {
                    var preferredHashLength = Int32.Parse(preferredHashLengthString);
                    if (preferredHashLength < 0)
                        throw new FormatException("Expected PreferredHashLength to be positive.");
                    var hashScheme = Default.HashScheme();
                    hashScheme.LengthPreference = preferredHashLength;
                    container.Register(hashScheme);
                }
                catch (FormatException exception)
                {
                    Trace.TraceError("Failed to parse PreferredHashLength. Exception: {0}",
                        exception);
                }
            }
        }

        private void SetupAuthentication(Container container)
        {
            // We use the [Authenticate] attribute to control access to creation of short URLs.
            // Therefore we have to set up an IAuthProvider even if the administrator doesn't want authentication.
            var appSettings = ConfigurationManager.AppSettings;
            var useAad = !appSettings["oauth.aad.ClientId"].IsNullOrEmpty() &&
                         !appSettings["oauth.aad.ClientSecret"].IsNullOrEmpty();            
            var adminPassword = appSettings["AdminPassword"];
            var hasAdminPassword = !adminPassword.IsNullOrEmpty();
            var authProviders = new List<IAuthProvider>();
            string htmlRedirect = null;
            if (hasAdminPassword)
            {
                authProviders.Add(new CredentialsAuthProvider(AppSettings)); //HTML Form post of UserName/Password credentials
                container.Register<ICacheClient>(new MemoryCacheClient());
                var userRep = new InMemoryAuthRepository();
                userRep.CreateUserAuth(new UserAuth {UserName = "admin"}, adminPassword);
                container.Register<IUserAuthRepository>(userRep);
            }
            if (useAad)
            {
                htmlRedirect = "/auth/aad";
                authProviders.Add(new AadAuthProvider(AppSettings));
            }
            if (!useAad && !hasAdminPassword)
                authProviders.Add(new AlwaysAuthorizedAuthProvider());
            Plugins.Add(new AuthFeature(() => new AuthUserSession(), authProviders.ToArray(), htmlRedirect));
        }
    }
}
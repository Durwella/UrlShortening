﻿using System;
using System.Configuration;
using System.Diagnostics;
using Durwella.UrlShortening.Web.ServiceInterface;
using Funq;
using ServiceStack;
using ServiceStack.Api.Swagger;
using ServiceStack.Auth;
using ServiceStack.Caching;
using ServiceStack.Configuration;
using ServiceStack.Razor;

namespace Durwella.UrlShortening.Web
{
    public class AppHost : AppHostBase
    {
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

        private static void SetupUrlShortening(Container container)
        {
            container.Register<IResolver>(container);
            var aliasRepository = SetupAzureStorageAliasRepository() ?? new MemoryAliasRepository();
            container.Register(aliasRepository);
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
            var adminPassword = ConfigurationManager.AppSettings["AdminPassword"];
            var requirePassword = !String.IsNullOrWhiteSpace(adminPassword);
            var authProviders = requirePassword
                ? new IAuthProvider[]
                {
                    new BasicAuthProvider(), //Sign-in with Basic Auth
                    new CredentialsAuthProvider(), //HTML Form post of UserName/Password credentials
                }
                : new IAuthProvider[]
                {
                    new AlwaysAuthorizedAuthProvider(),
                };
            Plugins.Add(new AuthFeature(() => new AuthUserSession(), authProviders));
            if (requirePassword)
            {
                container.Register<ICacheClient>(new MemoryCacheClient());
                var userRep = new InMemoryAuthRepository();
                userRep.CreateUserAuth(new UserAuth {UserName = "admin"}, adminPassword);
                container.Register<IUserAuthRepository>(userRep);
            }
        }
    }
}
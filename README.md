# URL Shortening

A simple URL shortening service using .NET and Azure.

[![Deploy to Azure](https://img.shields.io/badge/deploy!-Azure-6EC0D9.svg)](https://azuredeploy.net/)
[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](https://raw.githubusercontent.com/Durwella/UrlShortening/master/LICENSE)
[![Build status](https://ci.appveyor.com/api/projects/status/bdr6q9t088l8c81c?svg=true)](https://ci.appveyor.com/project/jfoshee/urlshortening)

## Deployment

You can deploy to Azure using the above button. Prior to deployment you should have an existing Azure Storage Account. Copy the connection string to the clipboard; you will be prompted for it during deployment in a field labeled AzureStorageConnectionString. 

Once deployed you can add or modify the connection string for `AzureStorage`. 

1. Click **Manage** when deployment is finished (or browse to the web app on the [Azure Portal](https://portal.azure.com)
1. Click **Settings**
1. Select **Application settings**
1. Click **Show Connection Strings**
1. Add a new **Custom** connection string named **AzureStorage** and paste the connection string to an Azure Storage Account

If you do not provide the Azure Storage connection string an in-memory URL Alias repository will be used. 
In that case short URLs will break when the web app is restarted.

## Architecture

[UrlShortener](Durwella.UrlShortening/UrlShortener.cs) contains the core logic. 
There are interfaces you can use to customize the specific mechanics:

- [IHashScheme](Durwella.UrlShortening/IHashScheme.cs) - Used to generate the short URL. Should generate an appropriate short "hash" from a long string. A *poor* [default](Durwella.UrlShortening/DefaultHashScheme.cs) imlpementation is provided which generates 6 character strings from [String.GetHashCode](https://msdn.microsoft.com/en-us/library/system.string.gethashcode). Should also provide a way to iteratetively generate new hashes from the same input in the case of a hash collision.
- [IAliasRepository](Durwella.UrlShortening/IAliasRepository.cs) - Dictionary-like persistence of "alias" or "hash" of one string (the key) to another (the value). Used to save mapping between short and long URLs. The [default](Durwella.UrlShortening/AzureTableAliasRepository.cs) uses Azure Table storage. The [fallback](Durwella.UrlShortening/MemoryAliasRepository.cs) uses an in-memory Dictionary.
- [IUrlUnwrapper](Durwella.UrlShortening/IUrlUnwrapper.cs) - Responsible for resolving a direct URL to a resource. For example the provided URL might already be a 'short URL', which could lead to multiple redirects or a redirect loop. The [default](Durwella.UrlShortening/WebClientUrlUnwrapper.cs) uses [WebClient](https://msdn.microsoft.com/en-us/library/system.net.webclient).

## Contributing

- Natural enhancements to this project would be: 
	- Other persistence options implementing [IAliasRepository](Durwella.UrlShortening/IAliasRepository.cs)
	- Other hashing schemes implementing [IHashScheme](Durwella.UrlShortening/IHashScheme.cs)
- Core logic in the Durwella.UrlShortening project must be unit tested.
- Unit tests should be written using 3 paragaphs corresponding to [Arrange, Act and Assert](http://c2.com/cgi/wiki?ArrangeActAssert)
- Build must remain clean (no warnings, tests passing)
- Code Analysis issues should not be introduced
- Must be deployable to Azure using [AzureDeploy.net](https://azuredeploy.net/)

## References

[URL Shortening: Hashes In Practice](http://blog.codinghorror.com/url-shortening-hashes-in-practice/)  
Nice article by Jeff Atwood. Historical yet relevant.

[Using Custom Azure Resource Management Templates](https://elliotthamai.wordpress.com/2014/11/15/using-custom-arm-templates-with-the-deploy-to-azure-button/)  
Helpful for understanding what is going on with the [azuredeploy.json](azuredeploy.json) file

Testing Azure deployment can be done from Azure PowerShell something like this...

    Switch-AzureMode -Name AzureResourceManager
    $site = "uniqueNameARMTest123"
    $pw = read-host -AsSecureString
    New-AzureResourceGroup -TemplateFile .\azuredeploy.json -Name $site -DeploymentName $site `
        -Verbose -Location "South Central US" -hostingPlanName $site -hostingPlanLocation "South Central US" `
        -siteName $site -repoUrl "https://github.com/Durwella/UrlShortening" -branch "master" `
        -title $site -adminPassword $pw


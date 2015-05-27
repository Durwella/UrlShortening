# URL Shortening

A simple URL shortening service using .NET and Azure.

[![Build status](https://ci.appveyor.com/api/projects/status/bdr6q9t088l8c81c?svg=true)](https://ci.appveyor.com/project/jfoshee/urlshortening)
[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](https://raw.githubusercontent.com/Durwella/UrlShortening/master/LICENSE)
[![Deploy to Azure](https://img.shields.io/badge/deploy!-Azure-6EC0D9.svg)](https://azuredeploy.net/)


You can deploy to Azure using the above button. Once deployed you need to add a connection string 
for `AzureStorage`. 

1. Click **Manage** when deployment is finished (or browse to the web app on the [Azure Portal](https://portal.azure.com)
1. Click **Settings**
1. Select **Application settings**
1. Click **Show Connection Strings**
1. Add a new **Custom** connection string named **AzureStorage** and paste the connection string to an Azure Storage Account

If you do not provide the Azure Storage connection string an in-memory URL Alias repository will be used. 
In that case short URLs will break when the web app is restarted.


## References

[URL Shortening: Hashes In Practice](http://blog.codinghorror.com/url-shortening-hashes-in-practice/)  
Nice article by Jeff Atwood. Historical yet relevant.


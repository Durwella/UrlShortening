using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using System;

namespace Durwella.UrlShortening
{
    public class AzureTableAliasRepository : IAliasRepository
    {
        public AzureTableAliasRepository(string azureStorageAccountName, string azureStorageAccessKey, string tablePrefix)
        {
            var credentials = new StorageCredentials(azureStorageAccountName, azureStorageAccessKey);
            var account = new CloudStorageAccount(credentials, useHttps: true);
            var tableClient = account.CreateCloudTableClient();
            var table = tableClient.GetTableReference(tablePrefix);
            table.CreateIfNotExists();
        }

        public void Add(string key, string value)
        {
            throw new NotImplementedException();
        }

        public bool ContainsKey(string key)
        {
            throw new NotImplementedException();
        }

        public bool ContainsValue(string value)
        {
            throw new NotImplementedException();
        }

        public string GetKey(string value)
        {
            throw new NotImplementedException();
        }

        public string GetValue(string key)
        {
            throw new NotImplementedException();
        }
    }
}

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Linq;

namespace Durwella.UrlShortening
{
    public class AzureTableAliasRepository : IAliasRepository
    {
        public static readonly string Partition = "Global";

        public class Entity : TableEntity
        {
            public string Value { get; set; }

            public Entity()
            {
            }

            public Entity(string key, string value)
            {
                PartitionKey = Partition;
                RowKey = key;
                Value = value;
            }
        }

        public AzureTableAliasRepository(string azureStorageAccountName, string azureStorageAccessKey, string tablePrefix)
        {
            var credentials = new StorageCredentials(azureStorageAccountName, azureStorageAccessKey);
            var account = new CloudStorageAccount(credentials, useHttps: true);
            var tableClient = account.CreateCloudTableClient();
            _table = tableClient.GetTableReference(tablePrefix);
            _table.CreateIfNotExists();
        }

        public void Add(string key, string value)
        {
            var entity = new Entity(key, value);
            var insertOperation = TableOperation.InsertOrReplace(entity);
            _table.Execute(insertOperation);
        }

        public bool ContainsKey(string key)
        {
            var op = TableOperation.Retrieve(Partition, key);
            var result = _table.Execute(op);
            return result.Result != null;
        }

        public bool ContainsValue(string value)
        {
            var query = new TableQuery<Entity>().Where(TableQuery.GenerateFilterCondition("Value", QueryComparisons.Equal, value));
            return _table.ExecuteQuery(query).Any();
        }

        public string GetKey(string value)
        {
            var query = new TableQuery<Entity>().Where(TableQuery.GenerateFilterCondition("Value", QueryComparisons.Equal, value));
            var entity = _table.ExecuteQuery(query).Single();
            return entity.RowKey;
        }

        public string GetValue(string key)
        {
            var op = TableOperation.Retrieve<Entity>(Partition, key);
            var result = _table.Execute(op);
            var entity = (Entity)result.Result;
            return entity.Value;
        }

        private CloudTable _table;
    }
}

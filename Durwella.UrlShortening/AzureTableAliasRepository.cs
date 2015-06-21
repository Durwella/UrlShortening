using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Linq;

namespace Durwella.UrlShortening
{
    public class AzureTableAliasRepository : IAliasRepository
    {
        public const string Partition = "Global";
        public const string DefaultTablePrefix = "UrlShortening";

        /// <summary>
        /// The age after which the key cannot be removed.
        /// </summary>
        public TimeSpan LockAge { get; set; }

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

        /// <summary>
        /// The default repository uses the Development Storage Emulator which must be running. 
        /// See: http://azure.microsoft.com/en-us/documentation/articles/storage-use-emulator/
        /// </summary>
        public AzureTableAliasRepository()
            : this(CloudStorageAccount.DevelopmentStorageAccount, DefaultTablePrefix)
        {
        }

        public AzureTableAliasRepository(string azureStorageAccountName, string azureStorageAccessKey, string tablePrefix = DefaultTablePrefix)
            : this(new StorageCredentials(azureStorageAccountName, azureStorageAccessKey), tablePrefix)
        {
        }

        public AzureTableAliasRepository(StorageCredentials credentials, string tablePrefix = DefaultTablePrefix)
            : this(new CloudStorageAccount(credentials, useHttps: true), tablePrefix)
        {
        }

        public AzureTableAliasRepository(string connectionString, string tablePrefix = DefaultTablePrefix)
            : this(CloudStorageAccount.Parse(connectionString), tablePrefix)
        {
        }

        public AzureTableAliasRepository(CloudStorageAccount account, string tablePrefix)
        {
            LockAge = TimeSpan.FromHours(12);
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

        public bool Remove(string key)
        {
            var entity = RetrieveEntity(key);
            if (entity == null)
                return false;
            var entityAge = DateTimeOffset.UtcNow.Subtract(entity.Timestamp);
            if (entityAge > LockAge)
                throw new InvalidOperationException("Cannot change a short URL that is too old.");
            var removeOperation = TableOperation.Delete(entity);
            _table.Execute(removeOperation);
            return true;
        }

        public bool ContainsKey(string key)
        {
            var entity = RetrieveEntity(key);
            return entity != null;
        }

        public bool ContainsValue(string value)
        {
            return _table.ExecuteQuery(WhereValueIs(value)).Any();
        }

        public string GetKey(string value)
        {
            var entity = _table.ExecuteQuery(WhereValueIs(value)).Single();
            return entity.RowKey;
        }

        public string GetValue(string key)
        {
            var entity = RetrieveEntity(key);
            return entity.Value;
        }

        private Entity RetrieveEntity(string key)
        {
            var op = TableOperation.Retrieve<Entity>(Partition, key);
            var result = _table.Execute(op);
            var entity = (Entity) result.Result;
            return entity;
        }
        private static TableQuery<Entity> WhereValueIs(string value)
        {
            return new TableQuery<Entity>().Where(TableQuery.GenerateFilterCondition("Value", QueryComparisons.Equal, value));
        }

        private readonly CloudTable _table;
    }
}

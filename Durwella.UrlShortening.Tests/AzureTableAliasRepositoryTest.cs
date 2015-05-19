using FluentAssertions;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using NUnit.Framework;
using System;
using System.IO;

/// The tests here run against a live Azure Storage account.
/// So, to run them you need to populate AzureTestCredentials.txt with two lines:
/// Line 1: Your Azure Storage Account Name
/// Line 2: Your Azure Storage Access Key
/// It is strongly recommended you avoid committing your credentials 
/// by invoking the following command from this project's directory:
///     git update-index --assume-unchanged AzureTestCredentials.txt
/// 

namespace Durwella.UrlShortening.Tests
{
    [Explicit]
    public class AzureTableAliasRepositoryTest
    {
        private static string _azureStorageAccountName;
        private static string _azureStorageAccessKey;
        private static bool _enabled = false;
        private static readonly string _tablePrefix = "UrlShorteningTest";
        private static CloudTable _table;
        private AzureTableAliasRepository subject;

        [TestFixtureSetUp]
        public static void LoadAzureCredentials()
        {
            var credentialsFile = File.ReadAllLines("AzureTestCredentials.txt");
            if (credentialsFile.Length < 2
                || String.IsNullOrWhiteSpace(credentialsFile[0])
                || String.IsNullOrWhiteSpace(credentialsFile[1]))
                return;
            _azureStorageAccountName = credentialsFile[0];
            _azureStorageAccessKey = credentialsFile[1];
            _enabled = true;
            var credentials = new StorageCredentials(_azureStorageAccountName, _azureStorageAccessKey);
            var account = new CloudStorageAccount(credentials, useHttps: true);
            var tableClient = account.CreateCloudTableClient();
            _table = tableClient.GetTableReference(_tablePrefix);
        }

        [SetUp]
        public void IgnoreIfDisabled()
        {
            if (!_enabled) 
                Assert.Ignore("Populate AzureTestCredentials.txt for this test");
            subject = new AzureTableAliasRepository(_azureStorageAccountName, _azureStorageAccessKey, _tablePrefix);
        }

        [Test]
        public void ShouldCreateTableWhenConstructed()
        {
            _table.Exists().Should().BeTrue();
        }

        [Test]
        public void ShouldAddKeyValueEntity()
        {
            var key = "TheTestKey";
            var value = "TheTestValue";

            subject.Add(key, value);

            var retrieveOp = TableOperation.Retrieve(AzureTableAliasRepository.Partition, key);
            var result = _table.Execute(retrieveOp);
            result.Should().NotBeNull();
            var entity = (ITableEntity)result.Result;
            entity.RowKey.Should().Be(key);
            var properties = entity.WriteEntity(new OperationContext());
            properties["Value"].StringValue.Should().Be(value);
        }


    }
}

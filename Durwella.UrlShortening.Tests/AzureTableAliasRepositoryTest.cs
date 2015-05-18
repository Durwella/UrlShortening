using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using NUnit.Framework;
using System;
using System.IO;

namespace Durwella.UrlShortening.Tests
{
    [Explicit]
    public class AzureTableAliasRepositoryTest
    {
        private static string _azureStorageAccountName;
        private static string _azureStorageAccessKey;
        private static bool _enabled = false;

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
        }

        private static CloudTableClient MakeTableClient()
        {
            var credentials = new StorageCredentials(_azureStorageAccountName, _azureStorageAccessKey);
            var account = new CloudStorageAccount(credentials, useHttps: true);
            return account.CreateCloudTableClient();
        }

        [Test]
        public void ShouldCreateTable()
        {
            if (!_enabled) Assert.Ignore("Populate AzureTestCredentials.txt for this test");
            string tablePrefix = "UrlShortening";

            var subject = new AzureTableAliasRepository(_azureStorageAccountName, _azureStorageAccessKey, tablePrefix);

            var tableClient = MakeTableClient();
            var table = tableClient.GetTableReference(tablePrefix);
            Assert.IsTrue(table.Exists());
        }
    }
}

using LL.B2CFunctions.Abstractions;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LL.B2CFunctions.Services
{
    public abstract class TableStorageDataService<T> : ITableStorageService<T> where T : TableEntity, new()
    {
        protected IConfiguration Configuration;
        protected string? TableName;
        protected ILogger<TableStorageDataService<T>> Logger;

        public TableStorageDataService(IConfiguration configuration, ILogger<TableStorageDataService<T>> logger)
        {
            Configuration = configuration;
            Logger = logger;
        }

        protected async Task<object> ExecuteTableOperation(TableOperation tableOperation)
        {
            var table = await GetCloudTable();
            var tableResult = await table.ExecuteAsync(tableOperation);
            return tableResult.Result;
        }

        protected async Task<CloudTable> GetCloudTable()
        {
            // this should be set in the constructor of the concrete class that inherits from this
            if (string.IsNullOrWhiteSpace(TableName))
                throw new ArgumentNullException("Storage table name not specified");

            var storageAccount = CloudStorageAccount.Parse(Configuration["AzureWebJobsStorage"]);
            var tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
            var table = tableClient.GetTableReference(TableName);
            await table.CreateIfNotExistsAsync();

            return table;
        }

        public async Task<T?> RetrieveEntityAsync(string filters)
        {
            return (await RetrieveCollectionAsync(filters)).FirstOrDefault();
        }

        public async Task<IEnumerable<T>> RetrieveCollectionAsync(string filters)
        {
            try
            {
                var table = await GetCloudTable();
                return await table.ExecuteQuerySegmentedAsync(new TableQuery<T>().Where(filters), null);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                return Enumerable.Empty<T>();
            }
        }
    }
}

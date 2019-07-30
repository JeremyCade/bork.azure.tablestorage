namespace Bork.Azure.TableStorage.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Azure.Cosmos.Table;

    public class BorkedCosmosService<T>
        where T : ITableEntity, new()
    {
        private readonly CloudTable table;

        public BorkedCosmosService(string connectionString, string tableName)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            var storageAccount = CloudStorageAccount.Parse(connectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            this.table = tableClient.GetTableReference(tableName);
        }

        public async Task InsertAsync(T entity)
        {
            await this.table.CreateIfNotExistsAsync();
            var operation = TableOperation.InsertOrReplace(entity);
            await this.table.ExecuteAsync(operation);
            Console.WriteLine($"Inserted Entity {entity.GetType().Name} with Partition Key {entity.PartitionKey} and Row Key {entity.RowKey}");
        }

        public async Task<T> RetrieveAsync(string partitionKey, string rowKey)
        {
            Console.WriteLine($"Retrieving Entity with Partition Key {partitionKey} and Row Key {rowKey}");
            var operation = TableOperation.Retrieve<T>(partitionKey, rowKey);
            var results = await this.table.ExecuteAsync(operation);
            return (T)results.Result;
        }

        public async Task<IEnumerable<T>> PrepareForTheBork()
        {
            var results = new List<T>();

            var query = new TableQuery<T>();
            TableContinuationToken token = null;

            do
            {
                var segment = await table.ExecuteQuerySegmentedAsync(query, token);
                token = segment.ContinuationToken;
                results.AddRange(segment.Results);
            }
            while (token != null);

            Console.WriteLine($"Retrieved {results.Count} entities");
            return results;
        }

        public async Task Cleanup()
        {
            await this.table.DeleteIfExistsAsync();
        }
    }
}
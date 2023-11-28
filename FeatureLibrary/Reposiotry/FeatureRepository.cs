using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

public class FeatureRepository : IFeatureRepository
{

    private readonly CloudTable _featureTable;

    //public FeatureRepository(string connectionString, string tableName)
    //{
    //    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
    //    CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
    //    _featureTable = tableClient.GetTableReference(tableName);
    //}
    public FeatureRepository(string connectionString, string tableName)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentNullException(nameof(connectionString), "Connection string cannot be null or empty.");
        }

        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
        CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
        _featureTable = tableClient.GetTableReference(tableName);
    }



    public async Task<FeatureEntity> RetrieveFeatureAsync(string partitionKey, string rowKey)
    {
        TableOperation retrieveOperation = TableOperation.Retrieve<FeatureEntity>(partitionKey, rowKey);
        TableResult result = await _featureTable.ExecuteAsync(retrieveOperation);
        var retrievedEntity = (FeatureEntity)result.Result;

        return retrievedEntity;
    }


    public async Task<FeatureEntity> SearchFeatureAsync(string searchKey)
    {
        var filter = TableQuery.CombineFilters(
        TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, searchKey),
        TableOperators.Or,
        TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, searchKey)
    );

        var query = new TableQuery<FeatureEntity>().Where(filter);
        var queryResult = await _featureTable.ExecuteQuerySegmentedAsync(query, null);

        // Return the first matching entity (if any)
        return queryResult.Results.FirstOrDefault();
    }

    //public async Task<FeatureEntity> RetrieveFeatureAsync(string partitionKey, string rowKey)
    //{
    //    TableOperation retrieveOperation = TableOperation.Retrieve<FeatureEntity>(partitionKey, rowKey);
    //    TableResult result = await _featureTable.ExecuteAsync(retrieveOperation);
    //    var retrievedEntity = (FeatureEntity)result.Result;

    //    return retrievedEntity;
    //}


}

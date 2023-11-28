using Microsoft.WindowsAzure.Storage.Table;

public class FeatureEntity : TableEntity
{
    public FeatureEntity()//empty constructor to access the data from the table storage
    {

    }

    public FeatureEntity(string entityname, int featureid)
    {
        PartitionKey = entityname;
        RowKey = featureid.ToString();
    }


    public string FeatureName { get; set; }

    public string Value { get; set; }

}

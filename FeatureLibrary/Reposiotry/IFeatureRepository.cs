using System.Collections.Generic;
using System.Threading.Tasks;

public interface IFeatureRepository
{
    Task<FeatureEntity> RetrieveFeatureAsync(string partitionKey, string rowKey);

    Task<FeatureEntity> SearchFeatureAsync(string searchKey);

}
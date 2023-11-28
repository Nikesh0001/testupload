
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class CommonController : ControllerBase
{
    private readonly IFeatureRepository _featureRepository;

    public CommonController(IFeatureRepository featureRepository)
    {
        _featureRepository = featureRepository;
    }


    //GetbyId data
    //[HttpGet("{partitionKey}/{rowKey}")]
    //public async Task<ActionResult> GetFeature(string partitionKey, string rowKey)
    //{
    //    var feature = await _featureRepository.RetrieveFeatureAsync(partitionKey, rowKey);

    //    FeatureEntity entity = new FeatureEntity();
    //    entity.FeatureName = feature.FeatureName;
    //    entity.Value = feature.Value;

    //    return Ok(entity);
    //}



    [HttpGet("{partitionKey}/{rowKey}")]
    public async Task<ActionResult> GetFeature(string partitionKey, string rowKey)
    {
        try
        {
            var feature = await _featureRepository.RetrieveFeatureAsync(partitionKey, rowKey);

            if (feature == null)
            {
                return NotFound();
            }

            FeatureEntity entity = new FeatureEntity
            {
                FeatureName = feature.FeatureName,
                Value = feature.Value
            };

            return Ok(entity);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchFeature(string searchKey)
    {
        try
        {
            var result = await _featureRepository.SearchFeatureAsync(searchKey);

            if (result == null)
            {
                return NotFound($"No feature found with key: {searchKey}");

            }
            else
            {
                FeatureEntity entity = new FeatureEntity
                {
                    FeatureName = result.FeatureName,
                    Value = result.Value
                };
                return Ok(entity);
            }
        }
        catch (Exception ex)
        {
            // Log the exception or handle it appropriately
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }







}




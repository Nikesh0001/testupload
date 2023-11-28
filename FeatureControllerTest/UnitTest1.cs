using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Description;
using Microsoft.WindowsAzure.Storage;

public class FeatureControllerTests
{
    [Fact]
    public async Task GetFeature_WhenFeatureExists_ShouldReturnOk()
    {
        // Arrange
        var partitionKey = "Character";
        var rowKey = "15";
        var expectedFeature = new FeatureEntity { FeatureName = "Height", Value = "5" };

        var featureRepositoryMock = new Mock<IFeatureRepository>();
        featureRepositoryMock
            .Setup(x => x.RetrieveFeatureAsync(partitionKey, rowKey))
            .ReturnsAsync(expectedFeature);

        var controller = new CommonController(featureRepositoryMock.Object);

        // Act
        var result = await controller.GetFeature(partitionKey, rowKey);

        // Assert
        Assert.IsType<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        Assert.NotNull(okResult.Value);
        var entity = okResult.Value as FeatureEntity;
        Assert.Equal(expectedFeature.FeatureName, entity.FeatureName);
        Assert.Equal(expectedFeature.Value, entity.Value);
    }




    [Fact]
    public async Task GetFeature_WhenFeatureDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var partitionKey = "Character";
        var rowKey = "14";

        var featureRepositoryMock = new Mock<IFeatureRepository>();
        featureRepositoryMock
            .Setup(x => x.RetrieveFeatureAsync(partitionKey, rowKey))
            .ReturnsAsync((FeatureEntity)null);

        var controller = new CommonController(featureRepositoryMock.Object);

        // Act
        var result = await controller.GetFeature(partitionKey, rowKey);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetFeature_WhenExceptionThrown_ShouldReturnInternalServerError()
    {
        // Arrange
        var partitionKey = "Driver";
        var rowKey = "12";

        var featureRepositoryMock = new Mock<IFeatureRepository>();
        featureRepositoryMock
            .Setup(x => x.RetrieveFeatureAsync(partitionKey, rowKey))
            .ThrowsAsync(new Exception("Test exception"));

        var controller = new CommonController(featureRepositoryMock.Object);

        // Act
        var result = await controller.GetFeature(partitionKey, rowKey);

        // Assert
        Assert.IsType<ObjectResult>(result);
        var objectResult = result as ObjectResult;
        Assert.Equal(500, objectResult.StatusCode);
        Assert.Equal("Internal Server Error: Test exception", objectResult.Value);
    }

    [Fact]
    public async Task GetFeature_WhenFeatureRepositoryReturnsNull_ShouldReturnNotFound()
    {
        // Arrange
        var partitionKey = "Character";
        var rowKey = "15";

        var featureRepositoryMock = new Mock<IFeatureRepository>();
        featureRepositoryMock
            .Setup(x => x.RetrieveFeatureAsync(partitionKey, rowKey))
            .ReturnsAsync((FeatureEntity)null);

        var controller = new CommonController(featureRepositoryMock.Object);

        // Act
        var result = await controller.GetFeature(partitionKey, rowKey);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetFeature_WhenFeatureRepositoryThrowsException_ShouldReturnInternalServerError()
    {
        // Arrange
        var partitionKey = "Driver";
        var rowKey = "13";

        var featureRepositoryMock = new Mock<IFeatureRepository>();
        featureRepositoryMock
            .Setup(x => x.RetrieveFeatureAsync(partitionKey, rowKey))
            .ThrowsAsync(new Exception("Test exception"));

        var controller = new CommonController(featureRepositoryMock.Object);

        // Act
        var result = await controller.GetFeature(partitionKey, rowKey);

        // Assert
        Assert.IsType<ObjectResult>(result);
        var objectResult = result as ObjectResult;
        Assert.Equal(500, objectResult.StatusCode);
        Assert.Equal("Internal Server Error: Test exception", objectResult.Value);
    }

    [Fact]
    public void FeatureEntity_EmptyConstructor_ShouldCreateInstance()
    {
        // Arrange
        var featureEntity = new FeatureEntity();

        // Act and Assert
        Assert.NotNull(featureEntity);
    }

    [Fact]
    public void FeatureEntity_ParameterizedConstructor_ShouldSetProperties()
    {
        // Arrange
        string entityName = "TestEntity";
        int featureId = 123;

        // Act
        var featureEntity = new FeatureEntity(entityName, featureId);

        // Assert
        Assert.Equal(entityName, featureEntity.PartitionKey);
        Assert.Equal(featureId.ToString(), featureEntity.RowKey);
    }

    [Fact]
    public void FeatureEntity_SetProperties_ShouldSetValuesCorrectly()
    {
        // Arrange
        var featureEntity = new FeatureEntity();

        // Act
        featureEntity.FeatureName = "Character";
        featureEntity.Value = "12";

        // Assert
        Assert.Equal("Character", featureEntity.FeatureName);
        Assert.Equal("12", featureEntity.Value);
    }


    [Fact]
    public void FeatureRepository_Constructor_NullConnectionString_ShouldThrowArgumentNullException()
    {
        // Arrange
        string connectionString = null;
        string tableName = "TestTable";

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new FeatureRepository(connectionString, tableName));
    }

    [Fact]
    public void FeatureEntity_SetPropertiesAndGetProperties_ShouldMatch()
    {
        // Arrange
        var featureEntity = new FeatureEntity();

        // Act
        featureEntity.FeatureName = "Character";
        featureEntity.Value = "13";

        // Assert
        Assert.Equal("Character", featureEntity.FeatureName);
        Assert.Equal("13", featureEntity.Value);
    }



    //Searchmethod
    [Fact]
    public async Task SearchFeature_ReturnsOkResult_WhenFeatureFound()
    {
        // Arrange
        var searchKey = "yourSearchKey";
        var featureEntity = new FeatureEntity
        {
            FeatureName = "FeatureName",
            Value = "FeatureValue"
        };

        var featureRepositoryMock = new Mock<IFeatureRepository>();
        featureRepositoryMock.Setup(repo => repo.SearchFeatureAsync(searchKey))
                             .ReturnsAsync(featureEntity);

        var controller = new CommonController(featureRepositoryMock.Object);

        // Act
        var result = await controller.SearchFeature(searchKey);

        // Assert
        var okObjectResult = Assert.IsType<OkObjectResult>(result);
        var model = Assert.IsType<FeatureEntity>(okObjectResult.Value);

        Assert.Equal(featureEntity.FeatureName, model.FeatureName);
        Assert.Equal(featureEntity.Value, model.Value);
    }

    [Fact]
    public async Task SearchFeature_ReturnsNotFound_WhenFeatureNotFound()
    {
        // Arrange
        var searchKey = "nonexistentKey";

        var featureRepositoryMock = new Mock<IFeatureRepository>();
        featureRepositoryMock.Setup(repo => repo.SearchFeatureAsync(searchKey))
                             .ReturnsAsync((FeatureEntity)null);

        var controller = new CommonController(featureRepositoryMock.Object);

        // Act
        var result = await controller.SearchFeature(searchKey);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal($"No feature found with key: {searchKey}", notFoundResult.Value);
    }

    [Fact]
    public async Task SearchFeature_ReturnsCorrectEntity_WhenEntityExists()
    {
        // Arrange
        var searchKey = "existingKey";
        var expectedEntity = new FeatureEntity { /* initialize with expected values */ };

        var featureRepositoryMock = new Mock<IFeatureRepository>();
        featureRepositoryMock.Setup(repo => repo.SearchFeatureAsync(searchKey))
                             .ReturnsAsync(expectedEntity);

        var controller = new CommonController(featureRepositoryMock.Object);

        // Act
        var result = await controller.SearchFeature(searchKey);

        // Assert
        var okObjectResult = Assert.IsType<OkObjectResult>(result);
        var actualEntity = Assert.IsType<FeatureEntity>(okObjectResult.Value);

        Assert.Equal(expectedEntity.FeatureName, actualEntity.FeatureName);
        Assert.Equal(expectedEntity.Value, actualEntity.Value);
    }

    [Fact]
    public async Task SearchFeature_ReturnsInternalServerError_OnException()
    {
        // Arrange
        var searchKey = "yourSearchKey";

        var featureRepositoryMock = new Mock<IFeatureRepository>();
        featureRepositoryMock.Setup(repo => repo.SearchFeatureAsync(searchKey))
                             .ThrowsAsync(new Exception("Simulated error"));

        var controller = new CommonController(featureRepositoryMock.Object);

        // Act
        var result = await controller.SearchFeature(searchKey) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(500, result.StatusCode);
        Assert.Equal("Internal server error: Simulated error", result.Value);
    }


    [Fact]
    public void Constructor_SetsPartitionKeyAndRowKey_WhenFeatureIdIsZero()
    {
        // Arrange
        var entityName = "TestEntity";
        var featureId = 0;

        // Act
        var featureEntity = new FeatureEntity(entityName, featureId);

        // Assert
        Assert.NotNull(featureEntity);
        Assert.Equal(entityName, featureEntity.PartitionKey);
        Assert.Equal("0", featureEntity.RowKey);
    }

    [Fact]
    public void Constructor_SetsPartitionKeyAndRowKey_WhenEntityNameIsEmpty()
    {
        // Arrange
        var entityName = "";
        var featureId = 42;

        // Act
        var featureEntity = new FeatureEntity(entityName, featureId);

        // Assert
        Assert.NotNull(featureEntity);
        Assert.Equal(string.Empty, featureEntity.PartitionKey);
        Assert.Equal(featureId.ToString(), featureEntity.RowKey);
    }


    [Fact]
    public void Constructor_SetsPartitionKeyAndRowKey_WhenEntityNameIsNull()
    {
        // Arrange
        string entityName = null;
        var featureId = 42;

        // Act
        var featureEntity = new FeatureEntity(entityName, featureId);

        // Assert
        Assert.Null(featureEntity.PartitionKey);
        Assert.Equal(featureId.ToString(), featureEntity.RowKey);
    }

   

    //Empty constructor

    [Fact]
    public void EmptyConstructor_SetsPropertiesToDefaultValues()
    {
        // Act
        var featureEntity = new FeatureEntity();

        // Assert
        Assert.Null(featureEntity.PartitionKey);
        Assert.Null(featureEntity.RowKey);
        // Add assertions for other properties if any
    }

    [Fact]
    public void Constructor_SetsUpFeatureTable()
    {
        // Arrange
        var connectionString = "DefaultEndpointsProtocol=https;AccountName=featuremeshstorage;AccountKey=DdSeZrTIDey6TjbISsZy2FiYB9FtA+p1ZD72i8XdImSjfomeSQEvGJ2tNcdsCQO3vAMyPG4q/DzQ+AStuKqMnA==;EndpointSuffix=core.windows.net";
        var tableName = "OnlineStorageTable";

        var storageAccount = CloudStorageAccount.Parse(connectionString);
        var tableClient = storageAccount.CreateCloudTableClient();

        // Act
        var featureRepository = new FeatureRepository(connectionString, tableName);

        // Assert
        var featureTableField = (CloudTable)typeof(FeatureRepository)
                                    .GetField("_featureTable", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                                    .GetValue(featureRepository);

        Assert.NotNull(featureTableField);
        Assert.Equal(tableName, featureTableField.Name);
    }


    [Fact]
    public void Constructor_ThrowsException_WhenConnectionStringIsNull()
    {
        // Arrange
        string connectionString = null;
        string tableName = "OnlineStorageTable";

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new FeatureRepository(connectionString, tableName));
    }
}

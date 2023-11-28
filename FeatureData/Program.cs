var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



var azureTableStorageSettings = builder.Configuration.GetSection("AzureTableStorageSettings");
string connectionString = builder.Configuration.GetConnectionString("constring");
string tableName = azureTableStorageSettings.GetValue<string>("TableName");



builder.Services.AddScoped<IFeatureRepository, FeatureRepository>(provider =>
{
    return new FeatureRepository(connectionString, tableName);
});





var app = builder.Build();





// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())    
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

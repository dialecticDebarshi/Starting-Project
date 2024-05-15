
using Starting_Project.Repositories;
using Microsoft.OpenApi.Models;
using Starting_Project.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Azure.Cosmos;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;




var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddSingleton((provider) =>
{
	var endpointUri = configuration["CosmosDbSettings:EndpointUri"];
	var primaryKey = configuration["CosmosDbSettings:PrimaryKey"];
	var databaseName = configuration["CosmosDbSettings:DatabaseName"];

	var cosmosClientOptions = new CosmosClientOptions
	{
		ApplicationName = databaseName
	};

	var loggerFactory = LoggerFactory.Create(builder =>
	{
		builder.AddConsole();
	});

	var cosmosClient = new CosmosClient(endpointUri, primaryKey, cosmosClientOptions);
	cosmosClient.ClientOptions.ConnectionMode = ConnectionMode.Direct;

	return cosmosClient;
});
builder.Services.AddSingleton<IQuestionRepository, QuestionRepository>();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();
//builder.Services.AddSwaggerGen(c =>
//{
//	c.SwaggerDoc("v1", new OpenApiInfo { Title = "API_CosmosDB", Version = "v1" });
//});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
	//app.UseSwagger();

	//app.UseSwaggerUI(c =>
	//{
	//	c.SwaggerEndpoint("/swagger/v1/swagger.json", "API_CosmosDB v1");
	//});
}


app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();


app.Run();

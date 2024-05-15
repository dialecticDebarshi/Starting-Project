using Starting_Project.Models;

using Microsoft.Azure.Cosmos;
using System.Net;
//using System.ComponentModel;

namespace Starting_Project.Repositories
{
	public interface IQuestionRepository
	{
		Task<IEnumerable<QuestionDto>> GetQuestions();
		Task<QuestionDto> GetQuestion(string id);
		Task<QuestionDto> CreateQuestion(QuestionDto questionDto);
		Task<QuestionDto> UpdateQuestion(string id, QuestionDto questionDto);
		Task<bool> DeleteQuestion(string id);
	}
	public class QuestionRepository: IQuestionRepository
	{
		private readonly CosmosClient _cosmosclient;
		private readonly IConfiguration _configuration;
		//	private readonly CosmosDbHelper _cosmosDbHelper;
		private readonly Container _container;

		public QuestionRepository(IConfiguration configuration, CosmosClient cosmosClient)
		{
			//_cosmosDbHelper = cosmosDbHelper;
			_cosmosclient = cosmosClient;
			_configuration = configuration;
			var databaseName = configuration["CosmosDbSettings:DatabaseName"];
			var taskContainerName = "Question";
			_container = cosmosClient.GetContainer(databaseName, taskContainerName);
			//_container = container;
		}

		public async Task<IEnumerable<T>> GetItemsAsync<T>(string query)
		{
			var items = new List<T>();

			var iterator = _container.GetItemQueryIterator<T>(new QueryDefinition(query));
			while (iterator.HasMoreResults)
			{
				var response = await iterator.ReadNextAsync();
				items.AddRange(response.ToList());
			}

			return items;
		}
		public async Task<IEnumerable<QuestionDto>> GetQuestions()
		{
			var query = "SELECT * FROM c";
			var questions = await GetItemsAsync<QuestionDto>(query);
			return questions;
		}

		public async Task<QuestionDto> GetQuestion(string id)
		{
			try
			{
				var response = await _container.ReadItemAsync<QuestionDto>(id, new PartitionKey(id));
				return response.Resource;
			}
			catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
			{
				return null;
			}
		}
		public async Task<QuestionDto> CreateQuestion(QuestionDto questionDto)
		{
			ItemResponse<QuestionDto> response;
			try
			{
				response = await _container.CreateItemAsync(questionDto, new PartitionKey(questionDto.Id));
			}
			catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
			{
				// Handle conflict if necessary
				throw;
			}
			return response.Resource;
		}

		public async Task<QuestionDto> UpdateQuestion(string id, QuestionDto questionDto)
		{
			try
			{
				var response = await _container.ReplaceItemAsync<QuestionDto>(questionDto, id, new PartitionKey(id));
				return response.Resource;
			}
			catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
			{
				return null;
			}
		}


		public async Task<bool> DeleteQuestion(string id)
		{
			try
			{
				await _container.DeleteItemAsync<QuestionDto>(id, new PartitionKey(id));
				return true;
			}
			catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
			{
				return false;
			}
		}
	}
}

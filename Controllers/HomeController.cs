using Microsoft.AspNetCore.Mvc;
using Starting_Project.Models;
using Starting_Project.Repositories;

namespace Starting_Project.Controllers
{
	[ApiController]
	[Route("[controller]")]

	public class HomeController : ControllerBase
	{
		private readonly IConfiguration _configuration;
		//private readonly QuestionRepository _repository;
		private readonly IQuestionRepository _repository;

		public HomeController(IConfiguration configuration, IQuestionRepository QuestionRepository)
		{
			_configuration = configuration;
			_repository = QuestionRepository;

		}
		[HttpGet("{id}")]
		public async Task<IActionResult> GetQuestion(string id)
		{
			var question = await _repository.GetQuestion(id);
			if (question == null)
			{
				return NotFound();
			}
			return Ok(question);
		}

		[HttpGet]
		public async Task<IActionResult> GetQuestions()
		{
			var questions = await _repository.GetQuestions();
			return Ok(questions);
		}
		[HttpPost]
		public async Task<IActionResult> CreateQuestion([FromBody] QuestionDto questionDto)
		{
			if (questionDto == null)
			{
				return BadRequest("QuestionDto is null");
			}

			var createdQuestion = await _repository.CreateQuestion(questionDto);
			return CreatedAtAction(nameof(GetQuestion), new { id = createdQuestion.Id }, createdQuestion);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateQuestion(string id, [FromBody] QuestionDto questionDto)
		{
			var existingQuestion = await _repository.GetQuestion(id);
			if (existingQuestion == null)
			{
				return NotFound();
			}

			var updatedQuestion = await _repository.UpdateQuestion(id, questionDto);
			return Ok(updatedQuestion);
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteQuestion(string id)
		{
			var existingQuestion = await _repository.GetQuestion(id);
			if (existingQuestion == null)
			{
				return NotFound();
			}

			var result = await _repository.DeleteQuestion(id);
			if (result)
			{
				return NoContent();
			}
			else
			{
				return StatusCode(500, "Failed to delete question");
			}
		}

	}
}

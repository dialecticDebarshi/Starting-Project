using Newtonsoft.Json;

namespace Starting_Project.Models
{
	public class QuestionDto
	{
		public string Id { get; set; }
		public string Text { get; set; }

		[JsonProperty("type")]
		public string Type { get; set; }
	}
}

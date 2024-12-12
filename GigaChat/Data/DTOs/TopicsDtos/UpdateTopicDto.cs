using System.ComponentModel.DataAnnotations;

namespace GigaChat.Data.DTOs.TopicsDtos
{
	public class UpdateTopicDto
	{
		[Required]
		[MinLength(3), MaxLength(500)]
		public string Description { get; set; }
	}
}

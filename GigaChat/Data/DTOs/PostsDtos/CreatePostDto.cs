using System.ComponentModel.DataAnnotations;

namespace GigaChat.Data.DTOs.PostsDtos
{
	public class CreatePostDto
	{
		[Required]
		[MinLength(2), MaxLength(200)]
		public string Title { get; set; }

		[Required]
		[MinLength(3), MaxLength(5000)]
		public string Body { get; set; }
	}
}

using System.ComponentModel.DataAnnotations;

namespace GigaChat.Data.DTOs.PostsDtos
{
	public class UpdatePostDto
	{
		[Required]
		[MinLength(3), MaxLength(5000)]
		public string Body { get; set; }
	}
}

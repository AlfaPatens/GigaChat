using System.ComponentModel.DataAnnotations;

namespace GigaChat.Data.DTOs.CommentsDtos
{
	public class CreateCommentDto
	{
		[Required]
		[MinLength(1), MaxLength(1000)]
		public string Content { get; set; }
	}
}

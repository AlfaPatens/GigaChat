using System.ComponentModel.DataAnnotations;

namespace GigaChat.Data.DTOs.CommentsDtos
{
	public class UpdateCommentDto
	{
		[Required]
		[MinLength(1), MaxLength(1000)]
		public string Content { get; set; }
	}
}

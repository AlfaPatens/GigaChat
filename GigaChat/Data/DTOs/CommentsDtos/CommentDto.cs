namespace GigaChat.Data.DTOs.CommentsDtos
{
	public class CommentDto
	{
		public int CommentId { get; set; }
		public string Content { get; set; }
		public DateTime CreatedAt { get; set; }
		public int PostId { get; set; }
	}
}

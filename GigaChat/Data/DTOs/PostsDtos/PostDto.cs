namespace GigaChat.Data.DTOs.PostsDtos
{
	public class PostDto
	{
		public int PostId { get; set; }
		public string Content { get; set; }
		public DateTime CreatedAt { get; set; }
		public int TopicId { get; set; }
	}
}

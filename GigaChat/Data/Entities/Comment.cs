using GigaChat.Auth.Model;

namespace GigaChat.Data.Entities
{
	public class Comment
	{
		public int Id { get; set; }
		public required string Content { get; set; }
		public required DateTime CreationDate { get; set; }

		// Foreign key
		public int PostId { get; set; }
		public Post Post { get; set; }

		// Relationship to ForumUser
		public required string UserId { get; set; }
		public ForumUser User { get; set; }
	}
}

using GigaChat.Auth.Model;

namespace GigaChat.Data.Entities
{
	public class Post
	{
		public int Id { get; set; }
		public required string Title { get; set; }
		public required string Body { get; set; }
		public required DateTime CreationDate { get; set; }

		//Foreign key
		public int TopicId { get; set; }
		public Topic Topic { get; set; }

		// Navigation properties
		public ICollection<Comment> Comments { get; set; }

		// Relationship to ForumUser
		public required string UserId { get; set; }
		public ForumUser User { get; set; }
	}
}

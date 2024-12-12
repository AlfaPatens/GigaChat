using GigaChat.Auth.Model;
using Microsoft.Extensions.Hosting;

namespace GigaChat.Data.Entities
{
	public class Topic
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public DateTime CreationDate { get; set; }

		// Only can be set/seen by admin
		public bool IsHidden { get; set; }

		public ICollection<Post> Posts { get; set; }

		public required string UserID { get; set; }
		public ForumUser User { get; set; }
	}
}
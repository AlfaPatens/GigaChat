using GigaChat.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace GigaChat.Auth.Model
{
	public class ForumUser : IdentityUser
	{
		public ICollection<Topic> Topics { get; set; }
		public ICollection<Post> Posts { get; set; }
		public ICollection<Comment> Comments { get; set; }
	}
}

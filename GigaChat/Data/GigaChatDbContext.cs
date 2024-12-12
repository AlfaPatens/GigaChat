using GigaChat.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using GigaChat.Auth.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace GigaChat.Data
{
	public class GigaChatDbContext : IdentityDbContext<ForumUser>
	{
		public GigaChatDbContext(DbContextOptions<GigaChatDbContext> options) : base(options) { }

		public DbSet<Topic> Topics { get; set; }
		public DbSet<Post> Posts { get; set; }
		public DbSet<Comment> Comments { get; set; }
		public DbSet<Session> Sessions { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// Define relationships and configurations
			modelBuilder.Entity<Topic>()
			   .HasOne(t => t.User)
			   .WithMany(u => u.Topics)
			   .HasForeignKey(t => t.UserID)
			   .OnDelete(DeleteBehavior.Restrict); // Prevent user deletion from cascading

			// One-to-Many relationship between Topic and Post with Cascade delete
			modelBuilder.Entity<Post>()
				.HasOne(p => p.Topic)
				.WithMany(t => t.Posts)
				.HasForeignKey(p => p.TopicId)
				.OnDelete(DeleteBehavior.Cascade); // Deleting a Topic deletes its Posts

			// Define relationship between Post and ForumUser
			modelBuilder.Entity<Post>()
				.HasOne(p => p.User)
				.WithMany(u => u.Posts)
				.HasForeignKey(p => p.UserId)
				.OnDelete(DeleteBehavior.Restrict);

			// One-to-Many relationship between Post and Comment with Cascade delete
			modelBuilder.Entity<Comment>()
				.HasOne(c => c.Post)
				.WithMany(p => p.Comments)
				.HasForeignKey(c => c.PostId)
				.OnDelete(DeleteBehavior.Cascade); // Deleting a Post deletes its Comments

			// Define relationship between Comment and ForumUser
			modelBuilder.Entity<Comment>()
				.HasOne(c => c.User)
				.WithMany(u => u.Comments)
				.HasForeignKey(c => c.UserId)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}
using GigaChat.Auth.Model;
using GigaChat.Data;
using GigaChat.Data.DTOs.TopicsDtos;
using GigaChat.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace GigaChat.Controllers
{
	[ApiController]
	[Route("api/v1/[controller]")]
	public class TopicsController : ControllerBase
	{
		private readonly GigaChatDbContext _context;

		public TopicsController(GigaChatDbContext context)
		{
			_context = context;
		}

		// GET api/v1/topics
		// 
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Topic>>> GetAllTopics()
		{
			return Ok(await _context.Topics.ToListAsync());
		}

		// GET api/v1/topics/{topicId}
		[HttpGet("{topicId}")]
		public async Task<ActionResult<Topic>> GetTopic(int topicId)
		{
			var topic = await _context.Topics.FindAsync(topicId);

			if (topic == null)
			{
				return NotFound();
			}

			return Ok(topic);
		}

		// POST api/v1/topics
		[Authorize(Roles = ForumRoles.ForumUser)]
		[HttpPost]
		public async Task<ActionResult<Topic>> CreateTopic([FromBody] CreateTopicDto createTopicDto)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var topic = new Topic
			{
				Title = createTopicDto.Title,
				Description = createTopicDto.Description,
				CreationDate = DateTime.UtcNow,
				UserID = HttpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub)
			};

			_context.Topics.Add(topic);
			await _context.SaveChangesAsync();

			return CreatedAtAction(nameof(GetTopic), new { topicId = topic.Id }, topic);
		}

		// PUT api/v1/topics/{topicId}
		[Authorize]
		[HttpPut("{topicId}")]
		public async Task<ActionResult<Topic>> UpdateTopic(int topicId, [FromBody] UpdateTopicDto updateTopicDto)
		{
			var topic = await _context.Topics.FindAsync(topicId);

			if (topic == null)
			{
				return NotFound();
			}

			if (!HttpContext.User.IsInRole(ForumRoles.Admin) &&
				HttpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub) != topic.UserID)
			{
				return Forbid();
			}

			topic.Description = updateTopicDto.Description;

			_context.Entry(topic).State = EntityState.Modified;
			await _context.SaveChangesAsync();

			return Ok(topic);
		}

		// DELETE api/v1/topics/{topicId}
		[Authorize]
		[HttpDelete("{topicId}")]
		public async Task<IActionResult> DeleteTopic(int topicId)
		{
			var topic = await _context.Topics.Include(t => t.Posts).ThenInclude(p => p.Comments).FirstOrDefaultAsync(t => t.Id == topicId);

			if (topic == null)
			{
				return NotFound();
			}

			if (!HttpContext.User.IsInRole(ForumRoles.Admin) &&
			  HttpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub) != topic.UserID)
			{
				return Forbid();
			}
			_context.Topics.Remove(topic);
			await _context.SaveChangesAsync();

			return NoContent();
		}
	}
}

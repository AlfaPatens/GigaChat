using GigaChat.Data.DTOs;
using GigaChat.Data.Entities;
using AutoMapper;
using GigaChat.Data.DTOs.CommentsDtos;
using GigaChat.Data.DTOs.PostsDtos;
using GigaChat.Data.DTOs.TopicsDtos;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace api.Data.Mappings
{
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			// Topic -> DTO Mappings
			CreateMap<Topic, CreateTopicDto>().ReverseMap(); // CreateTopicDto to Topic and vice versa
			CreateMap<Topic, UpdateTopicDto>().ReverseMap(); // UpdateTopicDto to Topic and vice versa
			CreateMap<Topic, TopicDto>().ReverseMap();       // TopicDto (you can create one for detailed return views)

			// Post -> DTO Mappings
			CreateMap<Post, CreatePostDto>().ReverseMap();
			CreateMap<Post, UpdatePostDto>().ReverseMap();
			CreateMap<Post, PostDto>().ReverseMap();

			// Comment -> DTO Mappings
			CreateMap<Comment, CreateCommentDto>().ReverseMap();
			CreateMap<Comment, UpdateCommentDto>().ReverseMap();
			CreateMap<Comment, CommentDto>().ReverseMap();
		}
	}
}
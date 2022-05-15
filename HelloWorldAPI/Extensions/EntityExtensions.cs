using HelloWorldAPI.Contracts.V1.Responses;
using HelloWorldAPI.Domain.Database;
using HelloWorldAPI.Services;

namespace HelloWorldAPI.Extensions
{
    public static class EntityExtensions
    {
        public static ArticleResponse ToResponse(this Article article)
        {
            return new ArticleResponse
            {
                CreatedAt = article.CreatedAt,
                Content = article.Content,
                CreatorId = article.CreatorId,
                DiscussionId = article.DiscussionId,
                Id = article.Id,
                UserLikedIds = article.UserLiked.Select(x => x.Id).ToList()
            };
        }

        public static CommentResponse ToResponse(this Comment comment)
        {
            return new CommentResponse
            {
                Id = comment.Id,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt,
                CreatorId = comment.CreatorId,
                PostId = comment.PostId,
                Replies = comment.Replies.Select(x => x.ToResponse()).ToList(),
                UserLikedIds = comment.UserLiked.Select(x => x.Id).ToList()
            };
        }

        public static DiscussionResponse ToResponse(this Discussion discussion)
        {
            return new DiscussionResponse
            {
                Articles = discussion.Articles.Select(x => x.ToResponse()).ToList(),
                CreatedAt = discussion.CreatedAt,
                CreatorId = discussion.CreatorId,
                Id = discussion.Id,
                StartMessage = discussion.StartMessage,
                Tags = discussion.Tags.Select(x => x.ToResponse()).ToList(),
                Title = discussion.Title,
                UserIds = discussion.Users.Select(x => x.Id).ToList()
            };
        }

        public static PartialDiscussionResponse ToPartialResponse(this Discussion discussion)
        {
            return new PartialDiscussionResponse
            {
                CreatedAt = discussion.CreatedAt,
                CreatorId = discussion.CreatorId,
                Id = discussion.Id,
                Title = discussion.Title,
                UpdatedAt = discussion.UpdatedAt,
                Users = discussion.Users.Count
            };
        }

        public static PostResponse ToResponse(this Post post)
        {
            return new PostResponse
            {
                CreatedAt = post.CreatedAt,
                CreatorId = post.CreatorId,
                Comments = post.Comments.Count,
                Id = post.Id,
                Tags = post.Tags.Select(x => x.ToResponse()).ToList(),
                Title = post.Title,
                UpdatedAt = post.UpdatedAt,
                UserLikedIds = post.UserLiked.Select(x => x.Id).ToList()
            };
        }

        public static PartialPostResponse ToPartialResponse(this Post post)
        {
            return new PartialPostResponse
            {
                CreatedAt = post.CreatedAt,
                CreatorId = post.CreatorId,
                Comments = post.Comments.Count,
                Content = post.Content,
                Id = post.Id,
                Title = post.Title,
                Tags = post.Tags.Select(x => x.ToResponse()).ToList(),
                UpdatedAt = post.UpdatedAt,
                UserLiked = post.UserLiked.Count
            };
        }

        public static ProjectResponse ToResponse(this Project project)
        {
            return new ProjectResponse
            {
                CreatedAt = project.CreatedAt,
                CreatorId = project.CreatorId,
                Desciption = project.Desciption,
                Id = project.Id,
                MemberIds = project.Members.Select(x => x.Id).ToList(),
                Tags = project.Tags.Select(x => x.ToResponse()).ToList(),
                Title = project.Title,
                UserLikedIds = project.UserLiked.Select(x => x.Id).ToList()
            };
        }

        public static PartialProjectResponse ToPartialResponse(this Project project)
        {
            return new PartialProjectResponse
            {
                CreatedAt = project.CreatedAt,
                CreatorId = project.CreatorId,
                Description = project.Desciption,
                Id = project.Id,
                Members = project.Members.Count,
                Title = project.Title,
                Tags = project.Tags.Select(x => x.ToResponse()).ToList(),
                UpdatedAt = project.UpdatedAt,
                UserLiked = project.UserLiked.Count
            };
        }

        public static ReplyResponse ToResponse(this Reply reply)
        {
            return new ReplyResponse
            {
                Content = reply.Content,
                CreatorId = reply.CreatorId,
                Id = reply.Id,
                RepliedOnId = reply.RepliedOnCommentId != null ? reply.RepliedOnCommentId : reply.RepliedOnReplyId,
                Replies = reply.Replies?.Select(x => x.ToResponse()).ToList() ?? new List<ReplyResponse>()
            };
        }

        public static TagResponse ToResponse(this Tag tag)
        {
            return new TagResponse
            {
                Name = tag.Name,
            };
        }

        public static async Task<UserResponse> ToResponseAsync(this User user, IIdentityService identityService)
        {
            return new UserResponse
            {
                CreatedAt = user.CreatedAt,
                Description = user.Description,
                UpdatedAt = user.UpdatedAt,
                Discussions = user.Discussions.Select(x => x.ToPartialResponse()).ToList(),
                Email = user.Email,
                Id = user.Id,
                Posts = user.Posts.Select(x => x.ToPartialResponse()).ToList(),
                Projects = user.Projects.Select(x => x.ToPartialResponse()).ToList(),
                Roles = await identityService.GetAllRolesOfUserAsync(user),
                Tags = user.Tags.Select(x => x.ToResponse()).ToList(),
                UserName = user.UserName
            };
        }
    }
}

using API.Contracts.V1.Responses;
using API.Domain.Database;
using API.Services;
using System.Collections.Immutable;

namespace API.Extensions
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
                UserLikedIds = article.UsersLiked.Select(x => x.Id).ToList(),
                UpdatedAt = article.UpdatedAt,
                Replies = article.Replies.Select(x => x.ToResponse()).ToList()
            };
        }

        public static MinimalArticleResponse ToMinimalResponse(this Article article)
        {
            return new MinimalArticleResponse
            {
                CreatedAt = article.CreatedAt,
                Content = article.Content,
                CreatorId = article.CreatorId,
                DiscussionId = article.DiscussionId,
                Id = article.Id,
                UpdatedAt = article.UpdatedAt,
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
                UsersLikedIds = comment.UsersLiked.Select(x => x.Id).ToList()
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
                Tags = discussion.Tags.Select(x => x.Name).ToList(),
                Title = discussion.Title,
                UpdatedAt = discussion.UpdatedAt,
                UsersLikedIds = discussion.UsersLiked.Select(x => x.Id).ToList()
            };
        }

        public static PartialDiscussionResponse ToPartialResponse(this Discussion discussion)
        {
            return new PartialDiscussionResponse
            {
                CreatedAt = discussion.CreatedAt,
                CreatorId = discussion.CreatorId,
                Id = discussion.Id,
                LastArticle = discussion.Articles.OrderByDescending(x => x.UpdatedAt).FirstOrDefault()?.ToMinimalResponse(),
                Title = discussion.Title,
                UpdatedAt = discussion.UpdatedAt,
                Tags = discussion.Tags.Select(x => x.Name).ToList(),
                StartMessage = discussion.StartMessage,
                UsersLikedIds = discussion.UsersLiked.Select(x => x.Id).ToList()
            };
        }

        public static PostResponse ToResponse(this Post post, IFileManager fileManager)
        {
            return new PostResponse
            {
                Content = post.Content,
                CreatedAt = post.CreatedAt,
                CreatorId = post.CreatorId,
                Comments = post.Comments.Select(x => x.ToResponse()).ToList(),
                Id = post.Id,
                ImageUrls = post.ImagePaths.Select(x => fileManager.GetImageUrl(x.Url)).ToList(),
                Tags = post.Tags.Select(x => x.Name).ToList(),
                UpdatedAt = post.UpdatedAt,
                UsersLikedIds = post.UsersLiked.Select(x => x.Id).ToList()
            };
        }

        public static MinimalPostResponse ToMinPostResponse(this Post post, IFileManager fileManager)
        {
            return new MinimalPostResponse
            {
                Content = post.Content,
                CreatedAt = post.CreatedAt,
                CreatorId = post.CreatorId,
                Id = post.Id,
                Tags = post.Tags.Select(x => x.ToResponse()).ToList(),
                UpdatedAt = post.UpdatedAt,
                ImageUrls = post.ImagePaths.Select(x => fileManager.GetImageUrl(x.Url)).ToList()
            };
        }

        public static PartialPostResponse ToPartialResponse(this Post post, IFileManager fileManager)
        {
            return new PartialPostResponse
            {
                CreatedAt = post.CreatedAt,
                CreatorId = post.CreatorId,
                Comments = post.Comments.Count,
                Content = post.Content,
                Id = post.Id,
                ImageUrls = post.ImagePaths.Select(x => fileManager.GetImageUrl(x.Url)).ToList(),
                Tags = post.Tags.Select(x => x.Name).ToList(),
                UpdatedAt = post.UpdatedAt,
                UsersLikedIds = post.UsersLiked.Select(x => x.Id).ToList()
            };
        }

        public static ProjectResponse ToResponse(this Project project, IFileManager fileManager)
        {
            return new ProjectResponse
            {
                CreatedAt = project.CreatedAt,
                CreatorId = project.CreatorId,
                Description = project.Desciption,
                Id = project.Id,
                ImageUrls = project.ImagePaths.Select(x => fileManager.GetImageUrl(x.Url)).ToList(),
                MemberIds = project.Members.Select(x => x.Id).ToList(),
                Tags = project.Tags.Select(x => x.Name).ToList(),
                Title = project.Title,
                UsersLikedIds = project.UsersLiked.Select(x => x.Id).ToList(),
                UpdatedAt = project.UpdatedAt
            };
        }

        public static PartialProjectResponse ToPartialResponse(this Project project, IFileManager fileManager)
        {
            return new PartialProjectResponse
            {
                CreatedAt = project.CreatedAt,
                CreatorId = project.CreatorId,
                Description = project.Desciption,
                Id = project.Id,
                ImageUrls = project.ImagePaths.Select(x => fileManager.GetImageUrl(x.Url)).ToList(),
                Title = project.Title,
                Tags = project.Tags.Select(x => x.Name).ToList(),
                UpdatedAt = project.UpdatedAt,
                UsersLikedIds = project.UsersLiked.Select(x => x.Id).ToList()
            };
        }

        public static ReplyResponse ToResponse(this Reply reply)
        {
            return new ReplyResponse
            {
                Content = reply.Content,
                CreatorId = reply.CreatorId,
                CreatedAt = reply.CreatedAt,
                Id = reply.Id,
                RepliedOnId = reply.RepliedOnCommentId ?? reply.RepliedOnArticleId ?? reply.RepliedOnReplyId,
                Replies = reply.Replies?.Select(x => x.ToResponse()).ToList() ?? new List<ReplyResponse>(),
                UpdatedAt = reply.UpdatedAt,
                UserLikedIds = reply.UsersLiked.Select(x => x.Id).ToList()
            };
        }

        public static PartialReplyResponse ToPartialResponse(this Reply reply)
        {
            return new PartialReplyResponse
            {
                Content = reply.Content,
                CreatorId = reply.CreatorId,
                CreatedAt = reply.CreatedAt,
                Id = reply.Id,
                RepliedOnId = reply.RepliedOnCommentId ?? reply.RepliedOnArticleId ?? reply.RepliedOnReplyId,
                Replies = reply.Replies.Count,
                UpdatedAt = reply.UpdatedAt,
                UserLiked = reply.Replies.Count
            };
        }

        public static TagResponse ToResponse(this Tag tag)
        {
            return new TagResponse
            {
                Name = tag.Name,
                DiscussionsTaged = tag.Discussions.Count,
                PostsTaged = tag.Posts.Count,
                ProjectsTaged = tag.Projects.Count,
                UsersTaged = tag.Users.Count
            };
        }

        public static async Task<UserResponse> ToResponseAsync(this User user, IIdentityService identityService, IFileManager fileManager)
        {
            return new UserResponse
            {
                CreatedAt = user.CreatedAt,
                Description = user.Description,
                UpdatedAt = user.UpdatedAt,
                Email = user.Email,
                Id = user.Id,
                ImageUrl = fileManager.GetImageUrl(user.ImageUrl),
                Roles = await identityService.GetAllRolesOfUserAsync(user),
                Tags = user.Tags.Select(x => x.Name).ToList(),
                UserName = user.UserName
            };
        }

        public static MinimalUserResponse ToMinmalResponse(this User user, IFileManager fileManager)
        {
            return new MinimalUserResponse
            {
                Id = user.Id,
                ImageUrl = fileManager.GetImageUrl(user.ImageUrl),
                UserName = user.UserName
            };
        }

        public static async Task<PartialUserResponse> ToPartialResponseAsync(this User user, IIdentityService identityService, IFileManager fileManager)
        {
            return new PartialUserResponse
            {
                CreatedAt = user.CreatedAt,
                Description = user.Description,
                UpdatedAt = user.UpdatedAt,
                Email = user.Email,
                Id = user.Id,
                ImageUrl = fileManager.GetImageUrl(user.ImageUrl),
                Roles = await identityService.GetAllRolesOfUserAsync(user),
                UserName = user.UserName,
                Tags = user.Tags.Select(x => x.Name).ToList()
            };
        }
    }
}

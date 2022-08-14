using Microsoft.Extensions.Options;

namespace API.Contracts.V1
{
    public static class ApiRoutes
    {
        public const string Root = "api";
        public const string Version = "v1";

        public const string Base = $"{Root}/{Version}";

        public static class Image
        {
            public const string Get = Base + "/image/get/{userId}/{id}";
        }

        public static class Identity
        {
            public const string Register = Base + "/identity/register";

            public const string Refresh = Base + "/identity/refresh";

            public const string Login = Base + "/identity/login";


            public const string Create = Base + "/user/create";

            public const string Get = Base + "/user/get/{id}";

            public const string GetOwn = Base + "/user/get";

            public const string GetSaved = Base + "/user/get_saved/{id}";

            public const string GetMinimal = Base + "/user/get_minimal/{id}";

            public const string GetAll = Base + "/user/get_all";

            public const string Delete = Base + "/user/delete/{id}";

            public const string Update = Base + "/user/update";

            public const string UpdateFollowing = Base + "/user/update_following/{id}";

            public const string UpdateLogin = Base + "/user/update_login/{id}";
        }

        public static class Article
        {
            public const string Create = Base + "/article/create/{discussionId}";

            public const string Delete = Base + "/article/delete/{id}";

            public const string Update = Base + "/article/update/{id}";

            public const string UpdateRating = Base + "/article/update_rating/{id}";

            public const string Get = Base + "/article/get/{id}";

            public const string GetAll = Base + "/article/get_all";
        }

        public static class Comment
        {
            public const string Create = Base + "/comment/create/{postId}";

            public const string Update = Base + "/comment/update/{id}";

            public const string UpdateRating = Base + "/comment/update_rating/{id}";

            public const string Delete = Base + "/comment/delete/{id}";

            public const string Get = Base + "/comment/get/{id}";

            public const string GetAll = Base + "/comment/get_all";
        }

        public static class Discussion
        {
            public const string Create = Base + "/discussion/create";

            public const string Update = Base + "/discussion/update/{id}";

            public const string UpdateMembers = Base + "/discussion/update_members/{id}";

            public const string UpdateSave = Base + "/discussion/update_saving/{id}";

            public const string Delete = Base + "/discussion/delete/{id}";

            public const string DeleteAll = Base + "/discussion/delete_all";

            public const string Get = Base + "/discussion/get/{id}";

            public const string GetAll = Base + "/discussion/get_all";

            public const string UpdateRating = Base + "/discussion/update_rating/{id}";
        }

        public static class Project
        {
            public const string Create = Base + "/project/create";

            public const string Update = Base + "/project/update/{id}";

            public const string UpdateRating = Base + "/project/update_rating/{id}";

            public const string UpdateSave = Base + "/project/update_saving/{id}";

            public const string Delete = Base + "/project/delete/{id}";

            public const string DeleteAll = Base + "/project/delete_all";

            public const string Get = Base + "/project/get/{id}";

            public const string GetAll = Base + "/project/get_all";

        }

        public static class Post
        {

            public const string Create = Base + "/post/create";

            public const string Update = Base + "/post/update/{id}";

            public const string UpdateRating = Base + "/post/update_rating/{id}";

            public const string UpdateSave = Base + "/post/update_saving/{id}";

            public const string Delete = Base + "/post/delete/{id}";

            public const string DeleteAll = Base + "/post/delete_all";

            public const string Get = Base + "/post/get/{id}";

            public const string GetAll = Base + "/post/get_all";

            public const string GetAllMinimal = Base + "/post/get_all_minimal";

            public const string GetSaved = Base + "/post/get_saved/{id}";
        }

        public static class Reply
        {
            public const string CreateOnArticle = Base + "/reply/create/article/{id}";

            public const string CreateOnComment = Base + "/reply/create/comment/{id}";

            public const string CreateOnReply = Base + "/reply/create/reply/{id}";

            public const string Delete = Base + "/reply/delete/{id}";

            public const string DeleteAll = Base + "/reply/delete_all";

            public const string Update = Base + "/reply/update/{id}";

            public const string UpdateRating = Base + "/reply/update_rating/{id}";

            public const string Get = Base + "/reply/get/{id}";

            public const string GetAll = Base + "/reply/get_all";
        }

        public static class Tag
        {
            public const string GetAll = Base + "/tag/get_all";

            public const string DeleteAll = Base + "/tag/delete_all";
        }
    }
}

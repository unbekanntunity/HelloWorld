using Microsoft.Extensions.Options;

namespace HelloWorldAPI.Contracts.V1
{
    public static class ApiRoutes
    {
        public const string Root = "api";
        public const string Version = "v1";

        public const string Base = $"{Root}/{Version}";

        public static class Identity
        {
            public const string Register = Base + "/identity/register";

            public const string Refresh = Base + "/identity/refresh";

            public const string Login = Base + "/identity/login";


            public const string Create = Base + "/users/create";

            public const string Get = Base + "/users/get/{id}";

            public const string GetAll = Base + "/users/get_all";

            public const string GetIdByName = Base + "/users/get_id/{userName}";

            public const string Delete = Base + "/users/delete/{id}";

            public const string Update = Base + "/users/update/";

            public const string UpdateLogin = Base + "/users/update_login/{id}";
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

            public const string Delete = Base + "/discussion/delete/{id}";

            public const string Get = Base + "/discussion/get/{id}";

            public const string GetAll = Base + "/discussion/get_all";

            public const string GetAllFast = Base + "/discussion/get_all_fast";

        }

        public static class Project
        {
            public const string Create = Base + "/project/create";

            public const string Update = Base + "/project/update/{id}";

            public const string UpdateRating = Base + "/project/update_rating/{id}";

            public const string Delete = Base + "/project/delete/{id}";

            public const string Get = Base + "/project/get/{id}";

            public const string GetAll = Base + "/project/get_all";

        }

        public static class Post
        {
            public const string Create = Base + "/post/create";

            public const string Update = Base + "/post/update/{id}";

            public const string UpdateRating = Base + "/post/update_rating/{id}";

            public const string Delete = Base + "/post/delete/{id}";

            public const string Get = Base + "/post/get/{id}";

            public const string GetAll = Base + "/post/get_all";
        }

        public static class Reply
        {
            public const string CreateOnArticle = Base + "reply/create/article/{id}";

            public const string CreateOnComment = Base + "reply/create/comment/{id}";

            public const string CreateOnReply = Base + "reply/create/reply/{id}";

            public const string Delete = Base + "reply/delete/{id}";

            public const string Update = Base + "reply/update/{id}";

            public const string UpdateRating = Base + "reply/update_rating/{id}";

            public const string Get = Base + "reply/get/{id}";

            public const string GetAll = Base + "reply/get_all";
        }

        public static class Tag
        {
            public const string GetAll = Base + "tag/get_all";
        }
    }
}

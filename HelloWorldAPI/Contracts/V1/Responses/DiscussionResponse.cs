﻿using HelloWorldAPI.Domain.Database;

namespace HelloWorldAPI.Contracts.V1.Responses
{
    public class DiscussionResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string StartMessage { get; set; }

        public string CreatorId { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }


        public List<TagResponse> Tags { get; set; }
        public List<ArticleResponse> Articles { get; set; }
    }
}

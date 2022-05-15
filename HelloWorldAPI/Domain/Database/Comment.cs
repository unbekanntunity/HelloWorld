﻿using System.ComponentModel.DataAnnotations;

namespace HelloWorldAPI.Domain.Database
{
    public class Comment : IRateable
    {
        [Key]
        public Guid Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string CreatorId { get; set; }
        public User Creator { get; set; }

        public Guid PostId { get; set; }
        public Post Post { get; set; }

        public List<User> UserLiked { get; set; } = new();

        public List<Reply> Replies { get; set; } = new();
    }
}

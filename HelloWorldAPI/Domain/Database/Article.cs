using System.ComponentModel.DataAnnotations;

namespace HelloWorldAPI.Domain.Database
{
    public class Article : IRateable
    {
        [Key]
        public Guid Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string CreatorId { get; set; }
        public User Creator { get; set; }

        public Guid DiscussionId { get; set; }
        public Discussion Discussion { get; set; }

        public List<User> UserLiked { get; set; } = new();

        //public Guid RepliedOnId { get; set; }
        //public Article RepliedOn { get; set; }

        //public List<Article> Replies { get; set; }
    }
}

//eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJBZG1pbkBnbWFpbC5jb20iLCJuYW1lIjoiQWRtaW4iLCJqdGkiOiI1ZGZiOTUxOS0wNzlkLTRlNjMtYjc3OS1iNWVjNzVjNWM2YTUiLCJlbWFpbCI6IkFkbWluQGdtYWlsLmNvbSIsImlkIjoiNjA3YzhiOTctMjQ0Ni00ZTdiLWFjMjAtMjYwOTQ5YjE1MDJlIiwicm9sZSI6IlJvb3RBZG1pbiIsIm5iZiI6MTY1MTY2MTQ5MSwiZXhwIjoxNjUxNjk4MTM2LCJpYXQiOjE2NTE2NjE0OTF9.ovpKj3N743-vKG5Kn-gvik8nkJH7LtcT6EEb6rdxG64
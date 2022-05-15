namespace HelloWorldAPI.Domain.Database
{
    public interface IRateable
    {
        public List<User> UserLiked { get; set; }
    }
}

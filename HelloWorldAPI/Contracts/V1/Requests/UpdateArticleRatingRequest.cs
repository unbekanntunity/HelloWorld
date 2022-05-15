namespace HelloWorldAPI.Contracts.V1.Requests
{
    public class UpdateArticleRatingRequest
    {
        public int LikeChange { get; set; }
        public int DislikeChange { get; set; }
    }
}

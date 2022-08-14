namespace API.Contracts.V1.Requests
{
    public class UpdatePostReqest
    {
        public string Content { get; set; }
        public List<string> TagNames { get; set; }

    }
}

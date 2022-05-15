namespace HelloWorldAPI.Domain
{
    public class Result
    {
        public bool Success { get; set; }
        
        public IEnumerable<string> Errors { get; set; }
    }

    public class Result<T> : Result
    {
        public T? Data { get; set; }
    }
}

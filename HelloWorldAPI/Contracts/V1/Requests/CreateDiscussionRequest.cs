﻿namespace HelloWorldAPI.Contracts.V1.Requests
{
    public class CreateDiscussionRequest
    {
        public string Title { get; set; }
        public string StartMessage { get; set; }
        public List<string> TagNames { get; set; }
    }
}

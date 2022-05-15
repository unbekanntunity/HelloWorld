using Testing2.Domain;
using Testing2.Services;

namespace Testing2.MockingControlers
{
    public class TestController
    {
        private readonly ITestService _service;
        public List<int> MyProperty;

        public TestController(ITestService service)
        {
            _service = service;
        }

        public TestController()
        {
        }


        public void CreateDiscussion(DiscussionRequest test)
        {
            var convTest = new Discussion
            {
                Content = test.Content,
                Tags = test.Tags.Select(x => new Tag { TagName = x }).ToList()
            };

            _service.CreateDiscussion(convTest);
        }

        public void GetEntities()
        {
            var result = _service.GetEntities();
            Console.WriteLine(result);
        }

        public void DeleteAll()
        {
            _service.DeleteAll();
        }
    }
}

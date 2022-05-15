using Testing2.Domain;

namespace Testing2.Services
{
    public interface ITestService
    {
        void CreateDiscussion(Discussion discussion);
        void DeleteAll();
        (List<Discussion>, List<Tag>) GetEntities();
    }
}

using Microsoft.EntityFrameworkCore;
using Testing2.Data;
using Testing2.Domain;
using Testing2.Extensions;

namespace Testing2.Services
{
    public class TestService : ITestService
    {
        private readonly ApplicationDbContext _context;

        public TestService(ApplicationDbContext context)
        {
            _context = context;
        }

        public void CreateDiscussion(Discussion discussion)
        {
            var existingTags = _context.Tags.AsNoTracking().ToList();

            for (int i = 0; i < discussion.Tags.Count; i++)
            {
                var tag = discussion.Tags[i];
                var tagInDb = _context.Tags.FirstOrDefault(x => x.TagName == tag.TagName);

                if (tagInDb != null)
                {
                    tagInDb.Discussions.Add(discussion);
                    discussion.Tags.Remove(tag);
                    _context.SaveChanges();

                    return;
                }
            }


            _context.Discussions.Add(discussion);
            _context.SaveChanges();
        }

        public void DeleteAll()
        {
            _context.Tags.Clear();
            _context.Discussions.Clear();
            _context.SaveChanges();
        }

        public (List<Discussion>, List<Tag>) GetEntities() => (_context.Discussions.Include(x => x.Tags).ToList(), _context.Tags.Include(x => x.Discussions).ToList());
    }
}

using System.ComponentModel.DataAnnotations;

namespace Testing2.Domain
{
    public class Tag
    {
        [Key]
        public string TagName { get; set; }
        public List<Discussion> Discussions { get; set; }
    }
}

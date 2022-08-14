namespace API.Domain.Database.Interfaces
{
    public interface ISavable
    {
        public List<User> SavedBy { get; set; }
    }
}

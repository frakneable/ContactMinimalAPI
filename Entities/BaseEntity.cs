namespace ContactMinimalAPI.Models
{
    public class BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime DeletedAt { get; set; }
    }
}

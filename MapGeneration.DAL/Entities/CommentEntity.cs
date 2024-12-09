using MapGeneration.DAL.Entities.Users;

namespace MapGeneration.DAL.Entities;

public class CommentEntity
{
    public Guid Id { get; set; }
    public Guid AuthorId { get; set; }
    public UserEntity Author { get; set; }
    public Guid MapId { get; set; }
    public MapEntity Map { get; set; }
    public string Value { get; set; }
    public DateTime DateTime { get; set; }
}
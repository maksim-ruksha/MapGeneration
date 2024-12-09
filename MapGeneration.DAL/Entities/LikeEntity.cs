using MapGeneration.DAL.Entities.Users;

namespace MapGeneration.DAL.Entities;

public class LikeEntity
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public UserEntity Owner { get; set; }
    public Guid MapId { get; set; }
    public MapEntity Map { get; set; }
}
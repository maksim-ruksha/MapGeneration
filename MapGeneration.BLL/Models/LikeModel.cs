using MapGeneration.BLL.Models.Users;
using MapGeneration.DAL.Entities;

namespace MapGeneration.BLL.Models;

public class LikeModel
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public UserModel Owner { get; set; }
    public Guid MapId { get; set; }
    public MapEntity Map { get; set; }
}
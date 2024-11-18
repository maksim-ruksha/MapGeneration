using MapGeneration.BLL.Models.Users;

namespace MapGeneration.BLL.Models;

public class CommentModel
{
    public Guid Id { get; set; }
    public UserModel Author { get; set; }
    public MapModel Map { get; set; }
    public string Value { get; set; }
}
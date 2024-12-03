using MapGeneration.BLL.Models.Users;

namespace MapGeneration.BLL.Models;

public class MapModel
{
    public Guid Id { get; set; }
    public string Seed { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public UserModel Author { get; set; }
    public DateTime DateTime { get; set; }
}
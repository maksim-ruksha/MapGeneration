namespace MapGeneration.BLL.Models.Users;

public class UserModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public UserRole Role { get; set; }
    public string PasswordHash { get; set; }
    public List<MapModel> Maps { get; set; }
}
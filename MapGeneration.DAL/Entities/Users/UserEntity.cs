namespace MapGeneration.DAL.Entities.Users;

public class UserEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public UserRole Role { get; set; }
    public string PasswordHash { get; set; }
}
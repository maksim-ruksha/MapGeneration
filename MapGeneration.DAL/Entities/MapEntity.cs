namespace MapGeneration.DAL.Entities;

public class MapEntity
{
    public Guid Id { get; set; }
    public string Seed { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public UserEntity Author { get; set; }
}
using MapGeneration.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace MapGeneration.DAL.EF;

public class DatabaseContext : DbContext
{
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<MapEntity> Maps { get; set; }
    public DbSet<CommentEntity> Comments { get; set; }
    public DbSet<LikeEntity> Likes { get; set; }

    public DatabaseContext()
    {
        Database.EnsureCreated();
    }

    public DatabaseContext(IConfiguration configuration)
    {
        Database.EnsureCreated();
    }
}
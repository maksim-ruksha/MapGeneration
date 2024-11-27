using MapGeneration.BLL.Mapping;
using MapGeneration.BLL.Models;
using MapGeneration.BLL.Models.Users;
using MapGeneration.BLL.Services;
using MapGeneration.DAL.EF;
using MapGeneration.DAL.Entities;
using MapGeneration.DAL.Entities.Users;
using Microsoft.EntityFrameworkCore;
using NLog.Extensions.Logging;
using NLog.Web;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

IServiceCollection services = builder.Services; 
IConfiguration configuration = builder.Configuration;

/*builder.Logging.ClearProviders();
builder.Host.UseNLog();*/

services.AddLogging(loggingBuilder =>
{
    loggingBuilder.ClearProviders();
    loggingBuilder.AddDebug();
    loggingBuilder.AddConsole();
    loggingBuilder.AddNLog();
});
services.AddAutoMapper(typeof(MappingProfile));

services.AddScoped<INoiseGenerationService, NoiseGenerationService>();
services.AddScoped<IMapGenerationService, MapGenerationService>();

services.AddScoped<IRepository<UserEntity>, Repository<UserEntity>>();
services.AddScoped<IRepository<MapEntity>, Repository<MapEntity>>();
services.AddScoped<IRepository<LikeEntity>, Repository<LikeEntity>>();
services.AddScoped<IRepository<CommentEntity>, Repository<CommentEntity>>();

services.AddScoped<IService<UserModel, UserEntity>, Service<UserModel, UserEntity>>();
services.AddScoped<IService<MapModel, MapEntity>, Service<MapModel, MapEntity>>();
services.AddScoped<IService<LikeModel, LikeEntity>, Service<LikeModel, LikeEntity>>();
services.AddScoped<IService<CommentModel, CommentEntity>, Service<CommentModel, CommentEntity>>();

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
services.AddDbContext<DatabaseContext>(optionsActions => optionsActions.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
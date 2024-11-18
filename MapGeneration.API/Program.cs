using MapGeneration.BLL.Models;
using MapGeneration.BLL.Models.Users;
using MapGeneration.BLL.Services;
using MapGeneration.DAL.EF;
using MapGeneration.DAL.Entities;
using MapGeneration.DAL.Entities.Users;
using Microsoft.EntityFrameworkCore;
using NLog.Web;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

IServiceCollection services = builder.Services; 
IConfiguration configuration = builder.Configuration;

builder.Logging.ClearProviders();
builder.Host.UseNLog();

services.AddScoped<IService<UserModel, UserEntity>, Service<UserModel, UserEntity>>();
services.AddScoped<IService<MapModel, MapEntity>, Service<MapModel, MapEntity>>();
services.AddScoped<IService<LikeModel, LikeEntity>, Service<LikeModel, LikeEntity>>();
services.AddScoped<IService<CommentModel, CommentEntity>, Service<CommentModel, CommentEntity>>();

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
services.AddDbContext<DatabaseContext>(options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

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
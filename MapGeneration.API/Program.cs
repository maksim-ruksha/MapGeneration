using System.Text.Json.Serialization;
using MapGeneration.BLL.Mapping;
using MapGeneration.BLL.Models;
using MapGeneration.BLL.Models.Users;
using MapGeneration.BLL.Services;
using MapGeneration.BLL.Services.Auth;
using MapGeneration.DAL.EF;
using MapGeneration.DAL.Entities;
using MapGeneration.DAL.Entities.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

IServiceCollection services = builder.Services;
IConfiguration configuration = builder.Configuration;


/*builder.Logging.ClearProviders();
builder.Host.UseNLog();

services.AddLogging(loggingBuilder =>
{
    loggingBuilder.ClearProviders();
    loggingBuilder.AddDebug();
    loggingBuilder.AddConsole();
    loggingBuilder.AddNLog();
});*/


services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = Jwt.Issuer,
            ValidateAudience = true,
            ValidAudience = Jwt.Audience,
            ValidateLifetime = true,
            IssuerSigningKey = Jwt.GetSymmetricSecurityKey(),
            ValidateIssuerSigningKey = true
        };
    });


services.AddAutoMapper(typeof(MappingProfile));

services.AddSingleton<INoiseGenerationService, NoiseGenerationService>();
services.AddSingleton<IMapGenerationService, MapGenerationService>();
services.AddScoped<IAuthService, AuthService>();
services.AddScoped<IPasswordHasher<UserModel>, PasswordHasher<UserModel>>();

services.AddScoped<IRepository<UserEntity>, Repository<UserEntity>>();
services.AddScoped<IRepository<MapEntity>, Repository<MapEntity>>();
services.AddScoped<IRepository<LikeEntity>, Repository<LikeEntity>>();
services.AddScoped<IRepository<CommentEntity>, Repository<CommentEntity>>();

services.AddScoped<IService<UserModel, UserEntity>, Service<UserModel, UserEntity>>();
services.AddScoped<IService<MapModel, MapEntity>, Service<MapModel, MapEntity>>();
services.AddScoped<IMapService, MapService>();
services.AddScoped<IService<LikeModel, LikeEntity>, Service<LikeModel, LikeEntity>>();
services.AddScoped<IService<CommentModel, CommentEntity>, Service<CommentModel, CommentEntity>>();

services.AddScoped<IService<CommentModel, CommentEntity>, Service<CommentModel, CommentEntity>>();

services.AddControllers(options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true)
    .AddJsonOptions(x =>
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);;
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.AddDbContext<DatabaseContext>(optionsActions =>
{
    optionsActions.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
});

WebApplication app = builder.Build();

app.UseCors(policyBuilder => policyBuilder.WithOrigins("http://localhost:3000")
    .AllowAnyMethod()
    .AllowAnyHeader());

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
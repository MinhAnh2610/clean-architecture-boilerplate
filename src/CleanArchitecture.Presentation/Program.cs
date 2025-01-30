using CleanArchitecture.Application;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Infrastructure;
using CleanArchitecture.Infrastructure.Data;
using CleanArchitecture.Infrastructure.Data.Extensions;
using CleanArchitecture.Presentation;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
  .AddApiServices(builder.Configuration)
  .AddInfrastructureServices(builder.Configuration)
  .AddApplicationServices(builder.Configuration);

// Add identity
builder.Services.AddIdentity<User, Role>(options =>
{
  options.Password.RequiredLength = 5;
  options.Password.RequireNonAlphanumeric = false;
  options.Password.RequireUppercase = false;
  options.Password.RequireLowercase = false;
  options.Password.RequireDigit = false;
  options.Password.RequiredUniqueChars = 0;
})
  .AddEntityFrameworkStores<ApplicationDbContext>()
  .AddDefaultTokenProviders()
  .AddUserStore<UserStore<User, Role, ApplicationDbContext>>()
  .AddRoleStore<RoleStore<Role, ApplicationDbContext>>();

// Add authentication & authorization
builder.Services.AddAuthentication("Bearer")
  .AddJwtBearer("Bearer", options =>
  {
    options.Authority = "https://localhost:5051";
    options.TokenValidationParameters = new TokenValidationParameters
    {
      ValidateAudience = false,
    };
  });

builder.Services.AddAuthorization(options =>
{
  options.AddPolicy("ClientIdPolicy", policy => policy.RequireClaim("client_id", "client"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
  await app.InitializeDatabaseAsync();
}

app.UseApiServices();

app.UseHttpsRedirection();

app.UseRouting();

app.UseIdentityServer();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

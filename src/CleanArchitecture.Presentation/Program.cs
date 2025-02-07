using CleanArchitecture.Application;
using CleanArchitecture.Infrastructure;
using CleanArchitecture.Infrastructure.Data.Extensions;
using CleanArchitecture.Presentation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
  .AddApplicationServices(builder.Configuration)
  .AddInfrastructureServices(builder.Configuration)
  .AddApiServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
  await app.InitializeDatabaseAsync();
}

app.UseApiServices();

app.Run();

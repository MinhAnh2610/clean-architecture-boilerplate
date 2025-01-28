using CleanArchitecture.Application;
using CleanArchitecture.Infrastructure;
using CleanArchitecture.Presentation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
  .AddApiServices(builder.Configuration)
  .AddInfrastructureServices(builder.Configuration)
  .AddApplicationServices(builder.Configuration);

var app = builder.Build();

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

using Api;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .ConfigureDb(builder.Configuration)
    .ConfigureUseCases()
    .AddControllers()
    .AddSwaggerDocumentation()
    .AddAndConfigureControllers();

var app = builder.Build();

app.Services.CreateScope().ServiceProvider.GetRequiredService<EcommerceDbContext>().Database.Migrate();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();


app.Run();

public partial class Program { }
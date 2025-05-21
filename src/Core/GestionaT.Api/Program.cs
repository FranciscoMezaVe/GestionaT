using Serilog;
using GestionaT.Persistence;
using GestionaT.Application;
using GestionaT.Infraestructure;
using GestionaT.Infraestructure.Seeds;
using GestionaT.Persistence.PGSQL;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Persistence
builder.Services.AddPersistence();

//Application
builder.Services.AddApplicationLayer();

//Infraestructure
builder.Services.AddInfraestructureLayer(builder.Configuration);

builder.Host.UseSerilog((context, loggerConfig) =>
    loggerConfig.ReadFrom.Configuration(context.Configuration));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseSerilogRequestLogging();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<AppPostgreSqlDbContext>();
    await context.Database.MigrateAsync();

    // Seed the database with initial data
    await IdentitySeeder.SeedAdminUserAsync(services);
}

app.Run();

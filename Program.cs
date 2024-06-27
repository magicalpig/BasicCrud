using Microsoft.EntityFrameworkCore;
using BasicCrud.Persistence;
using BasicCrud.Dtos;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddDbContext<DataContext>(opt =>
    {
        opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
    })
    .AddAutoMapper(typeof(AutoMapperProfiles).Assembly)
    .AddControllers()
    .AddJsonOptions(options =>
    {
        // allows enum values to come in as strings in requests
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapControllers();


// stand up the database and seed it
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var context = services.GetRequiredService<DataContext>();
        await context.Database.MigrateAsync();
        var config = services.GetRequiredService<IConfiguration>();
        await Seeder.SeedData(context, config);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occured during database migration or seeding");
    }
}

app.Run();

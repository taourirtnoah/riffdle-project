using Microsoft.EntityFrameworkCore;
using Riffdle.Data;
using Riffdle.Models.Mock;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContextFactory<RiffdleDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("RiffdleDbContext")));
builder.Services.AddSingleton<GenreMockRepository>();
builder.Services.AddSingleton<BandMockRepository>();
builder.Services.AddSingleton<AlbumMockRepository>();
builder.Services.AddSingleton<SongMockRepository>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    try
    {
        var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<RiffdleDbContext>>();
        using var dbContext = dbContextFactory.CreateDbContext();
        dbContext.Database.EnsureCreated();
        RiffdleSeeder.Seed(dbContext);
    }
    catch (Exception exception)
    {
        Console.WriteLine($"Database initialization skipped: {exception.Message}");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();

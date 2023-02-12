using Microsoft.EntityFrameworkCore;
using TastyCook.RecepiesAPI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//builder.Services.AddDbContext<RecipesContext>(options =>
//{
//    options.UseSqlServer(builder.Configuration.GetConnectionString("RecipeContext"));
//});

#if DEBUG
Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Debug");
#else
    Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Production");
#endif

if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
{
    builder.Services.AddDbContext<RecipesContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("RecipeContextProd")));
}
else
{
    //builder.Services.AddDbContext<RecipesContext>(options =>
    //    options.UseSqlServer(builder.Configuration.GetConnectionString("RecipeContext")));
    builder.Services.AddDbContext<RecipesContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("RecipeContextProd")));
}

// To apply migration automatically.
//builder.Services.BuildServiceProvider().GetService<RecipesContext>().Database.Migrate();


builder.Services.AddScoped<RecipeService, RecipeService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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

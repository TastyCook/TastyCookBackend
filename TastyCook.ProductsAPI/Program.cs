using System.Reflection;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TastyCook.ProductsAPI;
using TastyCook.ProductsAPI.Settings;
using TastyCook.ProductsAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
#if DEBUG
Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Debug");
#else
    Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Production");
#endif

if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
{
    builder.Services.AddDbContext<ProductsContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("RecipeContextProd")));
}
else
{
    //builder.Services.AddDbContext<RecipesContext>(options =>
    //    options.UseSqlServer(builder.Configuration.GetConnectionString("RecipeContext")));
    builder.Services.AddDbContext<ProductsContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("RecipeContextProd")));
}
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<UserService>();
// To apply migration automatically.
//builder.Services.BuildServiceProvider().GetService<RecipesContext>().Database.Migrate();

builder.Services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
{
    builder.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
}));

builder.Services.AddMassTransit(x =>
{
    var entryAssembly = Assembly.GetExecutingAssembly();
    x.AddConsumers(entryAssembly);
    x.UsingRabbitMq((context, configurator) =>
    {
        var rabbitMqSettings = builder.Configuration.GetSection(RabbitMQSettingsOptions.RabbitMQSettings).Get<RabbitMQSettingsOptions>();
        configurator.Host(rabbitMqSettings.Host, "/", c =>
        {
            c.Username(rabbitMqSettings.UserName);
            c.Password(rabbitMqSettings.Password);
        });
        configurator.Host(rabbitMqSettings.Host);
        configurator.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter("Recipes", false));
    });
});
builder.Services.AddMassTransitHostedService();

builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
});

builder.Services.AddAuthorization();

var jwtConfig = builder.Configuration.GetSection("JwtConfig");
builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtConfig["Issuer"],
            ValidAudience = jwtConfig["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig["Secret"]))
        };
    });

builder.Services.AddControllers();
//    .AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

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

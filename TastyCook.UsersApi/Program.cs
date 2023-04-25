using System.Text;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TastyCook.UsersAPI;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using TastyCook.UsersAPI.Entities;
using Microsoft.Extensions.Options;
using TastyCook.UsersAPI.Settings;


var builder = WebApplication.CreateBuilder(args);


#if DEBUG
//Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Debug");
#else
    Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Production");
#endif

if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
{
    builder.Services.AddDbContext<UserContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("UserContextProd")));
}
else
{
    //builder.Services.AddDbContext<RecipesContext>(options =>
    //    options.UseSqlServer(builder.Configuration.GetConnectionString("UserContextDev")));
    builder.Services.AddDbContext<UserContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("UserContextProd")));
}


builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, configurator) =>
    {
        var rabbitMqSettings = builder.Configuration.GetSection(RabbitMQSettingsOptions.RabbitMQSettings).Get<RabbitMQSettingsOptions>();
        //configurator.Host(rabbitMqSettings.Host, "/", c => {
        //    c.Username(rabbitMqSettings.UserName);
        //    c.Password(rabbitMqSettings.Password);
        //});
        configurator.Host(rabbitMqSettings.Host);
        configurator.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter("Users", false));
    });
});
builder.Services.AddMassTransitHostedService();


builder.Services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
{
    builder.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
}));

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<UserContext>()
    .AddDefaultTokenProviders();
builder.Services.Configure<DataProtectionTokenProviderOptions>(opt =>
    opt.TokenLifespan = TimeSpan.FromHours(2));

builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings.
    options.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = false;
});

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
        opt.RequireHttpsMetadata = false;
        opt.SaveToken = true;
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

builder.Services.AddSwaggerGen(options => { options.SwaggerDoc("v1", new OpenApiInfo { Title = "TastyCook", Version = "v1" }); }
);


builder.Services.AddControllers();
var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();

    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TastyCook v1"));
}

app.UseCors("MyPolicy");
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();


app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
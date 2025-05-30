using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using api.Data;
using api.Helpers;
using api.Interceptor;
using api.Interfaces;
using api.Models;
using api.Repositories;
using api.Seeder;
using api.Service;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;

// using api.Interfaces;
// using api.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name:
        MyAllowSpecificOrigins,
        policy =>
        {
            policy.AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .WithOrigins("http://localhost:5173");
        }
    );
});

builder.Services.AddHttpsRedirection(options =>
{
    options.HttpsPort = 9261; // match docker port
});

//To make Json recurring doesnt happen
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });

//Connection

var env = builder.Environment.EnvironmentName;

string? connectionString;

if (env == Environments.Development)
{
    var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    connectionString = builder.Configuration.GetConnectionString(
        isWindows ? "WindowsConnection" : "LinuxConnection"
    );
}
else
{
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
}

builder.Services.AddDbContext<ApplicationDbContext>((options) =>
{
    options.UseSqlServer(connectionString)
        .AddInterceptors(new SlugInterceptor())
        .AddInterceptors(new SeriesFormatInterceptor());
});

//redis caching
builder.Services.AddStackExchangeRedisCache(options =>
 {
     options.Configuration = builder.Configuration.GetConnectionString("Redis");
     options.InstanceName = "SampleInstance";
 });

//Cached Memory
builder.Services.AddMemoryCache();

//Interface and Repository cached
//IMemory cached
builder.Services.AddScoped<SeriesRepository>();
builder.Services.AddScoped<ISeriesRepository, CachedSeriesRepository>();

//Redis
// builder.Services.AddScoped<EpisodeRepository>();
// builder.Services.AddScoped<IEpisodeRepository, CachedEpisodeRepository>();

//Interface and Repository
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<IEpisodeRepository, EpisodeRepository>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IVideoRepository, VideoRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();

//Global exception handler
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

//Make form able to send big data, in kernel also
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 104857600; // 100MB
});

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 104857600; // 100MB
});


// builder.Services.Configure<FormOptions>(options =>
// {
//     options.MultipartBodyLengthLimit = long.MaxValue; 
// });

// builder.WebHost.ConfigureKestrel(serverOptions =>
// {
//     serverOptions.Limits.MaxRequestBodySize = long.MaxValue; 
// });

//JWT Token Auth
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 12;
})
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme =
    options.DefaultChallengeScheme =
    options.DefaultForbidScheme =
    options.DefaultScheme =
    options.DefaultSignInScheme =
    options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddCookie()
.AddGoogle(options =>
{
    var clientId = builder.Configuration["Authentication:Google:ClientId"];

    if (clientId == null)
    {
        throw new ArgumentNullException(nameof(clientId));
    }

    var clientSecret = builder.Configuration["Authentication:Google:ClientSecret"];

    if (clientSecret == null)
    {
        throw new ArgumentNullException(nameof(clientSecret));
    }

    options.ClientId = clientId;
    options.ClientSecret = clientSecret;
    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

})
.AddJwtBearer(options =>
{
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            if (context.Request.Cookies.ContainsKey("jwt"))
            {
                context.Token = context.Request.Cookies["jwt"];
            }
            return Task.CompletedTask;
        }
    };

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWT:SigningKey"])
        )
    };
});

var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<ApplicationDbContext>();

        context.Database.Migrate();
        DbSeeder.Seeder(context);
    }
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseStaticFiles();
    app.UseExceptionHandler("/error-development");
}
else
{
    app.UseExceptionHandler("/error");
    app.UseStaticFiles(
    new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(
            Path.Combine(Directory.GetCurrentDirectory(), "uploads")),
        RequestPath = "/uploads"
    });
}

app.UseCors(MyAllowSpecificOrigins);
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

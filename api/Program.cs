using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using api.Extensions;
using api.Middleware;
using api.Seeder;
using infrastructure;
using infrastructure.Data;
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

//Make form able to send big data, in kernel also
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 104857600; // 100MB
});

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 104857600; // 100MB
});

builder.Services.ConfigureInfrastructure(builder.Configuration, builder.Environment);
builder.Services.ConfigureJWTPolicy(builder.Configuration);
builder.Services.ConfigureIdentityPolicy();
builder.Services.ConfigureCorsPolicy();


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

//Global exception handler
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();


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

using FitnessAPI;
using FitnessAPI.Entities;
using FitnessAPI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<CustomDbContext>();

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<CustomDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddIdentityCore<IdentityUser>(options =>
{
    // Configure identity options if needed
})
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<CustomDbContext>();
builder.Services.AddScoped<UserManager<User>>();
builder.Services.AddScoped<SignInManager<User>>();
builder.Services.AddScoped<AuthorizationService>();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseAuthentication();

app.MapControllers();

app.Run();

/*
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // Add JWT authentication
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "your-issuer",
                    ValidAudience = "your-audience",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your-secret-key"))
                };
            });

        // Add other services to the container
        services.AddControllers();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Use authentication
        app.UseAuthentication();

        // Use routing
        app.UseRouting();

        // Use authorization
        app.UseAuthorization();

        // Use endpoints
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
*/
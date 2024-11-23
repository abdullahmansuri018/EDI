using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Azure.Cosmos;
using JsonDataApi.Services;
using System.Text;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();

        // Add Cosmos DB configuration
        builder.Services.AddSingleton<CosmosClient>(sp =>
        {
            var cosmosDbConfig = builder.Configuration.GetSection("CosmosDb");
            return new CosmosClient(cosmosDbConfig["EndpointUri"], cosmosDbConfig["PrimaryKey"]);
        });

        // Add CORS policy to allow all origins, methods, and headers
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()  // Allow any origin
                      .AllowAnyHeader()  // Allow any header
                      .AllowAnyMethod();  // Allow any HTTP method (GET, POST, etc.)
            });
        });

        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var serverSecret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:ServerSecret"]));
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = serverSecret,
                    ValidIssuer = builder.Configuration["JWT:Issuer"],
                    ValidAudience = builder.Configuration["JWT:Audience"]
                };
            });

        // Add DbContext configuration for SQL Server
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
        );

        // Register Authentication Service (For handling user registration, login, password hashing)
        builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

        // Register the DataService and IDataService for Dependency Injection
        builder.Services.AddScoped<IDataService, DataService>();

        // Configure Swagger/OpenAPI
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Enable CORS globally
        app.UseCors("AllowAll");  // Use the "AllowAll" policy for all requests

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}

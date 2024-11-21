using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Azure.Cosmos;
using System.Text;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllersWithViews();
        // Add Cosmos DB configuration
builder.Services.AddSingleton<CosmosClient>(sp =>
{
    var cosmosDbConfig = builder.Configuration.GetSection("CosmosDb");
    return new CosmosClient(cosmosDbConfig["EndpointUri"], cosmosDbConfig["PrimaryKey"]);
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


        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
        var app = builder.Build();

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


using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using PaymentApi.Services;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using PaymentApi;
using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IPaymentService, PaymentService>();

// Add Cosmos DB Client
builder.Services.AddSingleton(serviceProvider =>
{
    var cosmosConfig = builder.Configuration.GetSection("CosmosDb");
    return new CosmosClient(cosmosConfig["EndpointUri"], cosmosConfig["PrimaryKey"]);
});

// Add SQL Server Context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Azure Service Bus Client
builder.Services.AddSingleton<ServiceBusClient>(serviceProvider =>
{
    var serviceBusConnectionString = builder.Configuration["AzureServiceBus:ConnectionString"];
    return new ServiceBusClient(serviceBusConnectionString);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()  // Allow any origin
              .AllowAnyHeader()  // Allow any header
              .AllowAnyMethod();  // Allow any HTTP method (GET, POST, etc.)
    });
});

// Add JWT Authentication
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
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();  // Enable authentication middleware
app.UseAuthorization();
app.MapControllers();

app.Run();

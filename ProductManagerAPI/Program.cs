using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProductManagerAPI.EFCore;
using ProductManagerAPI.Helper;
using ProductManagerAPI.Services;
using StackExchange.Redis;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IConnectionMultiplexer>(x => ConnectionMultiplexer.Connect(builder.Configuration["RedisConnection"]));

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
string connString = builder.Configuration.GetConnectionString("Connection");

builder.Services.AddMvc();
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connString));
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;  
}).AddJwtBearer(jwtBearerOptions =>
{
    jwtBearerOptions.RequireHttpsMetadata = true;
    jwtBearerOptions.SaveToken = true;
    jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters()
    {
        ClockSkew = TimeSpan.Zero,
        ValidateActor = true,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["TokenAuthentication:Issuer"],
        ValidAudience = builder.Configuration["TokenAuthentication:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["TokenAuthentication:SecretKey"]))
    };
});

builder.Services.Add(new ServiceDescriptor(typeof(IProductService), typeof(ProductService), ServiceLifetime.Transient));

//builder.Services.AddHostedService(provider =>
//                   new RabbitMQConsumerService("decreaseQuantityQueue", "a", "b", "c", provider.GetService<IServiceProvider>()));

//builder.Services.AddHostedService<RabbitMQConsumerService>();

var app = builder.Build(); 

app.UseMiddleware(typeof(ExceptionMiddleware));
app.UseAuthentication();
app.UseAuthorization();
app.UseCors(builder => builder
     .AllowAnyOrigin()
     .AllowAnyMethod()
     .AllowAnyHeader());

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

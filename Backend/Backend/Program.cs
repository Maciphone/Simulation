using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Backend.MongoDb;
using Backend.Service.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//Mongo nuggets:
//MongoDb.Driver
//identity.MongoDbCore
//


builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));
builder.Services.AddSingleton<MongoDbContext>();

builder.Services.AddSingleton<IUserRepository>(sp =>
{
    var dbContext = sp.GetRequiredService<MongoDbContext>();
    return new UserRepository(dbContext.Database, "UserData");
});

builder.Services.AddSingleton<ISimulationStateRepository>(sp =>
{
    var dbContext = sp.GetRequiredService<MongoDbContext>();
    return new SimulationStateRepository(dbContext.Database, "SimulationStates", dbContext.SimulationStates);
});


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder
            .WithOrigins("http://localhost:5173")  // Ezt állítsd be a frontend URL-jére
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//test

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("AllowAllOrigins");


//middleware sets from  each incoming requests cookies token into authorization header
app.Use(async (context, next) =>
{
    if (context.Request.Cookies.TryGetValue("access_token", out var token))
    {
        context.Request.Headers.Append("Authorization", $"Bearer {token}");
    }
    await next();
});


app.MapPost("/api/auth/guest", (IConfiguration config, HttpContext httpContext) =>
{
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        issuer: config["Jwt:Issuer"],
        audience: config["Jwt:Audience"],
        claims: new List<Claim> { new Claim("role", "guest") },
        expires: DateTime.UtcNow.AddMinutes(30), // 
        signingCredentials: creds
    );

    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
    
    //simple jwt token authentication
    // return Results.Ok(new { token = tokenString });
    //  });
    
    //add jwt token to cookie
    httpContext.Response.Cookies.Append("access_token", tokenString, new CookieOptions
    {
        HttpOnly = true, //against xss
        Secure = true, //https is needed
        SameSite = SameSiteMode.Strict, //against CSRF
        Expires = DateTime.UtcNow.AddMinutes(30)
    } );
    
    return Results.Ok(new {message = "access guaranted" });
});


app.Run();


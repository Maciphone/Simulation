using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Backend.Controller;
using Backend.MongoDb;
using Backend.Service.Authentication;
using Backend.Service.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>();

//config teszt
var config = builder.Configuration;
Console.WriteLine($"Simulation API beállítások: {config["SimulationApi:BaseUrl"]}");


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//http client for communication with simulations api
builder.Services.AddHttpClient<BackendSimulationController>();


//authentication cookie append on http client in backendSimulatorController for authentication in simulations api
// in backendSimulationController use _httpClient = httpClientFactory.CreateClient("SimulationApiClient"), pass factory in constructor
builder.Services.AddHttpClient("SimulationApiClient", client =>
    {
        var simulationApiBaseUrl = builder.Configuration["SimulationApi:BaseUrl"] 
                                   ?? "http://localhost:5050/api/simulation";
    
        client.BaseAddress = new Uri(simulationApiBaseUrl);
    })
    .ConfigurePrimaryHttpMessageHandler(() =>
    {
        var simulationApiBaseUrl = builder.Configuration["SimulationApi:BaseUrl"] 
                                   ?? "http://localhost:5050/api/simulation";
        var simulationApiUri = new Uri(simulationApiBaseUrl);
        
        var handler = new HttpClientHandler();
        handler.UseCookies = true;
        handler.CookieContainer = new System.Net.CookieContainer();
        var role = builder.Configuration["SimulationApi:Role"];
        var cookieName = builder.Configuration["SimulationApi:CookieName"];
        var backendAuthToken = GenerateAuthTokenSimulation.GenerateAuthToken(builder.Configuration, role);
        
        handler.CookieContainer.Add(simulationApiUri, new System.Net.Cookie(cookieName, backendAuthToken));

        return handler;
    });

builder.Services.AddControllers();
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

//check db connection

//TestDb();
CheckDb();



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

//exception handler for all mongodb exeptions
//app.UseMiddleware<MongoExceptionHandlerMiddleware>();

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

app.MapControllers();
//middleware sets from  each INCOMING requests cookies token into authorization header
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
    var role = config["Jwt:GuestRole"];
    var token = GenerateAuthTokenSimulation.GenerateAuthToken(builder.Configuration, role);
    // var token = new JwtSecurityToken(
    //     issuer: config["Jwt:Issuer"],
    //     audience: config["Jwt:Audience"],
    //     claims: new List<Claim> { new Claim("role", "guest") },
    //     expires: DateTime.UtcNow.AddMinutes(30), // 
    //     signingCredentials: creds
    // );
    //var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
    
    //simple jwt token authentication
    // return Results.Ok(new { token = tokenString });
    //  });
    
    //add jwt token to cookies and pass to response headers
    httpContext.Response.Cookies.Append("access_token", token, new CookieOptions
    {
        HttpOnly = true, //against xss
        Secure = true, //https is needed
        SameSite = SameSiteMode.Strict, //against CSRF
        Expires = DateTime.UtcNow.AddMinutes(30)
    } );
    
    return Results.Ok(new {message = "access guaranted" });
});


app.Run();

async void TestDb()
{
    var dbContext = builder.Services.BuildServiceProvider().GetRequiredService<MongoDbContext>();
    var usersCollection = dbContext.Database.GetCollection<BsonDocument>("Users");

    var testUser = new BsonDocument
    {
        { "name", "Test User" },
        { "email", "test@example.com" },
        { "gamesPlayed", 0 }
    };

    await usersCollection.InsertOneAsync(testUser);

    Console.WriteLine("Tesztfelhasználó sikeresen létrehozva!");
}

void CheckDb()
{


    var dbContext = builder.Services.BuildServiceProvider().GetRequiredService<MongoDbContext>();

    try
    {
        var databases = dbContext.Database.Client.ListDatabaseNames().ToList();
        Console.WriteLine("Sikeres MongoDB kapcsolat! Elérhető adatbázisok:");
        foreach (var db in databases)
        {
            Console.WriteLine($"- {db}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"MongoDB kapcsolat hiba: {ex.Message}");
    }
}

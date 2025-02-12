using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using StonePaperScissor.HubSignalWebsocket;
using StonePaperScissor.Service.Simulation.SimulationServices;
using StonePaperScissor.Service.Simulation.SimulationServices.Interfaces;
using StonePaperScissor.View;

var builder = WebApplication.CreateBuilder(args);



// Add services to the container.

builder.Services.AddControllers();
//Signal IR - websocket registri
builder.Services.AddSignalR();

builder.Services.AddScoped<IVisualiser, DotVisualiser>();
builder.Services.AddScoped<IGameStatistic, DotStatistic>();
builder.Services.AddSingleton<ISimulationStorage, SimulationStorage>();

builder.Services.AddScoped<ISimulatorService, SimulationService>();
builder.Services.AddScoped<ISimulatorService, SimulationService>();
builder.Services.AddScoped<IInitialiser, SimulationInitialiser>();
builder.Services.AddScoped<ISimulatorFactory, SimulatorFactory>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        policy =>
        {
            policy
                .WithOrigins("http://localhost:5173") // Ezt állítsd be a frontend URL-jére
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
                .SetIsOriginAllowed(_ => true);
        });
});

// Add authentication services
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
        
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                
                // SignalR esetén engedélyezzük az access_token használatát
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/simulationHub"))
                {
                    context.Token = accessToken;
                }
                else
                {
                    // 🔥 Backend API esetén a `backend_auth` cookie-t használjuk
                    if (context.Request.Cookies.TryGetValue("backend_auth", out var backendAuthToken))
                    {
                        context.Token = backendAuthToken;
                    }
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


//middleware sets from  each incoming requests cookies token into authorization header from frontend
// app.Use(async (context, next) =>
// {
//     if (context.Request.Cookies.TryGetValue("access_token", out var token))
//     {
//         context.Request.Headers.Append("Authorization", $"Bearer {token}");
//     }
//     await next();
// });

//middleware sets from  each incoming requests cookies token into authorization header
//from frontend + backend api
app.Use(async (context, next) =>
{
   
    if (context.Request.Path.StartsWithSegments("/simulationHub"))
    {
        if (context.Request.Cookies.TryGetValue("access_token", out var jwtToken))
        {
            context.Request.Headers.Append("Authorization", $"Bearer {jwtToken}");
        }
    }
    else
    {
        var cookieName = builder.Configuration["SimulationApi:CookieName"];
        if (context.Request.Cookies.TryGetValue(cookieName, out var backendAuthToken))
        {
            context.Request.Headers.Append("Authorization", $"Bearer {backendAuthToken}");
        }
    }

    await next();
});



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseWebSockets();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseCors("AllowAllOrigins");


//evolution of websocket
//app.MapHub<SimulationHub>("/simulationHub"); //endpoint for websocket
app.MapHub<SimulationHub>("/simulationHub").RequireAuthorization(); //endpoint for websocket with authentication
// app.MapHub<SimulationHub>("/simulationHub", options =>
// {
//     options.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets;
//     //http://localhost:5000/simulationHub if backend runs on http://localhost:5000/
//     
// }).RequireAuthorization();;
app.Run();


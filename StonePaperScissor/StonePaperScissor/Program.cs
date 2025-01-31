using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using StonePaperScissor.HubSignalWebsocket;
using StonePaperScissor.Service;
using StonePaperScissor.Service.Simulation;
using StonePaperScissor.Service.Simulation.Items;
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
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();



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


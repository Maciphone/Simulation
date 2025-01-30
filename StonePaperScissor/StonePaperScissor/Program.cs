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
        builder => builder
            .WithOrigins("http://localhost:5173")  // Ezt állítsd be a frontend URL-jére
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});



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

app.UseHttpsRedirection();

app.MapControllers();
app.UseCors("AllowAllOrigins"); 
app.MapHub<SimulationHub>("/simulationHub");
app.Run();


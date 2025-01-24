using StonePaperScissor.Service;
using StonePaperScissor.Service.Inicialiser;
using StonePaperScissor.Service.Simulation;
using StonePaperScissor.Service.Simulation.Items;
using StonePaperScissor.View;

var builder = WebApplication.CreateBuilder(args);



// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddScoped<IInicialiser, SimulationInicialiser>();
builder.Services.AddSingleton<IVisualiser, DotVisualiser>();
builder.Services.AddSingleton<IGameStatistic, DotStatistic>();
builder.Services.AddScoped<ISimulatorService, SimulatorService>();
builder.Services.AddScoped<IUtil, Util>();


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


app.Run();


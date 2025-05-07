using PatientManager.Managers; // Ensure this is the correct namespace for the PatientManager class
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration, sectionName: "Serilog")
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

// Fix: Ensure the correct class is being referenced, not the namespace
builder.Services.AddSingleton<IPatientManager>(sp =>
    new PatientManager.Managers.PatientManager(builder.Configuration["Data:PatientsFile"]));

builder.Services.AddControllers();
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

app.UseAuthorization();

app.MapControllers();

app.Run();

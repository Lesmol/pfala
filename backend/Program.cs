using Azure.Identity;
using backend.Data.Services;
using backend.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

string connectionString = builder.Configuration.GetConnectionString("AppConfig");

// Load configuration from Azure App Configuration
builder.Configuration.AddAzureAppConfiguration(options =>
{
    options.Connect(connectionString);

    options.ConfigureKeyVault(kv =>
    {
        kv.SetCredential(new AzureCliCredential());
    });
});


// Bind configuration "Pfala:AppConf" section to the Settings object
builder.Services.Configure<AppConfOptions>(builder.Configuration.GetSection("Pfala:AppConf"));

builder.Services.AddScoped<AzureBlobService, AzureBlobService>();


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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

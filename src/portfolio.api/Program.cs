using portfolio.api;
using portfolio.infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAWSLambdaHosting(LambdaEventSource.RestApi);

IConfigurationRoot configuration = await GetConfiguration(builder);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddServices(configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapEndpoints();

app.Run();

static async Task<IConfigurationRoot> GetConfiguration(WebApplicationBuilder builder)
{
    IConfigurationRoot configuration;
    if (builder.Environment.IsDevelopment())
        configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
                        .AddEnvironmentVariables()
                        .Build() ?? throw new Exception("Configuration is null");
    else
        configuration = await SecretsManagerHelper.GetConfigurationFromPlainText();

    return configuration;
}
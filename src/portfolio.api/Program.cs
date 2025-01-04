using portfolio.api;
using portfolio.infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
});

builder.Services.AddAWSLambdaHosting(LambdaEventSource.RestApi);

IConfigurationRoot configuration = await SecretsManagerHelper.GetConfiguration(builder);

var allowedDomains = configuration.GetSection("AllowedDomains").Get<string[]>() ?? [];

builder.Services.AddCors(item =>
{
    item.AddPolicy(portfolio.api.Constants.Policies.CorsPolicy, builder =>
    {
        builder.WithOrigins(allowedDomains)
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddServices(configuration);

var app = builder.Build();

app.UseMiddleware<ApiResponseExceptionMiddleware>();

app.UseCors(portfolio.api.Constants.Policies.CorsPolicy);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapEndpoints();

app.Run();


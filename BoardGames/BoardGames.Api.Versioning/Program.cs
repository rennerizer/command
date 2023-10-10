using Microsoft.AspNetCore.Cors;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;

using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc(
        "v1",
        new OpenApiInfo { Title = "BoardGames.Api", Version = "v1.0" });
    options.SwaggerDoc(
        "v2",
        new OpenApiInfo { Title = "BoardGames.Api", Version = "v2.0" });
});

// Manage CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(config =>
    {
        config.WithOrigins(builder.Configuration["AllowedOrigins"]!);
        config.AllowAnyHeader();
        config.AllowAnyMethod();
    });
    options.AddPolicy(name: "AnyOrigin", config =>
    {
        config.AllowAnyOrigin();
        config.AllowAnyHeader();
        config.AllowAnyMethod();
    });
    options.AddPolicy(name: "AnyOrigin_GetOnly", config =>
    {
        config.AllowAnyOrigin();
        config.AllowAnyHeader();
        config.WithMethods("GET");
    });
});

// Versioning
builder.Services.AddApiVersioning(options =>
{
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Configuration.GetValue<bool>("UseSwagger"))
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            $"/swagger/v1/swagger.json",
            $"BoardGames.Api v1");
        options.SwaggerEndpoint(
            $"/swagger/v2/swagger.json",
            $"BoardGames.Api v2");
    });
}

if (app.Configuration.GetValue<bool>("UseDeveloperExceptionPage"))
    app.UseDeveloperExceptionPage();
else
    app.UseExceptionHandler("/error");

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

// Minimal API
app.MapGet("/v{version:ApiVersion}/error",
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [EnableCors("AnyOrigin")]
    [ResponseCache(NoStore = true)]
    () => Results.Problem());

app.MapGet("/v{version:ApiVersion}/error/test",
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [EnableCors("AnyOrigin")]
    [ResponseCache(NoStore = true)] 
    () => { throw new Exception("test"); });

app.MapGet("/v{version:ApiVersion}/cod/test",
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [EnableCors("AnyOrigin_GetOnly")]
    [ResponseCache(NoStore = true)] () =>
    Results.Text(
        "<script>" +
            "window.alert('Your client supports JavaScript!" +
            "\\r\\n\\r\\n" +
            $"Server time (UTC): {DateTime.UtcNow.ToString("o")}" +
            "\\r\\n" +
            "Client time (UTC): ' + new Date().toISOString());" +
        "</script>" +
        "<noscript>Your client does not support JavaScript</noscript>",
        "text/html"));

app.MapControllers();

app.Run();

using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Configuration.GetValue<bool>("UseSwagger"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (app.Configuration.GetValue<bool>("UseDeveloperExceptionPage"))
    app.UseDeveloperExceptionPage();
else
    app.UseExceptionHandler("/error");

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

// Minimal API
app.MapGet("/error", 
    [EnableCors("AnyOrigin")]
    [ResponseCache(NoStore = true)]
    () => Results.Problem());

app.MapGet("/error/test", 
    [EnableCors("AnyOrigin")]
    [ResponseCache(NoStore = true)] 
    () => { throw new Exception("test"); });

app.MapGet("/cod/test",
    [EnableCors("AnyOrigin")]
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

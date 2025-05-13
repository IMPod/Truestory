using FluentValidation.AspNetCore;
using Microsoft.OpenApi.Models;
using Serilog;
using Truestory.WebAPI.Infrastructure;
using Truestory.WebAPI.Middlewares;
using MediatR;
using Truestory.WebAPI.PipelineBehaviors;
using Truestory.WebAPI.Infrastructure.Settings;

var builder = WebApplication.CreateBuilder(args);

// ==================================================================================
// == SERILOG CONFIGURATION ========================================================
// ==================================================================================
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .MinimumLevel.Debug()
    .CreateLogger();

builder.Host.UseSerilog(); // Use Serilog for logging

// ==================================================================================
// == SERVICE REGISTRATIONS =========================================================
// ==================================================================================

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblies(typeof(MediatRCommandAssemblyMarker).Assembly));

// Add FluentValidation auto-validation (for IRequestHandler pipelines)
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection(nameof(ApiSettings)));

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

// Add HTTP Client services for making external requests
builder.Services.AddHttpClient();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Truestory API", Version = "v1" });
    c.EnableAnnotations();
});

// ==================================================================================
// == MIDDLEWARE PIPELINE SETUP =====================================================
// ==================================================================================
var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.Use(async (context, next) =>
    {
        if (context.Request.Path == "/")
        {
            context.Response.Redirect("/swagger");
            return;
        }

        await next();
    });
}

app.UseAuthorization();
app.MapControllers();
app.Run();
using Microsoft.IdentityModel.Tokens;
using Product_API_JWT.Configs;
using Product_API_JWT.Data;
using Product_API_JWT.Exceptions.Handlers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ProductNotFoundExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.AddAuthentication()
	.AddJwtBearer(o =>
    {
        o.RequireHttpsMetadata = false; // Para facilitar rodar na rede do docker compose
        o.MetadataAddress = builder.Configuration["Authentication:MetadataAddress"]!;
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = builder.Configuration["Authentication:ValidIssuer"],
            ValidAudience = builder.Configuration["Authentication:Audience"]
        };
    });
builder.Services.AddAuthorization();

builder.Services.ResolveDependencies();
builder.Services.ConfigureDatabase(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.DoMigrations(app.Environment.IsProduction());
app.SeedDatabase();

app.Run();

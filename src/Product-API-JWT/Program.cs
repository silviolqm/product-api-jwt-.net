using Product_API_JWT.Configs;
using Product_API_JWT.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.ResolveDependencies();
builder.Services.ConfigureDatabase(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.DoMigrations(app.Environment.IsProduction());
app.SeedDatabase();

app.Run();

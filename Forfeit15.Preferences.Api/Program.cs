using System.Text.Json.Serialization;
using Forfeit15.Postgres.Extensions;
using Forfeit15.Preferences.Core.Services.Preferences;
using Forfeit15.Preferences.Core.Services.Preferences.Implementations;
using Forfeit15.Preferences.Postgres.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPreferencesPostgres(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IPreferenceService, PreferenceService>();

builder.Services
    .AddControllers().AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MigrateDatabases();
app.MapControllers();

app.Run();
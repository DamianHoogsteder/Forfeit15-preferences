using System.Text.Json.Serialization;
using Forfeit15.Postgres.Extensions;
using Forfeit15.Preferences.Core.Services;
using Forfeit15.Preferences.Core.Services.MessageConsumer;
using Forfeit15.Preferences.Core.Services.Preferences;
using Forfeit15.Preferences.Core.Services.Preferences.Implementations;
using Forfeit15.Preferences.Postgres.Extensions;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPreferencesPostgres(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<UpdateHub>();
builder.Services.AddHostedService<MessageConsumer>();
builder.Services.AddScoped<IPreferenceService, PreferenceService>();

// Add SignalR services
builder.Services.AddSignalR();

builder.Services
    .AddControllers().AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("ClientPermission", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseCors("ClientPermission");

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();

    // Map SignalR hub
    endpoints.MapHub<UpdateHub>("/updateHub");
});

app.MigrateDatabases();

app.Run();
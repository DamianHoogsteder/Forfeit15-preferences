using System.Text.Json;
using Forfeit15.Postgres.Contexts;
using Forfeit15.Postgres.Models.Preferences;
using Forfeit15.Preferences.Core.Services.Preferences.Contracts;
using Forfeit15.Preferences.Core.Services.Preferences.ViewModels;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Forfeit15.Preferences.Core.Services.Preferences.Implementations;

public class PreferenceService : IPreferenceService
{
    private readonly PreferenceDbContext _preferenceDbContext;
    private readonly UpdateHub _updateHub;

    public PreferenceService(PreferenceDbContext preferenceDbContext, UpdateHub updateHub)
    {
        _preferenceDbContext = preferenceDbContext;
        _updateHub = updateHub;
    }
    
    public async Task<ICollection<string>> GetPreferencesAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _preferenceDbContext.Preferences.Where(x => x.UserId == userId).Select(x => x.Content)
            .ToListAsync(cancellationToken);
    }

    public async Task<PostPreferencesResponse> SetPreferencesAsync(IEnumerable<string> subscribedPreferences,
        Guid userId, CancellationToken cancellationToken)
    {
        var response = new PostPreferencesResponse();
        var responsePreferences = subscribedPreferences.ToList();
        foreach (var preference in responsePreferences)
        {
            var item = new Preference
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Content = preference,
                TimeStamp = DateTime.UtcNow
            };
            await _preferenceDbContext.Preferences.AddAsync(item, cancellationToken);
            await _preferenceDbContext.SaveChangesAsync(cancellationToken);
        }

        response.Preferences = responsePreferences;
        response.Result = PostPreferenceResult.SuccesvolToegevoegd;
        return response;
    }

    public async Task<UpdatePreferencesResponse> UpdatePreferencesAsync(IEnumerable<string> newPreferences, Guid userId,
        CancellationToken cancellationToken)
    {
        var response = new UpdatePreferencesResponse();
        var currentPreferences = await _preferenceDbContext.Preferences
            .Where(x => x.UserId == userId)
            .ToListAsync(cancellationToken);

        if (currentPreferences.Count == 0)
        {
            response.Result = UpdatePreferencesResult.GeenPreferencesGevonden;
            return response;
        }

        _preferenceDbContext.Preferences.RemoveRange(currentPreferences);

        var responsePreferences = newPreferences.Select(preference => new Preference
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Content = preference,
            TimeStamp = DateTime.UtcNow
        }).ToList();

        _preferenceDbContext.Preferences.AddRange(responsePreferences);
        await _preferenceDbContext.SaveChangesAsync(cancellationToken);

        response.Preferences = (responsePreferences as IEnumerable<string>)!;
        response.Result = UpdatePreferencesResult.SuccesvolGeüpdate;
        return response;
    }

    public async Task PushNotificationAsync(UpdateMessageVM message, CancellationToken cancellationToken)
    {
        var userIds = await _preferenceDbContext.Preferences.AsNoTracking().Where(x => x.Content == message.Type)
            .Select(x => x.UserId)
            .ToListAsync(cancellationToken);

        var clientIds = userIds.Select(g => g.ToString()).ToArray();
        var messageObject = JsonSerializer.Serialize(message);
        
        //foreach signalR client 
        await _updateHub.SendUpdate(clientIds, messageObject, cancellationToken);
    }
}
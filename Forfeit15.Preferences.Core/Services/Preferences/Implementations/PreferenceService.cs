using Forfeit15.Postgres.Contexts;
using Forfeit15.Postgres.Models.Preferences;
using Forfeit15.Preferences.Core.Services.Preferences.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Forfeit15.Preferences.Core.Services.Preferences.Implementations;

public class PreferenceService : IPreferenceService
{
    private readonly PreferenceDbContext _preferenceDbContext;

    public PreferenceService(PreferenceDbContext preferenceDbContext)
    {
        _preferenceDbContext = preferenceDbContext;
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
        var currentPreferences = await _preferenceDbContext.Preferences.AsNoTracking().Where(x => x.UserId == userId)
            .ToListAsync(cancellationToken);

        if (currentPreferences.Count == 0)
        {
            response.Result = UpdatePreferencesResult.GeenPreferencesGevonden;
            return response;
        }

        _preferenceDbContext.Preferences.RemoveRange(currentPreferences);
        await _preferenceDbContext.SaveChangesAsync(cancellationToken);

        var responsePreferences = newPreferences.ToList();
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
        response.Result = UpdatePreferencesResult.SuccesvolGeüpdate;
        return response;
    }
}
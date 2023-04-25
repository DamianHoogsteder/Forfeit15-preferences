using Forfeit15.Preferences.Core.Services.Preferences.Contracts;

namespace Forfeit15.Preferences.Core.Services.Preferences;

public interface IPreferenceService
{
    Task<PostPreferencesResponse> SetPreferencesAsync(IEnumerable<string> subscribedPreferences, Guid userId,
        CancellationToken cancellationToken);

    Task<UpdatePreferencesResponse> UpdatePreferencesAsync(IEnumerable<string> newPreferences, Guid userId,
        CancellationToken cancellationToken);
}
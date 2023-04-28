using Forfeit15.Postgres.Models.Preferences;
using Forfeit15.Preferences.Core.Services.Preferences.Contracts;
using Forfeit15.Preferences.Core.Services.Preferences.ViewModels;

namespace Forfeit15.Preferences.Core.Services.Preferences;

public interface IPreferenceService
{
    Task<ICollection<string>> GetPreferencesAsync(Guid userId, CancellationToken cancellationToken);

    Task<PostPreferencesResponse> SetPreferencesAsync(IEnumerable<string> subscribedPreferences, Guid userId,
        CancellationToken cancellationToken);

    Task<UpdatePreferencesResponse> UpdatePreferencesAsync(IEnumerable<string> newPreferences, Guid userId,
        CancellationToken cancellationToken);

    Task PushNotificationAsync(UpdateMessageVM message, CancellationToken cancellationToken);
}
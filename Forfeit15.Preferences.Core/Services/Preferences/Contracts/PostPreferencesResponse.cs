using System.Runtime.Serialization;

namespace Forfeit15.Preferences.Core.Services.Preferences.Contracts;

[DataContract]
public class PostPreferencesResponse
{
    [DataMember] 
    public IEnumerable<string> Preferences { get; set; } = null!;

    [DataMember] 
    public PostPreferenceResult Result { get; set; }
}
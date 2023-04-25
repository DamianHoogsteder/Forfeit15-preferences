using System.Runtime.Serialization;

namespace Forfeit15.Preferences.Core.Services.Preferences.Contracts;

[DataContract]
public class UpdatePreferencesResponse
{
    [DataMember] 
    public IEnumerable<string> Preferences { get; set; } = null!;

    [DataMember] 
    public UpdatePreferencesResult Result { get; set; }
}

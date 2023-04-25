using System.Runtime.Serialization;

namespace Forfeit15.Preferences.Core.Services.Preferences.Contracts;

[DataContract]
public enum PostPreferenceResult
{
    [DataMember]
    GebruikerNietGevonden,
    
    [DataMember]
    SuccesvolToegevoegd
}
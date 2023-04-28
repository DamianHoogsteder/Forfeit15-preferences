using System.Runtime.Serialization;

namespace Forfeit15.Preferences.Core.Services.Preferences.ViewModels;

[DataContract]
public class MessageBodyVM
{
    [DataMember] public string Title { get; set; } = null!;

    [DataMember] public string Description { get; set; } = null!;

    [DataMember] public DateTime TimeStamp { get; set; }
}
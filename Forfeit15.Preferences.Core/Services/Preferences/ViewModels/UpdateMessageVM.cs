using System.Runtime.Serialization;

namespace Forfeit15.Preferences.Core.Services.Preferences.ViewModels;

[DataContract]
public class UpdateMessageVM
{
    [DataMember] 
    public string Type { get; set; } = null!;

    [DataMember] 
    public Guid UserId { get; set; }
    
    [DataMember] 
    public string Title { get; set; }
    
    [DataMember] 
    public string Description { get; set; }
    
    [DataMember] 
    public DateTime TimeStamp { get; set; }
}
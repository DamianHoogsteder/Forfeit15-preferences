using Forfeit15.Preferences.Core.Services.Preferences;
using Microsoft.AspNetCore.Mvc;

namespace Forfeit15.Preferences.Api.Controllers;

[ApiController]
[Route("[controller]/{userId:guid}")]
[Consumes("application/json")]
[Produces("application/json")]
public class PreferenceController : ControllerBase
{
    private readonly IPreferenceService _preferenceService;

    public PreferenceController(IPreferenceService preferenceService)
    {
        _preferenceService = preferenceService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetPreferences( Guid userId,
        CancellationToken cancellationToken)
    {
        return Ok(await _preferenceService.GetPreferencesAsync(userId, cancellationToken));
    } 
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SetPreferences([FromBody] IEnumerable<string> subscribedPreferences, Guid userId,
        CancellationToken cancellationToken)
    {
        return Ok(await _preferenceService.SetPreferencesAsync(subscribedPreferences, userId, cancellationToken));
    }    
    
    [HttpPatch]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdatePreferences([FromBody] IEnumerable<string> newPreferences, Guid userId,
        CancellationToken cancellationToken)
    {
        return Ok(await _preferenceService.UpdatePreferencesAsync(newPreferences, userId, cancellationToken));
    }
}
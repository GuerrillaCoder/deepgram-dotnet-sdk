﻿namespace Deepgram.Models.Schemas;
public class UpdateProjectMemberScopeSchema
{
    /// <summary>
    /// Scope to add for memeber
    /// </summary>
    [JsonPropertyName("scope")]
    public string? Scope { get; set; }
}

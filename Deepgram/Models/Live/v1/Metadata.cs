﻿namespace Deepgram.Models.Live.v1;

public record MetaData
{
    /// <summary>
    /// TODO
    /// </summary>
    [JsonPropertyName("request_id")]
    public string? RequestId { get; set; }

    /// <summary>
    /// TODO
    /// </summary>
    [JsonPropertyName("model_uuid")]
    public string? ModelUUID { get; set; }

    /// <summary>
    /// IReadonlyDictionary of <see cref="ModelInfo"/>
    /// </summary>
    [JsonPropertyName("model_info")]
    public Dictionary<string, ModelInfo> ModelInfo { get; set; }

    /// <summary>
    /// Deepgram’s Extra Metadata feature allows you to attach arbitrary key-value pairs to your API requests that are attached to the API response for usage in downstream processing.
    /// Extra metadata is limited to 2048 characters per key-value pair.
    /// <see href="https://developers.deepgram.com/docs/extra-metadata"/>
    /// </summary>
    [JsonPropertyName("extra")]
    public Dictionary<string, string>? Extra { get; set; }
}
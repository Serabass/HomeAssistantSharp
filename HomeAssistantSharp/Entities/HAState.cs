namespace HomeAssistantSharp.Entities;

using Newtonsoft.Json;
using HomeAssistantSharp.Entities.Attributes;

public struct HAStateContext : IBaseAttributes
{
  [JsonProperty("friendly_name")] public string friendlyName { get; set; }
  [JsonProperty("id")] public string id { get; set; }
  [JsonProperty("parent_id")] public string? parentId { get; set; }
  [JsonProperty("user_id")] public string? userId { get; set; }
}

public struct HAState
{
  public readonly bool IsUnavailable => State == "unavailable" || Attributes == null || State == null;

  [JsonProperty("entity_id")] public string EntityId { get; set; }
  public string State { get; set; }
  public dynamic Attributes { get; set; }
  public HAStateContext Context { get; set; }
  [JsonProperty("last_changed")] public DateTime LastChanged { get; set; }
  [JsonProperty("last_updated")] public DateTime LastUpdated { get; set; }

  public T AttributesAs<T>()
  {
    return (T)Attributes;
  }
}

namespace HomeAssistantSharp.Entities;

using Newtonsoft.Json;

public struct HAStateContext : IBaseAttributes
{
  [JsonProperty("friendly_name")] public string friendlyName { get; set; }
  [JsonProperty("id")] public string id { get; set; }
  [JsonProperty("parent_id")] public string? parentId { get; set; }
  [JsonProperty("user_id")] public string? userId { get; set; }
}

public struct HAState
{
  public readonly bool IsUnavailable => state == "unavailable" || attributes == null || state == null;

  [JsonProperty("entity_id")] public string entityId { get; set; }
  public string state { get; set; }
  public object attributes { get; set; }
  public HAStateContext context { get; set; }
  [JsonProperty("last_changed")] public DateTime lastChanged { get; set; }
  [JsonProperty("last_updated")] public DateTime lastUpdated { get; set; }

  public T Attributes<T>()
  {
    if (attributes == null)
      return JsonConvert.DeserializeObject<T>("{}");

    var data = JsonConvert.DeserializeObject<T>(attributes.ToString());
    return data;
  }
}

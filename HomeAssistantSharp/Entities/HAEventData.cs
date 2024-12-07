using Newtonsoft.Json;

namespace HomeAssistantSharp.Entities;

public struct HAEventData
{
  [JsonProperty("entity_id")] public string EntityId { get; set; }
  [JsonProperty("old_state")] public HAState Old { get; set; }
  [JsonProperty("new_state")] public HAState New { get; set; }
}

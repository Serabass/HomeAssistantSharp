namespace HomeAssistantSharp.Client;

public class EventMessage
{
  public required string Source { get; set; }
  public required dynamic Data { get; set; }
}

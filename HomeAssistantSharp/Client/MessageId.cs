namespace HomeAssistantSharp.Client;

public class MessageId
{
  public int Id { get; set; } = 0;

  public int Next => ++Id;
}

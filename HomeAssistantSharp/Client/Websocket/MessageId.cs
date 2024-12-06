namespace HomeAssistantSharp.Client.Websocket;

public class MessageId
{
  public int Id { get; set; } = 0;

  public int Next => ++Id;
}

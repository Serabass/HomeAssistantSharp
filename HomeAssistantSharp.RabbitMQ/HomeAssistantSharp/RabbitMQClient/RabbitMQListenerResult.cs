namespace HomeAssistantSharp.RabbitMQClient;

public struct RabbitMQListenerResult<D>
{
  public D Data;
  public string Source;
}

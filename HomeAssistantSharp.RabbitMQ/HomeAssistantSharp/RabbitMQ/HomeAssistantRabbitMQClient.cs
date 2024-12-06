namespace HomeAssistantSharp.Client.RabbitMQ;

using HomeAssistantSharp.Client;
public class HomeAssistantRabbitMQClient : HomeAssistantClientBase
{
  private readonly string _ip;
  private readonly short _port;
  private readonly string _token;

  public HomeAssistantRabbitMQClient(string ip, short port, string token)
  {
    _ip = ip;
    _port = port;
    _token = token;
  }
}

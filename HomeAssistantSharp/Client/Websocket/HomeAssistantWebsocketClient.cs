namespace HomeAssistantSharp.Client.Websocket;

using System.Text;
using Newtonsoft.Json;

public class HomeAssistantWebsocketClient : HomeAssistantClientBase
{
  protected readonly WebsocketClient _websocketClient;
  protected readonly Uri _url;
  protected readonly string _token;

  public string HAVersion { get; private set; }

  public HomeAssistantWebsocketClient(string url, string token) : this(new Uri(url), token) { }
  public HomeAssistantWebsocketClient(string host, short port, string token) : this(new Uri($"ws://{host}:{port}/api/websocket"), token) { }
  public HomeAssistantWebsocketClient(Uri url, string token)
  {
    _url = url;
    _token = token;
    _websocketClient = new WebsocketClient(_url);
  }

  public override async Task Init()
  {
    _websocketClient.MessageReceived += async (sender, message) =>
    {
      Console.WriteLine($"Message received: {message.Source}");
      var messageType = GetMessageType((string)message.Data.type);

      switch (messageType)
      {
        case MessageType.AuthRequired:
          Console.WriteLine("Auth required");
          await _websocketClient.Send(new { type = "auth", access_token = _token });
          break;
        case MessageType.AuthOk:
          HAVersion = message.Data.ha_version;
          InvokeAuthOk();
          await Subscribe("state_changed");
          break;
        case MessageType.AuthInvalid:
          InvokeAuthInvalid();
          break;
        case MessageType.Result:
          if ((int)message.Data.id == _subscribeId)
          {
            if ((bool)message.Data.success)
            {
              Console.WriteLine("Subscribed");
              InvokeReady();
            }
            else
            {
              throw new Exception($"Failed to subscribe: {message.Data.error}");
            }
          }
          break;
        case MessageType.Event:
          InvokeEvent(this, new EventMessage
          {
            Source = message.Source,
            Data = message.Data
          });
          break;
      }
    };

    await _websocketClient.ConnectAsync();
  }

  private async Task Subscribe(string type)
  {
    await _websocketClient.Send(new
    {
      id = _subscribeId = _messageId.Next,
      type = "subscribe_events",
      event_type = type
    });
  }

  public override void Dispose()
  {
    _websocketClient.Dispose();
  }
}

namespace HomeAssistantSharp.Client.Websocket;

using System.Net.WebSockets;
using System.Text;
using Newtonsoft.Json;

public class HomeAssistantWebsocketClient : HomeAssistantClientBase
{
  protected ClientWebSocket _webSocket = new ClientWebSocket();
  protected readonly Uri _url;
  protected readonly string _token;

  public string HAVersion { get; private set; }

  public HomeAssistantWebsocketClient(string url, string token)
  {
    _url = new Uri(url);
    _token = token;
  }

  public HomeAssistantWebsocketClient(Uri url, string token)
  {
    _url = url;
    _token = token;
  }

  public override async Task ConnectAsync()
  {
    await _webSocket.ConnectAsync(_url, CancellationToken.None);
    await HandleMessages();
  }

  private async Task HandleMessages()
  {
    var buffer = new byte[4096 * 4];

    while (_webSocket.State == WebSocketState.Open)
    {
      var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
      var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
      var json = JsonConvert.DeserializeObject<dynamic>(message);
      buffer = new byte[4096 * 4];

      if (json == null)
        continue;

      // Console.WriteLine($"Received message: {message}");
      // Console.WriteLine($"Received json.type: {json.type}");

      var messageType = GetMessageType((string)json.type);

      switch (messageType)
      {
        case MessageType.AuthRequired:
          Console.WriteLine("Auth required");
          await Send(new { type = "auth", access_token = _token });
          break;
        case MessageType.AuthOk:
          HAVersion = json.ha_version;
          InvokeAuthOk();
          await Subscribe("state_changed");
          break;
        case MessageType.AuthInvalid:
          InvokeAuthInvalid();
          break;
        case MessageType.Result:
          if ((int)json.id == _subscribeId)
          {
            if ((bool)json.success)
            {
              Console.WriteLine("Subscribed");
              InvokeReady();
            }
            else
            {
              throw new Exception($"Failed to subscribe: {json.error}");
            }
          }
          break;
        case MessageType.Event:
          Console.WriteLine("Event received");
          break;
      }
    }
  }

  private async Task Subscribe(string type)
  {
    await Send(new
    {
      id = _subscribeId = _messageId.Next,
      type = "subscribe_events",
      event_type = type
    });
  }

  public async Task Send<T>(T message)
  {
    var json = JsonConvert.SerializeObject(message);
    var bytes = Encoding.UTF8.GetBytes(json);
    var seg = new ArraySegment<byte>(bytes);
    Console.WriteLine($"Sending: {json}");

    await _webSocket.SendAsync(seg, WebSocketMessageType.Text, true, CancellationToken.None);
  }

  public override void Dispose()
  {
    _webSocket.Dispose();
  }
}

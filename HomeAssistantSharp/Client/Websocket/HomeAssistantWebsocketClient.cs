namespace HomeAssistantSharp.Client.Websocket;

using System.Net.WebSockets;
using System.Text;
using Newtonsoft.Json;

public class HomeAssistantWebsocketClient : HomeAssistantClientBase, IDisposable
{
  private readonly Uri _url;
  private readonly string _token;

  public event EventHandler AuthOk;
  public event EventHandler AuthInvalid;
  public event EventHandler Ready;

  private ClientWebSocket _webSocket = new ClientWebSocket();
  private MessageId _messageId = new();
  private int _subscribeId = 0;

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

  private MessageType GetMessageType(string type) => type switch
  {
    "auth_required" => MessageType.AuthRequired,
    "auth_ok" => MessageType.AuthOk,
    "auth_invalid" => MessageType.AuthInvalid,
    "result" => MessageType.Result,
    "event" => MessageType.Event,
    _ => MessageType.Unknown
  };

  public async Task ConnectAsync()
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
          AuthOk?.Invoke(this, EventArgs.Empty);
          await Subscribe("state_changed");
          break;
        case MessageType.AuthInvalid:
          AuthInvalid?.Invoke(this, EventArgs.Empty);
          break;
        case MessageType.Result:
          if ((int)json.id == _subscribeId)
          {
            if ((bool)json.success)
            {
              Console.WriteLine("Subscribed");
              Ready?.Invoke(this, EventArgs.Empty);
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

  public void Dispose()
  {
    _webSocket.Dispose();
  }
}

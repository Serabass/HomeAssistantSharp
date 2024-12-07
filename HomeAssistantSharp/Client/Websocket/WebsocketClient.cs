namespace HomeAssistantSharp.Client.Websocket;

using System.Net.WebSockets;
using System.Text;
using Newtonsoft.Json;

public class WebsocketClient : IDisposable
{
  public class WebsocketMessage
  {
    public string Source { get; set; }
    public dynamic Data { get; set; }
  }

  protected ClientWebSocket _webSocket = new ClientWebSocket();

  public event EventHandler<WebsocketMessage> MessageReceived;

  protected readonly Uri _url;

  public WebsocketClient(string url)
  {
    _url = new Uri(url);
  }

  public WebsocketClient(Uri url)
  {
    _url = url;
  }

  public async Task ConnectAsync()
  {
    await _webSocket.ConnectAsync(_url, CancellationToken.None);
    new Thread(() => HandleMessages()).Start();
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

      MessageReceived?.Invoke(this, new WebsocketMessage
      {
        Source = message,
        Data = json
      });
    }

    _webSocket.Dispose();
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

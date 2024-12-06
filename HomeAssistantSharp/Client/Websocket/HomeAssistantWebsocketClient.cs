namespace HomeAssistantSharp.Client.Websocket;

using System.Net.WebSockets;

public class HomeAssistantWebsocketClient : HomeAssistantClientBase
{
  private readonly string _url;
  private readonly string _token;

  public event EventHandler AuthOk;
  public event EventHandler Ready;

  public HomeAssistantWebsocketClient(string url, string token)
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
    var client = new ClientWebSocket();
    await client.ConnectAsync(new Uri(_url), CancellationToken.None);
  }
}

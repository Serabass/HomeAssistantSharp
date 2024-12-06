namespace HomeAssistantSharp.Client;

public abstract class HomeAssistantClientBase : IDisposable
{
  public event EventHandler AuthOk;
  public event EventHandler AuthInvalid;
  public event EventHandler Ready;

  protected MessageId _messageId = new();
  protected int _subscribeId = 0;

  protected MessageType GetMessageType(string type) => type switch
  {
    "auth_required" => MessageType.AuthRequired,
    "auth_ok" => MessageType.AuthOk,
    "auth_invalid" => MessageType.AuthInvalid,
    "result" => MessageType.Result,
    "event" => MessageType.Event,
    _ => MessageType.Unknown
  };
  protected void InvokeAuthOk() => AuthOk?.Invoke(this, EventArgs.Empty);
  protected void InvokeAuthInvalid() => AuthInvalid?.Invoke(this, EventArgs.Empty);
  protected void InvokeReady() => Ready?.Invoke(this, EventArgs.Empty);
  public abstract Task ConnectAsync();
  public abstract void Dispose();
}

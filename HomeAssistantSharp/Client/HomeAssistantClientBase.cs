namespace HomeAssistantSharp.Client;

public abstract class HomeAssistantClientBase : IDisposable
{
  public event EventHandler AuthOk;
  public event EventHandler AuthInvalid;
  public event EventHandler Ready;
  public event EventHandler<EventMessage> Event;

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
  protected void InvokeEvent(object sender, EventMessage message) => Event?.Invoke(sender, message);

  public virtual async Task Init()
  {
    Ready += (sender, args) =>
    {
      Console.WriteLine("Ready");

      Event += (sender, msg) =>
      {
        // Console.WriteLine($"Event received - {msg.Source}: {msg.Data}");
      };
    };
  }

  public abstract void Dispose();
}

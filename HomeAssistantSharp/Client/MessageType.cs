namespace HomeAssistantSharp.Client;

public enum MessageType
{
  AuthRequired,
  AuthOk,
  AuthInvalid,
  Result,
  Event,
  Unknown
}

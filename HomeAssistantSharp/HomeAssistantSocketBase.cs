namespace Serabass.HomeAssistantSharp;

public abstract class HomeAssistantSocketBase
{
  public required abstract string Ip { get; init; }
  public required abstract short Port { get; init; }
  public required abstract string Token { get; init; }
}

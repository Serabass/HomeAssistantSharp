namespace HomeAssistantSharp.Entities.Typed;

using HomeAssistantSharp.Entities.Attributes;
public abstract class EnumEntity<T, A>(string entityId) : Entity<T, A>(entityId)
  where A : IBaseAttributes
  where T : struct, Enum
{
  public override abstract T ConvertStateValue(HAState state);

  public override async Task Init()
  {
    await base.Init();
  }
}
